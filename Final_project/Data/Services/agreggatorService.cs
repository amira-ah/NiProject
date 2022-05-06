using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Linq;
using System.Threading.Tasks;

namespace Final_project.Data.Services
{
    public class agreggatorService
    {
        private readonly IConfiguration configuration;

        public agreggatorService(IConfiguration config)
        {
            configuration = config;
        }

        public void aggregator(long idfile)
        {
           
            string conString = Microsoft.Extensions.Configuration.ConfigurationExtensions.GetConnectionString(configuration, "VerticaConnectionstring");
            /*
            string queryString = "select distinct DATE_TRUNC('Hour',DATETIME_KEY) from TRANS_MW_AGG_SLOT_HOURLY;";

            using (OdbcConnection connection = new OdbcConnection(conString))
            {
        

                OdbcDataAdapter adapter = new OdbcDataAdapter(queryString, connection);
                DataSet dataSet = new DataSet();
                // Open the connection and fill the DataSet.
                try
                {
                    connection.Open();
                    adapter.Fill(dataSet);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);

                }

                connection.Close();

            }
string queryString2 = "Select NETWORK_SID, NEID, OBJECTCol, TIMECol, INTERVALCol, DIRECTION, NEALIAS, NETYPE,max(MAXRXLEVEL),max(MAXTXLEVEL),LINK,SLOT, max(MAXTXLEVEL) * (-1) - max(MAXRXLEVEL) * (-1) from TRANS_MW_ERC_PM_TN_RADIO_LINK_POWER group by NETWORK_SID,NEID,OBJECTCol,TIMECol,INTERVALCol,DIRECTION,NEALIAS,NETYPE,LINK,SLOT;";
            DataTable dt = new DataTable("td");
            using (OdbcConnection connection2 = new OdbcConnection(conString))
            {
                

                OdbcDataAdapter adapter2 = new OdbcDataAdapter(queryString2, connection2);
                DataSet dataSet2 = new DataSet();
                // Open the connection and fill the DataSet.
                try
                {
                    connection2.Open();
                    //  adapter2.Fill(dataSet2);
                    adapter2.Fill(dt);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);

                }

                connection2.Close();

            }*/

            // Execute the DataReader and access the data.
            /* OdbcDataReader reader = command.ExecuteReader();
             while (reader.Read())
             {
                 Console.WriteLine("CustomerID={0}", reader[0]);

             }

             // Call Close when done reading.
             reader.Close();


         }*/

            string queryString = @"INSERT INTO TRANS_MW_AGG_SLOT_HOURLY 
                                    SELECT NETWORK_SID, NEID, DATE_TRUNC('Hour',TIMECol), INTERVALCol, DIRECTION, NEALIAS, NETYPE,
                                    MAX(MAXRXLEVEL) AS 'MAX_RX_LEVEL',
                                    MAX(MAXTXLEVEL) AS 'MAX_TX_LEVEL',
                                    LINK, SLOT, 
                                    ABS(""MAX_RX_LEVEL"") - ABS(""MAX_TX_LEVEL"") AS 'RSL_DEVIATION' 
                                    FROM TRANS_MW_ERC_PM_TN_RADIO_LINK_POWER 
                                    WHERE filenum='"+idfile+"'  And (DATE_TRUNC('Hour',TIMECol) not in (select distinct DATETIME_KEY from TRANS_MW_AGG_SLOT_HOURLY ))  GROUP BY NETWORK_SID, NEID, OBJECTCol, TIMECol, INTERVALCol, DIRECTION, NEALIAS, NETYPE, LINK, SLOT";


                 using (OdbcConnection connection = new OdbcConnection(conString))
                                 {
                            OdbcCommand command = new OdbcCommand(queryString, connection);

     
                                 try
                                  {
                                  connection.Open();
                                  command.ExecuteNonQuery();
                                    
                                   }
                              catch (Exception ex)
                                    {
                                Console.WriteLine(ex.Message);

                                   }

                               connection.Close();



                        }

              string querystring2 = @"INSERT INTO TRANS_MW_AGG_SLOT_Daily
                                    SELECT NETWORK_SID, NEID,  DATE_TRUNC('Day',DATETIME_KEY), INTERVALCol, DIRECTION, NEALIAS, NETYPE,
                                    MAX(MAX_RX_LEVEL) AS 'MAXRXLEVEL',
                                    MAX(MAX_TX_LEVEL) AS 'MAXTXLEVEL',
                                    LINK, SLOT, 
                                    ABS(""MAXRXLEVEL"") - ABS(""MAXTXLEVEL"") AS 'RSLDEVIATION'
                                    FROM TRANS_MW_AGG_SLOT_HOURLY
                                    WHERE DATE_TRUNC('Day',DATETIME_KEY) not in (select distinct DATETIME_KEY from TRANS_MW_AGG_SLOT_Daily )
                                    GROUP BY NETWORK_SID, NEID, DATETIME_KEY, INTERVALCol, DIRECTION, NEALIAS, NETYPE, LINK, SLOT";


            using (OdbcConnection connection2 = new OdbcConnection(conString))
            {
                OdbcCommand command = new OdbcCommand(querystring2, connection2);

           

                try
                {
                    connection2.Open();
                    command.ExecuteNonQuery();

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);

                }
                connection2.Close();

            }
            string queryUpdate = @"UPDATE LoggingInfo
                                          SET aggregated = true,aggregatedTime=NOW()
                                           WHERE LoggingInfo.Id = '" + idfile + "'";
            using (OdbcConnection conUp = new OdbcConnection(conString))
            {
                OdbcCommand command = new OdbcCommand(queryUpdate, conUp);

                //command.Parameters.Add(new VerticaParameter( “path”, VerticaType.VarChar));
                //command.Parameters["path"].Value = fullpath;
                try
                {
                    conUp.Open();
                    command.ExecuteNonQuery();
                    conUp.Close();

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);

                }
            }

            /*
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
              
                
                CREATE TABLE trans_H(
                NETWORK_SID   INTEGER NOT NULL,
                DATETIME_KEY  timestamp NOT NULL,
                LINK VARCHAR(50),
                SLOT INTEGER,
                MAX_RX_LEVEL FLOAT,
                MAX_TX_LEVEL FLOAT,
                RSL_DEVIATION FLOAT,
              PARTITION BY DATE_TRUNC('Hour', DATETIME_KEY);
                )*/




        }
    }
}
