using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using server.DTOs;
using server.DTOs.Message;
using server.Entities;
using server.Extensions;
using server.Interfaces;

namespace server.SignalR;

[Authorize]
public class MessageHub(
    IMessageRepository messageRepository,
    IUserRepository userRepository,
    IMapper mapper,
    IHubContext<PresenceHub> presenceHub) : Hub
{
    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        var otherUser = httpContext?.Request.Query["user"];

        if (Context.User == null || string.IsNullOrEmpty(otherUser)) throw new Exception("Cannot join group");

        var groupName = GetGroupName(Context.User?.GetUsername(), otherUser);
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        var group = await AddToGroup(groupName);

        await Clients.Group(groupName).SendAsync("UpdatedGroup", group);

        var messages = await messageRepository.GetMessageThread(Context.User!.GetUsername(), otherUser!);
        await Clients.Caller.SendAsync("ReceiveMessageThread", messages);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var group = await RemoveFromMessageGroup();
        await Clients.Group(group.Name).SendAsync("UpdatedGroup", group);
        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(CreateMessageDto createMessageDto)
    {
        var username = Context.User?.GetUsername() ?? throw new Exception("Could not get user");

        if (username == createMessageDto.RecipientUsername.ToLower())
            throw new HubException("Cannot message yourself");

        var sender = await userRepository.GetUserByUsernameAsync(username);
        var recipient = await userRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);

        if (sender == null || recipient == null || sender.UserName == null || recipient.UserName == null)
            throw new HubException("Cannot send message at this time");

        var message = new Message
        {
            Sender = sender,
            Recipient = recipient,
            SenderUsername = sender.UserName,
            RecipientUsername = recipient.UserName,
            Content = createMessageDto.Content
        };

        var groupName = GetGroupName(sender.UserName, recipient.UserName);
        var group = await messageRepository.GetMessageGroup(groupName);

        if (group != null && group.Connections.Any(x => x.Username == recipient.UserName))
        {
            message.DateRead = DateTime.UtcNow;
        }
        else
        {
            var connections = await PresenceTracker.GetConnectionsForUser(recipient.UserName);
            if (connections != null || connections?.Count != null)
            {
                await presenceHub.Clients.Clients(connections).SendAsync("NewMessageNotified",
                    new
                    {
                        username = sender.UserName,
                        knownAs = sender.KnownAs
                    });
            }
        }

        messageRepository.Add(message);

        if (await messageRepository.SaveAllAsync())
        {
            await Clients.Group(groupName).SendAsync("NewMessage", mapper.Map<MessageDto>(message));
        }

        throw new HubException("Cannot send message at this time");
    }

    private string GetGroupName(string? caller, string? other)
    {
        var stringCompare = string.CompareOrdinal(caller, other) < 0;
        return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
    }

    private async Task<Group> AddToGroup(string groupName)
    {
        var username = Context.User?.GetUsername() ?? throw new Exception("Cannot get username");
        var group = await messageRepository.GetMessageGroup(groupName);

        if (group == null)
        {
            group = new Group { Name = groupName };
            messageRepository.AddGroup(group);
        }

        var connection = new Connection { ConnectionId = Context.ConnectionId, Username = username };
        group.Connections.Add(connection);

        if (await messageRepository.SaveAllAsync())
        {
            return group;
        }

        throw new HubException("Failed to join group");
    }

    private async Task<Group> RemoveFromMessageGroup()
    {
        var group = await messageRepository.GetGroupForConnection(Context.ConnectionId);
        var connection = group?.Connections.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
        if (connection != null && group != null)
        {
            messageRepository.RemoveConnection(connection);
            if(await messageRepository.SaveAllAsync()) return group;
        }

        throw new HubException("Failed to remove from group");
    }
}