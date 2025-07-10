using Microsoft.AspNetCore.Mvc;
using MyDotNetProject.Services;

namespace MyDotNetProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MmaController : ControllerBase
    {
        private readonly IMmaApiService _mmaApiService;

        public MmaController(IMmaApiService mmaApiService)
        {
            _mmaApiService = mmaApiService;
        }

        [HttpGet("fights")]
        public async Task<ActionResult> GetFights([FromQuery] string date)
        {
            if (string.IsNullOrEmpty(date))
            {
                return BadRequest("Date parameter is required");
            }

            try
            {
                var response = await _mmaApiService.FetchMmaEventsAsync(date);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("fights/{date}")]
        public async Task<ActionResult> GetFightsByDate(string date)
        {
            try
            {
                var response = await _mmaApiService.FetchMmaEventsAsync(date);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
