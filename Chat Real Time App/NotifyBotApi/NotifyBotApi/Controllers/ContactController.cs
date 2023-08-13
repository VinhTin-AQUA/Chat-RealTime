using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NotifyBotApi.DTOs.Contact;
using NotifyBotApi.Interfaces;
using NotifyBotApi.Models;

namespace NotifyBotApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly IContactRepository contactRepository;

        public ContactController(IContactRepository contactRepository)
        {
            this.contactRepository = contactRepository;
        }

        [HttpPost("add-contact")]
        public async Task<IActionResult> AddContact(ContactDto model)
        {
            if (model == null)
            {
                return BadRequest(ModelState);
            }

            var contact = new Contact
            {
                Id = Guid.NewGuid(),
                Title = model.Title,
                Email = model.Email,
                Description = model.Description,
                DateCreated = DateTime.UtcNow
            };

            var result = await contactRepository.AddContact(contact);
            if (result == false)
            {
                return BadRequest("Something error when send contact.");
            }
            return Ok(new JsonResult(new { message = "Send contact successfully" }));
        }

        [HttpDelete("delete-contact")]
        public async Task<IActionResult> DeleteContact([FromQuery] Guid contactId)
        {
            if (contactId == Guid.Empty)
            {
                return BadRequest(ModelState);
            }

            var contact = await contactRepository.GetContact(contactId);

            if (contact == null)
            {
                return BadRequest("Contact not found when delete contact");
            }
            var result = await contactRepository.RemoveContact(contact);
            if(result == false)
            {
                return BadRequest("Something error when delete contact");
            }
            return Ok(new JsonResult(new { }));
        }

        [HttpDelete("delete-all-contacts")]
        public async Task<IActionResult> DeleteAdllContacts()
        {
            var result = await contactRepository.DeleteAllContacts();
            if (result == false)
            {
                return BadRequest("Something error when delete all contacts");
            }
            return Ok(new JsonResult(new { }));
        }

        [HttpGet("get-contacts")]
        public async Task<IActionResult> GetAllContacts([FromQuery]int pagIndex, [FromQuery] int pageSize)
        {
            var _contacts = await contactRepository.GetContacts(pagIndex, pageSize);
            var _length = await contactRepository.GetCountContacts();
            return Ok(new JsonResult(new
            {
                contacts = _contacts,
                size = _length
            }));
        }
    }
}
