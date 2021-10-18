using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fixer_MVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IHostBuilder hostBuilder = CreateHostBuilder(args);

            IHost host = hostBuilder.Build();

            host.Run();

        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>().
                    //UseKestrel();
                    UseIISIntegration();
                });
    }
}
