using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NekoShare.Data;
using NekoShare.DTOs;
using NekoShare.Entities;
using NekoShare.Interfaces;

namespace NekoShare.Controllers;

public class AccountController(DataContext context, ITokenService tokenService) : BaseApiController
{
    [HttpPost("register")]
    public async Task<ActionResult<AuthenticateResponseDto>> Register(AuthenticateRequestDto authenticateRequestDto)
    {
        if(await UserExists(authenticateRequestDto.Username)) return BadRequest("Username already exists");
        
        using HMACSHA512 hmac = new HMACSHA512();

        AppUser user = new AppUser
        {
            UserName = authenticateRequestDto.Username.ToLower(),
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(authenticateRequestDto.Password)),
            PasswordSalt = hmac.Key
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        return new AuthenticateResponseDto()
        {
            Username = user.UserName,
            Token = tokenService.CreateToken(user)
        };
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthenticateResponseDto>> Login(AuthenticateRequestDto authenticateRequestDto)
    {
        AppUser? user = await context.Users.FirstOrDefaultAsync(x => x.UserName == authenticateRequestDto.Username.ToLower());
        
        if (user == null) return Unauthorized("Invalid username or password");
        
        using HMACSHA512 hmac = new HMACSHA512(user.PasswordSalt);
        
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(authenticateRequestDto.Password));

        for (int i = 0; i < computedHash.Length; i++)
        {
            if(user.PasswordHash[i] != computedHash[i]) return Unauthorized("Invalid username or password");
        }

        return new AuthenticateResponseDto()
        {
            Username = user.UserName,
            Token = tokenService.CreateToken(user)
        };
    }

    private async Task<bool> UserExists(string username)
    {
        return await context.Users.AnyAsync(x => x.UserName.ToLower() == username.ToLower());
    }
}