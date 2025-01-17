using AutoMapper.Internal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using server.DTOs.Photo;
using server.Entities;
using server.Enums;
using server.Exceptions;
using server.Interfaces;

namespace server.Controllers;

[Authorize(Policy = "ModeratePhotoRole")]
public class AdminController(
    UserManager<AppUser> userManager,
    IPhotoService photoService,
    IUserService userService) : BaseApiController
{
    [Authorize(Policy = "RequireAdminRole")]
    [HttpGet("users-with-roles")]
    public async Task<ActionResult> GetUsersWithRoles()
    {
        var users = await userManager.Users
            .OrderBy(x => x.UserName)
            .Select(x => new
            {
                x.Id,
                Username = x.UserName,
                Roles = x.UserRoles.Select(r => r.Role.Name).ToList()
            }).ToListAsync();

        return Ok(users);
    }

    [Authorize(Policy = "RequireAdminRole")]
    [HttpPost("edit-roles/{username}")]
    public async Task<ActionResult> EditRoles(string username, string roles)
    {
        if (string.IsNullOrEmpty(roles)) return BadRequest("You must select at least one role");

        var selectedRoles = roles.Split(",").ToArray();

        var user = await userManager.FindByNameAsync(username);

        if (user == null) return BadRequest("User not found");

        var userRoles = await userManager.GetRolesAsync(user);

        var result = await userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

        if (!result.Succeeded) return BadRequest("Failed to add to roles");

        result = await userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));

        if (!result.Succeeded) return BadRequest("Failed to remove roles");

        return Ok(await userManager.GetRolesAsync(user));
    }

    [HttpGet("photos-to-moderate")]
    public ActionResult GetPhotosForModeration()
    {
        return Ok("Admins or moderators can see this");
    }

    [HttpGet("photos-for-approval")]
    public async Task<ActionResult<IEnumerable<PhotoDto>>> GetPhotosForApproval()
    {
        try
        {
            var photos = await photoService.GetUnapprovedPhotos();

            return Ok(photos);
        }
        catch (Exception ex)
        {
            return StatusCode(500);
        }
    }

    [HttpPut("approve/photo")]
    public async Task<ActionResult> ApprovePhoto(int id)
    {
        try
        {
            if (await photoService.AprrovePhoto(id))
            {
                return Ok();
            }
            else return StatusCode(500);
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

    [HttpPut("reject/photo")]
    public async Task<ActionResult> RejectPhoto(int id)
    {
        try
        {
            if (await photoService.RejectPhoto(id)) return Ok();
            else return StatusCode(500);
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
}