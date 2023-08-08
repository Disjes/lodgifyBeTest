using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Services.Exceptions;
using Services.Models;

namespace Services.Services
{
    public class ContactsApiClient : IContactsApiClient
    {
        private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;
        private readonly AsyncCircuitBreakerPolicy<HttpResponseMessage> _circuitBreakerPolicy;
        private readonly ExternalContactsApiOptions _externalContactsApiOptions;
        private IOptions<ExternalContactsApiOptions> externalContactsApiOptions;
        
        public ContactsApiClient(IOptions<ExternalContactsApiOptions> externalContactsApiOptions)
        {
            _externalContactsApiOptions = externalContactsApiOptions.Value;
            // Configure the retry policy (retries number from config) with exponential backoff
            _retryPolicy = Policy.HandleResult<HttpResponseMessage>(response => !response.IsSuccessStatusCode)
                .WaitAndRetryAsync(_externalContactsApiOptions.RateLimit, retryAttempt => 
                    TimeSpan.FromMilliseconds(Math.Pow(_externalContactsApiOptions.PeriodSecondsRateLimit, retryAttempt)));

            // Configure the circuit breaker policy (Circuit will open after 3 consecutive failures)
            _circuitBreakerPolicy = Policy.HandleResult<HttpResponseMessage>(response => !response.IsSuccessStatusCode)
                .CircuitBreakerAsync(3, TimeSpan.FromSeconds(30),
                    onBreak: (result, timespan) => Console.WriteLine("Circuit opened."),
                    onReset: () => Console.WriteLine("Circuit closed."));
        }

        public async Task<Contact> GetUserContactByEmailAsync(string email)
        {
            try
            {
                using HttpClient httpClient = new HttpClient();
                string url = $"{_externalContactsApiOptions.BaseUrl}/{email}";
                //Combining the retry and circuit breaker policies
                var policyWrap = Policy.WrapAsync(_retryPolicy, _circuitBreakerPolicy);
                HttpResponseMessage response = await policyWrap.ExecuteAsync(() => httpClient.GetAsync(url));
                
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    Contact contact = JsonConvert.DeserializeObject<Contact>(content);
                    return contact;
                }

                return null;
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
                using HttpClient httpClient = new HttpClient();
                string url = $"{_externalContactsApiOptions.BaseUrl}/{id}";
                //Combining the retry and circuit breaker policies
                var policyWrap = Policy.WrapAsync(_retryPolicy, _circuitBreakerPolicy);
                HttpResponseMessage response = await policyWrap.ExecuteAsync(() => httpClient.GetAsync(url));

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    Contact contact = JsonConvert.DeserializeObject<Contact>(content);
                    return contact;
                }
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
                using HttpClient httpClient = new HttpClient();
                string url = $"{_externalContactsApiOptions.BaseUrl}/{id}";
                //Combining the retry and circuit breaker policies
                var policyWrap = Policy.WrapAsync(_retryPolicy, _circuitBreakerPolicy);
                HttpResponseMessage response = await policyWrap.ExecuteAsync(() => httpClient.GetAsync(url));

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    Contact contact = JsonConvert.DeserializeObject<Contact>(content);
                    return contact;
                }
                return null;
            }
            catch (BrokenCircuitException)
            {
                throw new ContactsApiException("Error occurred while connecting to the contacts API.");
            }
        }
    }
}