using NotifyBotApi.Models;

namespace NotifyBotApi.Interfaces
{
    public interface IContactRepository
    {
        Task<bool> SaveChangeAsync();
        Task<bool> AddContact(Contact contact);

        Task<bool> RemoveContact(Contact contact);
        Task<bool> DeleteAllContacts();

        Task<Contact> GetContact(Guid contactId);
        Task<ICollection<Contact>> GetContacts(int pageIndex, int pageSize);

        Task<int> GetCountContacts();
    }
}
