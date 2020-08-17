// Ref: https://stackoverflow.com/questions/9021075/how-to-create-an-installer-for-a-net-windows-service-using-visual-studio

using ConsoleChromeSpeechProxy;
using System;
using System.Threading;
using System.ServiceProcess;

namespace ChromeSpeechProxySvc
{
    static class Program
    {
        private static App _mApp = null;

        private static void AppSetup()
        {
            Console.WriteLine("App: {0}", AppDomain.CurrentDomain.FriendlyName);
            Console.WriteLine("OSVersion: {0}", Environment.OSVersion.VersionString);

            Server.SetAppName(AppDomain.CurrentDomain.FriendlyName);

            _mApp = new App();
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ThreadStart ts = new ThreadStart(AppSetup);
            Thread thread = new Thread(ts);
            thread.Start();
            

            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new Service1()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
