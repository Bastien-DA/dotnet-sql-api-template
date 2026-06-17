using Domain.ResultPattern;
using Domain.Users;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Users;

public class UserDbAction(AppDbContext dbContext) : IUserDbAction
{
    public async Task<Result<User?>> GetUserById(Guid id, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        return user is null ? UserErrors.NotFound(id) : user.ToModel();
    }

    public async Task<List<User>> GetAllUsers(CancellationToken cancellationToken)
    {
        var users = await dbContext.Users
            .AsNoTracking()
            .Select(u => u.ToModel())
            .ToListAsync(cancellationToken);
        
        return users;
    }

    public async Task<Result<User?>> CreateUser(User user, CancellationToken cancellationToken)
    {
        var newUser = await dbContext.Users
            .AddAsync(user.ToEntity(), cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return newUser?.Entity.ToModel();
    }

    public async Task<Result<User>> UpdateUser(User user, CancellationToken cancellationToken)                                         
    {                                                                                                                          
        var rows = await dbContext.Users                                                                                       
            .Where(u => u.Id == user.Id)                                                                                       
            .ExecuteUpdateAsync(setters => setters                                                                             
                    .SetProperty(u => u.Email, user.Email)                                                                         
                    .SetProperty(u => u.FirstName, user.FirstName)                                                                 
                    .SetProperty(u => u.LastName, user.LastName),                                                                  
                cancellationToken);                                                                                            
                                                                                                                             
        return rows == 0 ? throw new InvalidOperationException($"User {user.Id} not found.") : user;
    }    

    public Task<Result> DeleteUser(Guid id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}