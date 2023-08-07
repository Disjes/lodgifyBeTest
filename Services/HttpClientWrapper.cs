using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Services.Models;

namespace Services;

class HttpClientWrapper : IHttpClientWrapper
{
    private readonly HttpClient _httpClient;
    private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;
    private readonly AsyncCircuitBreakerPolicy<HttpResponseMessage> _circuitBreakerPolicy;
    private readonly ExternalContactsApiOptions _externalContactsApiOptions;
    
    public HttpClientWrapper(HttpClient httpClient, IOptions<ExternalContactsApiOptions> externalContactsApiOptions)
    {
        _httpClient = httpClient;
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
    
    public async Task<HttpResponseMessage> GetAsync(string email)
    {
        string url = $"{_externalContactsApiOptions.BaseUrl}/{email}";
        //Combining the retry and circuit breaker policies
        var policyWrap = Policy.WrapAsync(_retryPolicy, _circuitBreakerPolicy);
        HttpResponseMessage response = await policyWrap.ExecuteAsync(() => _httpClient.GetAsync(url));

        if (response.IsSuccessStatusCode)
        {
            return response;
        }
        response.EnsureSuccessStatusCode();
        return null;
    }

    public Task<HttpResponseMessage> GetAsync(string requestUri, HttpContent content)
    {
        throw new NotImplementedException();
    }

    public Task<HttpResponseMessage> PutAsync(string requestUri, HttpContent content)
    {
        throw new NotImplementedException();
    }
}