namespace Api.Users;

public record UserResponse(Guid Id, string Email, string FirstName, string LastName, DateTime CreatedAt);

public record CreateUserRequest(string Email, string FirstName, string LastName);

public record UpdateUserRequest(string Email, string FirstName, string LastName);