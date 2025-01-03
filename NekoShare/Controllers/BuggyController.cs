using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NekoShare.Data;
using NekoShare.Entities;

namespace NekoShare.Controllers;

public class BuggyController(DataContext context) : BaseApiController
{
    [Authorize]
    [HttpGet("auth")]
    public ActionResult<string> GetAuth()
    {
        return "secret text";
    }

    [HttpGet("not-found")]
    public ActionResult<AppUser> GetNotFound()
    {
        AppUser? user = context.Users.Find(-1);
        
        if(user == null) return NotFound();

        return user;
    }

    [HttpGet("server-error")]
    public ActionResult<AppUser> GetServerError()
    {
        AppUser user = context.Users.Find(-1) ?? throw new Exception ("A bad thing has happened");

        return user;
    }
    
    [HttpGet("bad-request")]
    public ActionResult<string> GetBadRequest() 
    {
        return BadRequest("This is not a good request");
    }
}