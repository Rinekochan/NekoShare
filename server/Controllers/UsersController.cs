using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.DTOs.User;
using server.Enums;
using server.Exceptions;
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

    [HttpPut]
    public async Task<ActionResult> UpdateUser(UserUpdateDto userDto)
    {
        var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (username == null) return BadRequest("Invalid token. Please login again.");
        
        try
        {
            if (await service.UpdateUser(userDto, username)) return NoContent();
        }
        catch (ItemNotFoundException ex)
        {
            return BadRequest(ex.Message + Enum.GetName(typeof(EntityEnum), ex.Type));
        }

        return BadRequest("Failed to update the user");
    }
}