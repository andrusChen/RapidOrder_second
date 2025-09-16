using Microsoft.AspNetCore.Mvc;
using RapidOrder.Api.Services;

namespace RapidOrder.Api.Controllers
{
    [ApiController]
    [Route("api/learning-mode")]
    public class LearningModeController : ControllerBase
    {
        private readonly LearningModeService _learningModeService;

        public LearningModeController(LearningModeService learningModeService)
        {
            _learningModeService = learningModeService;
        }

        [HttpGet]
        public IActionResult GetLearningMode()
        {
            return Ok(new { enabled = _learningModeService.IsLearningModeEnabled });
        }

        [HttpPost]
        public IActionResult SetLearningMode([FromBody] SetLearningModeRequest request)
        {
            if (request.Enabled)
            {
                _learningModeService.EnableLearningMode();
            }
            else
            {
                _learningModeService.DisableLearningMode();
            }
            return Ok(new { enabled = _learningModeService.IsLearningModeEnabled });
        }
    }

    public class SetLearningModeRequest
    {
        public bool Enabled { get; set; }
    }
}
