using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HrmsApi.Modules.MoodChanger.Application.DTOs;
using HrmsApi.Modules.MoodChanger.Domain.Entities;
using HrmsApi.Modules.MoodChanger.Domain.Enums;
using HrmsApi.Modules.MoodChanger.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;

namespace HrmsApi.Modules.MoodChanger.Application.Services
{
    public class MoodAnalysisService
    {
        private readonly MoodEntryRepository _moodEntryRepository;
        private readonly ILogger<MoodAnalysisService> _logger;
        private readonly Dictionary<MoodType, List<string>> _moodResponses;
        private readonly Dictionary<MoodType, string> _moodColors;
        private readonly Dictionary<MoodType, string> _moodEmojis;

        public MoodAnalysisService(
            MoodEntryRepository moodEntryRepository,
            ILogger<MoodAnalysisService> logger)
        {
            _moodEntryRepository = moodEntryRepository;
            _logger = logger;
            
            // Initialize mood responses
            _moodResponses = new Dictionary<MoodType, List<string>>
            {
                { MoodType.Happy, new List<string> {
                    "Awesome energy today! Keep inspiring others. 🚀",
                    "Your positive energy is contagious! Keep spreading that joy around you.",
                    "Happiness looks great on you! Enjoy this wonderful feeling.",
                    "That's the spirit! Your positive attitude will make today even better.",
                    "Love that positivity! Remember this feeling and carry it with you all day."
                }},
                { MoodType.Sad, new List<string> {
                    "Don't worry, rough mornings don't define your whole day. You got this! 💪",
                    "It's okay to feel down sometimes. Remember: this feeling is temporary.",
                    "Here's your quote: 'Every day may not be good, but there's something good in every day.'",
                    "Take a moment for yourself today. Small acts of self-care can make a big difference.",
                    "You're stronger than you think. This moment will pass, and better days are ahead."
                }},
                { MoodType.Angry, new List<string> {
                    "Take a deep breath. Count to ten. Your feelings are valid, but you control how you respond.",
                    "It's okay to feel frustrated. Try stepping away for a 5-minute break to reset.",
                    "Channel that energy into something productive. You might be surprised at what you accomplish.",
                    "Quick tip: Splash some cold water on your face or step outside for fresh air. It helps reset your mind.",
                    "Remember that this feeling is temporary. You have the power to turn your day around."
                }},
                { MoodType.Stressed, new List<string> {
                    "Breathe in deep. Here's a tip: Take a 2-minute walk, it helps clear the mind. 🌿",
                    "One thing at a time. Break down what's overwhelming you into smaller, manageable tasks.",
                    "Try this: Close your eyes and take 5 deep breaths. Small moments of calm make a difference.",
                    "Your worth isn't measured by productivity. Be kind to yourself today.",
                    "Stress is your body's way of saying you care. That's admirable, but remember to care for yourself too."
                }},
                { MoodType.Neutral, new List<string> {
                    "Today is full of possibilities! What small win can you aim for?",
                    "Sometimes a neutral start is the perfect canvas for creating a great day.",
                    "Here's a thought: Try one small thing outside your routine today. It might brighten your mood!",
                    "Neutral days are opportunities in disguise. What would make today a bit more special?",
                    "Every day is a new beginning. What would you like to accomplish today?"
                }}
            };
            
            // Initialize mood colors
            _moodColors = new Dictionary<MoodType, string>
            {
                { MoodType.Happy, "#FFD700" },    // Gold
                { MoodType.Sad, "#B0E0E6" },      // Light Blue
                { MoodType.Angry, "#FFA07A" },    // Light Salmon
                { MoodType.Stressed, "#E6E6FA" }, // Lavender
                { MoodType.Neutral, "#F0F8FF" }   // Alice Blue
            };
            
            // Initialize mood emojis
            _moodEmojis = new Dictionary<MoodType, string>
            {
                { MoodType.Happy, "🌞" },
                { MoodType.Sad, "😔" },
                { MoodType.Angry, "😤" },
                { MoodType.Stressed, "😰" },
                { MoodType.Neutral, "😐" }
            };
        }

