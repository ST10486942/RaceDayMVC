using System.ComponentModel.DataAnnotations;

namespace RaceDayAPI.Models;

public class Enrolment
{
    public int EnrolmentId { get; set; }

    public int ParticipantId { get; set; }

    public int CategoryId { get; set; }

    public DateTime EnrolmentDate { get; set; } = DateTime.Now;

    public string Status { get; set; } = "Confirmed";

    public int? BibNumber { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }

    public User? Participant { get; set; }

    public Category? Category { get; set; }

    public Result? Result { get; set; }
}