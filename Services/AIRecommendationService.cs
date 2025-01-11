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
        private readonly JsonSerializerOptions _jsonOptions;

        public AIRecommendationService(IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            _groqApiKey = configuration["GroqApiKey"];
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_groqApiKey}");
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
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
                             "Consider factors like preferred categories. " +
                             "Return ONLY a valid JSON object in this exact format, nothing else: " +
                             "{ \"recommendationReasoning\": \"your reasoning here\", " +
                             "\"recommendedCategories\": [{ \"category\": \"Football\", \"score\": 0.8 }] }"
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
                        content = "You are a betting recommendation system. Analyze user betting history and provide structured recommendations. Always respond with valid JSON only."
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
            
            Console.WriteLine($"Raw Groq API Response: {responseContent}");
            
            return responseContent;
        }

        private List<int> ParseRecommendationResponse(string response)
        {
            try
            {
                var groqResponse = JsonSerializer.Deserialize<GroqApiResponse>(response, _jsonOptions);
                
                if (groqResponse?.Choices == null || groqResponse.Choices.Length == 0)
                {
                    Console.WriteLine("Error: Groq response has no choices array or it's empty");
                    Console.WriteLine($"Groq response: {response}");
                    return new List<int>();
                }

                var messageContent = groqResponse.Choices[0].Message?.Content;
                if (string.IsNullOrEmpty(messageContent))
                {
                    Console.WriteLine("Error: Message content is null or empty");
                    return new List<int>();
                }

                Console.WriteLine($"Extracted message content: {messageContent}");

                var recommendation = JsonSerializer.Deserialize<AIRecommendation>(messageContent, _jsonOptions);
                
                if (recommendation?.RecommendedCategories == null)
                {
                    Console.WriteLine("Error: Could not parse recommendation or categories are null");
                    return new List<int>();
                }

                return recommendation.RecommendedCategories
                    .OrderByDescending(c => c.Score)
                    .Select(c => (int)Enum.Parse<Category>(c.Category))
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing AI recommendation: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return new List<int>();
            }
        }
    }

    public class GroqApiResponse
    {
        public string Id { get; set; }
        public string Object { get; set; }
        public long Created { get; set; }
        public string Model { get; set; }
        public Choice[] Choices { get; set; }
        public Usage Usage { get; set; }
    }

    public class Choice
    {
        public int Index { get; set; }
        public Message Message { get; set; }
        public object LogProbs { get; set; }
        public string FinishReason { get; set; }
    }

    public class Message
    {
        public string Role { get; set; }
        public string Content { get; set; }
    }

    public class Usage
    {
        public int PromptTokens { get; set; }
        public int CompletionTokens { get; set; }
        public int TotalTokens { get; set; }
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
