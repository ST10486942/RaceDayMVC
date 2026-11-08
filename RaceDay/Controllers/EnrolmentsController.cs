using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Data;
using RaceDayAPI.Models;

namespace RaceDayAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EnrolmentsController : ControllerBase
{
    private readonly AppDbContext _context;

    public EnrolmentsController(AppDbContext context)
    {
        _context = context;
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var enrolments = await _context.Enrolments
            .AsNoTracking()
            .ToListAsync();

        return Ok(enrolments);
    }

    [Authorize]
    [HttpGet("participant/{participantId:int}")]
    public async Task<IActionResult> GetByParticipant(int participantId)
    {
        var enrolments = await _context.Enrolments
            .AsNoTracking()
            .Where(e => e.ParticipantId == participantId)
            .ToListAsync();

        return Ok(enrolments);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(Enrolment enrolment)
    {
        bool participantExists = await _context.Users.AnyAsync(
            u => u.UserId == enrolment.ParticipantId &&
                 u.Role == "Participant");

        if (!participantExists)
        {
            return BadRequest(
                "ParticipantId must identify an existing participant.");
        }

        bool categoryExists = await _context.Categories
            .AnyAsync(c => c.CategoryId == enrolment.CategoryId);

        if (!categoryExists)
            return BadRequest("CategoryId does not exist.");

        bool alreadyEnrolled = await _context.Enrolments.AnyAsync(e =>
            e.ParticipantId == enrolment.ParticipantId &&
            e.CategoryId == enrolment.CategoryId);

        if (alreadyEnrolled)
        {
            return Conflict(
                "This participant is already enrolled in this category.");
        }

        enrolment.EnrolmentDate = DateTime.Now;

        _context.Enrolments.Add(enrolment);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetByParticipant),
            new { participantId = enrolment.ParticipantId },
            enrolment);
    }

    [Authorize]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        Enrolment updated)
    {
        var enrolment = await _context.Enrolments.FindAsync(id);

        if (enrolment == null)
            return NotFound("Enrolment not found.");

        enrolment.Status = updated.Status;
        enrolment.BibNumber = updated.BibNumber;
        enrolment.Notes = updated.Notes;

        await _context.SaveChangesAsync();

        return Ok(enrolment);
    }

    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var enrolment = await _context.Enrolments.FindAsync(id);

        if (enrolment == null)
            return NotFound("Enrolment not found.");

        _context.Enrolments.Remove(enrolment);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}