using Microsoft.EntityFrameworkCore;
using NotifyBotApi.Data;
using NotifyBotApi.Interfaces;
using NotifyBotApi.Models;
using Org.BouncyCastle.Bcpg.OpenPgp;

namespace NotifyBotApi.Repositories
{
    public class ContactRepository : IContactRepository
    {
        private readonly NotifyBotContext context;

        public ContactRepository(NotifyBotContext context)
        {
            this.context = context;
        }

        public async Task<bool> SaveChangeAsync()
        {
            var result = await context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> AddContact(Contact contact)
        {
            context.Contacts.Add(contact);
            return await SaveChangeAsync();
        }

        public async Task<bool> RemoveContact(Contact contact)
        {
            context.Contacts.Remove(contact);
            return await SaveChangeAsync();
        }

        public async Task<bool> DeleteAllContacts()
        {
            var contacts = await context.Contacts.ToListAsync();
            context.Contacts.RemoveRange(contacts);
            return await SaveChangeAsync();
        }

        public async Task<Contact> GetContact(Guid contactId)
        {
            var contact = await context.Contacts.FindAsync(contactId);
            return contact;
        }

        public async Task<ICollection<Contact>> GetContacts(int pageIndex, int pageSize)
        {
            var skipSize = pageSize * pageIndex;
            var contacts = await context.Contacts
                .Skip(skipSize)
                .Take(pageSize)
                .ToListAsync();

            return contacts;
        }

        public async Task<int> GetCountContacts()
        {
            var length = await context.Contacts.CountAsync();
            return length;
        }
    }
}
