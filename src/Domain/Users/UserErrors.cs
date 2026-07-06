using Domain.ResultPattern;

namespace Domain.Users;

public static class UserErrors
{
    public static Error NotFound(Guid id) => Error.NotFound("User.NotFound", $"User {id} not found.");
    public static Error EmailAlreadyUsed(string email) => Error.Conflict("User.EmailAlreadyUsed", $"Email {email} already used.");
}