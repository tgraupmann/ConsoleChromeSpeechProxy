// Ref: https://stackoverflow.com/questions/9021075/how-to-create-an-installer-for-a-net-windows-service-using-visual-studio

using System;
using System.ServiceProcess;

namespace ChromeSpeechProxySvc
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new Service1()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
