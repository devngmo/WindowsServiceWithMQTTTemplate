using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace WindowsServiceWithMQTTTemplate
{
    internal static class Program
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            LogManager.LoadConfiguration("NLog.config");
            logger.Info("Enter Program::main()");

            TheAwesomeService service = new TheAwesomeService();
            if (Environment.UserInteractive)
            {
                logger.Info("Run the Service in Debug Mode with Visual Studio");
                service.StartDebug(new string[] { "localhost", "1883" });
                service.WaitForPubSubConnection();
                logger.Info("Stop the Service in Debug Mode with Visual Studio");
                service.pingPubSubAsync().Wait();
                service.Stop();
            }
            else
            {
                ServiceBase.Run(service);
            }

            LogManager.Shutdown();

        }
    }
}
