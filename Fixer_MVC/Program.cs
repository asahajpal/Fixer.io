using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using ATGCustReg_MVC.DataModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore;
using Microsoft.Extensions.Configuration;

namespace ATGCustReg_MVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IHostBuilder hostBuilder = CreateHostBuilder(args);

            IHost host = hostBuilder.Build();

            CreateDbIfNotExists(host);

            host.Run();
        }

        private static void CreateDbIfNotExists(IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    //Get a database context instance from the dependency injection container
                    var context = services.GetRequiredService<ExchangeRateContext>();
                    // Call the DbInitializer.Initialize method
                    DbInitializer.Initialize(context);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred creating the DB.");
                }
            }
        }


        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return 
            Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>().
                    //UseKestrel();
                    UseIISIntegration();
            });

        }
    }

    internal class EmptyStartup
    {
        public void Configure() {}
    }
}
