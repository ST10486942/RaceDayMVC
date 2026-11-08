using System.ComponentModel.DataAnnotations;

namespace RaceDayAPI.Models;

public class User
{

    [MaxLength(500)]
    public string? ProfilePictureUrl { get; set; }
    public int UserId { get; set; }

    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string PasswordHash { get; set; } = string.Empty;

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

    [Required]
    [MaxLength(20)]
    public string Role { get; set; } = "Participant";

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public ICollection<Event> OrganisedEvents { get; set; }
        = new List<Event>();

    public ICollection<Enrolment> Enrolments { get; set; }
        = new List<Enrolment>();
}