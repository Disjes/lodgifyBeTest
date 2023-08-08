namespace Services;

public interface IHttpClientWrapper
{
    Task<HttpResponseMessage> GetAsync(string requestUri, HttpContent content);
    Task<HttpResponseMessage> PutAsync(string requestUri, HttpContent content);
}