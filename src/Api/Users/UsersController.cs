using Domain.Users;
using Microsoft.AspNetCore.Mvc;

namespace Api.Users;

[ApiController]
[Route("[controller]")]
public class UsersController(IUserDbAction users) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserResponse>>> GetAll(CancellationToken ct)
    {
        var models = await users.GetAllUsers(ct);
        return Ok(models.Select(u => u.ToResponse()));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserResponse>> GetById(Guid id, CancellationToken ct)
    {
        var user = await users.GetUserById(id, ct);
        if (user is null) return NotFound();
        return Ok(user.ToResponse());
    }

    [HttpPost]
    public async Task<ActionResult<UserResponse>> Create([FromBody] CreateUserRequest request, CancellationToken ct)
    {
        if (await users.EmailExists(request.Email, ct: ct))
            return Conflict(new { error = "Email already in use" });

        var created = await users.CreateUser(request.ToModel(), ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created.ToResponse());
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<UserResponse>> Update(Guid id, [FromBody] UpdateUserRequest request, CancellationToken ct)
    {
        var user = await users.GetUserById(id, ct);
        if (user is null) return NotFound();

        if (await users.EmailExists(request.Email, id, ct))
            return Conflict(new { error = "Email already in use" });

        user.Update(request.Email, request.FirstName, request.LastName);
        var updated = await users.UpdateUser(user, ct);

        return Ok(updated.ToResponse());
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var user = await users.GetUserById(id, ct);
        if (user is null) return NotFound();

        await users.DeleteUser(id, ct);
        return NoContent();
    }
}