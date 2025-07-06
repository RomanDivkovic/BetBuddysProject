using Microsoft.AspNetCore.Mvc;
using MyDotNetProject.Models;
using MyDotNetProject.Services;

namespace MyDotNetProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PredictionsController : ControllerBase
    {
        private readonly IPredictionService _predictionService;

        public PredictionsController(IPredictionService predictionService)
        {
            _predictionService = predictionService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Prediction>> GetPrediction(string id)
        {
            var prediction = await _predictionService.GetPredictionByIdAsync(id);
            if (prediction == null)
            {
                return NotFound();
            }
            return Ok(prediction);
        }

        [HttpGet("user/{userId}/event/{eventId}")]
        public async Task<ActionResult<IEnumerable<Prediction>>> GetUserPredictions(string userId, string eventId)
        {
            var predictions = await _predictionService.GetUserPredictionsAsync(userId, eventId);
            return Ok(predictions);
        }

        [HttpGet("match/{matchId}")]
        public async Task<ActionResult<IEnumerable<Prediction>>> GetMatchPredictions(string matchId)
        {
            var predictions = await _predictionService.GetMatchPredictionsAsync(matchId);
            return Ok(predictions);
        }

        [HttpPost]
        public async Task<ActionResult<Prediction>> CreatePrediction([FromBody] Prediction prediction)
        {
            try
            {
                var createdPrediction = await _predictionService.CreatePredictionAsync(prediction);
                return CreatedAtAction(nameof(GetPrediction), new { id = createdPrediction.Id }, createdPrediction);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Prediction>> UpdatePrediction(string id, [FromBody] Prediction prediction)
        {
            if (id != prediction.Id)
            {
                return BadRequest("ID mismatch");
            }

            try
            {
                var updatedPrediction = await _predictionService.UpdatePredictionAsync(prediction);
                return Ok(updatedPrediction);
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePrediction(string id)
        {
            var result = await _predictionService.DeletePredictionAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpPost("matches/{matchId}/score")]
        public async Task<ActionResult> ScorePredictions(string matchId, [FromBody] ScoreRequest request)
        {
            try
            {
                var result = await _predictionService.ScorePredictionsAsync(matchId, request.WinnerId, request.Method);
                if (!result)
                {
                    return BadRequest("Failed to score predictions");
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

    public class ScoreRequest
    {
        public string WinnerId { get; set; } = string.Empty;
        public string Method { get; set; } = string.Empty;
    }
}
