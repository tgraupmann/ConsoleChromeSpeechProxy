using ConsoleChromeSpeechProxy;
using System;
using System.ServiceProcess;
using System.Threading;

namespace ChromeSpeechProxySvc
{
    public partial class Service1 : ServiceBase
    {
        private App _mApp = null;

        public Service1()
        {
            InitializeComponent();
        }

        private void AppSetup()
        {
            Console.WriteLine("App: {0}", AppDomain.CurrentDomain.FriendlyName);
            Console.WriteLine("OSVersion: {0}", Environment.OSVersion.VersionString);

            Server.SetAppName(AppDomain.CurrentDomain.FriendlyName);

            _mApp = new App();
        }


        protected override void OnStart(string[] args)
        {
            ThreadStart ts = new ThreadStart(AppSetup);
            Thread thread = new Thread(ts);
            thread.Start();
        }

        protected override void OnStop()
        {
            _mApp.StopProxy();
            System.Environment.Exit(0);
        }
    }
}
