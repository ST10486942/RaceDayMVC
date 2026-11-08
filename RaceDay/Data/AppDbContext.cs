using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Models;

namespace RaceDayAPI.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    public DbSet<Destination> Destinations => Set<Destination>();

    public DbSet<Event> Events => Set<Event>();

    public DbSet<Category> Categories => Set<Category>();

    public DbSet<Enrolment> Enrolments => Set<Enrolment>();

    public DbSet<Result> Results => Set<Result>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User", table =>
                table.HasCheckConstraint(
                    "CK_User_Role",
                    "[Role] IN ('Organiser', 'Participant')"));

            entity.HasKey(u => u.UserId);

            entity.HasIndex(u => u.Email).IsUnique();

            entity.Property(u => u.Email)
                .HasMaxLength(255)
                .IsRequired();

            entity.Property(u => u.PasswordHash)
                .HasMaxLength(255)
                .IsRequired();

            entity.Property(u => u.FirstName)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(u => u.LastName)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(u => u.Phone).HasMaxLength(20);
            entity.Property(u => u.Gender).HasMaxLength(20);
            entity.Property(u => u.ClubName).HasMaxLength(100);
            entity.Property(u => u.Role).HasMaxLength(20).IsRequired();

            entity.Property(u => u.CreatedAt)
                .HasDefaultValueSql("GETDATE()");
        });

        modelBuilder.Entity<Destination>(entity =>
        {
            entity.ToTable("Destination");

            entity.HasKey(d => d.DestinationId);

            entity.Property(d => d.Name)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(d => d.City)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(d => d.Province)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(d => d.DistanceKm)
                .HasPrecision(8, 2);
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.ToTable("Event", table =>
                table.HasCheckConstraint(
                    "CK_Event_Status",
                    "[Status] IN ('Upcoming', 'Open', 'Closed', 'Completed', 'Cancelled')"));

            entity.HasKey(e => e.EventId);

            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Upcoming");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            entity.Property(e => e.RegistrationDeadline)
                .HasColumnType("date");

            entity.HasOne(e => e.Organiser)
                .WithMany(u => u.OrganisedEvents)
                .HasForeignKey(e => e.OrganiserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Destination)
                .WithMany(d => d.Events)
                .HasForeignKey(e => e.DestinationId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("Category", table =>
                table.HasCheckConstraint(
                    "CK_Category_DistanceKm",
                    "[DistanceKm] > 0"));

            entity.HasKey(c => c.CategoryId);

            entity.Property(c => c.Name)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(c => c.DistanceKm)
                .HasPrecision(8, 2);

            entity.Property(c => c.EntryFee)
                .HasPrecision(10, 2)
                .HasDefaultValue(0);

            entity.HasOne(c => c.Event)
                .WithMany(e => e.Categories)
                .HasForeignKey(c => c.EventId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Enrolment>(entity =>
        {
            entity.ToTable("Enrolment", table =>
                table.HasCheckConstraint(
                    "CK_Enrolment_Status",
                    "[Status] IN ('Pending', 'Confirmed', 'Cancelled', 'DNS', 'DNF')"));

            entity.HasKey(e => e.EnrolmentId);

            entity.HasIndex(e => new
            {
                e.ParticipantId,
                e.CategoryId
            }).IsUnique();

            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Confirmed");

            entity.Property(e => e.EnrolmentDate)
                .HasDefaultValueSql("GETDATE()");

            entity.HasOne(e => e.Participant)
                .WithMany(u => u.Enrolments)
                .HasForeignKey(e => e.ParticipantId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Category)
                .WithMany(c => c.Enrolments)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Result>(entity =>
        {
            entity.ToTable("Result", table =>
                table.HasCheckConstraint(
                    "CK_Result_Status",
                    "[Status] IN ('Pending', 'Finished', 'DNS', 'DNF', 'DQ')"));

            entity.HasKey(r => r.ResultId);

            entity.HasIndex(r => r.EnrolmentId).IsUnique();

            entity.Property(r => r.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Pending");

            entity.Property(r => r.RecordedAt)
                .HasDefaultValueSql("GETDATE()");

            entity.HasOne(r => r.Enrolment)
                .WithOne(e => e.Result)
                .HasForeignKey<Result>(r => r.EnrolmentId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}