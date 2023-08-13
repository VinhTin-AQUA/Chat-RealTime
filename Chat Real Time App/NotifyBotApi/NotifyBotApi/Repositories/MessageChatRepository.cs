using Microsoft.EntityFrameworkCore;
using NotifyBotApi.Data;
using NotifyBotApi.Interfaces;
using NotifyBotApi.Models;

namespace NotifyBotApi.Repositories
{
    public class MessageChatRepository : IMessageChatRepository
    {
        private readonly NotifyBotContext context;

        public MessageChatRepository(NotifyBotContext context)
        {
            this.context = context;
        }

        public async Task<bool> Save()
        {
            var result = await context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<ICollection<MessageChat>> GetMessagesChatOfGroup(string groupId)
        {
            var messages = await context.MessageChats
                .Where(x => x.GroupId == groupId).ToListAsync();
            return messages;
        }

        public async Task<bool> SendMessage(MessageChat message)
        {
            context.MessageChats.Add(message);
            return await Save();
        }
    }
}
