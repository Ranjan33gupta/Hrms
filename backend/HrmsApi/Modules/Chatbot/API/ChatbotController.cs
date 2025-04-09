using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using HrmsApi.Modules.Chatbot.Application;
using HrmsApi.Modules.Chatbot.Application.DTOs;
using HrmsApi.Modules.Chatbot.Application.Services;
using HrmsApi.Modules.Chatbot.Domain;

namespace HrmsApi.Modules.Chatbot.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatbotController : ControllerBase
    {
        private readonly IChatbotService _chatbotService;
        private readonly ILogger<ChatbotController> _logger;
        private readonly IntentRecognitionService _intentRecognitionService;

        public ChatbotController(
            IChatbotService chatbotService,
            ILogger<ChatbotController> logger,
            IntentRecognitionService intentRecognitionService)
        {
            _chatbotService = chatbotService;
            _logger = logger;
            _intentRecognitionService = intentRecognitionService;
        }

        // POST: api/Chatbot/ProcessQuery
        [HttpPost("ProcessQuery")]
        public async Task<ActionResult<ChatbotResponseDto>> ProcessQuery([FromBody] ChatbotQueryDto query)
        {
            if (query == null)
            {
                return BadRequest("Query cannot be null");
            }

            if (string.IsNullOrEmpty(query.Query) && string.IsNullOrEmpty(query.Message))
            {
                return BadRequest("Query text cannot be empty");
            }

            try
            {
                // Initialize the intent recognition service if needed
                await _intentRecognitionService.InitializeAsync();

                // Get the user's role (or use a default if not authenticated)
                string userRole = User.Identity.IsAuthenticated ?
                    User.FindFirst("Role")?.Value ?? "Guest" :
                    "Guest";

                // Process the query
                var response = await _chatbotService.ProcessQueryAsync(query, userRole);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing chatbot query");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        // POST: api/Chatbot/Query (main endpoint used by the frontend)
        [HttpPost("Query")]
        public async Task<ActionResult<ChatbotResponseDto>> QueryBackwardCompatibility([FromBody] object rawQuery)
        {
            try
            {
                // Log the received query for debugging
                _logger.LogInformation($"Received query: {System.Text.Json.JsonSerializer.Serialize(rawQuery)}");

                // Extract query text from the dynamic object
                string queryText = "";
                Guid? employeeId = null;
                Guid? conversationId = null;

                // Try to parse the dynamic object
                try {
                    var queryObj = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(System.Text.Json.JsonSerializer.Serialize(rawQuery));

                    if (queryObj.ContainsKey("query") && queryObj["query"] != null)
                    {
                        queryText = queryObj["query"].ToString();
                    }
                    else if (queryObj.ContainsKey("message") && queryObj["message"] != null)
                    {
                        queryText = queryObj["message"].ToString();
                    }

                    if (queryObj.ContainsKey("employeeId") && queryObj["employeeId"] != null)
                    {
                        if (Guid.TryParse(queryObj["employeeId"].ToString(), out Guid empId))
                        {
                            employeeId = empId;
                        }
                    }

                    if (queryObj.ContainsKey("conversationId") && queryObj["conversationId"] != null)
                    {
                        if (Guid.TryParse(queryObj["conversationId"].ToString(), out Guid convId))
                        {
                            conversationId = convId;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error parsing query object");
                }

                // If we couldn't extract a query, return an error message
                if (string.IsNullOrEmpty(queryText))
                {
                    return Ok(new ChatbotResponseDto
                    {
                        Response = "I didn't understand your message. Could you try again?",
                        IntentName = "error",
                        Confidence = 0,
                        ConversationId = conversationId ?? Guid.NewGuid()
                    });
                }

                // Create a proper query DTO
                var queryDto = new ChatbotQueryDto
                {
                    Query = queryText,
                    EmployeeId = employeeId,
                    ConversationId = conversationId
                };

                // Get the user's role
                string userRole = User.Identity.IsAuthenticated ?
                    User.FindFirst("Role")?.Value ?? "Guest" :
                    "Guest";

                // Process the query using the chatbot service
                try
                {
                    await _intentRecognitionService.InitializeAsync();
                    var response = await _chatbotService.ProcessQueryAsync(queryDto, userRole);
                    return Ok(response);
                }
                catch
                {
                    // If the service fails, fall back to a simple response
                    return Ok(new ChatbotResponseDto
                    {
                        Response = GetFallbackResponse(queryText),
                        IntentName = "fallback",
                        Confidence = 0.5,
                        ConversationId = conversationId ?? Guid.NewGuid()
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in chatbot query endpoint");
                return Ok(new ChatbotResponseDto
                {
                    Response = "Sorry, I'm having trouble connecting to the server. Please try again later.",
                    IntentName = "error",
                    Confidence = 0,
                    ConversationId = Guid.NewGuid()
                });
            }
        }

        [HttpPost("Voice")]
        public async Task<ActionResult<ChatbotResponseDto>> ProcessVoiceCommand([FromBody] ChatbotQueryDto query)
        {
            try
            {
                if (query.VoiceData == null || query.VoiceData.Length == 0)
                {
                    return BadRequest("Voice data is required");
                }

                query.IsVoiceCommand = true;

                string userRole = User.Identity.IsAuthenticated
                    ? User.FindFirst("Role")?.Value ?? "Employee"
                    : "Anonymous";

                var response = await _chatbotService.ProcessQueryAsync(query, userRole);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing voice command");
                return StatusCode(500, "An error occurred while processing your voice command.");
            }
        }

        [HttpGet("Conversation/{id}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<ChatbotMessage>>> GetConversationHistory(Guid id)
        {
            try
            {
                var messages = await _chatbotService.GetConversationHistoryAsync(id);
                return Ok(messages);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving conversation history");
                return StatusCode(500, "An error occurred while retrieving the conversation history.");
            }
        }

        [HttpGet("UserConversations/{employeeId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<ChatbotConversation>>> GetUserConversations(Guid employeeId)
        {
            try
            {
                // Only allow admins or the employee themselves to access their conversations
                if (!User.IsInRole("Admin") && User.FindFirst("EmployeeId")?.Value != employeeId.ToString())
                {
                    return Forbid();
                }

                var conversations = await _chatbotService.GetUserConversationsAsync(employeeId);
                return Ok(conversations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user conversations");
                return StatusCode(500, "An error occurred while retrieving the user conversations.");
            }
        }

        // POST: api/Chatbot/TestQuery
        [HttpPost("TestQuery")]
        public ActionResult<object> TestQuery([FromBody] object query)
        {
            try
            {
                // Log the received query for debugging
                _logger.LogInformation($"Received query: {System.Text.Json.JsonSerializer.Serialize(query)}");

                // Return a simple response for testing
                return Ok(new {
                    message = "This is a test response from the chatbot API",
                    response = "This is a test response from the chatbot API",
                    intentName = "test",
                    confidence = 1.0,
                    conversationId = Guid.NewGuid(),
                    requiresAuth = false
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in test query endpoint");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // Helper method to generate fallback responses
        private string GetFallbackResponse(string query)
        {
            // Simple keyword-based responses
            query = query.ToLower();

            if (query.Contains("leave") || query.Contains("vacation") || query.Contains("time off"))
            {
                return "To request leave, go to the 'Request Leave' section in the sidebar. You can select dates and provide a reason for your leave request.";
            }

            if (query.Contains("attendance") || query.Contains("clock in") || query.Contains("clock out"))
            {
                return "You can view your attendance records in the 'Attendance' section. To clock in or out, use the buttons on your dashboard.";
            }

            if (query.Contains("salary") || query.Contains("pay") || query.Contains("payroll"))
            {
                return "Payroll information can be found in the 'Payroll' section. If you have specific questions about your salary, please contact HR.";
            }

            if (query.Contains("profile") || query.Contains("account") || query.Contains("my info"))
            {
                return "You can view and update your profile information by clicking on your user icon in the top right and selecting 'Profile'.";
            }

            if (query.Contains("help") || query.Contains("support") || query.Contains("assistance"))
            {
                return "I'm here to help! You can ask me about leave requests, attendance, payroll, or your profile. If you need more assistance, please contact the HR department.";
            }

            // Default fallback response
            return "I'm not sure I understand. You can ask me about leave requests, attendance, payroll, or your profile information.";
        }

        // GET: api/Chatbot/Test
        [HttpGet("Test")]
        public ActionResult<string> Test()
        {
            return Ok("Chatbot API is working!");
        }

        #region Admin Endpoints

        [HttpGet("Intents")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<ChatbotIntent>>> GetAllIntents()
        {
            try
            {
                var intents = await _chatbotService.GetAllIntentsAsync();
                return Ok(intents);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving intents");
                return StatusCode(500, "An error occurred while retrieving the intents.");
            }
        }

        [HttpGet("Intent/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ChatbotIntent>> GetIntentById(Guid id)
        {
            try
            {
                var intent = await _chatbotService.GetIntentByIdAsync(id);
                if (intent == null)
                {
                    return NotFound();
                }
                return Ok(intent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving intent");
                return StatusCode(500, "An error occurred while retrieving the intent.");
            }
        }

        [HttpPost("Intent")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ChatbotIntent>> CreateIntent([FromBody] ChatbotIntent intent)
        {
            try
            {
                var createdIntent = await _chatbotService.CreateIntentAsync(intent);
                return CreatedAtAction(nameof(GetIntentById), new { id = createdIntent.Id }, createdIntent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating intent");
                return StatusCode(500, "An error occurred while creating the intent.");
            }
        }

        [HttpPut("Intent/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateIntent(Guid id, [FromBody] ChatbotIntent intent)
        {
            try
            {
                if (id != intent.Id)
                {
                    return BadRequest("Intent ID mismatch");
                }

                var success = await _chatbotService.UpdateIntentAsync(intent);
                if (!success)
                {
                    return NotFound();
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating intent");
                return StatusCode(500, "An error occurred while updating the intent.");
            }
        }

        [HttpDelete("Intent/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteIntent(Guid id)
        {
            try
            {
                var success = await _chatbotService.DeleteIntentAsync(id);
                if (!success)
                {
                    return NotFound();
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting intent");
                return StatusCode(500, "An error occurred while deleting the intent.");
            }
        }

        [HttpPost("Train")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> TrainChatbot()
        {
            try
            {
                var success = await _chatbotService.TrainChatbotAsync();
                if (!success)
                {
                    return StatusCode(500, "Failed to train the chatbot");
                }
                return Ok("Chatbot trained successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error training chatbot");
                return StatusCode(500, "An error occurred while training the chatbot.");
            }
        }

        #endregion
    }
}