        public async Task<MoodResponseDto> AnalyzeMoodAsync(MoodInputDto input)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(input.Input))
                {
                    return new MoodResponseDto
                    {
                        Mood = MoodType.Neutral.ToString(),
                        Response = "Thanks for sharing. You're not alone—we care about you.",
                        BackgroundColor = _moodColors[MoodType.Neutral],
                        Emoji = _moodEmojis[MoodType.Neutral],
                        Id = Guid.NewGuid()
                    };
                }

                // Analyze the mood using simple keyword matching
                // In a production environment, you would use a more sophisticated NLP model
                var detectedMood = DetectMood(input.Input);
                
                // Get a random response for the detected mood
                var random = new Random();
                var responseIndex = random.Next(0, _moodResponses[detectedMood].Count);
                var response = _moodResponses[detectedMood][responseIndex];
                
                // Create and save the mood entry
                var moodEntry = new MoodEntry
                {
                    Id = Guid.NewGuid(),
                    EmployeeId = input.IsAnonymous ? null : input.EmployeeId,
                    UserInput = input.Input,
                    DetectedMood = detectedMood.ToString(),
                    ResponseContent = response,
                    CreatedAt = DateTime.UtcNow,
                    IsAnonymous = input.IsAnonymous
                };
                
                await _moodEntryRepository.AddAsync(moodEntry);
                
                // Return the response
                return new MoodResponseDto
                {
                    Mood = detectedMood.ToString(),
                    Response = response,
                    BackgroundColor = _moodColors[detectedMood],
                    Emoji = _moodEmojis[detectedMood],
                    Id = moodEntry.Id
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error analyzing mood");
                
                // Fallback response
                return new MoodResponseDto
                {
                    Mood = MoodType.Neutral.ToString(),
                    Response = "Thanks for sharing. You're not alone—we care about you.",
                    BackgroundColor = _moodColors[MoodType.Neutral],
                    Emoji = _moodEmojis[MoodType.Neutral],
                    Id = Guid.NewGuid()
                };
            }
        }

        private MoodType DetectMood(string input)
        {
            // Convert input to lowercase for case-insensitive matching
            input = input.ToLower();
            
            // Define mood keywords
            var happyKeywords = new[] { "happy", "great", "awesome", "excellent", "joy", "excited", "wonderful", "fantastic", "good", "positive", "love", "smile", "laugh", "cheerful", "delighted" };
            var sadKeywords = new[] { "sad", "unhappy", "depressed", "down", "blue", "gloomy", "miserable", "disappointed", "upset", "heartbroken", "crying", "tears", "lonely", "hopeless", "sorrow" };
            var angryKeywords = new[] { "angry", "mad", "furious", "annoyed", "irritated", "frustrated", "rage", "hate", "upset", "temper", "outraged", "hostile", "enraged", "infuriated", "agitated" };
            var stressedKeywords = new[] { "stressed", "anxious", "worried", "nervous", "overwhelmed", "pressure", "tense", "panic", "fear", "dread", "uneasy", "restless", "concerned", "troubled", "apprehensive" };
            
            // Count occurrences of each mood type
            int happyCount = happyKeywords.Count(keyword => Regex.IsMatch(input, $"\\b{keyword}\\b"));
            int sadCount = sadKeywords.Count(keyword => Regex.IsMatch(input, $"\\b{keyword}\\b"));
            int angryCount = angryKeywords.Count(keyword => Regex.IsMatch(input, $"\\b{keyword}\\b"));
            int stressedCount = stressedKeywords.Count(keyword => Regex.IsMatch(input, $"\\b{keyword}\\b"));
            
            // Determine the dominant mood
            var moodCounts = new Dictionary<MoodType, int>
            {
                { MoodType.Happy, happyCount },
                { MoodType.Sad, sadCount },
                { MoodType.Angry, angryCount },
                { MoodType.Stressed, stressedCount }
            };
            
            var dominantMood = moodCounts.OrderByDescending(m => m.Value).First();
            
            // If no mood is detected or all counts are 0, return Neutral
            return dominantMood.Value > 0 ? dominantMood.Key : MoodType.Neutral;
        }
    }
}
