using System;
using System.Collections.Generic;
using System.Diagnostics;
using DotWriterServer.Middlewares;
using DotWriterServer.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using MovementManager;
using MovementManager.Service;

namespace DotWriterServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureLogger();

            services.Configure<Setting>(Configuration.GetSection("DotWriterSetting"));

            INotificationService notifService = new NotificationService("movementnotif", 2000000)
            {
                WorkerApplication = "..\\MovementManager\\WorkerService\\MovementManagerWorker.exe"
            };
            services.AddSingleton(notifService);
            services.AddSingleton<IActuatorService, ActuatorService>();
            services.AddSingleton<IDotWriterService, DotWriterService>();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "DotWriterServer", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "DotWriterServer v1"));
            }

            app.UseHttpsRedirection();
            app.UseMiddleware<CustomFilterMiddleware>();
            app.UseRouting();

            // app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseExceptionHandler("/error");

        }

        private static void ConfigureLogger()
        {
            //File
            // _logFile = File.Create($"Logs/RoverSimulation-Log-{DateTime.Now.ToString("yyyy-MM-dd_HH.mm.ss")}.log");
            // TextWriterTraceListener myTextListener = new TextWriterTraceListener(_logFile);
            // Trace.Listeners.Add(myTextListener);

            //Debug
            TextWriterTraceListener writer = new TextWriterTraceListener(System.Console.Out);
            Trace.Listeners.Add(writer);
        }
    }
}
