using Microsoft.EntityFrameworkCore;
using NotificationsAPI.Domain.Entities;
using NotificationsAPI.Domain.Enums;

namespace NotificationsAPI.Infrastructure.Data;

public class NotificationsDbContext : DbContext
{
    public NotificationsDbContext(DbContextOptions<NotificationsDbContext> options)
        : base(options)
    {
    }

    public DbSet<Notification> Notifications { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.ToTable("notifications");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedNever();

            entity.Property(e => e.UserId)
                .HasColumnName("user_id")
                .IsRequired();

            entity.Property(e => e.OrderId)
                .HasColumnName("order_id");

            entity.Property(e => e.Type)
                .HasColumnName("type")
                .HasConversion<string>()
                .IsRequired();

            entity.Property(e => e.Title)
                .HasColumnName("title")
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(e => e.Message)
                .HasColumnName("message")
                .HasMaxLength(1000)
                .IsRequired();

            entity.Property(e => e.Status)
                .HasColumnName("status")
                .HasConversion<string>()
                .IsRequired();

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();

            entity.Property(e => e.ReadAt)
                .HasColumnName("read_at");

            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.OrderId);
            entity.HasIndex(e => new { e.UserId, e.Status });
        });
    }
}
