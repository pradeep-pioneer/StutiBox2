using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace StutiBox.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var envVar = Environment.GetEnvironmentVariable("DEVICE");
            var device = String.IsNullOrWhiteSpace(envVar)?"mac": envVar;
            Console.WriteLine($"[env:DEVICE]: {device}");
            Console.WriteLine($"StutiBox service starting up in '{Environment.CurrentDirectory}/'...");
			Console.WriteLine($"Detected Platform: {Environment.OSVersion.Platform.ToString()}");
			Console.WriteLine($"Version: {Environment.OSVersion.VersionString}");
			Console.WriteLine($"Host: {Environment.MachineName}");
			Console.WriteLine($"Processors: {Environment.ProcessorCount}");
            CreateHostBuilder(args,device).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args, string environment="pi") =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(options=>
                {
                    options.AddJsonFile(
                        environment=="pi"
                        ?"LibraryConfiguration.pi.json"
                        :"LibraryConfiguration.mac.json", optional: false, reloadOnChange: false);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>().UseUrls("http://::5000");
                });
    }
}
