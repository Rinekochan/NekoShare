using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using server.Entities;

namespace server.Data;

public class Seed
{
    public static async Task SeedUsers(DataContext context)
    {
        if (await context.Users.AnyAsync()) return;
        
        var userData = await File.ReadAllTextAsync("Data/Seeds/AppUserSeed.json");
        
        JsonSerializerOptions options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        
        List<AppUser>? users = JsonSerializer.Deserialize<List<AppUser>>(userData, options);

        if (users == null) return;
        
        foreach (AppUser user in users)
        {
            using var hmac = new HMACSHA512();
            
            user.UserName = user.UserName.ToLower();
            user.PasswordHash = hmac.ComputeHash("password"u8.ToArray());
            user.PasswordSalt = hmac.Key;

            context.Users.Add(user);
        }

        await context.SaveChangesAsync();
    }
}