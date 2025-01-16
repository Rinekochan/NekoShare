using System.Security.Claims;
using server.Enums;
using server.Exceptions;

namespace server.Extensions;

public static class ClaimsPrincipleExtensions
{
    public static string GetUsername(this ClaimsPrincipal? user)
    {
        var username = user.FindFirstValue(ClaimTypes.Name)
                       ?? throw new ItemNotFoundException("Cannot get username from ", EntityEnum.Token);

        return username;
    }

    public static int GetUserId(this ClaimsPrincipal user)
    {
        var userId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)
                       ?? throw new ItemNotFoundException("Cannot get userId from ", EntityEnum.Token));

        return userId;
    }
}