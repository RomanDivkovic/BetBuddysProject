using System.Text.Json;

namespace MyDotNetProject.Services
{
    public interface IMmaApiService
    {
        Task<MmaApiResponse> FetchMmaEventsAsync(string date);
        Task<MmaApiResponse> FetchMmaEventsAsync(DateTime date);
    }

    public class MmaApiService : IMmaApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<MmaApiService> _logger;

        public MmaApiService(HttpClient httpClient, IConfiguration configuration, ILogger<MmaApiService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;

            var baseUrl = _configuration["MmaApi:BaseUrl"];
            // Try to get API key from environment variable first, then fall back to configuration
            var apiKey = Environment.GetEnvironmentVariable("MMA_API_KEY")
                        ?? _configuration["MmaApi:ApiKey"];

            if (string.IsNullOrEmpty(apiKey))
            {
                throw new InvalidOperationException("MMA API key not found. Set MMA_API_KEY environment variable or configure MmaApi:ApiKey in appsettings.");
            }

            _httpClient.BaseAddress = new Uri(baseUrl!);
            _httpClient.DefaultRequestHeaders.Add("x-apisports-key", apiKey);
            _httpClient.DefaultRequestHeaders.Add("x-rapidapi-host", "v1.mma.api-sports.io");
        }

        public async Task<MmaApiResponse> FetchMmaEventsAsync(string date)
        {
            try
            {
                _logger.LogInformation("Fetching MMA events for date: {Date}", date);

                var response = await _httpClient.GetAsync($"/fights?date={date}");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("MMA API request failed with status: {StatusCode}", response.StatusCode);
                    throw new HttpRequestException($"MMA API request failed with status: {response.StatusCode}");
                }

                var jsonContent = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<MmaApiResponse>(jsonContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                _logger.LogInformation("Successfully fetched {Count} MMA events", apiResponse?.Response?.Count ?? 0);
                return apiResponse ?? new MmaApiResponse();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching MMA events for date: {Date}", date);
                throw;
            }
        }

        public async Task<MmaApiResponse> FetchMmaEventsAsync(DateTime date)
        {
            return await FetchMmaEventsAsync(date.ToString("yyyy-MM-dd"));
        }
    }

    // MMA API Response Models
    public class MmaApiResponse
    {
        public List<ApiFight> Response { get; set; } = new();
    }

    public class ApiFight
    {
        public int Id { get; set; }
        public string? Slug { get; set; }
        public string? Date { get; set; }
        public string? Venue { get; set; }
        public string? City { get; set; }
        public ApiLeague? League { get; set; }
        public ApiFighters? Fighters { get; set; }
        public ApiStatus? Status { get; set; }
    }

    public class ApiLeague
    {
        public string? Logo { get; set; }
    }

    public class ApiFighters
    {
        public ApiFighter? First { get; set; }
        public ApiFighter? Second { get; set; }
    }

    public class ApiFighter
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Record { get; set; }
        public string? Country { get; set; }
        public string? Logo { get; set; }
    }

    public class ApiStatus
    {
        public string? Short { get; set; }
    }
}
