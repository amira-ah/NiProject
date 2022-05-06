using Final_project.Controllers;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Final_project.Data.Services
{
    public class LoaderService
    {
        private readonly IConfiguration configuration;
        bool loadeddata = false;
        public agreggatorService _aggregatorService;
        public agreggatorController _aggregatorController;
    
        public LoaderService(IConfiguration config)
        {
            configuration = config;
            _aggregatorService = new agreggatorService(configuration);
            _aggregatorController = new agreggatorController(_aggregatorService);
        }
        public void Loadingstratetgy(string fullpath,string pathexcep, string loaded_Data_path)
        {
            //ma fe de3e jeeb count la2an eza eja bado ya3ml loaing la7 ykoun 3aml parsing

            var varname1 = fullpath.LastIndexOf("\\");
            var filename = fullpath.Substring(fullpath.LastIndexOf("\\") + 1, fullpath.LastIndexOf(".") - varname1 - 1);
            string conString = Microsoft.Extensions.Configuration.ConfigurationExtensions.GetConnectionString(configuration, "VerticaConnectionstring");
            long Id = 1;
            using (OdbcConnection ConSelect = new OdbcConnection(conString))
            {


                string querySelect = @"Select Id  from LoggingInfo where filename ilike '" + filename + "";
                OdbcCommand cmdSELECT = new OdbcCommand(querySelect, ConSelect);
                try
                {
                    ConSelect.Open();
                    Id = (Int64)cmdSELECT.ExecuteScalar();

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);


                }


                ConSelect.Close();
            }
            DateTime creation = File.GetCreationTime(fullpath);
            string queryUpdatepar = @"UPDATE LoggingInfo
                                          SET parsed = true,parsedTime='"+creation+"'  WHERE LoggingInfo.Id = '" + Id + "'";
            using (OdbcConnection conUp = new OdbcConnection(conString))
            {
                OdbcCommand command = new OdbcCommand(queryUpdatepar, conUp);

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




            string loadedQuery = @"Select loaded from LoggingInfo where filename ilike '" + filename + "'";
            bool loaded=true;
            using (OdbcConnection secondCon = new OdbcConnection(conString))
            {
                OdbcCommand cmd = new OdbcCommand(loadedQuery, secondCon);


                try
                {
                    secondCon.Open();
                    loaded = (bool)cmd.ExecuteScalar();

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                secondCon.Close();
            }
            if (!loaded)
            {
                string queryString = $"COPY TRANS_MW_ERC_PM_TN_RADIO_LINK_POWER FROM LOCAL '{fullpath}' DELIMITER ','";

                using (OdbcConnection connection = new OdbcConnection(conString))
                {
                    OdbcCommand command = new OdbcCommand(queryString, connection);

                    //command.Parameters.Add(new VerticaParameter( “path”, VerticaType.VarChar));
                    //command.Parameters["path"].Value = fullpath;
                    try
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                        loadeddata = true;

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);

                    }

                    connection.Close();
                    // Call Close when done reading.

                }

                //bady e5la2 file in a directory feyo esm filename 
                if (loadeddata)
                {

                    using (OdbcConnection ConSelect = new OdbcConnection(conString))
                    {
                        long getId = 1;

                        string querySelect = @"Select Id  from LoggingInfo where filename ilike '" + filename + "";
                        OdbcCommand cmdSELECT = new OdbcCommand(querySelect, ConSelect);
                        try
                        {
                            ConSelect.Open();
                            getId = (Int64)cmdSELECT.ExecuteScalar();
                            ConSelect.Close();
                            string queryUpdate = @"UPDATE LoggingInfo
                                          SET loaded = true,loadedTime=NOW()
                                           WHERE LoggingInfo.Id = '" + getId + "'";
                            using (OdbcConnection conUp = new OdbcConnection(conString))
                            {
                                OdbcCommand command = new OdbcCommand(queryUpdate, conUp);
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

                                _aggregatorService.aggregator(getId);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);


                        }


                        ConSelect.Close();
                    }

                   

                }
            }

        }
    }
}
