using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WindowsServiceWithMQTTTemplate
{
    public partial class TheAwesomeService : ServiceBase
    {
        PubSubService pubSubService = new PubSubService();
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public TheAwesomeService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            logger.Info($"TheAwesomeService::OnStart");
            for (int i = 0;i < args.Length; i++)
            {
                logger.Info($"ARGS[{i}]={args[i]}");
            }
            pubSubService.initAsync(args[0], int.Parse(args[1])).Wait();

        }

        protected override void OnStop()
        {
            logger.Info($"TheAwesomeService::OnStop");
            pubSubService.StopAsync().Wait();
        }

        internal void StartDebug(string[] args)
        {
            OnStart(args);
        }

        internal async Task pingPubSubAsync()
        {
            logger.Info($"TheAwesomeService::pingPubSub...");
            await pubSubService.pingAsync();
            logger.Info($"TheAwesomeService::pingPubSub...Done");
        }

        internal void WaitForPubSubConnection()
        {
            while (true)
            {
                if (pubSubService.isConnected) return;
                Thread.Sleep(1000);
            }
        }
    }
}
