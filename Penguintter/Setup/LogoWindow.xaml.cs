using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Penguintter.Views;

namespace Penguintter.Setup
{
    public partial class LogoWindow : Window
    {
        private DispatcherTimer Timer { get; set; } = new DispatcherTimer(DispatcherPriority.Normal)
        {
            Interval = TimeSpan.FromSeconds(2),
        };

        public LogoWindow()
        {
            InitializeComponent();

            ContentRendered += (_, __) => {
                AddLogLine("Activate Penguintter");
                AddLogLine("Registering global hotkey...");
                var hotkey = new GlobalHotkey(this);
                hotkey.Regist(ModifierKeys.Control | ModifierKeys.Alt, Key.T, new EventHandler(ShowUpdateView));
                hotkey.Regist(ModifierKeys.Control | ModifierKeys.Alt, Key.L, new EventHandler(ShowLogView));
                hotkey.Regist(ModifierKeys.Control | ModifierKeys.Alt, Key.C, new EventHandler(ShowConfigView));
                AddLogLine("-> done");

                AddLogLine("Initializing cache...");
                var app = (App)Application.Current;
                app.LoadCache();
                AddLogLine("-> done");

                AddLogLine("Checking " + Config.ConfigFileName + "...");
                if (app.LoadConfig())
                {
                    AddLogLine("-> Detected");
                }
                else
                {
                    AddLogLine("-> Not found or Deserialize Error: Apply default configuration");
                }

                AddLogLine("Checking " + Account.AccountFileName + "...");
                if (app.LoadAccount())
                {
                    AddLogLine("-> Detected: Loaded account data");
                }
                else
                {
                    AddLogLine("-> Not found or Deserialize Error: Creating and saving new account data...");
                    if (app.CreateAccount())
                    {
                        AddLogLine("-> Saved successfully");
                    }
                    else
                    {
                        AddLogLine("-> FAILED");
                        AddLogLine("アプリケーションを終了してください");
                        return;
                    }
                    app.LoadAccount();
                    AddLogLine("Loaded account data");
                }

                AddLogLine("Start streaming");
                Timer.Tick += (___, ____) => {
                    WindowState = WindowState.Minimized;
                    Timer.Stop();
                };
                Timer.Start();
                ((App)Application.Current).StartStream();
            };
        }

        public void AddLogLine(string message)
        {
            var line = new TextBlock();
            line.Text = message;
            Loglines.Children.Add(line);
            Scroller.ScrollToBottom();
        }

        public void ShowUpdateView(object sender, EventArgs e)
        {
            UpdateView.GetInstance().Show();
        }

        public void ShowLogView(object sender, EventArgs e)
        {
            LogView.GetInstance().Show();
        }

        public void ShowConfigView(object sender, EventArgs e)
        {
            ConfigView.GetInstance().Show();
        }

        private void Config_Click(object sender, RoutedEventArgs e)
        {
            ConfigView.GetInstance().Show();
        }

        private void Shutdown_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
