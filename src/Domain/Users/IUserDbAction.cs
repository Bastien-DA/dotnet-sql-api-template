namespace Domain.Users;

public interface IUserDbAction
{
    public Task<User?> GetUserById(Guid id, CancellationToken cancellationToken);

    public Task<List<User>> GetAllUsers(CancellationToken cancellationToken);

    public Task<bool> EmailExists(string email);

    public Task<User?> CreateUser(User user, CancellationToken cancellationToken);

    public Task<User> UpdateUser(User user, CancellationToken cancellationToken);

    public Task DeleteUser(Guid id, CancellationToken cancellationToken);
}