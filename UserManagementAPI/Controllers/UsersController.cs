using Microsoft.AspNetCore.Mvc;
using UserManagementAPI.Models;
using UserManagementAPI.Services;

namespace UserManagementAPI.Controllers;

[ApiController]
[Route("api/users")]
public sealed class UsersController(UserStore userStore, IHostEnvironment environment) : ControllerBase
{
    [HttpGet("diagnostics/error")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public IActionResult TriggerError()
    {
        if (!environment.IsDevelopment())
        {
            return NotFound();
        }

        throw new InvalidOperationException("Intentional middleware test exception.");
    }

    [HttpGet]
    public ActionResult<IEnumerable<User>> GetUsers() => Ok(userStore.GetAll());

    [HttpGet("{id:int}")]
    public ActionResult<User> GetUser(int id)
    {
        var user = userStore.GetById(id);
        return user is null ? NotFound() : Ok(user);
    }

    [HttpPost]
    public ActionResult<User> CreateUser(CreateUserRequest request)
    {
        try
        {
            var user = userStore.Add(request);
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }
        catch (DuplicateEmailException exception)
        {
            return Conflict(new ProblemDetails { Title = "Duplicate email", Detail = exception.Message, Status = StatusCodes.Status409Conflict });
        }
    }

    [HttpPut("{id:int}")]
    public ActionResult<User> UpdateUser(int id, UpdateUserRequest request)
    {
        try
        {
            var user = userStore.Update(id, request);
            return user is null ? NotFound() : Ok(user);
        }
        catch (DuplicateEmailException exception)
        {
            return Conflict(new ProblemDetails { Title = "Duplicate email", Detail = exception.Message, Status = StatusCodes.Status409Conflict });
        }
    }

    [HttpDelete("{id:int}")]
    public IActionResult DeleteUser(int id) => userStore.Delete(id) ? NoContent() : NotFound();
}
