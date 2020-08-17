using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace ChromeSpeechProxySvc
{
    public partial class Service1 : ServiceBase
    {
        private static bool _sWaitForExit = true;
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _sWaitForExit = true;
        }

        protected override void OnStop()
        {
            _sWaitForExit = false;
        }
    }
}
