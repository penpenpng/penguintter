using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace Penguintter.Views
{
    public partial class ConfigView : Window
    {
        static private ConfigView Instance { get; set; }

        private ConfigView()
        {
            InitializeComponent();
            DataContext = new
            {
                Config = ((App)Application.Current).Config
            };
        }

        static public ConfigView GetInstance()
        {
            return Instance = Instance ?? new ConfigView();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            e.Cancel = true;
            Hide();
        }

        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            UpdateConfig();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Default_Click(object sender, RoutedEventArgs e)
        {
            ((App)Application.Current).SetDefaultConfig();
            DataContext = new
            {
                Config = ((App)Application.Current).Config
            };
        }

        private void UpdateConfig()
        {
            _UpdateConfig(this);
            try
            {
                ((App)Application.Current).Config.SaveConfig();
            }
            catch
            {

            }
        }

        private void _UpdateConfig(FrameworkElement parent)
        {
            if (parent == null)
            {
                return;
            }
            foreach (var child in LogicalTreeHelper.GetChildren(parent))
            {
                (child as TextBox)?.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
                (child as CheckBox)?.GetBindingExpression(CheckBox.IsCheckedProperty)?.UpdateSource();
                (child as RadioButton)?.GetBindingExpression(RadioButton.IsCheckedProperty)?.UpdateSource();
                _UpdateConfig(child as FrameworkElement);
            }
        }
    }
}
