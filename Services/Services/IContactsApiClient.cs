using Services.Models;

namespace Services.Services
{
    public interface IContactsApiClient
    {
        Task<Contact> GetUserContactByEmailAsync(string email);
        Task<Contact> GetUserContactByIdAsync(long id);
        Task<Contact> GDPRRequest(long id);
    }
}