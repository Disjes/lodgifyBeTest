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

        public async Task DeleteContact(int id)
        {
            try
            {
                //Getting contact's external id
                var user = await _userRepository.FindById(id);
                if (user == null)
                {
                    throw new NotFoundException("User does not exist.");
                }

                //Whether the contact exists or not we should always delete the local user from InMemory user's list
                _userRepository.Remove(user.Id);

                var contact = await _contactsApiClient.GetUserContactByEmailAsync(user.Email);

                //If contact was null means it doesn't exist so we shouldn't do the GDPRRequest, returning here
                if (contact == null)
                {
                    return;
                }

                //GDRP Request for equivalent user's contact
                var gdprContact = await _contactsApiClient.GDPRRequest(contact.Id);

            }
            catch (NotFoundException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ContactsApiException("There was an error trying to delete the contact");
            }
        }

        public async Task BatchGDPR(int[] ids)
        {
            try
            {
                foreach (var id in ids)
                {
                    var user = await _userRepository.FindById(id);
                    //We get the external api contact to get the external Id for the GDPR call
                    var contact = await _contactsApiClient.GetUserContactByEmailAsync(user.Email);
                    //GDRP Request for equivalent user's contact
                    var gdprContact = await _contactsApiClient.GDPRRequest(contact.Id);
                }
            }
            catch (ContactsApiException ex)
            {
                throw ex;
            }
        }
    }
}