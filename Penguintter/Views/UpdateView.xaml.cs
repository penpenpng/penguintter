using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
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
using CoreTweet;

namespace Penguintter.Views
{
    /// <summary>
    /// UpdateView.xaml の相互作用ロジック
    /// </summary>
    public partial class UpdateView : Window
    {
        static private UpdateView Instance { get; set; }

        private UpdateView()
        {
            InitializeComponent();
        }

        static public UpdateView GetInstance()
        {
            return Instance = Instance ?? new UpdateView();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            e.Cancel = true;
            Hide();
        }

        public new void Show()
        {
            Show(null);
        }

        public void Show(Status status)
        {
            var inputs = new UpdateViewInputs(status);
            var update_command = new UpdateCommand(inputs, status);
            var update_hotkey = new KeyBinding(update_command, new KeyGesture(Key.Enter, ModifierKeys.Control));
            DevTool.Print("count", InputBindings.Count);
            InputBindings.Clear();
            InputBindings.Add(update_hotkey);
            DataContext = new
            {
                Status = status,
                Inputs = inputs,
                Config = ((App)Application.Current).Config,
                Commands = new
                {
                    Update = update_command,
                },
            };
            Visibility = Visibility.Visible;
            Activate();
        }

        public class UpdateViewInputs
        {
            public string Text { get; set; } = "";
            public long? StatusId { get; } = null;

            public UpdateViewInputs(Status status)
            {
                if (status != null)
                {
                    Text = string.Format("@{0} ", status.User.ScreenName);
                    StatusId = status.Id;
                }
            }
        }

        public class UpdateCommand : ICommand
        {
            public event EventHandler CanExecuteChanged;
            private UpdateViewInputs Inputs { get; }
            private bool WaitingTask { get; set; } = false;
            private Status Status { get; }

            public UpdateCommand(UpdateViewInputs inputs, Status status)
            {
                Inputs = inputs;
                Status = status;
            }

            public bool CanExecute(object parameter)
            {
                return !WaitingTask;
            }

            public async void Execute(object parameter)
            {
                try
                {
                    WaitingTask = true;
                    var result = await ((App)Application.Current).Token.Statuses.UpdateAsync(
                        status => Inputs.Text,
                        in_reply_to_status_id => Inputs.StatusId);
                    GetInstance().Hide();
                }
                catch
                {
                    MessageBox.Show("投稿エラー");
                }
                finally
                {
                    WaitingTask = false;
                }
            }
        }
    }

    public class ExisitenceToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DevTool.Print(targetType);
            return value != null ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


}
