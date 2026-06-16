using Domain.Users;
using Infrastructure.Users.Entity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<UserEntity> Users => Set<UserEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserEntity>(builder =>
        {
            builder.ToTable("users");
            
            builder.HasKey(userEntity => userEntity.Id);
            builder.Property(userEntity => userEntity.Id)
                .HasColumnName("id")
                .IsRequired();
            
            builder.HasIndex(userEntity => userEntity.Email).IsUnique();
            builder.Property(userEntity => userEntity.Email)
                .HasColumnName("email")
                .IsRequired()
                .HasMaxLength(256);
            
            builder.Property(userEntity => userEntity.FirstName)
                .HasColumnName("first_name")
                .IsRequired()
                .HasMaxLength(128);
            
            builder.Property(userEntity => userEntity.LastName)
                .HasColumnName("last_name")
                .IsRequired()
                .HasMaxLength(128);

            builder.Property(userEntity => userEntity.CreatedAt)
                .HasColumnName("created_at");

        });
    }
}