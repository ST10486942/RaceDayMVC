using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Data;
using RaceDayAPI.Models;

namespace RaceDayAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DestinationsController : ControllerBase
{
    private readonly AppDbContext _context;

    public DestinationsController(AppDbContext context)
    {
        _context = context;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var destinations = await _context.Destinations
            .AsNoTracking()
            .ToListAsync();

        return Ok(destinations);
    }

    [AllowAnonymous]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var destination = await _context.Destinations
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.DestinationId == id);

        if (destination == null)
            return NotFound("Destination not found.");

        return Ok(destination);
    }

    [Authorize(Roles = "Organiser")]
    [HttpPost]
    public async Task<IActionResult> Create(Destination destination)
    {
        _context.Destinations.Add(destination);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetById),
            new { id = destination.DestinationId },
            destination);
    }

    [Authorize(Roles = "Organiser")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        Destination updated)
    {
        var destination = await _context.Destinations.FindAsync(id);

        if (destination == null)
            return NotFound("Destination not found.");

        destination.Name = updated.Name;
        destination.City = updated.City;
        destination.Province = updated.Province;
        destination.Latitude = updated.Latitude;
        destination.Longitude = updated.Longitude;
        destination.RouteDescription = updated.RouteDescription;
        destination.DistanceKm = updated.DistanceKm;
        destination.ElevationGainM = updated.ElevationGainM;
        destination.StartPoint = updated.StartPoint;
        destination.FinishPoint = updated.FinishPoint;

        await _context.SaveChangesAsync();

        return Ok(destination);
    }

    [Authorize(Roles = "Organiser")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var destination = await _context.Destinations.FindAsync(id);

        if (destination == null)
            return NotFound("Destination not found.");

        bool isInUse = await _context.Events
            .AnyAsync(e => e.DestinationId == id);

        if (isInUse)
        {
            return Conflict(
                "This destination is used by an event and cannot be deleted.");
        }

        _context.Destinations.Remove(destination);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}