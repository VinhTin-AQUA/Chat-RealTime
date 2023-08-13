using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using NotifyBotApi.DTOs.Chat;
using NotifyBotApi.Interfaces;
using NotifyBotApi.Models;

namespace NotifyBotApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Manager,User")]
    public class MessageChatController : ControllerBase
    {
        private readonly IMessageChatRepository messageChatRepository;

        public MessageChatController(IMessageChatRepository messageChatRepository)
        {
            this.messageChatRepository = messageChatRepository;
        }


        [HttpGet("messages-of-group")]
        public async Task<IActionResult> GetMessagesOfGroup([FromQuery] string groupId)
        {
            if (string.IsNullOrEmpty(groupId))
            {
                return BadRequest(ModelState);
            }
            var messages = await messageChatRepository.GetMessagesChatOfGroup(groupId);

            var messageToView = messages
                .OrderBy(x => x.DateSend)
                .Select(m => new MessageChatToView
                {
                    Id = m.Id,
                    Sender = m.Sender,
                    Content = m.Content,
                    DateSend = m.DateSend
                }).ToList();
            return Ok(messageToView);
        }

        [HttpPost("send-message")]
        public async Task<IActionResult> SendMessage(MessageChatToSend message, [FromQuery]string groupId)
        {
            if(message == null || string.IsNullOrEmpty(message.Content) || string.IsNullOrEmpty(message.Sender))
            {
                return BadRequest(ModelState);
            }

            var messageToSend = new MessageChat
            {
                Id = Guid.NewGuid().ToString(),
                Sender = message.Sender,
                Content = message.Content,
                DateSend = DateTime.UtcNow,
                GroupId = groupId
            };

            if(await messageChatRepository.SendMessage(messageToSend))
            {
                var messageToView = new MessageChatToView
                {
                    Id = messageToSend.Id,
                    Sender = messageToSend.Sender,
                    Content = messageToSend.Content,
                    DateSend = DateTime.UtcNow,
                };
                return Ok(messageToView);
            }
            return BadRequest(ModelState);
        }
    }
}
