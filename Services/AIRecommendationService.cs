using System.Text.Json;
using banbet.Models;
using banbet.Models.DTOs;

namespace banbet.Services
{
    public class AIRecommendationService
    {
        private readonly HttpClient _httpClient;
        private readonly string _groqApiKey;
        private readonly string _groqApiUrl = "https://api.groq.com/openai/v1/chat/completions";

        public AIRecommendationService(IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            _groqApiKey = configuration["GroqApiKey"];
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_groqApiKey}");
        }

        public async Task<List<int>> GetAIRecommendedEventIds(List<Bet> userBets)
        {
            var context = PrepareContextFromBets(userBets);
            var response = await GetGroqRecommendation(context);
            return ParseRecommendationResponse(response);
        }

        private string PrepareContextFromBets(List<Bet> userBets)
        {
            var betsContext = userBets.Select(bet => new
            {
                Category = bet.Event.Category.ToString(),
                BetType = bet.BetType.ToString(),
                Result = bet.BetStatus.ToString(),
                Amount = bet.BetAmount,
                Date = bet.BetDate
            }).ToList();

            return JsonSerializer.Serialize(new
            {
                userBettingHistory = betsContext,
                instruction = "Based on this user's betting history, analyze their preferences and betting patterns. " +
                             "Consider factors like preferred categories, bet types, typical bet amounts, and success rate. " +
                             "Return a JSON object with a 'recommendationReasoning' field explaining the logic and a " +
                             "'recommendedCategories' array listing categories in order of preference with scores."
            });
        }

        private async Task<string> GetGroqRecommendation(string context)
        {
            var requestBody = new
            {
                model = "mixtral-8x7b-32768",
                messages = new[]
                {
                    new
                    {
                        role = "system",
                        content = "You are a betting recommendation system. Analyze user betting history and provide structured recommendations."
                    },
                    new
                    {
                        role = "user",
                        content = context
                    }
                },
                temperature = 0.7,
                max_tokens = 1000
            };

            var response = await _httpClient.PostAsJsonAsync(_groqApiUrl, requestBody);
            var responseContent = await response.Content.ReadAsStringAsync();
            
            // Przykładowa struktura odpowiedzi:
            // {
            //   "recommendationReasoning": "User shows strong preference for football matches...",
            //   "recommendedCategories": [
            //     { "category": "Football", "score": 0.8 },
            //     { "category": "Basketball", "score": 0.4 }
            //   ]
            // }

            return responseContent;
        }

        private List<int> ParseRecommendationResponse(string response)
        {
            try
            {
                var recommendation = JsonSerializer.Deserialize<AIRecommendation>(response);
                return recommendation.RecommendedCategories
                    .OrderByDescending(c => c.Score)
                    .Select(c => (int)Enum.Parse<Category>(c.Category))
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing AI recommendation: {ex.Message}");
                return new List<int>();
            }
        }
    }

    public class AIRecommendation
    {
        public string RecommendationReasoning { get; set; }
        public List<CategoryRecommendation> RecommendedCategories { get; set; }
    }

    public class CategoryRecommendation
    {
        public string Category { get; set; }
        public double Score { get; set; }
    }
}
