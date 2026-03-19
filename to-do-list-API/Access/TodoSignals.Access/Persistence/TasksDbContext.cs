using Microsoft.EntityFrameworkCore;
using TodoSignals.Access.Entities;

namespace TodoSignals.Access.Persistence;

public sealed class TasksDbContext(DbContextOptions<TasksDbContext> options) : DbContext(options)
{
    public DbSet<TaskEntity> Tasks => Set<TaskEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TaskEntity>(entity =>
        {
            entity.ToTable("Tasks");
            entity.HasKey(task => task.Id);
            entity.Property(task => task.Title)
                .HasMaxLength(200)
                .IsRequired();
        });
    }
}
