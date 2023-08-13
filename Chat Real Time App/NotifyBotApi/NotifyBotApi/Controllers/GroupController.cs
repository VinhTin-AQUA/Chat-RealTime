
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotifyBotApi.DTOs.Chat;
using NotifyBotApi.Interfaces;
using NotifyBotApi.Models;
using NotifyBotApi.Services;

namespace NotifyBotApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Manager,User")]
    public class GroupController : ControllerBase
    {
        private readonly IGroupRepository groupRepository;
        private readonly IUserRepository userRepository;
        private readonly ChatService chatService;

        public GroupController(IGroupRepository groupRepository,
            IUserRepository userRepository,
            ChatService chatService)
        {
            this.groupRepository = groupRepository;
            this.userRepository = userRepository;
            this.chatService = chatService;
        }

        [HttpGet("get-groups")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllGroups()
        {
            var groups = await groupRepository.GetAllGroups();

            var groupToViews = groups.Select(g => new GroupToViewDto
            {
                Id = g.Id,
                Name = g.Name,
                DateCreated = g.DateCreated,
                HasNewMessage = false,
            }).ToList();

            foreach(var group in groupToViews)
            {
                var members = await groupRepository.GetUsersOfGroup(group.Id);

                var online = chatService.GetUsersOnlineAGroup(group.Name).ToArray().Length;

                group.Memebers = members.ToArray().Length;
                group.Onlines = online;
            }

            return Ok(groupToViews);
        }

        [HttpGet("get-group-of-user")]
        public async Task<IActionResult> GetGroupOfUser([FromQuery] string userId)
        {
            if(string.IsNullOrEmpty(userId))
            {
                return BadRequest(ModelState);
            }

            var groups = await groupRepository.GetGroupOfUser(userId);

            var groupToViews = groups.Select(g => new GroupToViewDto
            {
                Id = g.Id,
                Name = g.Name,
                DateCreated = g.DateCreated,
                HasNewMessage = g.HasNewMessage
            }).ToList();

            return Ok(groupToViews);
        }

        /* vừa tạo nhóm vừa add trưởng nhóm vào nhóm vừa tạo */
        [HttpPost("add-group-user")]
        public async Task<ActionResult<GroupToViewDto>> AddGroupUser(GroupToAddDto model, [FromQuery] string userId) 
        {

            if(string.IsNullOrEmpty(userId) || model == null) {
                return BadRequest(ModelState);
            }

            if(await userRepository.GetUserById(userId) == null)
            {
                return BadRequest("User not found");
            }

            if(await groupRepository.GroupNameExist(model.Name, userId) != null)
            {
                return BadRequest("Group is taken, plese choose other name");
            }

            // add group to Groups Table
            var group = new Group
            {
                Id = Guid.NewGuid().ToString(),
                Name = model.Name,
                DateCreated = DateTime.UtcNow,
            };
            if(await groupRepository.AddGroup(group) == false) {
                return BadRequest("Error when add group");
            }

            // add GroupUser to GroupUsers Table
            var groupUser = new GroupUser
            {
                UserId = userId,
                GroupId = group.Id
            };

            if(await groupRepository.AddGroupUser(groupUser) == false) {
                return BadRequest("Error when add group");
            }

            var groupToView = new GroupToViewDto
            {
                Id = group.Id,
                Name = group.Name,
                DateCreated = group.DateCreated,
                HasNewMessage = true
            };

            return Ok(groupToView);
        }

        /* thêm 1 thành viên vào nhóm*/
        [HttpPost("add-user-to-group")]
        public async Task<IActionResult> AddUserToGroup([FromQuery] string email, [FromQuery] string groupId)
        {

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(groupId))
            {
                return BadRequest(ModelState);
            }

            var user = await userRepository.GetUserByEmail(email);
            if (user == null)
            {
                return BadRequest("User not found");
            }

            if(await groupRepository.GroupExistById(groupId) == false) 
            {
                return BadRequest("Group not found");
            }

            var groupUser = new GroupUser
            {
                UserId = user.Id,
                GroupId = groupId
            };
            var reusult = await groupRepository.AddUserToGroup(groupUser);
            if(reusult ==  false)
            {
                return BadRequest("Something error when add user to group");
            } 
            return Ok(new {userName = user.FirstName + " " + user.LastName });
        }

        /* leave group */
        [HttpPut("leave-group")]
        public async Task<IActionResult> LeaveGroup([FromQuery] string groupId, [FromQuery] string userId)
        {
            if (string.IsNullOrEmpty(groupId) || string.IsNullOrEmpty(userId))
            {
                return BadRequest(ModelState);
            }

            if (await groupRepository.GroupExistById(groupId) == false)
            {
                return BadRequest("Group not found");
            }
            var user = await userRepository.GetUserById(userId);
            if (user == null)
            {
                return BadRequest("User not found");
            }

            var groupUser = new GroupUser { UserId = user.Id, GroupId = groupId };
            if (await groupRepository.LaaveGroup(groupUser) == false)
            {
                return BadRequest("Something error when leave group");
            }

            return Ok(new JsonResult(new { title = "Success", message = "Leave group successfully" }));
        }

        /* users of a group*/
        [HttpGet("users-of-a-group")]
        public async Task<IActionResult> UsersOfAGroup([FromQuery]string groupId)
        {
            if(string.IsNullOrEmpty(groupId))
            {
                return BadRequest(ModelState);
            }

            if(await groupRepository.GroupExistById(groupId) == false)
            {
                return BadRequest("Group not found");
            }

            var users = await groupRepository.GetUsersOfGroup(groupId);
            return Ok(users);
        }

        /* user Online of a group*/
        [HttpGet("get-users-online-of-group")]
        public IActionResult GetOnlineUsers([FromQuery] string groupName)
        {
            if (string.IsNullOrEmpty(groupName))
            {
                return BadRequest(ModelState);
            }
            var users = chatService.GetUsersOnlineAGroup(groupName);
            return Ok(users);
        }

        /* add Online of a group*/
        [HttpPost("add-user-online")]
        public IActionResult AddOnlineUsers([FromQuery] string groupName, string userName)
        {
            if (string.IsNullOrEmpty(groupName) || string.IsNullOrEmpty(userName))
            {
                return BadRequest(ModelState);
            }

            if (chatService.AddUserOnline(groupName, userName) == false)
            {
                return BadRequest(ModelState);
            }
            return Ok(new { userOnline = userName });
        }

        /* remove Online of a group*/
        [HttpPost("remove-user-online")]
        public IActionResult RemoveOnlineUsers([FromQuery] string groupName, string userName)
        {
            if (string.IsNullOrEmpty(groupName) || string.IsNullOrEmpty(userName))
            {
                return BadRequest(ModelState);
            }

            if (chatService.RemoveUserOnline(groupName, userName) == false)
            {
                return BadRequest(ModelState);
            }
            return NoContent();
        }
    }
}
