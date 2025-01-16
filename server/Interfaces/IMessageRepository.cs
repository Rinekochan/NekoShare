using server.DTOs.Message;
using server.Entities;
using server.Helpers;

namespace server.Interfaces;

public interface IMessageRepository
{
    void Add(Message message);
    void Delete(Message message);
    Task<Message?> GetMessage(int id);
    Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams);
    Task<IEnumerable<MessageDto>> GetMessageThread(string sender, string recipient);
    void AddGroup(Group group);
    void RemoveConnection(Connection connection);
    Task<Connection?> GetConnection(string connectionId);
    Task<Group?> GetMessageGroup(string groupName);
    Task<Group?> GetGroupForConnection(string connectionId);
}