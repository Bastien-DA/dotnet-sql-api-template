using Domain.ResultPattern;

namespace Domain.Users;

public interface IUserDbAction
{
    public Task<Result<User?>> GetUserById(Guid id, CancellationToken cancellationToken);

    public Task<List<User>> GetAllUsers(CancellationToken cancellationToken);
    
    public Task<Result<User>> CreateUser(User user, CancellationToken cancellationToken);

    public Task<Result<User>> UpdateUser(User user, CancellationToken cancellationToken);

    public Task<Result> DeleteUser(Guid id, CancellationToken cancellationToken);
}