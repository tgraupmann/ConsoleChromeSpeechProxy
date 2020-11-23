﻿using System;
using System.IO;
using System.Threading;

namespace ConsoleChromeSpeechProxy
{
    class MainClass
    {
        private static App _mApp = null;

        public static void Main(string[] args)
        {
            Console.WriteLine("App: {0}", AppDomain.CurrentDomain.FriendlyName);
            Console.WriteLine("OSVersion: {0}", Environment.OSVersion.VersionString);

            Server.SetAppName(AppDomain.CurrentDomain.FriendlyName);

            if (args.Length > 0)
            {
                string strPort = args[0];
                int port;
                if (int.TryParse(strPort, out port))
                {
                    Server.SetProxyPort(port);
                }
            }
            string[] commandArgs = Environment.GetCommandLineArgs();
            if (commandArgs.Length > 0)
            {
                FileInfo fi = new FileInfo(commandArgs[0]);
                string installDir = fi.DirectoryName;
                Server.SetInstallDirectory(installDir);
            }

            _mApp = new App();
            while (_mApp.IsRunning())
            {
                Thread.Sleep(1);
            }
        }
    }
}
