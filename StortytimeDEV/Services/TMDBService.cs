using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.Json;
using System.Diagnostics;

namespace StortytimeDEV.Services
{
    public class MovieApiResponse
    {
        public Dates Dates { get; set; }
        public int Page { get; set; }
        public List<Movie> Results { get; set; }
        public int TotalPages { get; set; }
        public int TotalResults { get; set; }
    }

    public class Dates
    {
        public string Maximum { get; set; }
        public string Minimum { get; set; }
    }

    public class Movie
    {
        public bool Adult { get; set; }
        public string BackdropPath { get; set; }
        public List<int> GenreIds { get; set; }
        public int Id { get; set; }
        public string OriginalLanguage { get; set; }
        public string OriginalTitle { get; set; }
        public string Overview { get; set; }
        public double Popularity { get; set; }
        public string PosterPath { get; set; }
        public string ReleaseDate { get; set; }
        public string Title { get; set; }
        public bool Video { get; set; }
        public double VoteAverage { get; set; }
        public int VoteCount { get; set; }
        public string TrailerKey { get; set; }
    }

    public class TMDBService
    {
        private readonly HttpClient _client;
        private const string BaseUrl = "https://api.themoviedb.org/3";
        private const string ApiKey = "552d072ad7ea1178a37fcdf1f61b3639";
        private const string AccessToken = "eyJhbGciOiJIUzI1NiJ9.eyJhdWQiOiI1NTJkMDcyYWQ3ZWExMTc4YTM3ZmNkZjFmNjFiMzYzOSIsIm5iZiI6MTc1NDM1OTEyNS42NzIsInN1YiI6IjY4OTE2NTU1NGVlNjJjZDA2MzE4NjcyMiIsInNjb3BlcyI6WyJhcGlfcmVhZCJdLCJ2ZXJzaW9uIjoxfQ.C-KGcvt0b2maEF7a-JY9__wZLVpoJav7NoqJqXOFn2c";

        public TMDBService()
        {
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Add("accept", "application/json");
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {AccessToken}");
            _client.BaseAddress = new Uri(BaseUrl);
            Debug.WriteLine("[TMDBService] Initialized with authorization headers");
        }

        public async Task<List<Movie>> GetNowPlayingMovies()
        {
            var url = $"/movie/now_playing?api_key={ApiKey}&language=en-US&page=1";
            return await FetchMovies(url);
        }

        public async Task<List<Movie>> GetUpcomingMovies()
        {
            var url = $"/movie/upcoming?api_key={ApiKey}&language=en-US&page=1";
            return await FetchMovies(url);
        }

        public async Task<List<Movie>> GetPopularMovies()
        {
            var url = $"/movie/popular?api_key={ApiKey}&language=en-US&page=1";
            return await FetchMovies(url);
        }

        private async Task<List<Movie>> FetchMovies(string url)
        {
            try
            {
                Debug.WriteLine($"[TMDBService] Fetching movies from URL: {url}");

                var response = await _client.GetAsync(url);
                Debug.WriteLine($"[TMDBService] API Response Status: {response.StatusCode}");
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"[TMDBService] API Response Content Length: {content.Length}");

                    var options = new JsonSerializerOptions 
                    { 
                        PropertyNameCaseInsensitive = true,
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    };
                    
                    var result = JsonSerializer.Deserialize<MovieApiResponse>(content, options);
                    Debug.WriteLine($"[TMDBService] Deserialized Movies Count: {result?.Results?.Count ?? 0}");
                    
                    if (result?.Results != null)
                    {
                        foreach (var movie in result.Results)
                        {
                            // Add the full URL for poster images
                            if (!string.IsNullOrEmpty(movie.PosterPath))
                            {
                                movie.PosterPath = $"https://image.tmdb.org/t/p/w500{movie.PosterPath}";
                            }
                            
                            // Format the release date
                            if (!string.IsNullOrEmpty(movie.ReleaseDate))
                            {
                                movie.ReleaseDate = DateTime.Parse(movie.ReleaseDate).ToString("MMMM dd, yyyy");
                            }
                        }
                        
                        Debug.WriteLine($"[TMDBService] Processed {result.Results.Count} movies");
                        return result.Results;
                    }
                    else
                    {
                        Debug.WriteLine("[TMDBService] No results found in API response");
                    }
                }
                else
                {
                    Debug.WriteLine($"[TMDBService] API call failed with status: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                // Log the error appropriately
                Console.WriteLine($"Error fetching movies: {ex.Message}");
            }
            
            return new List<Movie>();
        }

        private int _currentPage = 1;

        public async Task<List<Movie>> GetLatestTrailers(bool loadMore = false)
        {
            if (loadMore)
            {
                _currentPage++;
            }
            
            var movies = new List<Movie>();
            var response = await _client.GetAsync($"{BaseUrl}/movie/upcoming?api_key={ApiKey}&page={_currentPage}");
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var result = JsonSerializer.Deserialize<JsonElement>(content);
                
                foreach (var movie in result.GetProperty("results").EnumerateArray())
                {
                    var id = movie.GetProperty("id").GetInt32();
                    var videosResponse = await _client.GetAsync($"{BaseUrl}/movie/{id}/videos?api_key={ApiKey}");
                    
                    if (videosResponse.IsSuccessStatusCode)
                    {
                        var videosContent = await videosResponse.Content.ReadAsStringAsync();
                        var videosResult = JsonSerializer.Deserialize<JsonElement>(videosContent);
                        var videos = videosResult.GetProperty("results").EnumerateArray();
                        
                        string trailerKey = null;
                        foreach (var video in videos)
                        {
                            if (video.GetProperty("type").GetString() == "Trailer")
                            {
                                trailerKey = video.GetProperty("key").GetString();
                                break;
                            }
                        }

                        if (trailerKey != null)
                        {
                            movies.Add(new Movie
                            {
                                Title = movie.GetProperty("title").GetString(),
                                PosterPath = movie.GetProperty("poster_path").GetString(),
                                TrailerKey = trailerKey
                            });
                        }
                    }

                    if (!loadMore && movies.Count >= 6) // Limit to 6 movies initially
                        break;
                    else if (loadMore && movies.Count >= 6) // Load 6 more movies when clicking See More
                        break;
                }
            }

            return movies;
        }
    }
}
