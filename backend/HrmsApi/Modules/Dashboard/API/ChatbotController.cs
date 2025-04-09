using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HrmsApi.Data;
using HrmsApi.Modules.Dashboard.Domain;
using HrmsApi.Modules.Dashboard.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace HrmsApi.Modules.Dashboard.API
{
    [Route("api/Dashboard/Chatbot")]
    [ApiController]
    public class ChatbotController : ControllerBase
    {
        private readonly HrmsDbContext _context;

        public ChatbotController(HrmsDbContext context)
        {
            _context = context;
        }

        // POST: api/Dashboard/Chatbot/Query
        [HttpPost("Query")]
        public async Task<ActionResult<ChatbotResponseDTO>> ProcessQuery([FromBody] ChatbotQueryDTO query)
        {
            try
            {
                if (string.IsNullOrEmpty(query.Message))
                {
                    return BadRequest("Message cannot be empty");
                }

                // Get all intents with their training phrases
                var intents = await _context.DashboardChatbotIntents
                    .Include(i => i.TrainingPhrases)
                    .Include(i => i.Entities)
                    .ToListAsync();

                // Find the best matching intent
                var bestMatch = FindBestMatchingIntent(query.Message, intents);
                
                if (bestMatch.Intent == null)
                {
                    // No matching intent found
                    return Ok(new ChatbotResponseDTO
                    {
                        Message = "I'm sorry, I don't understand that. Could you please rephrase your question?",
                        Intent = "fallback",
                        Confidence = 0,
                        RequiresAction = false
                    });
                }

                // Extract entities from the query if needed
                var extractedEntities = ExtractEntities(query.Message, bestMatch.Intent.Entities);

                // Generate response based on the intent and extracted entities
                string response = GenerateResponse(bestMatch.Intent, extractedEntities, query.EmployeeId);

                // Check if we need to call an API endpoint
                bool requiresAction = !string.IsNullOrEmpty(bestMatch.Intent.ApiEndpoint);

                return Ok(new ChatbotResponseDTO
                {
                    Message = response,
                    Intent = bestMatch.Intent.Name,
                    Confidence = bestMatch.Confidence,
                    RequiresAction = requiresAction,
                    ApiEndpoint = bestMatch.Intent.ApiEndpoint,
                    Entities = extractedEntities
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Helper method to find the best matching intent
        private (ChatbotIntent? Intent, double Confidence) FindBestMatchingIntent(string query, List<ChatbotIntent> intents)
        {
            ChatbotIntent? bestIntent = null;
            double highestConfidence = 0.3; // Minimum threshold for confidence

            foreach (var intent in intents)
            {
                foreach (var phrase in intent.TrainingPhrases)
                {
                    double similarity = CalculateSimilarity(query.ToLower(), phrase.Phrase.ToLower());
                    
                    if (similarity > highestConfidence)
                    {
                        highestConfidence = similarity;
                        bestIntent = intent;
                    }
                }
            }

            return (bestIntent, highestConfidence);
        }

        // Simple similarity calculation (can be replaced with more sophisticated NLP)
        private double CalculateSimilarity(string query, string trainingPhrase)
        {
            // Simple word overlap similarity
            var queryWords = query.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var phraseWords = trainingPhrase.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            
            int matches = queryWords.Count(qw => phraseWords.Any(pw => pw.Contains(qw) || qw.Contains(pw)));
            
            return (double)matches / Math.Max(queryWords.Length, phraseWords.Length);
        }

        // Extract entities from the query
        private Dictionary<string, string> ExtractEntities(string query, ICollection<ChatbotEntity> entities)
        {
            var result = new Dictionary<string, string>();
            
            foreach (var entity in entities)
            {
                switch (entity.Type.ToLower())
                {
                    case "date":
                        // Simple date extraction (can be improved with NLP)
                        var dateMatch = Regex.Match(query, @"\d{1,2}[-/]\d{1,2}[-/]\d{2,4}|\d{1,2}\s+(?:Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec)[a-z]*\s+\d{2,4}");
                        if (dateMatch.Success)
                        {
                            result[entity.Name] = dateMatch.Value;
                        }
                        break;
                        
                    case "number":
                        // Extract numbers
                        var numberMatch = Regex.Match(query, @"\d+");
                        if (numberMatch.Success)
                        {
                            result[entity.Name] = numberMatch.Value;
                        }
                        break;
                        
                    // Add more entity types as needed
                }
            }
            
            return result;
        }

        // Generate a response based on the intent and entities
        private string GenerateResponse(ChatbotIntent intent, Dictionary<string, string> entities, Guid employeeId)
        {
            string response = intent.ResponseTemplate;
            
            // Replace entity placeholders in the response template
            foreach (var entity in entities)
            {
                response = response.Replace($"{{{entity.Key}}}", entity.Value);
            }
            
            // Replace {employeeId} with the actual employee ID if present
            response = response.Replace("{employeeId}", employeeId.ToString());
            
            return response;
        }

        // GET: api/Dashboard/Chatbot/Intents
        [HttpGet("Intents")]
        public async Task<ActionResult<IEnumerable<ChatbotIntent>>> GetIntents()
        {
            return await _context.DashboardChatbotIntents
                .Include(i => i.TrainingPhrases)
                .Include(i => i.Entities)
                .ToListAsync();
        }

        // POST: api/Dashboard/Chatbot/Intents
        [HttpPost("Intents")]
        public async Task<ActionResult<ChatbotIntent>> CreateIntent([FromBody] CreateIntentDTO intentDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var intent = new ChatbotIntent
            {
                Id = Guid.NewGuid(),
                Name = intentDto.Name,
                Description = intentDto.Description,
                ResponseTemplate = intentDto.ResponseTemplate,
                ApiEndpoint = intentDto.ApiEndpoint,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            };

            _context.DashboardChatbotIntents.Add(intent);
            
            // Add training phrases
            foreach (var phrase in intentDto.TrainingPhrases)
            {
                _context.DashboardChatbotTrainingPhrases.Add(new ChatbotTrainingPhrase
                {
                    Id = Guid.NewGuid(),
                    IntentId = intent.Id,
                    Phrase = phrase,
                    CreatedAt = DateTime.UtcNow
                });
            }
            
            // Add entities
            foreach (var entity in intentDto.Entities)
            {
                _context.DashboardChatbotEntities.Add(new ChatbotEntity
                {
                    Id = Guid.NewGuid(),
                    IntentId = intent.Id,
                    Name = entity.Name,
                    Type = entity.Type,
                    Description = entity.Description,
                    CreatedAt = DateTime.UtcNow
                });
            }

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetIntents), new { id = intent.Id }, intent);
        }
    }
}
