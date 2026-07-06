using Domain.ResultPattern;
using Domain.Users;
using Microsoft.AspNetCore.Mvc;

namespace Api.Users;

[ApiController]
[Route("user")]
public class UsersController(IUserDbAction users) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<UserResponse>>> GetAll(CancellationToken ct)
    {
        List<User> models = await users.GetAllUsers(ct);
        return Ok(models.Select(u => u.ToResponse()));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserResponse>> GetById(Guid id, CancellationToken ct)
    {
        Result<User?> user = await users.GetUserById(id, ct);
        if (!user.IsSuccess)
        {
            return Problem(
                type: "Not Found",
                title: "User not found",
                detail: UserErrors.NotFound(id).Description,
                statusCode: StatusCodes.Status404NotFound
                );
        }
        return Ok(user.Value!.ToResponse());
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UserResponse>> Create([FromBody] CreateUserRequest request, CancellationToken ct)
    {
        Result<User> createdUser = await users.CreateUser(request.ToModel(), ct);
        int userErrorType  = GetErrorType(createdUser.Error!.Type);
        if (!createdUser.IsSuccess)
        {
            return Problem(
                type: userErrorType.ToString(),
                title: "User creation failed",
                detail: createdUser.Error!.Description,
                statusCode: userErrorType
                );
        }
        return Created($"/users/{createdUser.Value.Id}", createdUser.Value.ToResponse());
    }

    [HttpPut]
    public async Task<ActionResult<UserResponse>> Update(Guid id, [FromBody] UpdateUserRequest request, CancellationToken ct)
    {
        Result<User> updatedUser = await users.UpdateUser(request.ToModel(id), ct);
        int updatedUserErrorType = GetErrorType(updatedUser.Error!.Type);
        if(!updatedUser.IsSuccess)
        {
            return Problem(
                type: updatedUserErrorType.ToString(),
                title: "User update failed",
                detail: updatedUser.Error!.Description,
                statusCode: updatedUserErrorType
                );
        }
        return Ok(updatedUser.Value.ToResponse());
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        Result deleteUser = await users.DeleteUser(id, ct);
        int deletedUserErrorType  = GetErrorType(deleteUser.Error!.Type);
        if (!deleteUser.IsSuccess)
        {
            return Problem(
                type: deletedUserErrorType.ToString(),
                title: "User deletion failed",
                detail: deleteUser.Error!.Description,
                statusCode: deletedUserErrorType
            );
        }
        return NoContent();
    }

    private static int GetErrorType(ErrorType type)
    {
        return type switch
        {
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status400BadRequest,
        };
    }
}   