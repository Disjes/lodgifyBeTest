using Newtonsoft.Json;
using Polly;
using Polly.CircuitBreaker;
using Services.Exceptions;
using Services.Models;

namespace Services.Services
{
    public class ContactsApiClient : IContactsApiClient
    {
        private readonly IHttpClientWrapper _httpClientWrapper;

        public ContactsApiClient(IHttpClientWrapper httpClientWrapper)
        {
            _httpClientWrapper = httpClientWrapper;
        }

        public async Task<Contact> GetUserContactByEmailAsync(string email)
        {
            try
            {
               
            }
            catch (BrokenCircuitException)
            {
                throw new ContactsApiException("Error occurred while connecting to the contacts API.");
            }
        }

        public async Task<Contact> GetUserContactByIdAsync(long id)
        {
            try
            {
                string url = $"{_externalContactsApiOptions.BaseUrl}/{id}";
                
                var policyWrap = Policy.WrapAsync(_retryPolicy, _circuitBreakerPolicy);
                HttpResponseMessage response = await policyWrap.ExecuteAsync(() => _httpClient.GetAsync(url));

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    Contact contact = JsonConvert.DeserializeObject<Contact>(content);
                    return contact;
                }
                response.EnsureSuccessStatusCode();
                return null;
            }
            catch (BrokenCircuitException)
            {
                throw new ContactsApiException("Error occurred while connecting to the contacts API.");
            }
        }

        public async Task<Contact> GDPRRequest(long id)
        {
            try
            {
                string url = $"{_externalContactsApiOptions.BaseUrl}/gdpr/{id}";
                //Combining the retry and circuit breaker policies
                var policyWrap = Policy.WrapAsync(_retryPolicy, _circuitBreakerPolicy);
                HttpResponseMessage response = await policyWrap.ExecuteAsync(() => _httpClient.GetAsync(url));

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    Contact contact = JsonConvert.DeserializeObject<Contact>(content);
                    return contact;
                }
                response.EnsureSuccessStatusCode();
                return null;
            }
            catch (BrokenCircuitException)
            {
                throw new ContactsApiException("Error occurred while connecting to the contacts API.");
            }
        }
    }
}