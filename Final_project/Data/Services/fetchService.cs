using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Final_project.Data.Services
{
    public class fetchService
    {

        private readonly IConfiguration configuration;

        public fetchService(IConfiguration config)
        {
            configuration = config;
        }
        public List<Dictionary<string, object>> getData() {
            var allData = new List<Dictionary<string, object>>();
            string con = Microsoft.Extensions.Configuration.ConfigurationExtensions.GetConnectionString(configuration, "VerticaConnectionstring");
            string query = @" SELECT DATETIME_KEY,NEALIAS,NETYPE,
                                MAX(MAX_RX_LEVEL) AS 'MAXRXLEVEL',
                                MAX(MAX_TX_LEVEL) AS 'MAXTXLEVEL',
                                LINK, sum(SLOT) AS 'SUM_SLOT', 
                                ABS(""MAXRXLEVEL"") - ABS(""MAXTXLEVEL"") AS 'RSLDEVIATION'
                                FROM TRANS_MW_AGG_SLOT_HOURLY
                                GROUP BY DATETIME_KEY,NEALIAS,NETYPE,LINK; ";
      
            using (OdbcConnection connection = new OdbcConnection(con))
            {
                OdbcCommand command = new OdbcCommand(query, connection);

                connection.Open();

             
                OdbcDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var newRow = new Dictionary<string, object>
                    {
                        { "DATETIME_KEY", (DateTime)reader[0] },
                        { "NEALIAS", (string)reader[1] },
                        { "NETYPE", (string)reader[2] },
                        { "MAXRXLEVEL", (double)reader[3] },
                        { "MAXTXLEVEL", (double)reader[4] },
                        { "LINK", (string)reader[5] },
                        {"SUM_SLOT" ,(Int64)reader[6]},
                        {"RSLDEVIATION" ,(double)reader[7]}
                    };


                    allData.Add(newRow);

                
                }

        
                reader.Close();
                return allData;

            }
        }
        public List<Dictionary<string, object>> GetDailyData()
        {
            var allData = new List<Dictionary<string, object>>();
            string con = Microsoft.Extensions.Configuration.ConfigurationExtensions.GetConnectionString(configuration, "VerticaConnectionstring");
            string query = @" SELECT DATETIME_KEY,NEALIAS,NETYPE,
                                MAX(MAX_RX_LEVEL) AS 'MAXRXLEVEL',
                                MAX(MAX_TX_LEVEL) AS 'MAXTXLEVEL',
                                LINK, sum(SLOT) AS 'SUM_SLOT', 
                                ABS(""MAXRXLEVEL"") - ABS(""MAXTXLEVEL"") AS 'RSLDEVIATION'
                                FROM TRANS_MW_AGG_SLOT_Daily
                                GROUP BY DATETIME_KEY,NEALIAS,NETYPE,LINK; ";

            using (OdbcConnection connection = new OdbcConnection(con))
            {
                OdbcCommand command = new OdbcCommand(query, connection);

                connection.Open();


                OdbcDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var newRow = new Dictionary<string, object>
                    {
                        { "DATETIME_KEY", (DateTime)reader[0] },
                        { "NEALIAS", (string)reader[1] },
                        { "NETYPE", (string)reader[2] },
                        { "MAXRXLEVEL", (double)reader[3] },
                        { "MAXTXLEVEL", (double)reader[4] },
                        { "LINK", (string)reader[5] },
                        {"SUM_SLOT" ,(Int64)reader[6]},
                        {"RSLDEVIATION" ,(double)reader[7]}
                    };


                    allData.Add(newRow);


                }


                reader.Close();
                return allData;

            }
        }

        public List<Dictionary<string, object>> GetDataDate(string datefrom, string dateto, string guess)
        {
            if (guess == "DailyData")
            {
                var allData = new List<Dictionary<string, object>>();
                string con = Microsoft.Extensions.Configuration.ConfigurationExtensions.GetConnectionString(configuration, "VerticaConnectionstring");
                string query = @" SELECT DATETIME_KEY,NEALIAS,NETYPE,
                                MAX(MAX_RX_LEVEL) AS 'MAXRXLEVEL',
                                MAX(MAX_TX_LEVEL) AS 'MAXTXLEVEL',
                                LINK, sum(SLOT) AS 'SUM_SLOT', 
                                ABS(""MAXRXLEVEL"") - ABS(""MAXTXLEVEL"") AS 'RSLDEVIATION'
                                FROM TRANS_MW_AGG_SLOT_Daily
                                where DATETIME_KEY between  '" + datefrom + "' AND '" + dateto + "' GROUP BY DATETIME_KEY,NEALIAS,NETYPE,LINK; ";

                using (OdbcConnection connection = new OdbcConnection(con))
                {
                    OdbcCommand command = new OdbcCommand(query, connection);

                    connection.Open();


                    OdbcDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var newRow = new Dictionary<string, object>
                    {
                        { "DATETIME_KEY", (DateTime)reader[0] },
                        { "NEALIAS", (string)reader[1] },
                        { "NETYPE", (string)reader[2] },
                        { "MAXRXLEVEL", (double)reader[3] },
                        { "MAXTXLEVEL", (double)reader[4] },
                        { "LINK", (string)reader[5] },
                        {"SUM_SLOT" ,(Int64)reader[6]},
                        {"RSLDEVIATION" ,(double)reader[7]}
                    };


                        allData.Add(newRow);


                    }


                    reader.Close();
                    return allData;

                }
            }
            else
            {
                var allData = new List<Dictionary<string, object>>();
                string con = Microsoft.Extensions.Configuration.ConfigurationExtensions.GetConnectionString(configuration, "VerticaConnectionstring");
                string query = @" SELECT DATETIME_KEY,NEALIAS,NETYPE,
                                MAX(MAX_RX_LEVEL) AS 'MAXRXLEVEL',
                                MAX(MAX_TX_LEVEL) AS 'MAXTXLEVEL',
                                LINK, sum(SLOT) AS 'SUM_SLOT', 
                                ABS(""MAXRXLEVEL"") - ABS(""MAXTXLEVEL"") AS 'RSLDEVIATION'
                                FROM TRANS_MW_AGG_SLOT_HOURLY
                                where DATETIME_KEY between  '" + datefrom + "' AND '" + dateto + "' GROUP BY DATETIME_KEY,NEALIAS,NETYPE,LINK; ";

                using (OdbcConnection connection = new OdbcConnection(con))
                {
                    OdbcCommand command = new OdbcCommand(query, connection);

                    connection.Open();


                    OdbcDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var newRow = new Dictionary<string, object>
                    {
                        { "DATETIME_KEY", (DateTime)reader[0] },
                        { "NEALIAS", (string)reader[1] },
                        { "NETYPE", (string)reader[2] },
                        { "MAXRXLEVEL", (double)reader[3] },
                        { "MAXTXLEVEL", (double)reader[4] },
                        { "LINK", (string)reader[5] },
                        {"SUM_SLOT" ,(Int64)reader[6]},
                        {"RSLDEVIATION" ,(double)reader[7]}
                    };


                        allData.Add(newRow);


                    }


                    reader.Close();
                    return allData;

                }
            }
        }
        }

    }


