using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using Microsoft.Extensions.Configuration;

namespace StortytimeDEV.view
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : Window
    {
        private IMongoCollection<User> _usersCollection;
        private IConfiguration _configuration;

        public LoginView()
        {
            InitializeComponent();
            Task.Run(async () => await InitializeDatabaseAsync()).ConfigureAwait(false);
        }

        private async Task InitializeDatabaseAsync()
        {
            try
            {
                // Wait for App configuration to be available
                while (App.Configuration == null)
                {
                    await Task.Delay(100);
                }

                // Use configuration from App class
                _configuration = App.Configuration;

                if (_configuration == null)
                {
                    throw new InvalidOperationException("Application configuration not initialized properly.");
                }

                // Initialize MongoDB connection settings with default values
                var settings = MongoClientSettings.FromConnectionString(_configuration["MongoDB:ConnectionString"]);
                settings.ServerApi = new ServerApi(ServerApiVersion.V1);

                // Parse timeout values with default fallbacks
                int connectTimeout = 30000;
                int.TryParse(_configuration["MongoDB:ConnectTimeoutMs"], out connectTimeout);
                settings.ConnectTimeout = TimeSpan.FromMilliseconds(connectTimeout);

                int serverSelectionTimeout = 30000;
                int.TryParse(_configuration["MongoDB:ServerSelectionTimeoutMs"], out serverSelectionTimeout);
                settings.ServerSelectionTimeout = TimeSpan.FromMilliseconds(serverSelectionTimeout);

                int socketTimeout = 60000;
                int.TryParse(_configuration["MongoDB:SocketTimeoutMs"], out socketTimeout);
                settings.SocketTimeout = TimeSpan.FromMilliseconds(socketTimeout);

                bool retryEnabled = true;
                bool.TryParse(_configuration["MongoDB:RetryEnabled"], out retryEnabled);
                settings.RetryWrites = retryEnabled;
                settings.RetryReads = retryEnabled;

                var mongoClient = new MongoClient(settings);
                var database = mongoClient.GetDatabase(_configuration["MongoDB:DatabaseName"]);
                _usersCollection = database.GetCollection<User>(_configuration["MongoDB:UsersCollection"]);

                // Verify database connection
                var pingCommand = new MongoDB.Bson.BsonDocument { { "ping", 1 } };
                database.RunCommand<MongoDB.Bson.BsonDocument>(pingCommand);

                // Check if admin user exists, if not create it
                var adminUsername = _configuration["Security:DefaultAdminUsername"] ?? "admin";
                var adminUser = _usersCollection.Find(u => u.Username == adminUsername).FirstOrDefault();
                if (adminUser == null)
                {
                    await CreateUserAsync(adminUsername, "password123", "admin");
                }
            }
            catch (MongoConfigurationException)
            {
                MessageBox.Show("Failed to connect to the database. Please check your configuration settings.", 
                    "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }
            catch (MongoAuthenticationException)
            {
                MessageBox.Show("Database authentication failed. Please check your credentials.", 
                    "Authentication Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }
            catch (MongoConnectionException)
            {
                MessageBox.Show("Could not connect to the database. Please check your network connection.", 
                    "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }
            catch (Exception)
            {
                MessageBox.Show("An error occurred while starting the application.", 
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }
        }

        public class User
        {
            [MongoDB.Bson.Serialization.Attributes.BsonId]
            [MongoDB.Bson.Serialization.Attributes.BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
            public string Id { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
            public string Role { get; set; }
        }

        private async Task<User> CreateUserAsync(string username, string password, string role)
        {
            var newUser = new User
            {
                Username = username,
                Password = password,
                Role = role
            };

            // Delete any existing user with the same username
            await _usersCollection.DeleteOneAsync(u => u.Username == username);

            // Insert the new user
            await _usersCollection.InsertOneAsync(newUser);
            
            return newUser;
        }

        private bool VerifyPassword(string password, string storedPassword)
        {
            return !string.IsNullOrEmpty(password) && password == storedPassword;
        }

        private void LoginView_Loaded(object sender, RoutedEventArgs e)
        {
            var fadeInAnimation = FindResource("FadeInAnimation") as Storyboard;
            fadeInAnimation?.Begin();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
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

        private async void btnlogin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtuser.Text) || string.IsNullOrWhiteSpace(txtpassword.Password))
                {
                    MessageBox.Show("Please enter both username and password.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string enteredUsername = txtuser.Text.Trim();
                string enteredPassword = txtpassword.Password;
                
                // Find user by username and password
                var user = await _usersCollection.Find(u => u.Username == enteredUsername && u.Password == enteredPassword).FirstOrDefaultAsync();

                if (user == null)
                {
                    MessageBox.Show("Invalid username or password.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                MainWindow main = new MainWindow();
                main.Show();
                this.Close();
            }
            catch (MongoException)
            {
                MessageBox.Show("Could not connect to the database. Please try again.", "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception)
            {
                MessageBox.Show("An error occurred. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                txtuser.Text = string.Empty;
                txtpassword.Password = string.Empty;
            }
        }

        private void Reset_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ResetPasswordView resetPasswordView = new ResetPasswordView();
            resetPasswordView.Show();
            this.Close();
        }
    }
}


