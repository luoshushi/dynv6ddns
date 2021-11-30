﻿using System.Diagnostics;
using System.Threading.Tasks;
using System.Net;
using System.Threading;
using System;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Formatting.Compact;

namespace dynv6ddns
{
    class Program
    {
        private static WebClient webClient = new WebClient();
        private static string localIp = "127.0.0.1";
        private static Timer timer = null;
        private static IConfigurationRoot configurationRoot => new ConfigurationBuilder().AddJsonFile("appsettings.json", false, true).Build();
        static void Main(string[] args)
        {
            InitLogs();
            Log.Information("Program start at {datetime}", DateTime.Now);
            var autoEvent = new AutoResetEvent(false);
            timer = new Timer(Run, autoEvent, 0, 10 * 60 * 1000);
            autoEvent.WaitOne();
        }
        private static void InitLogs()
        {
            Serilog.Log.Logger = new LoggerConfiguration()
            .Enrich.WithProperty("ProgramName","dyddns")
            .WriteTo.Console( outputTemplate:"[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u4}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.File("logs/logs_.txt", rollingInterval: RollingInterval.Day).CreateLogger();
        }
        private static void Run(object state)
        {
            try
            {
                string token = configurationRoot["token"];
                string hostname = configurationRoot["hostname"];
                string[] hostnames = hostname.Split(',');
                string ipaddress = webClient.DownloadString("https://api.ipify.org");
                if (localIp != ipaddress)
                {
                    Log.Information("ip is change to {publicIp}", ipaddress);
                    foreach (var item in hostnames)
                    {
                        string res = webClient.DownloadString($"http://dynv6.com/api/update?hostname={item}&token={token}&ipv4={ipaddress}");
                        Log.Information("ip:{ipaddress} domain:{item} result:{res}", ipaddress, item, res);
                    }
                    localIp = ipaddress;

                }
            }
            catch (Exception ex)
            {
                Log.Error(ex,ex.ToString());
            }
        }
    }
}