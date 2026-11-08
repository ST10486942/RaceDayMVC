using System.ComponentModel.DataAnnotations;

namespace RaceDayAPI.Models;

public class Category
{
    public int CategoryId { get; set; }

    public int EventId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public decimal DistanceKm { get; set; }

    public decimal? EntryFee { get; set; }

    public int? MinAge { get; set; }

    public int? MaxAge { get; set; }

    public string? GenderRestriction { get; set; }

    public int? MaxEntries { get; set; }

    public Event? Event { get; set; }

    public ICollection<Enrolment> Enrolments { get; set; }
        = new List<Enrolment>();
}