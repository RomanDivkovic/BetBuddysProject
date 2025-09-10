using Microsoft.AspNetCore.Mvc;
using MyDotNetProject.Services;
using MyDotNetProject.Models;

namespace MyDotNetProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BettingController : ControllerBase
    {
        private readonly IBettingService _bettingService;
        private readonly IMmaApiService _mmaApiService;

        public BettingController(IBettingService bettingService, IMmaApiService mmaApiService)
        {
            _bettingService = bettingService;
            _mmaApiService = mmaApiService;
        }

        [HttpPost("events")]
        public async Task<ActionResult<GroupEvent>> CreateGroupEvent([FromBody] CreateGroupEventRequest request)
        {
            try
            {
                // Fetch fights from MMA API
                var mmaResponse = await _mmaApiService.FetchMmaEventsAsync(request.Date);

                // Convert MMA API response to our fight models
                var fights = new List<MmaFight>();
                if (mmaResponse.Response != null)
                {
                    foreach (var fight in mmaResponse.Response)
                    {
                        fights.Add(new MmaFight
                        {
                            Id = Guid.NewGuid().ToString(),
                            ExternalId = fight.Id.ToString(),
                            FightDate = DateTime.Parse(fight.Date ?? DateTime.UtcNow.ToString()),
                            Category = fight.Venue ?? "",
                            Status = fight.Status?.Short ?? "Not Started",
                            IsMain = false, // Set to false since IsMain is not available in API
                            Fighter1Id = fight.Fighters?.First?.Id.ToString() ?? "",
                            Fighter1Name = fight.Fighters?.First?.Name ?? "",
                            Fighter1Logo = fight.Fighters?.First?.Logo ?? "",
                            Fighter2Id = fight.Fighters?.Second?.Id.ToString() ?? "",
                            Fighter2Name = fight.Fighters?.Second?.Name ?? "",
                            Fighter2Logo = fight.Fighters?.Second?.Logo ?? ""
                        });
                    }
                }

                var groupEvent = await _bettingService.CreateGroupEventAsync(
                    request.GroupId,
                    request.EventTitle,
                    DateTime.Parse(request.Date),
                    fights
                );

                return CreatedAtAction(nameof(GetGroupEvent), new { id = groupEvent.Id }, groupEvent);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("events/{id}")]
        public async Task<ActionResult<GroupEvent>> GetGroupEvent(string id)
        {
            var groupEvent = await _bettingService.GetGroupEventByIdAsync(id);
            if (groupEvent == null)
                return NotFound();

            return Ok(groupEvent);
        }

        [HttpGet("groups/{groupId}/events")]
        public async Task<ActionResult<IEnumerable<GroupEvent>>> GetGroupEvents(string groupId)
        {
            var events = await _bettingService.GetGroupEventsAsync(groupId);
            return Ok(events);
        }

        [HttpPost("bets")]
        public async Task<ActionResult<Bet>> PlaceBet([FromBody] PlaceBetRequest request)
        {
            try
            {
                var bet = await _bettingService.PlaceBetAsync(
                    request.UserId,
                    request.GroupEventId,
                    request.FightId,
                    request.PredictedWinner,
                    request.Method,
                    request.Confidence
                );

                return Ok(bet);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("events/{eventId}/bets")]
        public async Task<ActionResult<IEnumerable<Bet>>> GetEventBets(string eventId)
        {
            var bets = await _bettingService.GetGroupEventBetsAsync(eventId);
            return Ok(bets);
        }

        [HttpGet("users/{userId}/events/{eventId}/bets")]
        public async Task<ActionResult<IEnumerable<Bet>>> GetUserBets(string userId, string eventId)
        {
            var bets = await _bettingService.GetUserBetsAsync(userId, eventId);
            return Ok(bets);
        }
    }

    public class CreateGroupEventRequest
    {
        public required string GroupId { get; set; }
        public required string EventTitle { get; set; }
        public required string Date { get; set; } // YYYY-MM-DD format
    }

    public class PlaceBetRequest
    {
        public required string UserId { get; set; }
        public required string GroupEventId { get; set; }
        public required string FightId { get; set; }
        public required string PredictedWinner { get; set; }
        public required string Method { get; set; }
        public int Confidence { get; set; } = 50;
    }
}
