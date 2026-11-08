namespace RaceDayAPI.Models;

public class Result
{
    public int ResultId { get; set; }

    public int EnrolmentId { get; set; }

    public TimeSpan? FinishTime { get; set; }

    public int? OverallPosition { get; set; }

    public int? CategoryPosition { get; set; }

    public string Status { get; set; } = "Pending";

    public DateTime RecordedAt { get; set; } = DateTime.Now;

    public Enrolment? Enrolment { get; set; }
}