using System.Diagnostics;
using System.Threading.Tasks;
using System.Net;
using System.Threading;
using System;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Formatting.Compact;
using System.Collections.Generic;
using System.ComponentModel;

namespace dynv6ddns
{
    public enum ProtectionType
    {
        [Description("比赛详情")]
        MatchDetail = 1,
        [Description("直播")]
        Live = 2,
        [Description("集锦回放")]
        Video = 4
    }
    class Program
    {
        private static WebClient webClient = new WebClient();
        private static string localIp = "127.0.0.1";
        private static Timer timer = null;

        static void Main(string[] args)
        {
            InitLogs();
            Log.Information("Program start at {datetime}", DateTime.Now);
            var autoEvent = new AutoResetEvent(false);
            timer = new Timer(Run, autoEvent, 0, 10 * 1000 * 60);

            autoEvent.WaitOne();

        }



        private static void InitLogs()
        {
            Serilog.Log.Logger = new LoggerConfiguration()
            .Enrich.WithProperty("ProgramName", "dyddns")
            .WriteTo.Console(outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u4}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.File("logs/logs_.txt", rollingInterval: RollingInterval.Day, flushToDiskInterval: TimeSpan.Zero).CreateLogger();
        }
        private static void Run(object state)
        {
            try
            {
                string token = Environment.GetEnvironmentVariable("token");// string.Empty;
                string hostname = Environment.GetEnvironmentVariable("hostname");
                if (string.IsNullOrEmpty(token))
                {
                    Log.Error("未设置token环境变量");
                    return;
                }
                if (string.IsNullOrEmpty(hostname))
                {
                    Log.Error("未设置hostname环境变量");
                    return;
                }
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
                Log.Error(ex, ex.ToString());
            }
        }
    }
}
