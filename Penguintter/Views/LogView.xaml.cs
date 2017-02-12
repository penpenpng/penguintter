using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CoreTweet;

namespace Penguintter.Views
{
    /// <summary>
    /// LogView.xaml の相互作用ロジック
    /// </summary>
    public partial class LogView : Window
    {
        static private LogView Instance { get; set; }

        private LogView()
        {
            InitializeComponent();
        }

        static public LogView GetInstance()
        {
            return Instance = Instance ?? new LogView();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            e.Cancel = true;
            Hide();
        }

        public void AddStatus(Status status)
        {
            var c = (Control)Resources["Row"];
            var config = ((App)Application.Current).Config;
            c.DataContext = new
            {
                Status = status,
                Icon = ((App)Application.Current).Cache.GetIcon(status.User.Id),
                Config = config,
                Commands = new
                {
                    ShowDetail = new ShowDetailCommand(status),
                },
            };
            lock (LogColumn.Children)
            {
                LogColumn.Children.Insert(0, c);
                for (int i = config.MaxLogCount.Value; i < LogColumn.Children.Count; i++)
                {
                    LogColumn.Children.RemoveAt(i);
                }
            }

        }

        class ShowDetailCommand : ICommand
        {
            public event EventHandler CanExecuteChanged;
            private Status Status { get; }

            public ShowDetailCommand(Status status)
            {
                Status = status;
            }

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public void Execute(object parameter)
            {
                DetailView.GetInstance().Show(Status);
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            LogColumn.Children.Clear();
        }
    }
}
