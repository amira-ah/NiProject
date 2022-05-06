using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Odbc;
using Final_project.Data.Services;
using Microsoft.Extensions.FileProviders;
using System.IO;
using Final_project.Controllers;

namespace Final_project
{
    public class Startup
    {
        public string ConnectionString { get; set; }
        private static string _pathParsed;
        private static string _pathParsedone;
        private static string _pathParsedMoved;
        private readonly IWebHostEnvironment _hostingEnviroment;
        public ParserService _parserservice;
        public ParserController _parsercontroller;
        public LoaderService _LoaderService;
        public LoaderController _LoaderController;
        public KeyVAluespair _valu;
        public Startup(IConfiguration configuration, IWebHostEnvironment hostingEnviroment)
        {
            Configuration = configuration;
            ConnectionString = configuration.GetConnectionString("VerticaConnectionstring");
            _hostingEnviroment = hostingEnviroment;
            _pathParsed = Path.Combine(_hostingEnviroment.ContentRootPath.Substring(0, _hostingEnviroment.ContentRootPath.LastIndexOf("\\")), "FilesUploaded");
            _pathParsedone = Path.Combine(_hostingEnviroment.ContentRootPath.Substring(0, _hostingEnviroment.ContentRootPath.LastIndexOf("\\")), "FilesUploadedTo");
            _pathParsedMoved = Path.Combine(_hostingEnviroment.ContentRootPath.Substring(0, _hostingEnviroment.ContentRootPath.LastIndexOf("\\")), "FilesParsed");
            //Final_project.Controllers.ParserController._parserService = new ParserService(configuration);
            _parserservice = new ParserService(configuration);
            _parsercontroller = new ParserController(_parserservice, hostingEnviroment);
            _LoaderService= new LoaderService(configuration);
            _LoaderController = new LoaderController(_LoaderService, hostingEnviroment);
            _valu = new KeyVAluespair(configuration);
        }

        public IConfiguration Configuration { get; }
        public IServiceCollection getService;
    
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
             services.AddSingleton<IConfiguration>(Configuration);
            services.AddTransient<ParserService>();
            services.AddTransient<LoaderService>();
            services.AddTransient<agreggatorService>();
            services.AddTransient<fetchService>();

            // Microsoft.Extensions.DependencyInjection.OptionsConfigurationServiceCollectionExtensions.Configure<ConnectionStrings>(services, Configuration.GetSection("Smtp"));
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Final_project", Version = "v1" });
            });

            services.AddCors(options =>
            {
                options.AddPolicy("AllowOrigin",
                                options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Final_project v1"));
           
            


            }
        
            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions { 
                FileProvider= new PhysicalFileProvider(System.IO.Path.Combine(env.ContentRootPath, "..\\FilesUploaded")),
                RequestPath= "/FilesUploaded",
                

            });
            app.UseStaticFiles(new StaticFileOptions { 
                FileProvider= new PhysicalFileProvider(System.IO.Path.Combine(env.ContentRootPath, "..\\FilesUploadedTo")),
                RequestPath= "/FilesUploadedTo",
        
            });
            //Exceptions
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(System.IO.Path.Combine(env.ContentRootPath, "..\\Exceptions")),
                RequestPath = "/Exceptions",

            });
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(System.IO.Path.Combine(env.ContentRootPath, "..\\LoadedData")),
                RequestPath = "/LoadedData",

            });
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(System.IO.Path.Combine(env.ContentRootPath, "..\\FilesParsed")),
                RequestPath = "/FilesParsed",

            });
            app.UseFileServer(new FileServerOptions
            {
                FileProvider = new PhysicalFileProvider(
                  Path.Combine(Directory.GetCurrentDirectory(), "staticfiles")),
                RequestPath = "/staticfiles",
                EnableDefaultFiles = true
            });
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
              //  endpoints.MapControllerRoute(name: "default", pattern: "Home",
                //               defaults: new { controller = "Home", Action = "Index" });

              //  endpoints.MapControllerRoute(name:"default",pattern: "{controller}/{action}");
            });
        

            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = _pathParsed;
            watcher.NotifyFilter = NotifyFilters.Attributes
                                           | NotifyFilters.CreationTime
                                           | NotifyFilters.DirectoryName
                                           | NotifyFilters.FileName
                                           | NotifyFilters.LastAccess
                                           | NotifyFilters.LastWrite
                                           | NotifyFilters.Security
                                           | NotifyFilters.Size;
            watcher.Changed += Final_project.Controllers.ParserController.OnChanged;
            watcher.Filter = "*.txt";
            watcher.EnableRaisingEvents = true;

            FileSystemWatcher watcherLoader = new FileSystemWatcher();
            watcherLoader.Path = _pathParsedone;
            watcherLoader.NotifyFilter = NotifyFilters.Attributes
                               | NotifyFilters.CreationTime
                               | NotifyFilters.DirectoryName
                               | NotifyFilters.FileName
                               | NotifyFilters.LastAccess
                               | NotifyFilters.LastWrite
                               | NotifyFilters.Security
                               | NotifyFilters.Size;

            watcherLoader.Created += Final_project.Controllers.LoaderController.OnCreated;
            // watcherLoader.Changed += Final_project.Controllers.LoaderController.OnChanged;
            watcherLoader.Filter = "*.txt";
            watcherLoader.EnableRaisingEvents = true;


            String[] filesPresent = Directory.GetFiles(_pathParsed, "*.txt", SearchOption.AllDirectories);

              int i = 0;
               foreach (string ThisFile in filesPresent)
               {


                   _parserservice.stratetgyparsing(ThisFile, _pathParsedone, _pathParsedMoved);
                   i++;


               }
           
     //       int i = 0;
       //     foreach (string ThisFile in filesPresent)
         //   {


           //     _valu.par(ThisFile);
             //   i++;


            //}



        }
    
    }
}
