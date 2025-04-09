using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HrmsApi.Modules.Chatbot.Domain.Entities;
using HrmsApi.Modules.Chatbot.Infrastructure.Repositories;

namespace HrmsApi.Modules.Chatbot.Application.Services
{
    public class IntentRecognitionService
    {
        private readonly ChatbotIntentRepository _intentRepository;
        private List<ChatbotIntent> _intents;
        private Dictionary<string, List<string>> _intentPhrases;
        private bool _isInitialized = false;

        public IntentRecognitionService(ChatbotIntentRepository intentRepository)
        {
            _intentRepository = intentRepository;
            _intentPhrases = new Dictionary<string, List<string>>();
        }

        public async Task InitializeAsync()
        {
            if (_isInitialized)
                return;

            _intents = await _intentRepository.GetAllWithTrainingPhrasesAsync();
            
            foreach (var intent in _intents)
            {
                _intentPhrases[intent.Name] = intent.TrainingPhrases.Select(p => p.Phrase.ToLower()).ToList();
            }
            
            _isInitialized = true;
        }

        public async Task<(string intentName, double confidence, Dictionary<string, string> entities)> RecognizeIntentAsync(string query)
        {
            if (!_isInitialized)
                await InitializeAsync();

            if (string.IsNullOrWhiteSpace(query))
                return ("none", 0.0, new Dictionary<string, string>());

            query = query.ToLower().Trim();
            
            // Extract potential entities
            var entities = ExtractEntities(query);
            
            // Calculate similarity scores for each intent
            var scores = new Dictionary<string, double>();
            
            foreach (var intent in _intents)
            {
                double bestScore = 0;
                
                foreach (var phrase in _intentPhrases[intent.Name])
                {
                    double similarity = CalculateSimilarity(query, phrase);
                    if (similarity > bestScore)
                    {
                        bestScore = similarity;
                    }
                }
                
                scores[intent.Name] = bestScore;
            }
            
            // Find the intent with the highest score
            var bestMatch = scores.OrderByDescending(s => s.Value).First();
            
            // Only return an intent if the confidence is above threshold
            if (bestMatch.Value >= 0.6)
            {
                return (bestMatch.Key, bestMatch.Value, entities);
            }
            
            return ("none", bestMatch.Value, entities);
        }

        private Dictionary<string, string> ExtractEntities(string query)
        {
            var entities = new Dictionary<string, string>();
            
            // Extract dates (simple patterns for now)
            var datePattern = @"(?:from|on|between)\s+(\d{1,2}(?:st|nd|rd|th)?\s+(?:January|February|March|April|May|June|July|August|September|October|November|December)|tomorrow|yesterday|today|next\s+(?:Monday|Tuesday|Wednesday|Thursday|Friday|Saturday|Sunday))";
            var dateMatch = Regex.Match(query, datePattern, RegexOptions.IgnoreCase);
            
            if (dateMatch.Success)
            {
                entities["date"] = dateMatch.Groups[1].Value;
            }
            
            // Extract date ranges
            var dateRangePattern = @"from\s+(\d{1,2}(?:st|nd|rd|th)?\s+(?:to|till|until)\s+\d{1,2}(?:st|nd|rd|th)?\s+(?:January|February|March|April|May|June|July|August|September|October|November|December))";
            var dateRangeMatch = Regex.Match(query, dateRangePattern, RegexOptions.IgnoreCase);
            
            if (dateRangeMatch.Success)
            {
                entities["date_range"] = dateRangeMatch.Groups[1].Value;
            }
            
            // Extract leave types
            var leaveTypePattern = @"(casual|sick|annual|personal|emergency|maternity|paternity|compensatory)\s+leave";
            var leaveTypeMatch = Regex.Match(query, leaveTypePattern, RegexOptions.IgnoreCase);
            
            if (leaveTypeMatch.Success)
            {
                entities["leave_type"] = leaveTypeMatch.Groups[1].Value;
            }
            
            return entities;
        }

        private double CalculateSimilarity(string s1, string s2)
        {
            // Simple Jaccard similarity for now
            // In a production environment, you would use a more sophisticated algorithm
            // or a machine learning model for intent recognition
            
            var set1 = new HashSet<string>(s1.Split(' ', StringSplitOptions.RemoveEmptyEntries));
            var set2 = new HashSet<string>(s2.Split(' ', StringSplitOptions.RemoveEmptyEntries));
            
            if (!set1.Any() || !set2.Any())
                return 0;
                
            var intersection = set1.Intersect(set2).Count();
            var union = set1.Union(set2).Count();
            
            return (double)intersection / union;
        }
    }
}
