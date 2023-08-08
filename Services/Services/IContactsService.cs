using Services.Models;

namespace Services.Services
{
    public interface IContactsService
    {
        Task<Contact> GetContactByIdAsync(long id);
        Task<Contact> GetContactByEmailAsync(string email);
        Task DeleteContact(int id);
    }
}