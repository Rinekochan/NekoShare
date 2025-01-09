using System.Security.Claims;
using server.Enums;
using server.Exceptions;

namespace server.Extensions;

public static class ClaimsPrincipleExtensions
{
    public static string GetUsername(this ClaimsPrincipal user)
    {
        var username = user.FindFirstValue(ClaimTypes.NameIdentifier) 
                       ?? throw new ItemNotFoundException("Cannot get username from ", EntityEnum.Token);

        return username;
    }
}