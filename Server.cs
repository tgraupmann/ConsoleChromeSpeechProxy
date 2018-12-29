using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;

namespace ConsoleChromeSpeechProxy
{
    class Server
    {
        const string APP_CMD = @"C:\Windows\System32\cmd.exe";
        const string APP_CHROME_WIN = @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe";

        const string APP_BASH_MAC = "/bin/bash";
        const string APP_CHROME_MAC = @"/Applications/Google\ Chrome.app/Contents/MacOS/Google\ Chrome";

        const string KEY_CHROME_SPEECH_PROXY = "CHROME_SPEECH_PROXY";
        const string KEY_APP_NAME = "appName";
        const string KEY_INSTALL_DIRECTORY = "installDirectory";
        const string KEY_PROXY_PORT = "proxyPort";

        const string PATH_ROOT = "/";
        const string PATH_CROSS_DOMAIN_POLICY = "/crossdomain.xml";
        const string PATH_PROXY_DATA = "/ProxyData";

        const string PATH_CLOSE_BROWSER_TAB = "/CloseBrowserTab";
        const string PATH_CLOSE_PROXY = "/CloseProxy";
        const string PATH_OPEN_BROWSER_TAB = "/OpenBrowserTab";
        const string PATH_SET_PROXY_PORT = "/SetProxyPort";

        const string PATH_SPEECH_DETECTION_START_RECOGNITION = "/SpeechDetectionStartRecognition";
        const string PATH_SPEECH_DETECTION_STOP_RECOGNITION = "/SpeechDetectionStopRecognition";
        const string PATH_SPEECH_DETECTION_ABORT = "/SpeechDetectionAbort";
        const string PATH_SPEECH_DETECTION_INIT = "/SpeechDetectionInit";
        const string PATH_SPEECH_DETECTION_GET_LANGUAGES = "/SpeechDetectionGetLanguages";
        const string PATH_SPEECH_DETECTION_GET_RESULT = "/SpeechDetectionGetResult";
        const string PATH_SPEECH_DETECTION_SET_LANGUAGE = "/SpeechDetectionSetLanguage";

        const string PATH_SPEECH_SYNTHESIS_CANCEL = "/SpeechSynthesisCancel";
        const string PATH_SPEECH_SYNTHESIS_CONNECT = "/SpeechSynthesisConnect";
        const string PATH_SPEECH_SYNTHESIS_CREATE_SPEECH_SYNTHESIS_UTTERANCE = "/SpeechSynthesisCreateSpeechSynthesisUtterance";
        const string PATH_SPEECH_SYNTHESIS_GET_VOICES = "/SpeechSynthesisGetVoices";
        const string PATH_SPEECH_SYNTHESIS_PROXY_ON_END = "/SpeechSynthesisProxyOnEnd";
        const string PATH_SPEECH_SYNTHESIS_PROXY_UTTERANCE = "/SpeechSynthesisProxyUtterance";
        const string PATH_SPEECH_SYNTHESIS_PROXY_VOICES = "/SpeechSynthesisProxyVoices";
        const string PATH_SPEECH_SYNTHESIS_SET_PITCH = "/SpeechSynthesisSetPitch";
        const string PATH_SPEECH_SYNTHESIS_SET_RATE = "/SpeechSynthesisSetRate";
        const string PATH_SPEECH_SYNTHESIS_SET_TEXT = "/SpeechSynthesisSetText";
        const string PATH_SPEECH_SYNTHESIS_SET_VOICE = "/SpeechSynthesisSetVoice";
        const string PATH_SPEECH_SYNTHESIS_SET_VOLUME = "/SpeechSynthesisSetVolume";
        const string PATH_SPEECH_SYNTHESIS_SPEAK = "/SpeechSynthesisSpeak";

