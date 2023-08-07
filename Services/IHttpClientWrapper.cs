namespace Services;

public interface IHttpClientWrapper
{
    Task<HttpResponseMessage> GetAsync(string email);
    Task<HttpResponseMessage> PutAsync(string requestUri, HttpContent content);
}