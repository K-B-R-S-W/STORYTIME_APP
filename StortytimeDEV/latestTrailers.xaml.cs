using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using StortytimeDEV.Services;

namespace StortytimeDEV
{
    public partial class LatestTrailers : UserControl
    {
        private readonly TMDBService _tmdbService;
        private ObservableCollection<Movie> _trailers;
        private bool _hasLoadedMoreTrailers;

        public LatestTrailers()
        {
            InitializeComponent();
            _tmdbService = new TMDBService();
            _trailers = new ObservableCollection<Movie>();
            TrailersItemsControl.ItemsSource = _trailers;
            
            this.Loaded += LatestTrailers_Loaded;
        }

        private async void LatestTrailers_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                await LoadTrailers();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading trailers: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task LoadTrailers()
        {
            try
            {
                var trailers = await _tmdbService.GetLatestTrailers();
                _trailers.Clear();
                foreach (var trailer in trailers)
                {
                    if (!string.IsNullOrEmpty(trailer.PosterPath))
                    {
                        trailer.PosterPath = $"https://image.tmdb.org/t/p/w500{trailer.PosterPath}";
                    }
                    _trailers.Add(trailer);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load trailers: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Trailer_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.Tag is string trailerKey)
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = $"https://www.youtube.com/watch?v={trailerKey}",
                    UseShellExecute = true
                });
            }
        }

        private async void SeeMoreButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var trailers = await _tmdbService.GetLatestTrailers(loadMore: true);
                if (trailers.Count > 0)
                {
                    foreach (var trailer in trailers)
                    {
                        if (!string.IsNullOrEmpty(trailer.PosterPath))
                        {
                            trailer.PosterPath = $"https://image.tmdb.org/t/p/w500{trailer.PosterPath}";
                        }
                        _trailers.Add(trailer);
                    }
                }
                else
                {
                    SeeMoreButton.IsEnabled = false;
                    SeeMoreButton.Content = "No More Trailers";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading more trailers: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
