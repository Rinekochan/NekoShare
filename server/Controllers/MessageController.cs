using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.DTOs;
using server.DTOs.Message;
using server.Entities;
using server.Extensions;
using server.Helpers;
using server.Interfaces;

namespace server.Controllers;

[Authorize]
public class MessageController(IMessageRepository messageRepository, IUserRepository userRepository, IMapper mapper)
    : BaseApiController
{
    [HttpPost]
    public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
    {
        var username = User.GetUsername();

        if (username == createMessageDto.RecipientUsername.ToLower())
            return BadRequest("You cannot message yourself");

        var sender = await userRepository.GetUserByUsernameAsync(username);
        var recipient = await userRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);

        if (sender == null || recipient == null || sender.UserName == null || recipient.UserName == null)
            return BadRequest("Cannot send message at this time");

        var message = new Message
        {
            Sender = sender,
            Recipient = recipient,
            SenderUsername = sender.UserName,
            RecipientUsername = recipient.UserName,
            Content = createMessageDto.Content
        };

        messageRepository.Add(message);

        if (await messageRepository.SaveAllAsync()) return Ok(mapper.Map<MessageDto>(message));

        return BadRequest("Cannot send message at this time");
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesForUser([FromQuery] MessageParams messageParams)
    {
        messageParams.Username = User.GetUsername();

        var messages = await messageRepository.GetMessagesForUser(messageParams);

        Response.AddPaginationHeader(messages);

        return messages;
    }

    [HttpGet("thread/{partner}")]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string partner)
    {
        var currentUsername = User.GetUsername();

        return Ok(await messageRepository.GetMessageThread(currentUsername, partner));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteMessage(int id)
    {
        var username = User.GetUsername();

        var message = await messageRepository.GetMessage(id);

        if (message == null) return BadRequest("cannot delete this message");

        if (message.SenderUsername != username && message.RecipientUsername != username) return Forbid();

        if (message.SenderUsername == username) message.SenderDeleted = true;
        if (message.RecipientUsername == username) message.RecipientDeleted = true;

        if (message is { SenderDeleted: true, RecipientDeleted: true })
        {
            messageRepository.Delete(message);
        }

        if (await messageRepository.SaveAllAsync()) return Ok();

        return BadRequest("Problem deleting the message");
    }
}