        const string TOKEN_SPEECH_DETECTION_GET_LANGUAGES = "SpeechDetectionGetLanguages:";
        const string TOKEN_SPEECH_DETECTION_GET_LANGUAGES_CHUNK = "SpeechDetectionGetLanguagesChunk:";
        const string TOKEN_SPEECH_DETECTION_GET_LANGUAGES_CHUNK_FIRST = "SpeechDetectionGetLanguagesChunkFirst:";
        const string TOKEN_SPEECH_DETECTION_GET_LANGUAGES_CHUNK_LAST = "SpeechDetectionGetLanguagesChunkLast:";
        const string TOKEN_SPEECH_DETECTION_GET_RESULT = "SpeechDetectionGetResult:";
        const string TOKEN_SPEECH_DETECTION_INIT = "SpeechDetectionInit:";
        const string TOKEN_SPEECH_DETECTION_ON_END = "SpeechSynthesisOnEnd:";

        const string TOKEN_SPEECH_SYNTHESIS_CANCEL = "SpeechSynthesisCancel:";
        const string TOKEN_SPEECH_SYNTHESIS_CREATE_SPEECH_SYNTHESIS_UTTERANCE = "SpeechSynthesisCreateSpeechSynthesisUtterance:";
        const string TOKEN_SPEECH_SYNTHESIS_GET_VOICES = "SpeechSynthesisGetVoices:";
        const string TOKEN_SPEECH_SYNTHESIS_IDLE = "SpeechSynthesisIdle:";
        const string TOKEN_SPEECH_SYNTHESIS_SET_PITCH = "SpeechSynthesisSetPitch:";
        const string TOKEN_SPEECH_SYNTHESIS_SET_RATE = "SpeechSynthesisSetRate:";
        const string TOKEN_SPEECH_SYNTHESIS_SET_TEXT = "SpeechSynthesisSetText:";
        const string TOKEN_SPEECH_SYNTHESIS_SPEAK = "SpeechSynthesisSpeak:";

        /// <summary>
        /// Reference to form
        /// </summary>
        private IForm _mForm = null;

        private bool _mClosing = false;

        private HttpListener _mHttpListener = null;

        private Thread _mThread = null;

        private bool _mWaitForExit = true;


        private string _mWebGLSpeechDetectionPluginLanguages = null; //start as null

        private List<string> _mWebGLSpeechDetectionPluginResults = new List<string>();

        private List<string> _mWebGLSpeechDetectionPluginOnEnd = new List<string>();

        private List<string> _mPendingJavaScript = new List<string>();

        private StringBuilder _mStringBuilder = new StringBuilder();

        private List<int> _mWebGLSpeechSynthesisPluginUtterances = new List<int>();

        private List<string> _mWebGLSpeechSynthesisPluginVoices = new List<string>();

        private StringBuilder _mGetLanguagesChunks = new StringBuilder();


        public Server(IForm form)
        {
            _mForm = form;
        }


        private void AppendStatus(string msg, params object[] args)
        {
            if (_mClosing)
            {
                return;
            }

            string text = string.Format(msg, args) + Environment.NewLine;
            _mForm.AppendStatusText(text);
        }

        private void CloseApp()
        {
            if (_mClosing)
            {
                return;
            }

            _mForm.CloseForm();
        }

        public void CloseChrome()
        {
            DisconnectChrome();
            RunJavaScript("window.close()");
        }

        public void DetectedChrome()
        {
            if (!_mClosing)
            {
                _mForm.SetChromeConnectedText("Chrome: [Connected]");
            }
        }

        private void DetectedUnity()
        {
            if (_mClosing)
            {
                return;
            }

            _mForm.SetUnityConnectedText("Unity: [Connected]");
        }

        private void DisconnectChrome()
        {
            if (!_mClosing)
            {
                _mForm.SetChromeConnectedText("Chrome: [Disonnected]");
            }
        }

        private static string GetAppConfig()
        {
            string path = Path.Combine(GetAppDataFolder(), "app.config");
            return path;
        }

