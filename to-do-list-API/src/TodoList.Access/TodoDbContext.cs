using Microsoft.EntityFrameworkCore;
using TodoList.Access.Entities;

namespace TodoList.Access;

public sealed class TodoDbContext(DbContextOptions<TodoDbContext> options) : DbContext(options)
{
    public DbSet<TaskRecord> Tasks => Set<TaskRecord>();
    public DbSet<UserRecord> Users => Set<UserRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserRecord>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(user => user.Id);
            entity.HasIndex(user => user.NormalizedEmail).IsUnique();
            entity.Property(user => user.Id)
                .HasColumnName("id");
            entity.Property(user => user.FullName)
                .HasColumnName("full_name")
                .HasMaxLength(150)
                .IsRequired();
            entity.Property(user => user.Email)
                .HasColumnName("email")
                .HasMaxLength(320)
                .IsRequired();
            entity.Property(user => user.NormalizedEmail)
                .HasColumnName("normalized_email")
                .HasMaxLength(320)
                .IsRequired();
            entity.Property(user => user.PasswordHash)
                .HasColumnName("password_hash")
                .IsRequired();
            entity.Property(user => user.CreatedAtUtc)
                .HasColumnName("created_at_utc")
                .IsRequired();
        });

        modelBuilder.Entity<TaskRecord>(entity =>
        {
            entity.ToTable("tasks");
            entity.HasKey(task => task.Id);
            entity.Property(task => task.Id)
                .HasColumnName("id");
            entity.Property(task => task.UserId)
                .HasColumnName("user_id")
                .IsRequired();
            entity.Property(task => task.Title)
                .HasColumnName("title")
                .HasMaxLength(200)
                .IsRequired();
            entity.HasIndex(task => new { task.UserId, task.Id });
            entity.HasOne(task => task.User)
                .WithMany(user => user.Tasks)
                .HasForeignKey(task => task.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
