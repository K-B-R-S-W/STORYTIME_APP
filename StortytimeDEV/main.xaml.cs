using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
using StortytimeDEV.Services;

namespace StortytimeDEV
{
    /// <summary>
    /// Interaction logic for main.xaml
    /// </summary>
    public partial class Main : UserControl
    {
        private readonly TMDBService _tmdbService;
        private ObservableCollection<Movie> _nowPlayingMovies;
        private ObservableCollection<Movie> _upcomingMovies;
        private ObservableCollection<Movie> _popularMovies;

        public Main()
        {
            try
            {
                InitializeComponent();
                InitializeUserControl();
                _tmdbService = new TMDBService();
                
                // Initialize collections
                _nowPlayingMovies = new ObservableCollection<Movie>();
                _upcomingMovies = new ObservableCollection<Movie>();
                _popularMovies = new ObservableCollection<Movie>();

                // Set ItemsSource
                NowPlayingMovies.ItemsSource = _nowPlayingMovies;
                UpcomingMovies.ItemsSource = _upcomingMovies;
                PopularMovies.ItemsSource = _popularMovies;

                // Load movies when control loads
                this.Loaded += Main_Loaded;
                this.Unloaded += Main_Unloaded;
                
                MessageBox.Show("Main control initialized", "Debug");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to initialize main control: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void Main_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBox.Show("Loading movies...", "Debug");
                await Task.WhenAll(
                    LoadNowPlayingMovies(),
                    LoadUpcomingMovies(),
                    LoadPopularMovies()
                );
                MessageBox.Show("Movies loaded!", "Debug");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading movies: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task LoadAllMovies()
        {
            try
            {
                MessageBox.Show("Starting to load movies...", "Debug");

                // Clear existing items
                _nowPlayingMovies.Clear();
                _upcomingMovies.Clear();
                _popularMovies.Clear();

                // Load Now Playing movies
                MessageBox.Show("Loading Now Playing movies...", "Debug");
                var nowPlaying = await _tmdbService.GetNowPlayingMovies();
                MessageBox.Show($"Now Playing movies loaded. Count: {nowPlaying?.Count ?? 0}", "Debug");

                // Load Upcoming movies
                MessageBox.Show("Loading Upcoming movies...", "Debug");
                var upcoming = await _tmdbService.GetUpcomingMovies();
                MessageBox.Show($"Upcoming movies loaded. Count: {upcoming?.Count ?? 0}", "Debug");

                // Load Popular movies
                MessageBox.Show("Loading Popular movies...", "Debug");
                var popular = await _tmdbService.GetPopularMovies();
                MessageBox.Show($"Popular movies loaded. Count: {popular?.Count ?? 0}", "Debug");

                foreach (var movie in nowPlaying)
                    _nowPlayingMovies.Add(movie);

                foreach (var movie in upcoming)
                    _upcomingMovies.Add(movie);

                foreach (var movie in popular)
                    _popularMovies.Add(movie);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading movies: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void InitializeUserControl()
        {
            try
            {
                this.IsEnabled = true;
                this.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to initialize user control: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        private async Task LoadNowPlayingMovies()
        {
            try
            {
                var movies = await _tmdbService.GetNowPlayingMovies();
                _nowPlayingMovies.Clear();
                foreach (var movie in movies)
                {
                    _nowPlayingMovies.Add(movie);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load now playing movies: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task LoadUpcomingMovies()
        {
            try
            {
                var movies = await _tmdbService.GetUpcomingMovies();
                _upcomingMovies.Clear();
                foreach (var movie in movies)
                {
                    _upcomingMovies.Add(movie);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load upcoming movies: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task LoadPopularMovies()
        {
            try
            {
                var movies = await _tmdbService.GetPopularMovies();
                _popularMovies.Clear();
                foreach (var movie in movies)
                {
                    _popularMovies.Add(movie);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load popular movies: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
