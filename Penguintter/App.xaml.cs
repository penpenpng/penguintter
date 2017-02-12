using Penguintter.Views;
using System;
using System.Windows;
using System.Reactive.Linq;
using CoreTweet;
using CoreTweet.Streaming;

namespace Penguintter { 
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        public Tokens Token { get; private set; }
        public Config Config { get; private set; }
        public Cache Cache { get; private set; }

        [STAThread]
        public static void Main()
        {
            App app = new App() {
                StartupUri = new Uri(@"Setup\LogoWindow.xaml", UriKind.Relative),
            };
            app.InitializeComponent();
            app.Run();
        }

        public bool LoadCache()
        {
            Cache = new Cache();
            return true;
        }

        public bool LoadConfig()
        {
            try
            {
                Config = Config.LoadConfig();
            }
            catch
            {
                SetDefaultConfig();
                return false;
            }
            return true;
        }

        public void SetDefaultConfig()
        {
            Config = new Config();
        }

        public bool LoadAccount()
        {
            Token = Account.LoadAccount();
            if (Token != null)
            {
                Token.UserId = (long)Token.Account.VerifyCredentials().Id;
                return true;
            }
            return false;
        }

        public bool CreateAccount()
        {
            try
            {
                Account.CreateAccount();
            }
            catch
            {
                return false;
            }
            return true;
        }

        public void StartStream()
        {
            Token.Streaming
                .UserAsObservable()
                .OfType<StatusMessage>()
                .Select(m => m.Status)
                .Subscribe(s => Current.Dispatcher.Invoke(() => {
                    Cache.Store(s);
                    (new StatusView(s)).Show();
                    LogView.GetInstance().AddStatus(s);
                }));
        }
    }
}
