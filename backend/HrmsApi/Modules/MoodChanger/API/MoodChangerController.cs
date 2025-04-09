using System;
using System.Threading.Tasks;
using HrmsApi.Modules.MoodChanger.Application.DTOs;
using HrmsApi.Modules.MoodChanger.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HrmsApi.Modules.MoodChanger.API
{
    [ApiController]
    [Route("api/[controller]")]
    public class MoodChangerController : ControllerBase
    {
        private readonly ILogger<MoodChangerController> _logger;
        private readonly MoodAnalysisService _moodAnalysisService;

        public MoodChangerController(
            ILogger<MoodChangerController> logger,
            MoodAnalysisService moodAnalysisService)
        {
            _logger = logger;
            _moodAnalysisService = moodAnalysisService;
        }

        [HttpGet("Test")]
        public ActionResult<string> Test()
        {
            return "MoodChanger API is working!";
        }

        [HttpPost("AnalyzeMood")]
        public async Task<ActionResult<MoodResponseDto>> AnalyzeMood([FromBody] MoodInputDto input)
        {
            if (input == null)
            {
                return BadRequest("Input cannot be null");
            }

            try
            {
                // Get the user's ID if authenticated
                if (User.Identity.IsAuthenticated && !input.EmployeeId.HasValue)
                {
                    var userIdClaim = User.FindFirst("sub")?.Value;
                    if (!string.IsNullOrEmpty(userIdClaim) && Guid.TryParse(userIdClaim, out Guid userId))
                    {
                        input.EmployeeId = userId;
                    }
                }
                
                var response = await _moodAnalysisService.AnalyzeMoodAsync(input);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error analyzing mood");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }
    }
}
