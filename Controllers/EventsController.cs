using System.Runtime.Serialization;
using banbet.CustomExceptions;
using banbet.Data;
using banbet.Models;
using banbet.Models.DTOs;
using banbet.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace banbet.Controllers
{
    //[Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/{controller}")]
    public class EventsController: ControllerBase
    {
        private readonly EventsService _eventsService;
        private readonly RecommendationService _recommendationService;
        
        public EventsController(
            EventsService eventsService,
            RecommendationService recommendationService
            )
        {
            _eventsService = eventsService;
            _recommendationService = recommendationService;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> CreateEvent([FromBody] EventDto eventDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var newEvent = await _eventsService.CreateEvent(eventDto);
            return Ok(newEvent);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetEvents()
        {
            try {
                var eventDtos = await _eventsService.GetEvents();
                return Ok(eventDtos);
            }
            catch (Exception)
            {
                return StatusCode(500, "Błąd serwera");
            }
        }

        [HttpGet("recommended")]
        [Authorize]
        public async Task<IActionResult> GetRecommendedEvents()
        {
            try 
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                {
                    return BadRequest("Nieprawidłowy identyfikator użytkownika");
                }

                var recommendedEvents = await _recommendationService.GetRecommendedEvents(userId);
                return Ok(recommendedEvents);
            }
            catch (Exception)
            {
                return StatusCode(500, "Błąd podczas pobierania rekomendowanych wydarzeń");
            }
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetEvent(int id)
        {
            try {
                var eventDto = await _eventsService.GetEvent(id);
                return Ok(eventDto);
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            } 
            catch (Exception)
            {
                return StatusCode(500, "Błąd serwera");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent([FromRoute] int id)
        {
            try {
                await _eventsService.DeleteEvent(id);
                return Ok($"Usunieto event o id:{id}");                
            }
            catch (EntityNotFoundException ex) 
            {
                return NotFound(new { Message = ex.Message});
            }
            catch(Exception)
            {
                return StatusCode(500, "Błąd serwera");
            }
        }

        [HttpGet("Categories")]
        public IActionResult GetCategories()
        {
            var categories = _eventsService.GetCategories();
            return Ok(categories);
        }
    }
}
