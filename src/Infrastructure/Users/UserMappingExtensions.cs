using Domain.Users;
using Infrastructure.Users.Entity;

namespace Infrastructure.Users;

public static class UserMappingExtensions
{
    public static User ToModel(this UserEntity entity) =>
        User.FromPersistence(entity.Id, entity.Email, entity.FirstName, entity.LastName, entity.CreatedAt);

    public static UserEntity ToEntity(this User model) =>
        new()
        {
            Id = model.Id,
            Email = model.Email,
            FirstName = model.FirstName,
            LastName = model.LastName,
            CreatedAt = model.CreatedAt
        };
}