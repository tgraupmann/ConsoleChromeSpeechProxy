using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleChromeSpeechProxy
{
    class App : IForm
    {
        private Server _mServer = null;

        private bool _mIsRunning = true;

        public App()
        {
            _mServer = new Server(this);

            _mServer.StartProxy();
        }

        public void AppendStatusText(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }
            Console.WriteLine("StatusText: {0}", text);
        }

        public void CloseForm()
        {
            Console.WriteLine("Close App");
            _mIsRunning = false;
            _mServer.StopProxy();
        }

        public void DisplayUIStartProxy()
        {
        }

        public void DisplayUIStopProxy()
        {
        }

        public bool IsRunning()
        {
            return _mIsRunning;
        }

        public void RunOnMainThread(Action action)
        {
            action.Invoke();
        }

        public void SetChromeConnectedText(string text)
        {
            //Console.WriteLine("ChromeConnected: {0}", text);
        }

        public void SetPortText(string text)
        {
            Console.WriteLine("Port: {0}", text);
        }

        public void SetStatusText(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }
            Console.WriteLine("StatusText: {0}", text);
        }

        public void SetUnityConnectedText(string text)
        {
            //Console.WriteLine("UnityConnected: {0}", text);
        }

        public void StopProxy()
        {
            _mServer.StopProxy();
        }
    }
}
