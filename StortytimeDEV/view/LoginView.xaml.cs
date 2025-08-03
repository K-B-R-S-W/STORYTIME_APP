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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace StortytimeDEV.view
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : Window
    {
        public LoginView()
        {
            InitializeComponent();
            users = new List<User>
    {
        new User { Username = "admin", Password = "password123" },
        // Add more user credentials as needed
    };
        }
        private List<User> users;
        public class User
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }
        private void LoginView_Loaded(object sender, RoutedEventArgs e)
        {
            var fadeInAnimation = FindResource("FadeInAnimation") as Storyboard;
            fadeInAnimation?.Begin();
        }


        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void btnminize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void btnclose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnlogin_Click(object sender, RoutedEventArgs e)
        {
            string enteredUsername = txtuser.Text;
            string enteredPassword = txtpassword.Password;

            // Find the user with the entered username
            User user = users.FirstOrDefault(u => u.Username == enteredUsername);

            if (user != null && user.Password == enteredPassword)
            {
                // Successful login
                MessageBox.Show("Login successful!");
                MainWindow main= new MainWindow();
                this.Close();
                main.Show();
                // Navigate to the main application or perform desired actions

            }
            else
            {
                // Invalid credentials
                MessageBox.Show("Invalid username or password. Please try again.");
            }

            txtuser.Text = string.Empty;
            txtpassword.Password = string.Empty;

        }
    }
}


