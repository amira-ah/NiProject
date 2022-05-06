using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Final_project.Data.Services;
using System.Data.Odbc;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.Extensions.Primitives;
using Microsoft.Extensions.FileProviders;

namespace Final_project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParserController : ControllerBase
    {
        // private  static string _fileFilter ;
        public static ParserService _parserService;
        private static string _pathParsed;
        private static string _pathUploadedTo;
        private static string _pathParsedMoved;
        private readonly string _path;
        private readonly IWebHostEnvironment _hostingEnviroment;
        public ParserController(ParserService parserService, IWebHostEnvironment hostingEnviroment)
        {
            _parserService = parserService;
            _hostingEnviroment = hostingEnviroment;
            _pathParsed = Path.Combine(_hostingEnviroment.ContentRootPath.Substring(0, _hostingEnviroment.ContentRootPath.LastIndexOf("\\")), "FilesUploaded");
            _path = _hostingEnviroment.ContentRootPath.Substring(0, _hostingEnviroment.ContentRootPath.LastIndexOf("\\"));
            _pathUploadedTo = Path.Combine(_hostingEnviroment.ContentRootPath.Substring(0, _hostingEnviroment.ContentRootPath.LastIndexOf("\\")), "FilesUploadedTo");
            _pathParsedMoved= Path.Combine(_hostingEnviroment.ContentRootPath.Substring(0, _hostingEnviroment.ContentRootPath.LastIndexOf("\\")), "FilesParsed");
        }
       
        [HttpGet("parsing")]
        public IActionResult Parsing()
        {
              /*  FileSystemWatcher watcher = new FileSystemWatcher();
                 watcher.Path = _pathParsed;
                 watcher.NotifyFilter = NotifyFilters.Attributes
                                    | NotifyFilters.CreationTime
                                    | NotifyFilters.DirectoryName
                                    | NotifyFilters.FileName
                                    | NotifyFilters.LastAccess
                                    | NotifyFilters.LastWrite
                                    | NotifyFilters.Security
                                    | NotifyFilters.Size;

                 watcher.Changed += OnChanged;
                 watcher.Filter = "*.txt";
                 watcher.EnableRaisingEvents = true;
           
            */

            //check it later
            // _fileFilter = Path.Combine(_pathParsed, "*.txt");  
            //  var fileProvider = new PhysicalFileProvider(_pathParsed);
            //  IChangeToken token = fileProvider.Watch(_fileFilter);

            return Ok();


        }
       public static void OnChanged(object sender, FileSystemEventArgs e)
        {
            FileSystemWatcher watcherLoader = new FileSystemWatcher();
            watcherLoader.Path = _pathUploadedTo;
            watcherLoader.NotifyFilter = NotifyFilters.Attributes
                               | NotifyFilters.CreationTime
                               | NotifyFilters.DirectoryName
                               | NotifyFilters.FileName
                               | NotifyFilters.LastAccess
                               | NotifyFilters.LastWrite
                               | NotifyFilters.Security
                               | NotifyFilters.Size;

            watcherLoader.Created += Final_project.Controllers.LoaderController.OnCreated;
            watcherLoader.Filter = "*.txt";
            watcherLoader.EnableRaisingEvents = true;
          
            if (e.ChangeType != WatcherChangeTypes.Changed)
            {
                return;
            }
           
            _parserService.stratetgyparsing(e.FullPath, _pathUploadedTo, _pathParsedMoved);
       
        } 


    }





}


