using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using MongoDB.Driver;

namespace StortytimeDEV
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IConfiguration Configuration { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            try
            {
                try
                {
                    var currentDirectory = Directory.GetCurrentDirectory();
                    var configPath = Path.Combine(currentDirectory, "appsettings.json");
                    var projectConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
                    var executablePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                    var executableDir = Path.GetDirectoryName(executablePath);
                    var executableConfigPath = Path.Combine(executableDir, "appsettings.json");
                    
                    string debugInfo = $"Current Directory: {currentDirectory}\n";
                    debugInfo += $"Config Path: {configPath}\n";
                    debugInfo += $"Project Config Path: {projectConfigPath}\n";
                    debugInfo += $"Base Directory: {AppDomain.CurrentDomain.BaseDirectory}\n";
                    debugInfo += $"Executable Path: {executablePath}\n";
                    debugInfo += $"Executable Directory: {executableDir}\n";
                    debugInfo += $"Executable Config Path: {executableConfigPath}\n";
                    
                    string finalConfigPath = null;
                    if (File.Exists(configPath))
                    {
                        debugInfo += "\nConfig file found in current directory.";
                        finalConfigPath = configPath;
                    }
                    else if (File.Exists(projectConfigPath))
                    {
                        debugInfo += "\nConfig file found in base directory.";
                        finalConfigPath = projectConfigPath;
                    }
                    else if (File.Exists(executableConfigPath))
                    {
                        debugInfo += "\nConfig file found in executable directory.";
                        finalConfigPath = executableConfigPath;
                    }
                    else
                    {
                        throw new FileNotFoundException($"Configuration file not found in any location.", "appsettings.json");
                    }

                    try
                    {
                        var configContent = File.ReadAllText(finalConfigPath);
                        debugInfo += $"\n\nConfig Content from {finalConfigPath}:\n{configContent}";

                    var builder = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile(finalConfigPath, optional: false, reloadOnChange: true);

                    Configuration = builder.Build();

                    // Validate configuration
                    if (Configuration == null)
                    {
                        throw new InvalidOperationException("Failed to load configuration.");
                    }

                    // Validate MongoDB settings
                    var mongodbSection = Configuration.GetSection("MongoDB");
                    if (mongodbSection == null)
                    {
                        throw new InvalidOperationException("MongoDB configuration section not found.");
                    }                        var mongoConnectionString = Configuration["MongoDB:ConnectionString"];
                        if (string.IsNullOrEmpty(mongoConnectionString))
                        {
                            throw new InvalidOperationException("MongoDB connection string not found in configuration.");
                        }
                        
                        try
                        {
                            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
                            {
                                var assemblyName = new System.Reflection.AssemblyName(args.Name).Name;
                                return null;
                            };
                            
                            var connectionString = Configuration["MongoDB:ConnectionString"];
                            var settings = MongoClientSettings.FromConnectionString(connectionString);
                            settings.ServerApi = new ServerApi(ServerApiVersion.V1);
                            settings.DirectConnection = false;
                            settings.RetryWrites = true;
                            settings.RetryReads = true;
                            settings.ConnectTimeout = TimeSpan.FromSeconds(30);
                            settings.ServerSelectionTimeout = TimeSpan.FromSeconds(30);
                            settings.SocketTimeout = TimeSpan.FromMinutes(1);
                            settings.HeartbeatInterval = TimeSpan.FromSeconds(10);
                            settings.MaxConnectionPoolSize = 100;
                            settings.MinConnectionPoolSize = 1;
                            settings.WaitQueueTimeout = TimeSpan.FromSeconds(30);
                            settings.UseTls = true;
                            settings.AllowInsecureTls = true;
                            
                            var client = new MongoClient(settings);
                            var database = client.GetDatabase(Configuration["MongoDB:DatabaseName"]);
                            
                            // Test the connection
                            var pingCommand = new MongoDB.Bson.BsonDocument { { "ping", 1 } };
                            database.RunCommand<MongoDB.Bson.BsonDocument>(pingCommand);
                        }
                        catch (Exception ex)
                        {
                            throw new InvalidOperationException("Failed to connect to MongoDB.", ex);
                        }

                        var databaseName = Configuration["MongoDB:DatabaseName"];
                        var usersCollection = Configuration["MongoDB:UsersCollection"];
                        
                        if (string.IsNullOrEmpty(databaseName) || string.IsNullOrEmpty(usersCollection))
                        {
                            throw new InvalidOperationException("Required MongoDB configuration values are missing.");
                        }

                        // Configuration values validated
                    }
                    catch (JsonException ex)
                    {
                        throw new InvalidOperationException($"Error parsing JSON configuration file: {ex.Message}", ex);
                    }
                    catch (Exception ex) when (!(ex is InvalidOperationException || ex is JsonException))
                    {
                        throw new InvalidOperationException($"Error reading configuration file: {ex.Message}", ex);
                    }
                }
                catch (Exception ex)
                {
                    var errorMessage = $"Error initializing configuration:\n{ex.Message}";
                    if (ex.InnerException != null)
                    {
                        errorMessage += $"\n\nInner Exception:\n{ex.InnerException.Message}";
                    }
                    errorMessage += $"\n\nStack Trace:\n{ex.StackTrace}";
                    MessageBox.Show(errorMessage, "Configuration Error");
                    throw;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to initialize application: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
            }
        }
    }
}
