using NotifyBotApi.Models;

namespace NotifyBotApi.Interfaces
{
    public interface IMessageChatRepository
    {
        Task<bool> Save();
        Task<ICollection<MessageChat>> GetMessagesChatOfGroup(string groupId);
        Task<bool> SendMessage(MessageChat message);
    }
}
