using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using server.Extensions;

namespace server.SignalR;

[Authorize]
public class PresenceHub(PresenceTracker presenceTracker) : Hub
{
    public override async Task OnConnectedAsync()
    {
        if (Context.User == null) throw new HubException("Cannot get current user claim");

        var isOnline = await presenceTracker.UserConnected(Context.User.GetUsername(), Context.ConnectionId);
        if (isOnline)
        {
            await Clients.Others.SendAsync("UserIsOnline", Context.User.GetUsername());
        }

        var currentUsers = await presenceTracker.GetOnlineUsers();
        await Clients.Caller.SendAsync("GetOnlineUsers", currentUsers);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (Context.User == null) throw new HubException("Cannot get current user claim");
        var isOffline = await presenceTracker.UserDisconnected(Context.User.GetUsername(), Context.ConnectionId);
        if (isOffline)
        {
            await Clients.Others.SendAsync("UserIsOffline", Context.User?.GetUsername());
        }

        await base.OnDisconnectedAsync(exception);
    }
}