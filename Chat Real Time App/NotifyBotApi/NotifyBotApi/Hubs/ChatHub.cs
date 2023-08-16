using Bogus.DataSets;
using Microsoft.AspNetCore.SignalR;
using NotifyBotApi.DTOs.Chat;
using NotifyBotApi.Interfaces;
using NotifyBotApi.Models;
using NotifyBotApi.Services;
using System.Reflection;

namespace NotifyBotApi.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IGroupRepository groupRepository;
        private readonly ChatService chatService;
        private readonly IMessageChatRepository messageChatRepository;
        private readonly IUserRepository userRepository;

        public ChatHub(IGroupRepository groupRepository,
            ChatService chatService,
            IMessageChatRepository messageChatRepository,
            IUserRepository userRepository)
        {
            this.groupRepository = groupRepository;
            this.chatService = chatService;
            this.messageChatRepository = messageChatRepository;
            this.userRepository = userRepository;
        }

        public async Task ConnectGroup(string groupName, string userId)
        {
            await userRepository.UpdateConnectionId(userId, Context.ConnectionId);
            var connectionId = Context.ConnectionId;
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await Clients.Caller.SendAsync("UserConected", connectionId);
        }

        public async Task DisConnectedGroup(string groupName, string myName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }

        public async Task AddNewGroup(string groupName, string userId)
        {
            var groupExist = await groupRepository.GroupNameExist(groupName, userId);

            if (groupExist == null)
            {
                var groupModel = new Group
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = groupName,
                    DateCreated = DateTime.UtcNow,
                    HasNewMessage = false
                };
                // add group
                _ = await groupRepository.AddGroup(groupModel);

                var groupUserModel = new GroupUser
                {
                    UserId = userId,
                    GroupId = groupModel.Id
                };

                // add groupUser
                _ = await groupRepository.AddGroupUser(groupUserModel);
                GroupToViewDto groupToView = new GroupToViewDto
                {
                    Id = groupModel.Id,
                    Name = groupModel.Name,
                    DateCreated = groupModel.DateCreated
                };
                await Clients.Caller.SendAsync("NewGroup", groupToView);
                return;
            }
            await Clients.Caller.SendAsync("NewGroup", "Group name is taken, please choose other group name.");
        }

        public async Task LeaveGroup(string groupId, string userId)
        {
            GroupToViewDto group = null;
            if (string.IsNullOrEmpty(groupId) || string.IsNullOrEmpty(userId) ||
                await groupRepository.GroupExistById(groupId) == false)
            {
                group = null;
                await Clients.Caller.SendAsync("LeaveGroup", group);
                return;
            }

            var user = await userRepository.GetUserById(userId);

            if (user == null)
            {
                group = null;
                await Clients.Caller.SendAsync("LeaveGroup", group);
                return;
            }

            var groupUser = new GroupUser { UserId = user.Id, GroupId = groupId };

            if (await groupRepository.LaaveGroup(groupUser) == false)
            {
                await Clients.Caller.SendAsync("LeaveGroup", group);
                return;
            }


            await groupRepository.DeleteGroupEmpty(groupId);
            string onlineUser = user.FirstName + " " + user.LastName;
            await Clients.Caller.SendAsync("LeaveGroup", groupId);
            await Clients.Others.SendAsync("LeaveGroupOthers", onlineUser);
        }

        public async Task RecieveMessage(string groupName, string groupId, MessageChatToSend message)
        {
            var messageToSend = new MessageChat
            {
                Id = Guid.NewGuid().ToString(),
                Sender = message.Sender,
                Content = message.Content,
                DateSend = DateTime.UtcNow,
                GroupId = groupId
            };

            // lưu vào database
            var result = await messageChatRepository.SendMessage(messageToSend);
            if(result == false)
            {
                return;
            }
            var messageToView = new MessageChatToView
            {
                Id = messageToSend.Id,
                Sender = messageToSend.Sender,
                Content = messageToSend.Content,
                DateSend = messageToSend.DateSend,
            };
            //await Clients.OthersInGroup(groupName).SendAsync("GroupHasNewMessage", groupName);
            await Clients.Groups(groupName).SendAsync("NewMessage", messageToView);
            await Clients.Others.SendAsync("GroupHasNewMessage", groupName);
        }

        public async Task AddUsersOnline(string groupName, string userName)
        {
            await Clients.Group(groupName).SendAsync("NewOnlineUser", userName);
        }

        public async Task RemoveOnlineUser(string groupName, string onlineUser)
        {
            if(string.IsNullOrEmpty(groupName) == false)
            {
                chatService.RemoveUserOnline(groupName, onlineUser);
                await Clients.Groups(groupName).SendAsync("RemoveOnlineUser", onlineUser);
            }
        }

        //được người khác add vào
        public async Task AddedToGroup(string email, string groupId)
        {
            var user = await userRepository.GetUserByEmail(email);
            var group = await groupRepository.GetGroupById(groupId);
            var groupToView = new GroupToViewDto
            {
                Id = group.Id,
                Name = group.Name,
                DateCreated = DateTime.Now,
            };
            await Clients.Client(user.ConnectionId).SendAsync("AddedToGroup", groupToView);
        }

        // set HasNewMessage
        public async Task SetHasNewMessage(string groupId, bool hasNewMessage)
        {
            await groupRepository.SetHasNewMessage(groupId, hasNewMessage);
        }
    }
}
