using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.DTOs.User;
using server.Interfaces;

namespace server.Controllers;

[Authorize]
public class UsersController(IUserService service) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetUsers()
    {
        var users = await service.GetUsersAsync();
        
        return Ok(users);
    }

    [HttpGet("{username}")]
    public async Task<ActionResult<UserResponseDto>> GetUserByUsername(string username)
    {
        UserResponseDto? user = await service.GetUserByUsernameAsync(username);

        return user != null ? Ok(user) : NotFound();
    }
}