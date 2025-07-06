using Microsoft.AspNetCore.Mvc;
using MyDotNetProject.Models;
using MyDotNetProject.Services;

namespace MyDotNetProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventsController(IEventService eventService)
        {
            _eventService = eventService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Event>> GetEvent(string id)
        {
            var eventData = await _eventService.GetEventByIdAsync(id);
            if (eventData == null)
            {
                return NotFound();
            }
            return Ok(eventData);
        }

        [HttpGet("group/{groupId}")]
        public async Task<ActionResult<IEnumerable<Event>>> GetGroupEvents(string groupId)
        {
            var events = await _eventService.GetGroupEventsAsync(groupId);
            return Ok(events);
        }

        [HttpPost]
        public async Task<ActionResult<Event>> CreateEvent([FromBody] Event eventData)
        {
            try
            {
                var createdEvent = await _eventService.CreateEventAsync(eventData);
                return CreatedAtAction(nameof(GetEvent), new { id = createdEvent.Id }, createdEvent);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Event>> UpdateEvent(string id, [FromBody] Event eventData)
        {
            if (id != eventData.Id)
            {
                return BadRequest("ID mismatch");
            }

            try
            {
                var updatedEvent = await _eventService.UpdateEventAsync(eventData);
                return Ok(updatedEvent);
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
        public async Task<ActionResult> DeleteEvent(string id)
        {
            var result = await _eventService.DeleteEventAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpGet("{eventId}/matches")]
        public async Task<ActionResult<IEnumerable<Match>>> GetEventMatches(string eventId)
        {
            var matches = await _eventService.GetEventMatchesAsync(eventId);
            return Ok(matches);
        }

        [HttpGet("matches/{matchId}")]
        public async Task<ActionResult<Match>> GetMatch(string matchId)
        {
            var match = await _eventService.GetMatchByIdAsync(matchId);
            if (match == null)
            {
                return NotFound();
            }
            return Ok(match);
        }

        [HttpPost("matches")]
        public async Task<ActionResult<Match>> CreateMatch([FromBody] Match match)
        {
            try
            {
                var createdMatch = await _eventService.CreateMatchAsync(match);
                return CreatedAtAction(nameof(GetMatch), new { matchId = createdMatch.Id }, createdMatch);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("matches/{matchId}")]
        public async Task<ActionResult<Match>> UpdateMatch(string matchId, [FromBody] Match match)
        {
            if (matchId != match.Id)
            {
                return BadRequest("ID mismatch");
            }

            try
            {
                var updatedMatch = await _eventService.UpdateMatchAsync(match);
                return Ok(updatedMatch);
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

        [HttpDelete("matches/{matchId}")]
        public async Task<ActionResult> DeleteMatch(string matchId)
        {
            var result = await _eventService.DeleteMatchAsync(matchId);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
