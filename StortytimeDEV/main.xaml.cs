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

namespace StortytimeDEV
{
    /// <summary>
    /// Interaction logic for main.xaml
    /// </summary>
    public partial class main : UserControl
    {
        public main()
        {
            try
            {
                InitializeComponent();
                InitializeUserControl();
                this.Loaded += Main_Loaded;
                this.Unloaded += Main_Unloaded;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to initialize main control: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void InitializeUserControl()
        {
            try
            {
                // Initialize any required resources or state
                this.IsEnabled = true;
                this.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to initialize user control: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Main_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Add any additional initialization that requires the visual tree
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load main control: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Main_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Clean up any resources
                this.Loaded -= Main_Loaded;
                this.Unloaded -= Main_Unloaded;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during cleanup: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