        private static JObject GetAppConfigJson()
        {
            SetupAppDataFolder();
            string path = GetAppConfig();
            JObject json = null;
            try
            {
                if (File.Exists(path))
                {
                    using (FileStream fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        using (StreamReader sr = new StreamReader(fs))
                        {
                            using (JsonTextReader jr = new JsonTextReader(sr))
                            {
                                json = (JObject)JToken.ReadFrom(jr);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {

            }
            if (null == json)
            {
                json = new JObject();
            }
            return json;
        }

        private static string GetAppDataFolder()
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), KEY_CHROME_SPEECH_PROXY);
            return path;
        }

        private string GetPendingJavaScript()
        {
            while (_mPendingJavaScript.Count > 0)
            {
                _mStringBuilder.AppendLine(_mPendingJavaScript[0]);
                _mPendingJavaScript.RemoveAt(0);
            }

            string result = _mStringBuilder.ToString();

            if (_mStringBuilder.Length > 0)
            {
                _mStringBuilder.Remove(0, _mStringBuilder.Length);
            }

            return result;
        }

        public static int GetProxyPort()
        {
            int port = 5000;

            JObject json = GetAppConfigJson();
            if (json[KEY_PROXY_PORT] != null &&
                json[KEY_PROXY_PORT].Type == JTokenType.Integer)
            {
                port = (int)json[KEY_PROXY_PORT];
                Console.WriteLine("Use port: {0}", port);
            }
            else
            {
                SetProxyPort(port);
            }

            return port;
        }

        /// <summary>
        /// Handle requests with messages from the proxy
        /// </summary>
        /// <param name="request"></param>
        private void HandleProxyData(string request)
        {
            if (string.IsNullOrEmpty(request))
            {
                return;
            }

            else if (request.StartsWith(TOKEN_SPEECH_SYNTHESIS_IDLE))
            {
            }

            else if (request.StartsWith(TOKEN_SPEECH_DETECTION_GET_LANGUAGES_CHUNK_FIRST))
            {
                if (_mGetLanguagesChunks.Length > 0)
                {
                    _mGetLanguagesChunks.Remove(0, _mGetLanguagesChunks.Length);
                }
                string chunk = request.Substring(TOKEN_SPEECH_DETECTION_GET_LANGUAGES_CHUNK_FIRST.Length);
                if (!string.IsNullOrEmpty(chunk))
                {
                    _mGetLanguagesChunks.Append(chunk);
                }
            }

            else if (request.StartsWith(TOKEN_SPEECH_DETECTION_GET_LANGUAGES_CHUNK_LAST))
            {
                string chunk = request.Substring(TOKEN_SPEECH_DETECTION_GET_LANGUAGES_CHUNK_LAST.Length);
                if (!string.IsNullOrEmpty(chunk))
                {
                    _mGetLanguagesChunks.Append(chunk);
                    string encoded = _mGetLanguagesChunks.ToString();
                    string decoded = HttpUtility.UrlDecode(encoded);
                    _mWebGLSpeechDetectionPluginLanguages = decoded;
                }
            }

            else if (request.StartsWith(TOKEN_SPEECH_DETECTION_GET_LANGUAGES_CHUNK))
            {
                string chunk = request.Substring(TOKEN_SPEECH_DETECTION_GET_LANGUAGES_CHUNK.Length);
                if (!string.IsNullOrEmpty(chunk))
                {
                    _mGetLanguagesChunks.Append(chunk);
                }
            }

            else if (request.StartsWith(TOKEN_SPEECH_DETECTION_GET_LANGUAGES))
            {
                string jsonData = request.Substring(TOKEN_SPEECH_DETECTION_GET_LANGUAGES.Length);
                if (!string.IsNullOrEmpty(jsonData))
                {
                    string decoded = HttpUtility.UrlDecode(jsonData);
                    _mWebGLSpeechDetectionPluginLanguages = decoded;
                }
            }            

            else if (request.StartsWith(TOKEN_SPEECH_DETECTION_GET_RESULT))
            {
                string message = request.Substring(TOKEN_SPEECH_DETECTION_GET_RESULT.Length);
                string decoded = HttpUtility.UrlDecode(message);
                _mWebGLSpeechDetectionPluginResults.Add(decoded);
            }

            else if (request.StartsWith(TOKEN_SPEECH_DETECTION_INIT))
            {
                _mWebGLSpeechDetectionPluginResults.Clear(); //clear previous results
                _mWebGLSpeechDetectionPluginOnEnd.Clear(); //clear previous results
                RunJavaScript("console.log(\"Init Complete\")");
            }

            else if (request.StartsWith(TOKEN_SPEECH_SYNTHESIS_CANCEL))
            {
            }

            else if (request.StartsWith(TOKEN_SPEECH_DETECTION_ON_END))
            {
                string message = request.Substring(TOKEN_SPEECH_DETECTION_ON_END.Length);
                string decoded = HttpUtility.UrlDecode(message);
                _mWebGLSpeechDetectionPluginOnEnd.Add(decoded);
            }

            else if (request.StartsWith(TOKEN_SPEECH_SYNTHESIS_CREATE_SPEECH_SYNTHESIS_UTTERANCE))
            {
                string message = request.Substring(TOKEN_SPEECH_SYNTHESIS_CREATE_SPEECH_SYNTHESIS_UTTERANCE.Length);
                int index;
                if (int.TryParse(message, out index))
                {
                    _mWebGLSpeechSynthesisPluginUtterances.Add(index);
                }
            }

            else if (request.StartsWith(TOKEN_SPEECH_SYNTHESIS_GET_VOICES))
            {
                string jsonData = request.Substring(TOKEN_SPEECH_SYNTHESIS_GET_VOICES.Length);
                if (!string.IsNullOrEmpty(jsonData))
                {
                    string decoded = HttpUtility.UrlDecode(jsonData);
                    _mWebGLSpeechSynthesisPluginVoices.Add(decoded);
                }
            }

            else if (request.StartsWith(TOKEN_SPEECH_SYNTHESIS_SET_PITCH))
            {
            }

            else if (request.StartsWith(TOKEN_SPEECH_SYNTHESIS_SET_RATE))
            {
            }

            else if (request.StartsWith(TOKEN_SPEECH_SYNTHESIS_SET_TEXT))
            {
            }

            else if (request.StartsWith(TOKEN_SPEECH_SYNTHESIS_SPEAK))
            {
            }
        }

        public bool IsClosing()
        {
            return _mClosing;
        }

        public static bool IsMacOS()
        {
            return Environment.OSVersion.VersionString.ToUpper().StartsWith("UNIX");
        }

        public void OpenChrome()
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();

            string uri = string.Format("http://localhost:{0}", GetProxyPort());
            Console.WriteLine("Listening: {0}", uri);

            if (IsMacOS())
            {
				string args = string.Format("-c \"{0} {1}\"", APP_CHROME_MAC, uri);
				process.StartInfo = new System.Diagnostics.ProcessStartInfo(APP_BASH_MAC,
					args);
				process.Start();

				Console.WriteLine("{0} {1}", APP_BASH_MAC, args);
            }
            else // Windows
            {
				string args = string.Format("/c start \"\" \"{0}\" {1}", APP_CHROME_WIN, uri);
				process.StartInfo = new System.Diagnostics.ProcessStartInfo(APP_CMD,
					args);
				process.Start();
            }
		}

        private void RestartWorker()
        {
            SetStatus("Stopping proxy...");
            Thread.Sleep(1000);
            _mForm.DisplayUIStopProxy();
            StopProxy();
            Thread.Sleep(3000);
            SetPortText();
            SetStatus("Starting proxy...");
            Thread.Sleep(1000);
            _mForm.DisplayUIStartProxy();
            StartProxy();
        }

        private void RunJavaScript(string js)
        {
            _mPendingJavaScript.Add(js);
        }

        private static void SaveAppConfigJson(JObject json)
        {
            string path = GetAppConfig();
            try
            {
                using (FileStream fs = File.Open(path, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                {
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        if (json != null)
                        {
                            string contents = json.ToString();
                            sw.Write(contents);
                        }
                        sw.Flush();
                    }
                }
            }
            catch (Exception)
            {
                Console.Error.WriteLine("Failed to save app config!");
            }
        }

		public static void SetAppName(string appName)
		{
			SetupAppDataFolder();
			JObject json = GetAppConfigJson();
			json[KEY_APP_NAME] = appName;
			SaveAppConfigJson(json);
		}

        public void SetClosing()
        {
            _mClosing = true;
        }

        public static void SetInstallDirectory(string installDir)
        {
            SetupAppDataFolder();
            JObject json = GetAppConfigJson();
            json[KEY_INSTALL_DIRECTORY] = installDir;
            SaveAppConfigJson(json);
        }

        private void SetPortText()
        {
            string text = Server.GetProxyPort().ToString();
            _mForm.SetPortText(text);
        }

        public static void SetProxyPort(int port)
        {
            Console.WriteLine("Set port: {0}", port);
            SetupAppDataFolder();
            JObject json = GetAppConfigJson();
            json[KEY_PROXY_PORT] = port;
            SaveAppConfigJson(json);
        }

        private void SetStatus(string msg, params object[] args)
        {
            if (!_mClosing)
            {
                _mForm.SetStatusText("Status:");
            }
            AppendStatus(Environment.NewLine);
            AppendStatus(msg, args);
            AppendStatus(Environment.NewLine);
        }

        private static void SetupAppDataFolder()
        {
            string path = GetAppDataFolder();
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }
            catch (Exception)
            {
                
            }
        }

        public void StartProxy()
        {
            SetStatus("Starting Proxy...");

            try
            {
                _mWaitForExit = true;

                int port = GetProxyPort();

                string uri = string.Format("http://localhost:{0}", port);
                Console.WriteLine("Listening: {0}", uri);

                _mHttpListener = new HttpListener();

                _mHttpListener.Prefixes.Add(string.Format("http://*:{0}/", port));
                _mHttpListener.Start();

                ThreadStart ts = new ThreadStart(WorkerThread);
                _mThread = new Thread(ts);
                _mThread.Start();

                AppendStatus("Proxy Started!");
            }
            catch (Exception ex)
            {
                AppendStatus("Failed to start listener: {0}", ex);
            }
        }

        public void StopProxy()
        {
            SetStatus("Stopping Proxy...");

            _mWaitForExit = false;

            try
            {
                _mHttpListener.Abort();
            }
            catch (Exception)
            {

            }

            try
            {
                _mHttpListener.Stop();
            }
            catch (Exception)
            {

            }

            if (!_mClosing)
            {
                _mForm.SetChromeConnectedText("Chrome: [Not Connected]");
                _mForm.SetUnityConnectedText("Unity: [Not Connected]");
            }

            AppendStatus("Proxy Stopped!");
        }

        private void WorkerThread()
        {
            bool closeApp = false;
            // keep listening and send a response
            while (_mWaitForExit)
            {
                if (null != _mHttpListener)
                {
                    HttpListenerContext context = null;
                    try
                    {
                        context = _mHttpListener.GetContext();

                        string response = "";

                        if (string.IsNullOrEmpty(context.Request.Url.LocalPath))
                        {
                        }

                        else if (context.Request.Url.LocalPath.EndsWith(PATH_ROOT))
                        {
                            _mWebGLSpeechDetectionPluginResults.Clear(); //clear previous results
                            _mWebGLSpeechDetectionPluginOnEnd.Clear(); //clear previous results
                            DetectedChrome();
                            try
                            {
                                using (System.IO.StreamReader sr = new System.IO.StreamReader("proxy.html"))
                                {
                                    response = sr.ReadToEnd().Replace("__PROXY_PORT__", GetProxyPort().ToString());
                                }
                            }
                            catch (Exception)
                            {

                            }
                        }

                        else if (context.Request.Url.LocalPath.EndsWith(PATH_CROSS_DOMAIN_POLICY))
                        {
                            response = "<cross-domain-policy>" + Environment.NewLine;
                            response += "<allow-access-from domain=\"*\"/>" + Environment.NewLine;
                            response += "</cross-domain-policy>" + Environment.NewLine;
                        }

                        else if (context.Request.Url.LocalPath.EndsWith(PATH_PROXY_DATA))
                        {
                            try
                            {
                                DetectedChrome();
                                System.Collections.Specialized.NameValueCollection parameters = HttpUtility.ParseQueryString(context.Request.Url.Query);
                                string encodedString = parameters["message"];
                                if (!string.IsNullOrEmpty(encodedString))
                                {
                                    byte[] data = Convert.FromBase64String(encodedString);
                                    string request = Encoding.UTF8.GetString(data);
                                    HandleProxyData(request);
                                }
                            }
                            catch (Exception e)
                            {
                                Console.Error.WriteLine(e);
                            }

                            response = GetPendingJavaScript();
                        }

                        else if (context.Request.Url.LocalPath.EndsWith(PATH_CLOSE_BROWSER_TAB))
                        {
                            DetectedUnity();
                            CloseChrome();
                        }

                        else if (context.Request.Url.LocalPath.EndsWith(PATH_CLOSE_PROXY))
                        {
                            closeApp = true; //close app after response is sent
                        }

                        else if (context.Request.Url.LocalPath.EndsWith(PATH_OPEN_BROWSER_TAB))
                        {
                            DetectedUnity();
                            _mPendingJavaScript.Clear();
                            OpenChrome();
                        }

                        else if (context.Request.Url.LocalPath.EndsWith(PATH_SET_PROXY_PORT))
                        {
                            DetectedUnity();
                            System.Collections.Specialized.NameValueCollection parameters = HttpUtility.ParseQueryString(context.Request.Url.Query);
                            string strPort = parameters["port"];
                            int port;
                            if (int.TryParse(strPort, out port))
                            {
                                SetProxyPort(port);
                                ThreadStart ts = new ThreadStart(RestartWorker);
                                Thread thread = new Thread(ts);
                                thread.Start();
                            }
                        }

                        else if (context.Request.Url.LocalPath.EndsWith(PATH_SPEECH_DETECTION_GET_RESULT))
                        {
                            DetectedUnity();
                            if (_mWebGLSpeechDetectionPluginResults.Count > 0)
                            {
                                response = _mWebGLSpeechDetectionPluginResults[0];
                                _mWebGLSpeechDetectionPluginResults.RemoveAt(0);
                            }
                        }

                        else if (context.Request.Url.LocalPath.EndsWith(PATH_SPEECH_DETECTION_INIT))
                        {
                            _mWebGLSpeechDetectionPluginResults.Clear(); //clear previous results
                            DetectedUnity();
                        }

                        else if (context.Request.Url.LocalPath.EndsWith(PATH_SPEECH_DETECTION_START_RECOGNITION))
                        {
                            DetectedUnity();
                            RunJavaScript("WebGLSpeechDetectionPlugin.Start()");
                        }

                        else if (context.Request.Url.LocalPath.EndsWith(PATH_SPEECH_DETECTION_STOP_RECOGNITION))
                        {
                            DetectedUnity();
                            RunJavaScript("WebGLSpeechDetectionPlugin.Stop()");
                        }

                        else if (context.Request.Url.LocalPath.EndsWith(PATH_SPEECH_DETECTION_ABORT))
                        {
                            _mWebGLSpeechDetectionPluginResults.Clear(); //clear previous results
                            DetectedUnity();
                            RunJavaScript("WebGLSpeechDetectionPlugin.Abort()");
                        }

                        else if (context.Request.Url.LocalPath.EndsWith(PATH_SPEECH_DETECTION_GET_LANGUAGES))
                        {
                            DetectedUnity();
                            if (null == _mWebGLSpeechDetectionPluginLanguages)
                            {
                                RunJavaScript("WebGLSpeechDetectionPlugin.GetLanguages()");
                            }
                            else
                            {
                                response = _mWebGLSpeechDetectionPluginLanguages;
                            }
                        }

                        else if (context.Request.Url.LocalPath.EndsWith(PATH_SPEECH_DETECTION_SET_LANGUAGE))
                        {
                            _mWebGLSpeechDetectionPluginResults.Clear(); //clear previous results
                            DetectedUnity();
                            System.Collections.Specialized.NameValueCollection parameters = HttpUtility.ParseQueryString(context.Request.Url.Query);
                            string lang = parameters["lang"];
                            if (null != lang)
                            {
                                RunJavaScript(string.Format("WebGLSpeechDetectionPlugin.SetLanguage(\"{0}\")", lang));
                            }
                        }

                        else if (context.Request.Url.LocalPath.EndsWith(PATH_SPEECH_SYNTHESIS_CANCEL))
                        {
                            DetectedUnity();
                            RunJavaScript(string.Format("WebGLSpeechSynthesisPlugin.Cancel()"));
                        }

                        else if (context.Request.Url.LocalPath.EndsWith(PATH_SPEECH_SYNTHESIS_CONNECT))
                        {
                            DetectedUnity();
                            _mWebGLSpeechSynthesisPluginUtterances.Clear();
                        }

                        else if (context.Request.Url.LocalPath.EndsWith(PATH_SPEECH_SYNTHESIS_CREATE_SPEECH_SYNTHESIS_UTTERANCE))
                        {
                            DetectedUnity();
                            RunJavaScript(string.Format("WebGLSpeechSynthesisPlugin.CreateSpeechSynthesisUtterance()"));
                        }

                        else if (context.Request.Url.LocalPath.EndsWith(PATH_SPEECH_SYNTHESIS_GET_VOICES))
                        {
                            DetectedUnity();
                            RunJavaScript(string.Format("WebGLSpeechSynthesisPlugin.GetVoices()"));
                        }

                        else if (context.Request.Url.LocalPath.EndsWith(PATH_SPEECH_SYNTHESIS_PROXY_ON_END))
                        {
                            DetectedUnity();
                            if (_mWebGLSpeechDetectionPluginOnEnd.Count > 0)
                            {
                                response = _mWebGLSpeechDetectionPluginOnEnd[0];
                                _mWebGLSpeechDetectionPluginOnEnd.RemoveAt(0);
                            }
                        }

                        else if (context.Request.Url.LocalPath.EndsWith(PATH_SPEECH_SYNTHESIS_PROXY_UTTERANCE))
                        {
                            DetectedUnity();
                            if (_mWebGLSpeechSynthesisPluginUtterances.Count > 0)
                            {
                                response = _mWebGLSpeechSynthesisPluginUtterances[0].ToString();
                                _mWebGLSpeechSynthesisPluginUtterances.RemoveAt(0);
                            }
                        }

                        else if (context.Request.Url.LocalPath.EndsWith(PATH_SPEECH_SYNTHESIS_PROXY_VOICES))
                        {
                            DetectedUnity();
                            if (_mWebGLSpeechSynthesisPluginVoices.Count > 0)
                            {
                                response = _mWebGLSpeechSynthesisPluginVoices[0];
                                _mWebGLSpeechSynthesisPluginVoices.RemoveAt(0);
                            }
                        }

                        else if (context.Request.Url.LocalPath.EndsWith(PATH_SPEECH_SYNTHESIS_SET_PITCH))
                        {
                            DetectedUnity();
                            RunJavaScript(string.Format("WebGLSpeechSynthesisPlugin.SetUtterancePitch(0, 0)"));
                            System.Collections.Specialized.NameValueCollection parameters = HttpUtility.ParseQueryString(context.Request.Url.Query);
                            string utterance = parameters["utterance"];
                            int index;
                            string strPitch = parameters["pitch"];
                            float pitch;
                            if (int.TryParse(utterance, out index) &&
                                float.TryParse(strPitch, out pitch))
                            {
                                RunJavaScript(string.Format("WebGLSpeechSynthesisPlugin.SetUtterancePitch({0}, {1})", index, pitch));
                            }
                        }

                        else if (context.Request.Url.LocalPath.EndsWith(PATH_SPEECH_SYNTHESIS_SET_RATE))
                        {
                            DetectedUnity();
                            System.Collections.Specialized.NameValueCollection parameters = HttpUtility.ParseQueryString(context.Request.Url.Query);
                            string utterance = parameters["utterance"];
                            int index;
                            string strRate = parameters["rate"];
                            float rate;
                            if (int.TryParse(utterance, out index) &&
                                float.TryParse(strRate, out rate))
                            {
                                RunJavaScript(string.Format("WebGLSpeechSynthesisPlugin.SetUtteranceRate({0}, {1})", index, rate));
                            }
                        }

                        else if (context.Request.Url.LocalPath.EndsWith(PATH_SPEECH_SYNTHESIS_SET_TEXT))
                        {
                            DetectedUnity();
                            System.Collections.Specialized.NameValueCollection parameters = HttpUtility.ParseQueryString(context.Request.Url.Query);
                            string utterance = parameters["utterance"];
                            int index;
                            if (int.TryParse(utterance, out index))
                            {
                                string encoded = parameters["text"];
                                string text;
                                if (string.IsNullOrEmpty(encoded))
                                {
                                    text = string.Empty;
                                }
                                else
                                {
                                    byte[] decodedBytes = Convert.FromBase64String(encoded);
                                    text = UTF8Encoding.UTF8.GetString(decodedBytes);
                                }
                                RunJavaScript(string.Format("WebGLSpeechSynthesisPlugin.SetUtteranceText({0}, \"{1}\")", index, text));
                            }
                        }

                        else if (context.Request.Url.LocalPath.EndsWith(PATH_SPEECH_SYNTHESIS_SET_VOICE))
                        {
                            DetectedUnity();
                            System.Collections.Specialized.NameValueCollection parameters = HttpUtility.ParseQueryString(context.Request.Url.Query);
                            string utterance = parameters["utterance"];
                            int index;
                            if (int.TryParse(utterance, out index))
                            {
                                string encoded = parameters["voice"];
                                string voice;
                                if (string.IsNullOrEmpty(encoded))
                                {
                                    voice = string.Empty;
                                }
                                else
                                {
                                    byte[] decodedBytes = Convert.FromBase64String(encoded);
                                    voice = UTF8Encoding.UTF8.GetString(decodedBytes);
                                }
                                RunJavaScript(string.Format("WebGLSpeechSynthesisPlugin.SetUtteranceVoice({0}, \"{1}\")", index, voice));
                            }
                        }

                        else if (context.Request.Url.LocalPath.EndsWith(PATH_SPEECH_SYNTHESIS_SET_VOLUME))
                        {
                            DetectedUnity();
                            System.Collections.Specialized.NameValueCollection parameters = HttpUtility.ParseQueryString(context.Request.Url.Query);
                            string utterance = parameters["utterance"];
                            int index;
                            if (int.TryParse(utterance, out index))
                            {
                                string strVolume = parameters["volume"];
                                float volume;
                                if (float.TryParse(strVolume, out volume))
                                {
                                    RunJavaScript(string.Format("WebGLSpeechSynthesisPlugin.SetUtteranceVolume({0}, {1})", index, volume));
                                }
                            }
                        }

                        else if (context.Request.Url.LocalPath.EndsWith(PATH_SPEECH_SYNTHESIS_SPEAK))
                        {
                            DetectedUnity();
                            System.Collections.Specialized.NameValueCollection parameters = HttpUtility.ParseQueryString(context.Request.Url.Query);
                            string utterance = parameters["utterance"];
                            int index;
                            if (int.TryParse(utterance, out index))
                            {
                                RunJavaScript(string.Format("WebGLSpeechSynthesisPlugin.Speak({0})", index));
                            }
                        }

                        byte[] bytes = UTF8Encoding.UTF8.GetBytes(response);
                        context.Response.OutputStream.Write(bytes, 0, bytes.Length);
                        context.Response.OutputStream.Flush();
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine("WorkerThread: HTTPListener exception={0}", ex);
                    }
                    finally
                    {
                        try
                        {
                            // close the connection
                            if (null != context)
                            {
                                context.Response.Close();
                            }
                            if (closeApp)
                            {
                                CloseApp();
                            }
                        }
                        catch (Exception)
                        {

                        }
                    }
                }
                Thread.Sleep(0);
            }

            AppendStatus("Worker Exited!");
        }
    }
}
