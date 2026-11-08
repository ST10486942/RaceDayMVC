using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Data;
using RaceDayAPI.Models;

namespace RaceDayAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EventsController : ControllerBase
{
    private readonly AppDbContext _context;

    public EventsController(AppDbContext context)
    {
        _context = context;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var events = await _context.Events
            .AsNoTracking()
            .Select(e => new
            {
                e.EventId,
                e.OrganiserId,
                e.DestinationId,
                e.Name,
                e.Description,
                e.EventDate,
                e.StartTime,
                e.EndTime,
                e.Status,
                e.RegistrationDeadline,
                e.CreatedAt
            })
            .ToListAsync();

        return Ok(events);
    }

    [AllowAnonymous]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var eventItem = await _context.Events
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.EventId == id);

        if (eventItem == null)
            return NotFound("Event not found.");

        return Ok(eventItem);
    }

    [Authorize(Roles = "Organiser")]
    [HttpPost]
    public async Task<IActionResult> Create(Event eventItem)
    {
        bool organiserExists = await _context.Users.AnyAsync(
            u => u.UserId == eventItem.OrganiserId &&
                 u.Role == "Organiser");

        if (!organiserExists)
            return BadRequest(
                "OrganiserId must identify an existing organiser.");

        bool destinationExists = await _context.Destinations
            .AnyAsync(d => d.DestinationId == eventItem.DestinationId);

        if (!destinationExists)
            return BadRequest("DestinationId does not exist.");

        eventItem.CreatedAt = DateTime.Now;

        _context.Events.Add(eventItem);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetById),
            new { id = eventItem.EventId },
            eventItem);
    }

    [Authorize(Roles = "Organiser")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, Event updated)
    {
        var eventItem = await _context.Events.FindAsync(id);

        if (eventItem == null)
            return NotFound("Event not found.");

        bool destinationExists = await _context.Destinations
            .AnyAsync(d => d.DestinationId == updated.DestinationId);

        if (!destinationExists)
            return BadRequest("DestinationId does not exist.");

        eventItem.DestinationId = updated.DestinationId;
        eventItem.Name = updated.Name;
        eventItem.Description = updated.Description;
        eventItem.EventDate = updated.EventDate;
        eventItem.StartTime = updated.StartTime;
        eventItem.EndTime = updated.EndTime;
        eventItem.Status = updated.Status;
        eventItem.RegistrationDeadline = updated.RegistrationDeadline;

        await _context.SaveChangesAsync();

        return Ok(eventItem);
    }

    [Authorize(Roles = "Organiser")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var eventItem = await _context.Events.FindAsync(id);

        if (eventItem == null)
            return NotFound("Event not found.");

        bool hasCategories = await _context.Categories
            .AnyAsync(c => c.EventId == id);

        if (hasCategories)
        {
            return Conflict(
                "Delete the event's categories first.");
        }

        _context.Events.Remove(eventItem);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}