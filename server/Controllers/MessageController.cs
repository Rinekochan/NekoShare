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
public class MessageController(IUnitOfWork unitOfWork, IMapper mapper)
    : BaseApiController
{
    [HttpPost]
    public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
    {
        var username = User.GetUsername();

        if (username == createMessageDto.RecipientUsername.ToLower())
            return BadRequest("You cannot message yourself");

        var sender = await unitOfWork.UserRepository.GetUserByUsernameAsync(username);
        var recipient = await unitOfWork.UserRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);

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

        unitOfWork.MessageRepository.Add(message);

        if (await unitOfWork.Complete()) return Ok(mapper.Map<MessageDto>(message));

        return BadRequest("Cannot send message at this time");
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesForUser([FromQuery] MessageParams messageParams)
    {
        messageParams.Username = User.GetUsername();

        var messages = await unitOfWork.MessageRepository.GetMessagesForUser(messageParams);

        Response.AddPaginationHeader(messages);

        return messages;
    }

    [HttpGet("thread/{partner}")]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string partner)
    {
        var currentUsername = User.GetUsername();

        return Ok(await unitOfWork.MessageRepository.GetMessageThread(currentUsername, partner));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteMessage(int id)
    {
        var username = User.GetUsername();

        var message = await unitOfWork.MessageRepository.GetMessage(id);

        if (message == null) return BadRequest("cannot delete this message");

        if (message.SenderUsername != username && message.RecipientUsername != username) return Forbid();

        if (message.SenderUsername == username) message.SenderDeleted = true;
        if (message.RecipientUsername == username) message.RecipientDeleted = true;

        if (message is { SenderDeleted: true, RecipientDeleted: true })
        {
            unitOfWork.MessageRepository.Delete(message);
        }

        if (await unitOfWork.Complete()) return Ok();

        return BadRequest("Problem deleting the message");
    }
}