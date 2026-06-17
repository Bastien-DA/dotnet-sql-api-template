using Domain.ResultPattern;

namespace Domain.Users;

public static class UserErrors
{
    public static Error NotFound(Guid id) => Error.NotFound("User.NotFound", $"User {id} not found.");
    public static Error EmailExists(string email) => Error.Conflict("User.EmailExists", $"Email {email} already used.");
}