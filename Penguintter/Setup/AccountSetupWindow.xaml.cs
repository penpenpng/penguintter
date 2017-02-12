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
using System.Windows.Shapes;
using System.Diagnostics;
using CoreTweet;

namespace Penguintter.Setup
{
    /// <summary>
    /// AccountSetupWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class AccountSetupWindow : Window
    {
        public Tokens Token { get; private set; }
        public string PIN { get; set; }
        private OAuth.OAuthSession Session { get; set; }

        public AccountSetupWindow()
        {
            InitializeComponent();
            StartNewSession();
        }

        private void StartNewSession()
        {
            Session = OAuth.Authorize("C7dOOYyC0JLs8G4HVvqdJjGpj", "bgUxwxzg7zbwJYubnFTdjkXfkYh4EGVA0qO3qtlIEvNRnbwexC");
            var url = Session.AuthorizeUri.AbsoluteUri;
            Process.Start(url);
            DataContext = new
            {
                URL = url
            };
        }

        private void Regen_Click(object sender, RoutedEventArgs e)
        {
            StartNewSession();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Token = OAuth.GetTokens(Session, PINBox.Text);
                Close();
            }
            catch
            {
                Token = null;
                return;
            }
        }
    }
}
