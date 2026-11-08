using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Data;
using RaceDayAPI.Models;

namespace RaceDayAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ResultsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ResultsController(AppDbContext context)
    {
        _context = context;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var results = await _context.Results
            .AsNoTracking()
            .ToListAsync();

        return Ok(results);
    }

    [AllowAnonymous]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _context.Results
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.ResultId == id);

        if (result == null)
            return NotFound("Result not found.");

        return Ok(result);
    }

    [Authorize(Roles = "Organiser")]
    [HttpPost]
    public async Task<IActionResult> Create(Result result)
    {
        bool enrolmentExists = await _context.Enrolments
            .AnyAsync(e => e.EnrolmentId == result.EnrolmentId);

        if (!enrolmentExists)
            return BadRequest("EnrolmentId does not exist.");

        bool resultExists = await _context.Results
            .AnyAsync(r => r.EnrolmentId == result.EnrolmentId);

        if (resultExists)
        {
            return Conflict(
                "A result already exists for this enrolment.");
        }

        result.RecordedAt = DateTime.Now;

        _context.Results.Add(result);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.ResultId },
            result);
    }

    [Authorize(Roles = "Organiser")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        Result updated)
    {
        var result = await _context.Results.FindAsync(id);

        if (result == null)
            return NotFound("Result not found.");

        result.FinishTime = updated.FinishTime;
        result.OverallPosition = updated.OverallPosition;
        result.CategoryPosition = updated.CategoryPosition;
        result.Status = updated.Status;

        await _context.SaveChangesAsync();

        return Ok(result);
    }

    [Authorize(Roles = "Organiser")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _context.Results.FindAsync(id);

        if (result == null)
            return NotFound("Result not found.");

        _context.Results.Remove(result);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}