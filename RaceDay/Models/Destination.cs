using System.ComponentModel.DataAnnotations;

namespace RaceDayAPI.Models;

public class Destination
{
    public int DestinationId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string City { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Province { get; set; } = string.Empty;

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public string? RouteDescription { get; set; }

    public decimal? DistanceKm { get; set; }

    public int? ElevationGainM { get; set; }

    public string? StartPoint { get; set; }

    public string? FinishPoint { get; set; }

    public ICollection<Event> Events { get; set; }
        = new List<Event>();
}