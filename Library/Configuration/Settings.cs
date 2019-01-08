using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

[assembly: InternalsVisibleTo("Recurly.Test")]

namespace Recurly.Configuration
{
    internal class Settings
    {
        // non-static, as these change per instance, likely
        public string ApiKey
        {
            get
            {
                if (_hasLoaded == false)
                {
                    throw new Exception("The Recurly client has has no configuration initialized, please add your settings to web/app.config or call Recurly.Configuration.SettingsManager.Initialize(args)");
                }
                return _apiKey;
            }
            private set { _apiKey = value; }
        }

        public string PrivateKey
        {
            get
            {
                if (_hasLoaded == false)
                {
                    throw new Exception("The Recurly client has has no configuration initialized, please add your settings to web/app.config or call Recurly.Configuration.SettingsManager.Initialize(args)");
                }
                return _privateKey;
            }
            private set { _privateKey = value; }
        }

        public string Subdomain
        {
            get
            {
                if (_hasLoaded == false)
                {
                    throw new Exception("The Recurly client has has no configuration initialized, please add your settings to web/app.config or call Recurly.Configuration.SettingsManager.Initialize(args)");
                }
                return _subdomain;
            }
            private set { _subdomain = value; }
        }

        public int PageSize
        {
            get
            {
                if (_hasLoaded == false)
                {
                    throw new Exception("The Recurly client has has no configuration initialized, please add your settings to web/app.config or call Recurly.Configuration.SettingsManager.Initialize(args)");
                }
                return _pageSize;
            }
            private set { _pageSize = value; }
        }

        public int Port
        {
            get
            {
                if (_hasLoaded == false)
                {
                    throw new Exception("The Recurly client has has no configuration initialized, please add your settings to web/app.config or call Recurly.Configuration.SettingsManager.Initialize(args)");
                }
                return _port;
            }
            private set { _port = value; }
        }

        protected const string RecurlyServerUri = "https://{0}.recurly.com/v2{1}";
        protected const string RecurlyServerProxyUri = "http://{0}:{1}/v2{2}";

        public const string RecurlyApiVersion = "2.15";
        public const string ValidDomain = ".recurly.com";

        // static, unlikely to change
        public string UserAgent
        {
            get
            {
                return "Recurly C# Client v" + Assembly.GetExecutingAssembly().GetName().Version;
            }
        }

        public string AuthorizationHeaderValue
        {
            get
            {
                if (!ApiKey.IsNullOrEmpty())
                    return "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(ApiKey));
                return string.Empty;
            }
        }

        public string GetServerUri(string givenPath)
        {
            if (givenPath.Contains("://"))
                return givenPath;

            if (Port != 0)
            {
                Console.WriteLine("Configuring with a port");
                return string.Format(RecurlyServerProxyUri, Subdomain, Port, givenPath);
            }

            return string.Format(RecurlyServerUri, Subdomain, givenPath);
        }

        private static Settings _instance;
        private string _apiKey;
        private string _privateKey;
        private int _pageSize;
        private bool _hasLoaded;
        private string _subdomain;
        private int _port;

        public static Settings Instance
        {
            get { return _instance ?? (_instance = new Settings()); }
        }


        public void InitializeFromConfig()
        {
            ApiKey = Section.Current.ApiKey;
            Subdomain = Section.Current.Subdomain;
            PrivateKey = Section.Current.PrivateKey;
            PageSize = Section.Current.PageSize;
            Port = Section.Current.Port;
            _hasLoaded = true;
        }

        public void Initialize(string apiKey, string subdomain, string privateKey = "", int pageSize = 50, int port=0)
        {
            ApiKey = apiKey;
            Subdomain = subdomain;
            PrivateKey = privateKey;
            PageSize = pageSize;
            Port = port;
            _hasLoaded = true;
        }

        public Settings()
        {
            _hasLoaded = false;
            try
            {
                // Will try and load the settings from the config file by default so not to break existing integrations.
                InitializeFromConfig();
            }
            catch
            {
                // We can't find the settings, silently fail.
                System.Diagnostics.Debug.WriteLine("Recurly is unable to load settings from web/app.config.");
            }
        }
    }
}
