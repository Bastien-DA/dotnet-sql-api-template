using Domain.ResultPattern;
using Domain.Users;
using Infrastructure.Persistence;
using Infrastructure.Users.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Npgsql;

namespace Infrastructure.Users;

public class UserDbAction(AppDbContext dbContext) : IUserDbAction
{
    public async Task<Result<User?>> GetUserById(Guid id, CancellationToken cancellationToken)
    {
        UserEntity? user = await dbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        return user is null ? UserErrors.NotFound(id) : user.ToModel();
    }

    public async Task<List<User>> GetAllUsers(CancellationToken cancellationToken)
    {
        List<User> users = await dbContext.Users
            .AsNoTracking()
            .Select(u => u.ToModel())
            .ToListAsync(cancellationToken);
        
        return users;
    }

    public async Task<Result<User>> CreateUser(User user, CancellationToken cancellationToken)
    {
        EntityEntry<UserEntity> newUser = await dbContext.Users
            .AddAsync(user.ToEntity(), cancellationToken);
        
        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            return UserErrors.EmailAlreadyUsed(user.Email);
        }
        
        return newUser.Entity.ToModel();
    }

    public async Task<Result<User>> UpdateUser(User user, CancellationToken cancellationToken)                                         
    {                                                                                                                          
        int rows = await dbContext.Users                                                                                       
            .Where(u => u.Id == user.Id)                                                                                       
            .ExecuteUpdateAsync(setters => setters                                                                             
                    .SetProperty(u => u.Email, user.Email)                                                                         
                    .SetProperty(u => u.FirstName, user.FirstName)                                                                 
                    .SetProperty(u => u.LastName, user.LastName),                                                                  
                cancellationToken);                                                                                            
                                                                                                                             
        return rows == 0 ? UserErrors.NotFound(user.Id) : user;
    }    

    public async Task<Result> DeleteUser(Guid id, CancellationToken cancellationToken)
    {
        int rows = await dbContext.Users
            .Where(u => u.Id == id)
            .ExecuteDeleteAsync(cancellationToken);

        return rows > 0 ? Result.Success() : UserErrors.NotFound(id);
    }
}