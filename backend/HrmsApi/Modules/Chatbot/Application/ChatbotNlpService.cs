using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HrmsApi.Modules.Chatbot.Domain;
using HrmsApi.Modules.Chatbot.Infrastructure;
using Microsoft.Extensions.Logging;
using System.Text;

namespace HrmsApi.Modules.Chatbot.Application
{
    public class ChatbotNlpService : IChatbotNlpService
    {
        private readonly IChatbotRepository _chatbotRepository;
        private readonly ILogger<ChatbotNlpService> _logger;
        private Dictionary<string, List<(string phrase, Guid intentId)>> _trainingData;
        private bool _isModelTrained = false;

        public ChatbotNlpService(
            IChatbotRepository chatbotRepository,
            ILogger<ChatbotNlpService> logger)
        {
            _chatbotRepository = chatbotRepository;
            _logger = logger;
            _trainingData = new Dictionary<string, List<(string, Guid)>>();
        }

        public async Task<(ChatbotIntent intent, double confidence)> RecognizeIntentAsync(string query, string userRole)
        {
            if (!_isModelTrained)
            {
                await TrainModelAsync();
            }

            // Normalize the query
            string normalizedQuery = NormalizeText(query);

            // Calculate similarity scores for each intent
            var scores = new Dictionary<Guid, double>();
            double highestScore = 0;
            Guid? bestMatchIntentId = null;

            foreach (var roleGroup in _trainingData)
            {
                // Skip intents that require a specific role if the user doesn't have it
                if (roleGroup.Key != "all" && roleGroup.Key != userRole)
                    continue;

                foreach (var (phrase, intentId) in roleGroup.Value)
                {
                    double similarity = CalculateSimilarity(normalizedQuery, phrase);
                    
                    if (!scores.ContainsKey(intentId) || similarity > scores[intentId])
                    {
                        scores[intentId] = similarity;
                    }

                    if (similarity > highestScore)
                    {
                        highestScore = similarity;
                        bestMatchIntentId = intentId;
                    }
                }
            }

            // If we have a match with confidence above threshold
            if (bestMatchIntentId.HasValue && highestScore >= 0.6)
            {
                var intent = await _chatbotRepository.GetIntentByIdAsync(bestMatchIntentId.Value);
                return (intent, highestScore);
            }

            // Fallback to general help intent
            var fallbackIntent = await _chatbotRepository.GetIntentByNameAsync("help");
            return (fallbackIntent, 0.0);
        }

        public async Task<string> GenerateResponseAsync(ChatbotIntent intent, string query, Guid? employeeId)
        {
            if (intent == null)
            {
                return "I'm sorry, I didn't understand that. Can you please rephrase your question?";
            }

            // Get all responses for this intent
            var responses = await _chatbotRepository.GetResponsesByIntentIdAsync(intent.Id);
            
            if (responses == null || !responses.Any())
            {
                return "I understand you're asking about " + intent.Name + ", but I don't have a specific answer for that yet.";
            }

            // Select a response (prioritize by priority value, then random if multiple with same priority)
            var highestPriority = responses.Max(r => r.Priority);
            var highestPriorityResponses = responses.Where(r => r.Priority == highestPriority).ToList();
            
            // Pick a random response from the highest priority ones
            var random = new Random();
            var selectedResponse = highestPriorityResponses[random.Next(highestPriorityResponses.Count)];
            
            return selectedResponse.Response;
        }

        public async Task<bool> TrainModelAsync()
        {
            try
            {
                _trainingData.Clear();
                
                // Initialize role-based groups
                _trainingData["all"] = new List<(string, Guid)>(); // For intents available to all roles
                _trainingData["Admin"] = new List<(string, Guid)>();
                _trainingData["Manager"] = new List<(string, Guid)>();
                _trainingData["Employee"] = new List<(string, Guid)>();

                // Get all intents with their training phrases
                var intents = await _chatbotRepository.GetAllIntentsAsync();
                
                foreach (var intent in intents)
                {
                    foreach (var phrase in intent.TrainingPhrases)
                    {
                        string normalizedPhrase = NormalizeText(phrase.Phrase);
                        
                        // Add to appropriate role group
                        if (string.IsNullOrEmpty(intent.RequiredRole))
                        {
                            _trainingData["all"].Add((normalizedPhrase, intent.Id));
                        }
                        else
                        {
                            if (_trainingData.ContainsKey(intent.RequiredRole))
                            {
                                _trainingData[intent.RequiredRole].Add((normalizedPhrase, intent.Id));
                            }
                        }
                    }
                }

                _isModelTrained = true;
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error training chatbot model");
                return false;
            }
        }

        public async Task<string> ProcessVoiceCommandAsync(byte[] audioData, string userRole, Guid? employeeId)
        {
            try
            {
                // In a real implementation, this would use a speech-to-text service
                // For now, we'll simulate it by returning a placeholder
                string transcribedText = "This is a simulated voice transcription";
                
                // Process the transcribed text like a normal text query
                var (intent, confidence) = await RecognizeIntentAsync(transcribedText, userRole);
                return await GenerateResponseAsync(intent, transcribedText, employeeId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing voice command");
                return "Sorry, I couldn't process your voice command. Please try again or type your question.";
            }
        }

        #region Helper Methods

        private string NormalizeText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            // Convert to lowercase
            string normalized = text.ToLowerInvariant();
            
            // Remove punctuation
            normalized = Regex.Replace(normalized, @"[^\w\s]", "");
            
            // Remove extra whitespace
            normalized = Regex.Replace(normalized, @"\s+", " ").Trim();
            
            return normalized;
        }

        private double CalculateSimilarity(string text1, string text2)
        {
            // Simple implementation of cosine similarity based on word overlap
            if (string.IsNullOrEmpty(text1) || string.IsNullOrEmpty(text2))
                return 0;

            var words1 = text1.Split(' ').ToHashSet();
            var words2 = text2.Split(' ').ToHashSet();
            
            int intersection = words1.Intersect(words2).Count();
            double similarity = (double)intersection / Math.Sqrt(words1.Count * words2.Count);
            
            return similarity;
        }

        #endregion
    }
}
