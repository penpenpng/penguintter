using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CoreTweet;

namespace Penguintter.Views { 

    public partial class StatusView : Window
    {
        private Status Status { get; set; }

        public StatusView(Status status)
        {
            InitializeComponent();
            Status = status;
            var config = ((App)Application.Current).Config;
            DataContext = new
            {
                IsRetweet = status.RetweetedStatus != null,
                IsReply = status.InReplyToUserId == ((App)Application.Current).Token.UserId,
                Width = config.StatusDisplyType_ShowAll ? double.NaN : config.StatusDisplyType_WidthLimit.Value,
                WrapType = config.StatusDisplyType_Wrap ? TextWrapping.Wrap : TextWrapping.NoWrap,
                Status = status,
                Config = config,
                ScreenWidth = SystemParameters.PrimaryScreenWidth
            };
        }

        public new void Show()
        {
            Left = SystemParameters.PrimaryScreenWidth;
            base.Show();
            var anime = new DoubleAnimation(Left, -ActualWidth, new Duration(TimeSpan.FromSeconds(((App)Application.Current).Config.StatusLifeTime.Value)));
            Storyboard.SetTargetProperty(anime, new PropertyPath("Left"));
            Storyboard.SetTarget(anime, this);
            var story = new Storyboard();
            story.Completed += OnCompleted;
            story.Children.Add(anime);
            story.Begin();
        }

        private void OnCompleted(object sender, EventArgs e)
        {
            Close();
        }

        private void OnMouseHover(object sender, EventArgs e)
        {
            if (!((App)Application.Current).Config.NotShowDetailView)
            {
                DetailView.GetInstance().Show(Status);
            }
        }
    }

    public class StatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var defstr = (string)value;
            var str = Regex.Replace((string)value, @"s?https?://[-_.!~*'()a-zA-Z0-9;/?:@&=+$,%#]+", "");
            if (!((App)Application.Current).Config.AllowMultiLines){
                str = Regex.Replace(str, @"\n.*", "");
            }
            if (defstr != str)
            {
                str += "[…]";
            }
            return str;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
