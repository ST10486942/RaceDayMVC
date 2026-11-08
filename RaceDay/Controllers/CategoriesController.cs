using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Data;
using RaceDayAPI.Models;

namespace RaceDayAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoriesController : ControllerBase
{
    private readonly AppDbContext _context;

    public CategoriesController(AppDbContext context)
    {
        _context = context;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var categories = await _context.Categories
            .AsNoTracking()
            .ToListAsync();

        return Ok(categories);
    }

    [AllowAnonymous]
    [HttpGet("event/{eventId:int}")]
    public async Task<IActionResult> GetByEvent(int eventId)
    {
        var categories = await _context.Categories
            .AsNoTracking()
            .Where(c => c.EventId == eventId)
            .ToListAsync();

        return Ok(categories);
    }

    [Authorize(Roles = "Organiser")]
    [HttpPost]
    public async Task<IActionResult> Create(Category category)
    {
        bool eventExists = await _context.Events
            .AnyAsync(e => e.EventId == category.EventId);

        if (!eventExists)
            return BadRequest("EventId does not exist.");

        if (category.DistanceKm <= 0)
            return BadRequest("DistanceKm must be greater than zero.");

        if (category.EntryFee.HasValue &&
            category.EntryFee.Value < 0)
        {
            return BadRequest("EntryFee cannot be negative.");
        }

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetByEvent),
            new { eventId = category.EventId },
            category);
    }

    [Authorize(Roles = "Organiser")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        Category updated)
    {
        var category = await _context.Categories.FindAsync(id);

        if (category == null)
            return NotFound("Category not found.");

        if (updated.DistanceKm <= 0)
            return BadRequest("DistanceKm must be greater than zero.");

        if (updated.EntryFee.HasValue &&
            updated.EntryFee.Value < 0)
        {
            return BadRequest("EntryFee cannot be negative.");
        }

        category.Name = updated.Name;
        category.DistanceKm = updated.DistanceKm;
        category.EntryFee = updated.EntryFee;
        category.MinAge = updated.MinAge;
        category.MaxAge = updated.MaxAge;
        category.GenderRestriction = updated.GenderRestriction;
        category.MaxEntries = updated.MaxEntries;

        await _context.SaveChangesAsync();

        return Ok(category);
    }

    [Authorize(Roles = "Organiser")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var category = await _context.Categories.FindAsync(id);

        if (category == null)
            return NotFound("Category not found.");

        bool hasEnrolments = await _context.Enrolments
            .AnyAsync(e => e.CategoryId == id);

        if (hasEnrolments)
        {
            return Conflict(
                "This category has enrolments and cannot be deleted.");
        }

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}