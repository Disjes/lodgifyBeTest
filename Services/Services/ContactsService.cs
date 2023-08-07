using Infrastructure.Repositories;
using Services.Exceptions;
using Services.Models;

namespace Services.Services
{
    public class ContactsService : IContactsService
    {
        private readonly IContactsApiClient _contactsApiClient;
        private readonly IUserRepository _userRepository;

        public ContactsService(IContactsApiClient contactsApiClient, IUserRepository userRepository)
        {
            _contactsApiClient = contactsApiClient;
            _userRepository = userRepository;
        }

        public async Task<Contact> GetContactByIdAsync(long id)
        {
            try
            {
                return await _contactsApiClient.GetUserContactByIdAsync(id);
            }
            catch (ContactsApiException ex)
            {
                throw ex;
            }
        }

        public async Task<Contact> GetContactByEmailAsync(string email)
        {
            try
            {
                return await _contactsApiClient.GetUserContactByEmailAsync(email);
            }
            catch (ContactsApiException ex)
            {
                throw ex;
            }
        }

        public async Task DeleteContact(long id)
        {
            try
            {
                //Getting contact's external id
                var user = (await _userRepository.QueryAll()).FirstOrDefault(u => u.Id == id);
                if (user == null)
                {
                    throw new NotFoundException("User does not exist.");
                }
                Contact contact = await _contactsApiClient.GetUserContactByEmailAsync(user.Email);
                
                //Whether the contact exists or not we should always delete the local user from InMemory user's list
                _userRepository.Remove(user.Id);
                
                //If contact was null means it doesn't exist so we shouldn't do the GDPRRequest, returning here
                if (contact == null)
                {
                    return;
                }

                //GDRP Request for equivalent user's contact
                _contactsApiClient.GDPRRequest(contact.Id);
            }
            catch (Exception ex)
            {
                throw new ContactsApiException("There was an error trying to delete the contact");
            }
        }
    }
}