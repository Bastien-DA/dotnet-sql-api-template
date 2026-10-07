using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Users.Entity;

[Table("users")]
[Index(nameof(Email), IsUnique = true)]
public class UserEntity
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("email")]
    [Required]
    [MaxLength(256)]
    public string Email { get; set; } = null!;

    [Column("first_name")]
    [Required]
    [MaxLength(128)]
    public string FirstName { get; set; } = null!;

    [Column("last_name")]
    [Required]
    [MaxLength(128)]
    public string LastName { get; set; } = null!;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}
