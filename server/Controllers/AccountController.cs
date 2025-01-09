using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.DTOs;
using server.Entities;
using server.Interfaces;

namespace server.Controllers;

public class AccountController(DataContext context, ITokenService tokenService) : BaseApiController
{
    [HttpPost("register")]
    public async Task<ActionResult<AuthenticateResponseDto>> Register(AuthenticateRequestDto authenticateRequestDto)
    {
        if (await UserExists(authenticateRequestDto.Username)) return BadRequest("Username already exists");

        return Ok();
        // using HMACSHA512 hmac = new HMACSHA512();
        //
        // AppUser user = new AppUser
        // {
        //     UserName = authenticateRequestDto.Username.ToLower(),
        //     PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(authenticateRequestDto.Password)),
        //     PasswordSalt = hmac.Key
        // };
        //
        // // Save changes might be conflicted in case of race condition
        // try
        // {
        //     context.Users.Add(user);
        //     await context.SaveChangesAsync();
        // }
        // catch (DbUpdateException e) when (e.InnerException is SqliteException sqlEx
        //                                   && (sqlEx.SqliteErrorCode == 2601 || sqlEx.SqliteErrorCode == 2627))
        // {
        //     return BadRequest("Username already exists");
        // }
        //
        //
        // return new AuthenticateResponseDto()
        // {
        //     Username = user.UserName,
        //     Token = tokenService.CreateToken(user)
        // };
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthenticateResponseDto>> Login(AuthenticateRequestDto authenticateRequestDto)
    {
        var user = await context.Users
            .Include(photo => photo.Photos)
            .FirstOrDefaultAsync(user => user.UserName == authenticateRequestDto.Username.ToLower());

        if (user == null) return Unauthorized("Invalid username or password");

        using HMACSHA512 hmac = new HMACSHA512(user.PasswordSalt);

        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(authenticateRequestDto.Password));

        for (int i = 0; i < computedHash.Length; i++)
        {
            if (user.PasswordHash[i] != computedHash[i]) return Unauthorized("Invalid username or password");
        }

        return new AuthenticateResponseDto()
        {
            Username = user.UserName,
            Token = tokenService.CreateToken(user),
            PhotoUrl = user.Photos.FirstOrDefault(photo => photo.IsMain)?.Url
        };
    }

    private async Task<bool> UserExists(string username)
    {
        return await context.Users.AnyAsync(x => x.UserName.ToLower() == username.ToLower());
    }
}