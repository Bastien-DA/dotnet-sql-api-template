using Domain.Users;

namespace Api.Users;

public static class UserMappingExtensions
{
    public static UserResponse ToResponse(this User user) =>
        new(user.Id, user.Email, user.FirstName, user.LastName, user.CreatedAt);

    public static User ToModel(this CreateUserRequest request) =>
        User.Create(request.Email, request.FirstName, request.LastName);
}