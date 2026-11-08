using System.ComponentModel.DataAnnotations;

namespace RaceDayAPI.Models;

public class RegisterRequest
{
    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(8)]
    [MaxLength(100)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? Phone { get; set; }

    public DateTime? DateOfBirth { get; set; }

    [MaxLength(20)]
    public string? Gender { get; set; }

    [MaxLength(100)]
    public string? ClubName { get; set; }
}

public class LoginRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}

public class CreateEventRequest
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public int OrganiserId { get; set; }

    public int DestinationId { get; set; }

    public DateTime EventDate { get; set; }

    public TimeSpan? StartTime { get; set; }

    public TimeSpan? EndTime { get; set; }

    public string Status { get; set; } = "Upcoming";

    public DateTime? RegistrationDeadline { get; set; }
}

public class CreateEnrolmentRequest
{
    public int ParticipantId { get; set; }

    public int CategoryId { get; set; }

    public string Status { get; set; } = "Confirmed";

    public int? BibNumber { get; set; }

    public string? Notes { get; set; }
}

public class CreateResultRequest
{
    public int EnrolmentId { get; set; }

    public TimeSpan? FinishTime { get; set; }

    public int? OverallPosition { get; set; }

    public int? CategoryPosition { get; set; }

    public string Status { get; set; } = "Pending";
}