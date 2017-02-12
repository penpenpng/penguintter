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
using System.Windows.Threading;
using CoreTweet;


namespace Penguintter.Views
{
    /// <summary>
    /// DetailView.xaml の相互作用ロジック
    /// </summary>
    public partial class DetailView : Window
    {
        static private DetailView Instance { get; set; }
        private DispatcherTimer Timer { get; set; } = new DispatcherTimer(DispatcherPriority.Normal);

        private DetailView()
        {
            InitializeComponent();
            Timer.Interval = TimeSpan.FromSeconds(((App)Application.Current).Config.DetailLifeTime.Value);
            Timer.Tick += OnTimerTick;
        }

        static public DetailView GetInstance()
        {
            return Instance = Instance ?? new DetailView();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            Timer.Stop();
            Timer.Start();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            Timer.Stop();
            e.Cancel = true;
            Hide();
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            Visibility = Visibility.Hidden;
            Timer.Stop();
        }

        private new void Show() { }

        public void Show(Status status)
        {
            ReplyButton.DataContext = new
            {
                Command = new ReplyCommand(status),
            };
            RetweetButton.DataContext = new
            {
                Command = new RetweetCommand(status),
            };
            FavoriteButton.DataContext = new
            {
                Command = new FavoriteCommand(status),
            };
            DataContext = new
            {
                Status = status,
                Icon = ((App)Application.Current).Cache.GetIcon(status.User.Id),
                Config = ((App)Application.Current).Config,
            };
            Visibility = Visibility.Visible;
            Activate();
            Timer.Stop();
            Timer.Start();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }

    public class ReplyCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private Status Status { get; }

        public ReplyCommand(Status status)
        {
            Status = status;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            UpdateView.GetInstance().Show(Status);
        }
    }

    public class RetweetCommand : ICommand, INotifyPropertyChanged
    {
        public event EventHandler CanExecuteChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        private Status Status { get; }
        private bool WaitingTask { get; set; } = false;
        private bool _IsPushed;
        public bool IsPushed
        {
            get { return _IsPushed; }
            set
            {
                _IsPushed = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsPushed"));
            }
        }

        public RetweetCommand(Status status)
        {
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
                if (IsPushed)
                {
                    var result = await((App)Application.Current).Token.Statuses.UnretweetAsync(id => Status.Id);
                    IsPushed = false;
                }
                else
                {
                    var result = await((App)Application.Current).Token.Statuses.RetweetAsync(id => Status.Id);
                    IsPushed = true;
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                WaitingTask = false;
            }
        }
    }

    public class FavoriteCommand : ICommand, INotifyPropertyChanged
    {
        public event EventHandler CanExecuteChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        private Status Status { get; }
        private bool WaitingTask { get; set; } = false;
        private bool _IsPushed;
        public bool IsPushed
        {
            get { return _IsPushed; }
            set
            {
                _IsPushed = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsPushed"));
            }
        }

        public FavoriteCommand(Status status)
        {
            Status = status;
            IsPushed = status.IsFavorited is bool ? (bool)status.IsFavorited : false;
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
                if (IsPushed)
                {
                    var result = await ((App)Application.Current).Token.Favorites.DestroyAsync(id => Status.Id);
                    IsPushed = false;
                }
                else
                {
                    var result = await ((App)Application.Current).Token.Favorites.CreateAsync(id => Status.Id);
                    IsPushed = true;
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                WaitingTask = false;
            }
        }
    }

    public class CreatedAtConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var time = ((DateTimeOffset)value).DateTime.AddHours(9);
            var delta = DateTime.Now - time;
            if (delta < TimeSpan.FromMinutes(1))
            {
                return string.Format("{0}s", delta.Seconds);
            }
            else if (delta < TimeSpan.FromHours(1))
            {
                return string.Format("{0}m", delta.Minutes);
            }
            else if (delta < TimeSpan.FromHours(24))
            {
                return string.Format("{0}h{1}m", delta.Hours, delta.Minutes);
            }
            else
            {
                return time.ToString();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
