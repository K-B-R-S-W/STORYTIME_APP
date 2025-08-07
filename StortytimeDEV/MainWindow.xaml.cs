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
using StortytimeDEV.view;

namespace StortytimeDEV
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            try
            {
                InitializeComponent();
                
                // Set initial content
                MainContent.Content = new Main();
                
                // Set window state
                this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                
                MessageBox.Show("MainWindow initialized", "Debug");
                
                if (MainContent == null)
                {
                    MessageBox.Show("MainContent is null!", "Error");
                }
                this.ResizeMode = ResizeMode.NoResize;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to initialize main window: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (e.ChangedButton == MouseButton.Left)
                {
                    this.DragMove();
                }
            }
            catch (InvalidOperationException)
            {
                // Ignore drag operation if window is in an invalid state
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LoginView loginView = new LoginView();
                loginView.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to logout: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                WindowState = WindowState.Minimized;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to minimize window: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Application.Current.Shutdown();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to close application: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(1); // Force exit if normal shutdown fails
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            // Cleanup any resources if needed
        }

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button button)
                {
                    // Reset all buttons to default style
                    foreach (var child in ((StackPanel)button.Parent).Children)
                    {
                        if (child is Button menuButton)
                        {
                            menuButton.Style = (Style)FindResource("menuButton");
                        }
                    }

                    // Set clicked button to active style
                    button.Style = (Style)FindResource("menuButtonActive");

                    // Navigate to the appropriate page
                    switch (button.Name)
                    {
                        case "HomeButton":
                            MainContent.Content = new Main();
                            break;
                        case "TrailersButton":
                            MainContent.Content = new LatestTrailers();
                            break;
                        // Add other cases for additional pages
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to navigate: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
