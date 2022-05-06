using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Odbc;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Final_project.Data.Services
{
    public class KeyVAluespair
    {
        private readonly IConfiguration configuration;

        public KeyVAluespair(IConfiguration config)
        {
            configuration = config;
        }
        public void par(string fullpath)
        {
            Dictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>();
            //    dictionary.Add(File.ReadLines(fullpath).First().Split(",").ToString(), new ArrayList());
            dictionary = File.ReadLines(fullpath).First().Split(",").ToDictionary(line => line, line => new List<string>());

            foreach (var fe in File.ReadLines(fullpath).Skip(1).Select(line => line.Split(',')))
            {
                dictionary["NodeName"].Add(fe[0]);
                dictionary["NeId"].Add(fe[1]);
                dictionary["Object"].Add(fe[2]);
                dictionary["Time"].Add(fe[3]);
                dictionary["Interval"].Add(fe[4]);
                dictionary["Direction"].Add(fe[5]);
                dictionary["NeAlias"].Add(fe[6]);
                dictionary["NeType"].Add(fe[7]);
                dictionary["Position"].Add(fe[8]);
                dictionary["RxLevelBelowTS1"].Add(fe[9]);
                dictionary["RxLevelBelowTS2"].Add(fe[10]);
                dictionary["MinRxLevel"].Add(fe[11]);
                dictionary["MaxRxLevel"].Add(fe[12]);
                dictionary["TxLevelAboveTS1"].Add(fe[13]);
                dictionary["MinTxLevel"].Add(fe[14]);
                dictionary["MaxTxLevel"].Add(fe[15]);
                dictionary["IdLogNum"].Add(fe[16]);
                dictionary["FailureDescription"].Add(fe[17]);
            }
            //disabled columns:
 //           dictionary.Remove(dictionary.Where(line => { var name = new ArrayList() { "NodeName", "Position", "IdLogNum" }.Contains(line.Key); if (name) { return true; } else { return false; } }).Select(line => line.Key).ToString());
      /*      var v = dictionary.Select(line => line.Key).Where(line => {
                var name = new ArrayList() { "NodeName", "Position", "IdLogNum" }.Contains(line);
                if (name) { return true; } else { return false; } }).ToList();
            List<string> array =new List<string> { "NodeName", "Position", "IdLogNum" };
            var keys = dictionary.Where(x => "NodeName".Contains(x.Key)).Select(x => x.Key).ToList();
          */
            // remove set of disabled columns by updating on the dictionary
            dictionary =  dictionary.Where(x => {
              var name = new ArrayList() { "NodeName", "Position", "IdLogNum" }.Contains(x.Key);
              if (!name) {  return true; } else { return false; }
          }).ToDictionary(c => c.Key, c => c.Value);
           
            int j = 0;
            // remove based on unreachable bulk
            dictionary["Object"].RemoveAll(item => {
           
                var selectsTrue = item.Contains("Unreachable Bulk FC");
               if (selectsTrue)
               {
                   dictionary["NeId"].RemoveAt(j);
                   dictionary["Time"].RemoveAt(j);
                   dictionary["Interval"].RemoveAt(j);
                   dictionary["Direction"].RemoveAt(j);
                   dictionary["NeAlias"].RemoveAt(j);
                   dictionary["NeType"].RemoveAt(j);
                   dictionary["RxLevelBelowTS1"].RemoveAt(j);
                   dictionary["RxLevelBelowTS2"].RemoveAt(j);
                   dictionary["MinRxLevel"].RemoveAt(j);
                   dictionary["MaxRxLevel"].RemoveAt(j);
                   dictionary["TxLevelAboveTS1"].RemoveAt(j);
                   dictionary["MinTxLevel"].RemoveAt(j);
                   dictionary["MaxTxLevel"].RemoveAt(j);
                   dictionary["FailureDescription"].RemoveAt(j);
                    j--;
               }
               j++;
               return selectsTrue;
               });
            
            // remove based on -
            int K = 0;
            dictionary["FailureDescription"].RemoveAll(item => {
           
                var selectsTrue = item.Contains("-");
                if (!selectsTrue)
                {
                    dictionary["NeId"].RemoveAt(K);
                    dictionary["Time"].RemoveAt(K);
                    dictionary["Interval"].RemoveAt(K);
                    dictionary["Direction"].RemoveAt(K);
                    dictionary["NeAlias"].RemoveAt(K);
                    dictionary["NeType"].RemoveAt(K);
                    dictionary["RxLevelBelowTS1"].RemoveAt(K);
                    dictionary["RxLevelBelowTS2"].RemoveAt(K);
                    dictionary["MinRxLevel"].RemoveAt(K);
                    dictionary["MaxRxLevel"].RemoveAt(K);
                    dictionary["TxLevelAboveTS1"].RemoveAt(K);
                    dictionary["MinTxLevel"].RemoveAt(K);
                    dictionary["MaxTxLevel"].RemoveAt(K);
                    dictionary["Object"].RemoveAt(K);
                    K--;
                }
                K++;
                return !selectsTrue;
            });
          
            dictionary.Add("Link",new List<string>());
            dictionary.Add("Tid", new List<string>());
            dictionary.Add("FarEndTid", new List<string>());
            dictionary.Add("Slot", new List<string>());
            dictionary.Add("Port", new List<string>());
            dictionary.Add("NETWORK_SID", new List<string>());
            dictionary.Add("DATETIME_KEY", new List<string>());
            // datetimee_key
            var varname1 = fullpath.LastIndexOf("\\");
            var filename = fullpath.Substring(fullpath.LastIndexOf("\\") + 1, fullpath.LastIndexOf(".") - varname1 - 1);
            var file = filename.Split("_");
            var d = file[file.Length - 2] + " " + file[file.Length - 1];

            DateTime dt = DateTime.ParseExact(d, "yyyyMMdd HHmmss", null);
            string datestring= dt.ToString("yyyy-MM-dd HH:mm:ss");

            // 1/14+15/1__ATDd__A27b
            //  1 / 2 / 1__ATDm__ATJm
            // 1 / 6.2 / 1__ATDj__ATJo

            var something = dictionary["Object"].Select(item => item).ToList();
            List<string> Object = new List<string>();
            int i = 0;
            var oo = dictionary["Object"];
            foreach (var item in dictionary["Object"])
            {
                char operators = item.Contains("+") ? '+' : '.';
                char guess;
                if (operators != '+')
                {
                    guess = item.Contains(operators) ? '.' : '/';
                    operators = guess;
                }
                string[] getarray = item.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                string getTheFirstVal = getarray[0];
                string link = "", TID = "", FareEndId = "", Slot = "", Port = "";
                string[] slots = new string[2];
                int gettheindexof = item.IndexOf("/");
                int lasttheindexof = item.LastIndexOf("/");
                int halfentry = getarray.Length / 2;
                int lastentry = getarray.Length - 1;
                switch (operators)
                {
                    case '+':

                        slots[0] = getTheFirstVal.Substring(getTheFirstVal.IndexOf("+") + 1, getTheFirstVal.LastIndexOf("/") - getTheFirstVal.IndexOf("+") - 1);
                        slots[1] = getTheFirstVal.Substring(getTheFirstVal.IndexOf("/") + 1, getTheFirstVal.IndexOf("+") - getTheFirstVal.IndexOf("/") - 1);
                        TID = getarray[halfentry];
                        FareEndId = getarray[lastentry];
                        Port = getTheFirstVal.Substring(item.LastIndexOf("/") + 1);
                        link = getTheFirstVal.Substring(gettheindexof + 1, lasttheindexof - gettheindexof - 1) + "/" + Port;
                        Object.AddRange(new List<string> { item,item });
                        i++;
                        dictionary["NeId"].Insert(i, dictionary["NeId"][i-1]);
                        dictionary["Time"].Insert(i, dictionary["Time"][i-1]);
                        dictionary["Interval"].Insert(i, dictionary["Interval"][i-1]);
                        dictionary["Direction"].Insert(i, dictionary["Direction"][i-1]);
                        dictionary["NeAlias"].Insert(i, dictionary["NeAlias"][i-1]);
                        dictionary["NeType"].Insert(i, dictionary["NeType"][i-1]);
                        dictionary["RxLevelBelowTS1"].Insert(i, dictionary["RxLevelBelowTS1"][i-1]);
                        dictionary["RxLevelBelowTS2"].Insert(i, dictionary["RxLevelBelowTS2"][i-1]);
                        dictionary["MinRxLevel"].Insert(i, dictionary["MinRxLevel"][i-1]);
                        dictionary["MaxRxLevel"].Insert(i, dictionary["MaxRxLevel"][i-1]);
                        dictionary["TxLevelAboveTS1"].Insert(i, dictionary["TxLevelAboveTS1"][i-1]);
                        dictionary["MinTxLevel"].Insert(i, dictionary["MinTxLevel"][i-1]);
                        dictionary["MaxTxLevel"].Insert(i, dictionary["MaxTxLevel"][i-1]);
                        dictionary["FailureDescription"].Insert(i, dictionary["FailureDescription"][i - 1]);
                        byte[] bytes = new byte[32];
                        using (SHA256 sha256Hash = SHA256.Create())
                        {
                            bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(Object[i-1] + dictionary["NeAlias"][i - 1]));
                        }

                        var getnum = BitConverter.ToInt32(bytes, 0);
                      i++;
                        dictionary["Link"].AddRange(new List<string> { link, link });
                        dictionary["Tid"].AddRange(new List<string> { TID, TID });
                        dictionary["FarEndTid"].AddRange(new List<string> { FareEndId, FareEndId });
                        dictionary["Slot"].AddRange(new List<string> { slots[0], slots[1] });
                        dictionary["Port"].AddRange(new List<string> { Port, Port });
                        dictionary["DATETIME_KEY"].AddRange(new List<string> { datestring, datestring });
                        dictionary["NETWORK_SID"].AddRange(new List<string> { $"{getnum}", $"{getnum}" });
                        break;
                    case '.':
                        Slot = (getTheFirstVal.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries)[1]).Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries)[0];
                        Port = (getTheFirstVal.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries)[1]).Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries)[1];
                        link = Slot + "/" + Port;
                        TID = getarray[halfentry];
                        FareEndId = getarray[lastentry];
                        Object.Add(item);
                        dictionary["Link"].Add(link);
                        dictionary["Tid"].Add(TID);
                        dictionary["FarEndTid"].Add(FareEndId);
                        dictionary["Slot"].Add(Slot);
                        dictionary["Port"].Add(Port);
                        byte[] bytee = new byte[32];
                        using (SHA256 sha256Hash = SHA256.Create())
                        {
                            bytee = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(Object[i] + dictionary["NeAlias"][i]));
                        }
                       
                      

                        var getnumm = BitConverter.ToInt32(bytee, 0);
                        dictionary["NETWORK_SID"].Add($"{getnumm}");
                        dictionary["DATETIME_KEY"].Add(datestring);
                        i++;
                        break;

                    default:
                        Slot = getTheFirstVal.Substring(gettheindexof + 1, lasttheindexof - gettheindexof - 1);
                        Port = getTheFirstVal.Substring(lasttheindexof + 1);
                        link = Slot + "/" + Port;
                        TID = getarray[halfentry];
                        FareEndId = getarray[lastentry];
                        Object.Add(item);
                        dictionary["Link"].Add(link);
                        dictionary["Tid"].Add(TID);
                        dictionary["FarEndTid"].Add(FareEndId);
                        dictionary["Slot"].Add(Slot);
                        dictionary["Port"].Add(Port);
                        byte[] bytees = new byte[32];
                        using (SHA256 sha256Hash = SHA256.Create())
                        {
                            bytees = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(Object[i] + dictionary["NeAlias"][i]));
                        }

                        var getnummm = BitConverter.ToInt32(bytees, 0);
                        dictionary["NETWORK_SID"].Add($"{getnummm}");
                        dictionary["DATETIME_KEY"].Add(datestring);
                        i++;
                       
                        break;

                }
            }

            dictionary["Object"].Clear();
            dictionary["Object"].AddRange(Object);

           
         
            string text="";
            for(int h=0;h<dictionary["NETWORK_SID"].Count;h++)
            {
  text+= dictionary["NETWORK_SID"][h]
     +","+
     dictionary["DATETIME_KEY"][h]
     + ","+dictionary["NeId"][h] 
     +"," + dictionary["Object"][h]
     + ","+dictionary["Time"][h] 
     + ","+dictionary["Interval"][h]
     +"," + dictionary["Interval"][h]
     +"," + dictionary["Direction"][h]
     +"," + dictionary["NeAlias"][h]
     +"," + dictionary["NeType"][h]
     +"," + dictionary["RxLevelBelowTS1"][h]
     +"," + dictionary["RxLevelBelowTS2"][h]
     + "," + dictionary["MinRxLevel"][h]
     + "," + dictionary["MaxRxLevel"][h]
    + "," + dictionary["TxLevelAboveTS1"][h]
    + "," + dictionary["MinTxLevel"][h]
     + "," + dictionary["MaxTxLevel"][h]
     + "," + dictionary["FailureDescription"][h]
  + "," + dictionary["Link"][h]
   + "," + dictionary["Tid"][h]
     + "," + dictionary["FarEndTid"][h]
    + "," + dictionary["Slot"][h]
    + "," + dictionary["Port"][h]
        + Environment.NewLine;
            }
            
            File.WriteAllText(@"C:\Users\User\Desktop\s.txt", text);
        }
        public void Parsing()
        {
            string conString = Microsoft.Extensions.Configuration.ConfigurationExtensions.GetConnectionString(configuration, "VerticaConnectionstring");


            string queryString = "SELECT DISTINCT EMPLOYEE_NAME FROM employee";

            using (OdbcConnection connection = new OdbcConnection(conString))
            {
                OdbcCommand command = new OdbcCommand(queryString, connection);

                connection.Open();

                // Execute the DataReader and access the data.
                OdbcDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Console.WriteLine("CustomerID={0}", reader[0]);

                }

                // Call Close when done reading.
                reader.Close();
            }
        }
        public string[] getlinebyline(string oneline)
        {
            return oneline.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
        }
        public string[][] TrimArray(int columnToRemove, string[][] originalArray)
        {
            string[][] result = new string[originalArray.GetLength(0) - 1][];

            for (int i = 0, j = 0; i < originalArray.GetLength(0) - 1; i++)
            {

                result[i] = new string[originalArray[i].Length];
                for (int k = 0, u = 0; k < originalArray[i].Length; k++)
                {
                    if (k == columnToRemove)
                    { continue; }

                    result[j][u] = originalArray[i][k];
                    u++;

                }
                result[j] = result[j].Take(originalArray[i].Length - 1).ToArray();
                j++;
            }

            return result;
        }
        public string[][] TrimArrayAdv(int[] columnToRemove, string[][] originalArray)
        {
            string[][] result = new string[originalArray.GetLength(0) - 1][];
            bool check = false;
            int ct = 0;
            for (int i = 0, j = 0; i < originalArray.GetLength(0) - 1; i++)
            {
                ct = 0;
                result[i] = new string[originalArray[i].Length];
                for (int k = 0, u = 0; k < originalArray[i].Length; k++)
                {
                    for (int t = 0; t < columnToRemove.Length; t++)
                    {
                        if (k == columnToRemove[t])
                        {
                            check = true;
                            break;
                        }
                    }
                    if (check)
                    {
                        ct++;
                        check = false;
                        continue;
                    }

                    result[j][u] = originalArray[i][k];
                    u++;

                }
                for (int f = 1; f <= ct; f++)
                {
                    result[j] = result[j].Take(originalArray[i].Length - f).ToArray();
                }

                j++;
            }

            return result;
        }
        public string[][] removeitems(string[][] originalArray, int[] columns)
        {
            string[][] result;
            result = originalArray;
            string[][] r = new string[originalArray.GetLength(0)][];
            r[0] = new string[result[0].Length];
            r[0] = result[0];
            int ct = 1, seehowmanyrowsareremoved = 0;
            for (int i = 1; i < result.GetLength(0); i++)
            {

                if (result[i][columns[0]].ToLower() == "Unreachable Bulk FC".ToLower() || result[i][columns[1]].ToLower() != "-")
                {
                    seehowmanyrowsareremoved++;
                    continue;
                }
                r[ct] = new string[result[i].Length];
                r[ct] = result[i];
                ct++;


            }
            for (int f = 1; f <= seehowmanyrowsareremoved; f++)
            {
                r = r.Take(result.Length - f).ToArray();
            }

            return r;
        }
        public string[][] ItemsReturnArray(string[][] originalArray, int wheretoinsert, string value, string columnname)
        {
            ArrayList myAL = new ArrayList();
            string[][] result = new string[originalArray.GetLength(0)][];
            for (int i = 0; i < originalArray.GetLength(0); i++)
            {
                if (i == 0)
                {
                    myAL.AddRange(originalArray[i]);
                    myAL.Insert(wheretoinsert, columnname);
                    result[i] = new string[myAL.Count];
                    myAL.CopyTo(result[i], 0);
                    myAL.Clear();
                    continue;
                }

                myAL.AddRange(originalArray[i]);
                myAL.Insert(wheretoinsert, value);
                result[i] = new string[myAL.Count];
                myAL.CopyTo(result[i], 0);
                myAL.Clear();
            }
            return result;
        }
        public List<List<string>> itemsupdatedcols(string[][] originalArray, string[] columnNames)
        {
            ArrayList myAL = new ArrayList();
            string[][] result = new string[][] { };
            List<List<string>> c = new List<List<string>>();
            int j = 0;
            for (int i = 0; i < originalArray.GetLength(0); i++)
            {
                if (i == 0)
                {
                    c.Add(originalArray[i].ToList());
                    //   j = c[j].Count;
                    myAL.AddRange(originalArray[i]);
                    for (int f = 0; f < columnNames.Length; f++)
                    {

                        c[j].Add(columnNames[f]);

                    }


                    j++;
                    continue;
                }
                //1/14+15/1__ATDd__A27b
                //  1 / 2 / 1__ATDm__ATJm
                //1 / 6.2 / 1__ATDj__ATJo
                c.Add(originalArray[i].ToList());

                var getObje = originalArray[i][3];
                string[] getarray = getObje.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                int halfentry = getarray.Length / 2;
                int lastentry = getarray.Length - 1;
                string getTheFirstVal = getObje.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries)[0];
                char operators = getTheFirstVal.Contains("+") ? '+' : '.';
                char guess;
                if (operators != '+')
                {
                    guess = getTheFirstVal.Contains(operators) ? '.' : '/';
                    operators = guess;
                }
                string link = "", TID = "", FareEndId = "", Slot = "", Port = "";
                string[] slots = new string[2];
                int gettheindexof = getTheFirstVal.IndexOf("/");
                int lasttheindexof = getTheFirstVal.LastIndexOf("/");
                switch (operators)
                {
                    case '+':

                        slots[0] = getTheFirstVal.Substring(getTheFirstVal.IndexOf("+") + 1, getTheFirstVal.LastIndexOf("/") - getTheFirstVal.IndexOf("+") - 1);
                        slots[1] = getTheFirstVal.Substring(getTheFirstVal.IndexOf("/") + 1, getTheFirstVal.IndexOf("+") - getTheFirstVal.IndexOf("/") - 1);
                        TID = getarray[halfentry];

                        FareEndId = getarray[lastentry];
                        Port = getTheFirstVal.Substring(getTheFirstVal.LastIndexOf("/") + 1);
                        link = getTheFirstVal.Substring(gettheindexof + 1, lasttheindexof - gettheindexof - 1) + "/" + Port;
                        for (int f = 0; f < 2; f++)
                        {
                            c[j].AddRange(new string[] { link, TID, FareEndId, slots[f], Port });
                            if (f != 1)
                            {
                                c.Add(originalArray[i].ToList());
                            }
                            j++;
                        }



                        break;
                    case '.':
                        Slot = (getTheFirstVal.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries)[1]).Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries)[0];
                        Port = (getTheFirstVal.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries)[1]).Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries)[1];
                        link = Slot + "/" + Port;
                        TID = getarray[halfentry];
                        FareEndId = getarray[lastentry];
                        c[j].AddRange(new string[] { link, TID, FareEndId, Slot, Port });
                        j++;

                        break;

                    default:
                        Slot = getTheFirstVal.Substring(gettheindexof + 1, lasttheindexof - gettheindexof - 1);
                        Port = getTheFirstVal.Substring(lasttheindexof + 1);
                        link = Slot + "/" + Port;
                        TID = getarray[halfentry];
                        FareEndId = getarray[lastentry];
                        c[j].AddRange(new string[] { link, TID, FareEndId, Slot, Port });
                        j++;
                        break;

                }



            }
            return c;
        }
        public string[][] HashItemsReturned(string[][] originalArray, string columnName)
        {
            ArrayList myAL = new ArrayList();
            string[][] result = new string[originalArray.GetLength(0)][];
            for (int i = 0; i < originalArray.GetLength(0); i++)
            {
                if (i == 0)
                {
                    myAL.AddRange(originalArray[i]);
                    myAL.Insert(0, columnName);
                    result[i] = new string[myAL.Count];
                    myAL.CopyTo(result[i], 0);
                    myAL.Clear();
                    continue;
                }

                byte[] bytes = new byte[32];
                using (SHA256 sha256Hash = SHA256.Create())
                {
                    bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(originalArray[i][2] + originalArray[i][6]));
                }

                var getnum = BitConverter.ToInt32(bytes, 0);
                myAL.AddRange(originalArray[i]);
                myAL.Insert(0, $"{getnum}");
                result[i] = new string[myAL.Count];
                myAL.CopyTo(result[i], 0);
                myAL.Clear();
            }
            return result;
        }
        public void stratetgyparsing(string fullpath, string uploadToFolder, string destFile)
        {
            if (System.IO.File.Exists(fullpath))
            {
                string conString = Microsoft.Extensions.Configuration.ConfigurationExtensions.GetConnectionString(configuration, "VerticaConnectionstring");
                var varname1 = fullpath.LastIndexOf("\\");
                var filename = fullpath.Substring(fullpath.LastIndexOf("\\") + 1, fullpath.LastIndexOf(".") - varname1 - 1);
                bool parsethefile = false;
                string getSizeQuery = @"Select count(*) from LoggingInfo";
                long getSize;
                using (OdbcConnection connection = new OdbcConnection(conString))
                {
                    OdbcCommand command = new OdbcCommand(getSizeQuery, connection);


                    try
                    {
                        connection.Open();
                        getSize = (Int64)command.ExecuteScalar();
                        connection.Close();
                        if (getSize <= 0)
                        {
                            string querystring = @"INSERT INTO LoggingInfo(filename, parsed, parsedTime, loaded, loadedTime, aggregated, aggregatedTime)VALUES('" + filename + "', true,  NOW() , false, null, false, null)";

                            using (OdbcConnection con = new OdbcConnection(conString))
                            {
                                OdbcCommand cmd = new OdbcCommand(querystring, con);


                                try
                                {
                                    con.Open();
                                    cmd.ExecuteNonQuery();
                                    parsethefile = true;

                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);

                                }

                                con.Close();
                            }
                        }
                        else
                        {
                            string parsedQuery = @"Select parsed from LoggingInfo where filename ilike '" + filename + "'";
                            object parsed;

                            using (OdbcConnection secondCon = new OdbcConnection(conString))
                            {
                                OdbcCommand cmd = new OdbcCommand(parsedQuery, secondCon);


                                try
                                {
                                    secondCon.Open();
                                    parsed = cmd.ExecuteScalar();
                                    secondCon.Close();
                                    //   if (parsed=="false")
                                    // {
                                    //   parsethefile = true;
                                    //}
                                    if (parsed == null)
                                    {
                                        parsethefile = true;


                                    }



                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);

                                }

                                connection.Close();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                }
                if (parsethefile)
                {
                    string[] lines = File.ReadAllLines(fullpath);
                    string[][] arr = new string[lines.Length][];
                    string[] test;

                    for (int i = 0; i < lines.Length; i++)
                    {
                        test = getlinebyline(lines[i]);
                        arr[i] = new string[test.Length];
                        for (int j = 0; j < test.Length; j++)
                        {
                            arr[i][j] = test[j];
                        }


                    }
                    var trim = TrimArrayAdv(new int[] { 0, 8, 16 }, arr);

                    string[] searchForColumns = new string[] { "Object", "FailureDescription" };
                    int[] arrayOfIndexes = new int[searchForColumns.Length];

                    for (int k = 0; k < searchForColumns.Length; k++)
                    {
                        for (int l = 0; l < trim[0].Length; l++)
                        {
                            if (searchForColumns[k].ToLower() == trim[0][l].ToLower())
                            {
                                arrayOfIndexes[k] = l;
                                break;
                            }
                        }
                    }
                    var removingNullItems = removeitems(trim, arrayOfIndexes);
                    //_20200312_001500
                    var HourIndex = fullpath.Substring(fullpath.LastIndexOf("_") - 8, 15);
                    var splithour = HourIndex.Split("_")[0] + " " + HourIndex.Split("_")[1];
                    //var done=DateTime.Parse(splithour);
                    var year = splithour.Substring(0, 4);
                    var Month = splithour.Substring(4, 2);
                    var day = splithour.Substring(6, 2);
                    var hour = splithour.Substring(9, 2);
                    var minute = splithour.Substring(11, 2);
                    var seconds = splithour.Substring(13, 2);

                    var UpdatiingonArray = ItemsReturnArray(removingNullItems, 0, $"{year}-{Month}-{day} {hour}:{minute}:{seconds}", "DATETIME_KEY");

                    var AddingHashed = HashItemsReturned(UpdatiingonArray, "NETWORK_SID");
                    string[] ColumnsToinsert = new string[] { "LINK", "TID", "FARENDTID", "SLOT", "PORT" };
                    var AddedItems = itemsupdatedcols(AddingHashed, ColumnsToinsert);
                    string createText = "";
                    for (int k = 0; k < AddedItems.Count; k++)
                    {
                        for (int l = 0; l < AddedItems[k].Count; l++)
                        {
                            if (l < AddedItems[k].Count - 1)
                            {
                                createText += AddedItems[k][l] + ",";
                                continue;
                            }
                            createText += AddedItems[k][l];
                        }
                        if (k < AddedItems.Count - 1)
                        {
                            createText += Environment.NewLine;
                        }

                    }

                    //         var myUniqueFileName = string.Format(@"SOEM1_TN_RADIO_LINK_POWER_{0}_{1}.txt", DateTime.Now.ToString("yyyyMMdd"), DateTime.Now.ToString("hhmmss"));

                    var fullpathName = Path.Combine(uploadToFolder, filename + ".txt");

                    File.WriteAllText(fullpathName, createText);
                    DateTime creation = File.GetCreationTime(fullpathName);

                    using (OdbcConnection thirdCon = new OdbcConnection(conString))
                    {


                        string queryInsert = @"INSERT INTO LoggingInfo(filename, parsed, parsedTime, loaded, loadedTime, aggregated, aggregatedTime)VALUES('" + filename + "', true,  '" + creation + "' , false, null, false, null)";
                        OdbcCommand cmdtrue = new OdbcCommand(queryInsert, thirdCon);
                        try
                        {
                            thirdCon.Open();
                            cmdtrue.ExecuteNonQuery();

                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);


                        }


                        thirdCon.Close();
                    }
                    var filee = fullpath.Substring(fullpath.LastIndexOf("/") + 1, fullpath.LastIndexOf(".") - fullpath.IndexOf("/") - 1);



                    var dest = Path.Combine(destFile, filename + ".txt");


                    File.Move(fullpath, dest);
                }
            }
        }
    }
}
