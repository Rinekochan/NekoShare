using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.DTOs.Photo;
using server.DTOs.User;
using server.Enums;
using server.Exceptions;
using server.Extensions;
using server.Helpers;
using server.Interfaces;

namespace server.Controllers;

[Authorize]
public class UsersController(IUserService userService) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetUsers([FromQuery] UserParams userParams)
    {
        userParams.CurrentUsername = User.GetUsername();
        var users = await userService.GetUsersAsync(userParams);
        
        Response.AddPaginationHeader(users);
        
        return Ok(users);
    }

    [HttpGet("{username}")]
    public async Task<ActionResult<UserResponseDto>> GetUserByUsername(string username)
    {
        UserResponseDto? user = await userService.GetUserByUsernameAsync(username);

        return user != null ? Ok(user) : NotFound();
    }

    [HttpPut]
    public async Task<ActionResult> UpdateUser(UserUpdateDto userDto)
    {
        try
        {
            var username = User.GetUsername();
            if (await userService.UpdateUser(username, userDto)) return NoContent();
        }
        catch (ItemNotFoundException ex)
        {
            return BadRequest(ex.Message + Enum.GetName(typeof(EntityEnum), ex.Type));
        }

        return BadRequest("Failed to update the user");
    }

    [HttpPost("add-photo")]
    public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
    {
        try
        {
            var username = User.GetUsername();
            return CreatedAtAction(nameof(GetUserByUsername), 
            new {
                username
            },
            await userService.AddPhoto(username, file));
        }
        catch (ItemNotFoundException ex)
        {
            return BadRequest(ex.Message + Enum.GetName(typeof(EntityEnum), ex.Type));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("set-main-photo/{photoId:int}")]
    public async Task<ActionResult> SetMainPhoto(int photoId)
    {
        try
        {
            var username = User.GetUsername();
            if (await userService.SetMainPhoto(username, photoId)) return NoContent();
        }
        catch (ItemNotFoundException ex)
        {
            return BadRequest(ex.Message + Enum.GetName(typeof(EntityEnum), ex.Type));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }

        return BadRequest("Cannot set this photo as your main photo");
    }

    [HttpDelete("delete-photo/{photoId:int}")]
    public async Task<ActionResult> DeletePhoto(int photoId)
    {
        try
        {
            var username = User.GetUsername();
            if (await userService.DeletePhoto(username, photoId)) return NoContent();
        }
        catch (ItemNotFoundException ex)
        {
            return BadRequest(ex.Message + Enum.GetName(typeof(EntityEnum), ex.Type));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }

        return BadRequest("Cannot delete your photo");
    }
}