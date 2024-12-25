using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NekoShare.Data;
using NekoShare.Entities;

namespace NekoShare.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(DataContext context) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
    {
        List<AppUser> users = await context.Users.ToListAsync();

        return users;
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AppUser>> GetUserById(int id)
    {
        AppUser? user = await context.Users.FindAsync(id);

        return user != null ? user : NotFound();
    }
}