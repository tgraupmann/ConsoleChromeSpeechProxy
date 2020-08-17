using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleChromeSpeechProxy
{
    interface IForm
    {
        void CloseForm();

        void AppendStatusText(string text);

        void DisplayUIStartProxy();
        void DisplayUIStopProxy();

        void RunOnMainThread(Action action);

        void SetChromeConnectedText(string text);
        void SetPortText(string text);
        void SetStatusText(string text);
        void SetUnityConnectedText(string text);

    }
}
