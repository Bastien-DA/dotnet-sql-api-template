using Domain.Common;

namespace Domain.Users;

public class User : Entity<Guid>
{
    public string Email { get; private set; } = null!;
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }

    private User() { }

    public static User Create(string email, string firstName, string lastName)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static User FromPersistence(Guid id, string email, string firstName, string lastName, DateTime createdAt)
    {
        return new User
        {
            Id = id,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            CreatedAt = createdAt
        };
    }

    public void Update(string email, string firstName, string lastName)
    {
        Email = email;
        FirstName = firstName;
        LastName = lastName;
    }
}