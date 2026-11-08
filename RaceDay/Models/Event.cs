using System.ComponentModel.DataAnnotations;

namespace RaceDayAPI.Models;

public class Event
{
    [MaxLength(500)]
    public string? BannerImageURL { get; set; }
    public int EventId { get; set; }

    public int OrganiserId { get; set; }

    public int DestinationId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime EventDate { get; set; }

    public TimeSpan? StartTime { get; set; }

    public TimeSpan? EndTime { get; set; }

    public string Status { get; set; } = "Upcoming";

    public DateTime? RegistrationDeadline { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public User? Organiser { get; set; }

    public Destination? Destination { get; set; }

    public ICollection<Category> Categories { get; set; }
        = new List<Category>();
}