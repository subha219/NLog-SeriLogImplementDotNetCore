using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using NLog.Web;

namespace SerilogLogging
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logger = NLog.Web.NLogBuilder.ConfigureNLog("NLog.config").GetCurrentClassLogger();
            ConfigureLog();
            try
            {
                logger.Debug("Application Started....");
                Log.Information("Application Started");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception during execution.");
                Log.Error(ex.Message);
            }
            finally
            {
                NLog.LogManager.Shutdown();
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    //Uncomment under line for serilog 
                    //webBuilder.UseStartup<Startup>().UseSerilog();
                    webBuilder.UseStartup<Startup>();
                }).ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
            })
            .UseNLog();

        public static void ConfigureLog()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console(outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm} {MachineName} {ThreadId} {Message} {Exception:1} {NewLine}")
                .WriteTo.File(@"log.txt", outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm} {MachineName} {ThreadId} {Message} {Exception:1} {NewLine}")
                .Enrich.WithThreadId().Enrich.WithMachineName()
                .CreateLogger();
        }
    }
}
