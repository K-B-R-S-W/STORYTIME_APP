using System;
using System.Windows;
using System.Windows.Input;
using MongoDB.Driver;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace StortytimeDEV.view
{
    public partial class ResetPasswordView : Window
    {
        private readonly IMongoCollection<LoginView.User> _usersCollection;
        private readonly IConfiguration _configuration;

        public ResetPasswordView()
        {
            InitializeComponent();

            try
            {
                // Load configuration from appsettings.json
                _configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();

                var mongoClient = new MongoClient(_configuration["MongoDB:ConnectionString"]);
                var database = mongoClient.GetDatabase(_configuration["MongoDB:DatabaseName"]);
                _usersCollection = database.GetCollection<LoginView.User>(_configuration["MongoDB:UsersCollection"]);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to initialize application: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }
        }



        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
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

        private async void btnreset_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validate username
                if (string.IsNullOrWhiteSpace(txtemail.Text))
                {
                    MessageBox.Show("Please enter your username.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Validate passwords
                if (string.IsNullOrWhiteSpace(txtNewPassword.Password))
                {
                    MessageBox.Show("Please enter a new password.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtConfirmPassword.Password))
                {
                    MessageBox.Show("Please confirm your new password.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (txtNewPassword.Password != txtConfirmPassword.Password)
                {
                    MessageBox.Show("Passwords do not match.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string username = txtemail.Text.Trim();
                var user = await _usersCollection.Find(u => u.Username == username).FirstOrDefaultAsync();

                if (user != null) 
                {
                    // Update the password in MongoDB
                    var update = Builders<LoginView.User>.Update.Set(u => u.Password, txtNewPassword.Password);
                    await _usersCollection.UpdateOneAsync(u => u.Username == username, update);

                    MessageBox.Show("Your password has been successfully reset.\nYou can now login with your new password.", 
                        "Password Reset", MessageBoxButton.OK, MessageBoxImage.Information);

                    LoginView loginView = new LoginView();
                    loginView.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Username not found.", "Reset Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (MongoException)
            {
                MessageBox.Show("Could not connect to the database. Please try again.", 
                    "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception)
            {
                MessageBox.Show("An error occurred while resetting the password. Please try again.", 
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                txtemail.Text = string.Empty;
                txtNewPassword.Password = string.Empty;
                txtConfirmPassword.Password = string.Empty;
            }
        }

        private void Login_MouseDown(object sender, MouseButtonEventArgs e)
        {
            LoginView loginView = new LoginView();
            loginView.Show();
            this.Close();
        }
    }
}