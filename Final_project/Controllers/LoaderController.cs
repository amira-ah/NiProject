using Final_project.Data.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Final_project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoaderController : ControllerBase
    {
        public static LoaderService _LoaderService;
        private static string _pathParsed;
        private static string _pathParsedone;
        private static string _pathExceptions;
        private static string _loadedData;
        private readonly string _path;
        private readonly IWebHostEnvironment _hostingEnviroment;
        public LoaderController(LoaderService loaderService, IWebHostEnvironment hostingEnviroment)
        {
            _LoaderService = loaderService;
            _hostingEnviroment = hostingEnviroment;
            _pathParsed = Path.Combine(_hostingEnviroment.ContentRootPath.Substring(0, _hostingEnviroment.ContentRootPath.LastIndexOf("\\")), "FilesUploaded");
            _path = _hostingEnviroment.ContentRootPath.Substring(0, _hostingEnviroment.ContentRootPath.LastIndexOf("\\"));
            _pathParsedone = Path.Combine(_hostingEnviroment.ContentRootPath.Substring(0, _hostingEnviroment.ContentRootPath.LastIndexOf("\\")), "FilesUploadedTo");

            _pathExceptions = Path.Combine(_hostingEnviroment.ContentRootPath.Substring(0, _hostingEnviroment.ContentRootPath.LastIndexOf("\\")), "Exceptions\\exec.txt");
        _loadedData= Path.Combine(_hostingEnviroment.ContentRootPath.Substring(0, _hostingEnviroment.ContentRootPath.LastIndexOf("\\")), "LoadedData");

        }
        public static void OnCreated(object sender, FileSystemEventArgs e)
        {
            
            _LoaderService.Loadingstratetgy(e.FullPath, _pathExceptions, _loadedData);
        }


   /*    public static void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Changed)
            {
                return;
            }
            _LoaderService.Loadingstratetgy(e.FullPath, _pathExceptions);

        }
    */
    }
}
