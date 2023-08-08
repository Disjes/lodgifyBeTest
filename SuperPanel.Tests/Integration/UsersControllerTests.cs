using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using SuperPanel.App;
using Xunit;
using FluentAssertions;
using Infrastructure.Models;
using Microsoft.AspNetCore.Mvc.Testing;

namespace SuperPanel.Tests.Integration
{
    public class UsersControllerTests: IClassFixture<CustomWebApplicationFactory<Program>>,
        IDisposable
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        //private readonly IConfigurationSection _auth0Settings;

        public UsersControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient(
                new WebApplicationFactoryClientOptions()
                {
                    BaseAddress = new Uri("https://localhost:5001")
                });
        }

        //[Fact]
        public async void Get_WhenCalled_ReturnsOkResult()
        {
            // Arrange
            var getRequestUrl = $"Users/Index";
            
            // Act
            var response = await _client.GetAsync(getRequestUrl);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        //[Fact]
        public async void Get_WhenCalled_ReturnsList()
        {
            // Arrange
            //_client.DefaultRequestHeaders.Add("Content-Type", "application/json");
            var getRequestUrl = $"Users/Index";

            // Act
            var response = await _client.GetAsync(getRequestUrl);
            var responseString = await response.Content.ReadAsStringAsync();
            var tmConfigListFromResponse = JsonConvert.DeserializeObject<List<User>>(responseString);

            // Assert
            tmConfigListFromResponse.Should().BeOfType<List<User>>();
        }

        /*[Fact]
        public async void GetById_UnknownIdPassed_ReturnsNotFoundResult()
        {
            // Arrange
            var notExistingId = int.MinValue;
            var client = _factory.CreateClient();
            var getRequestUrl = $"api/TenantIntegrationConfigTM/{notExistingId}";

            // Act
            var response = await _client.GetAsync(getRequestUrl);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async void GetById_ExistingGuidPassed_ReturnsOkResult()
        {
            // Arrange
            var existingId = _snowflake.tmConfigs.First().id;
            var client = _factory.CreateClient();
            var getRequestUrl = $"api/tmConfigs/{existingId}";

            // Act
            var response = await _client.GetAsync(getRequestUrl);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async void GetById_ExistingGuidPassed_ReturnsRightItem()
        {
            // Arrange
            var existingtmConfig = _snowflake.tmConfigs.First();
            var existingId = existingtmConfig.id;
            var client = _factory.CreateClient();
            var getRequestUrl = $"api/tmConfigs/{existingId}";

            // Act
            var response = await _client.GetAsync(getRequestUrl);

            var responseString = await response.Content.ReadAsStringAsync();
            var tmConfigFromResponse = JsonConvert.DeserializeObject<tmConfig>(responseString);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            tmConfigFromResponse.Should().BeEquivalentTo(existingtmConfig);
        }

        [Fact]
        public async void Add_InvalidObjectPassed_ReturnsBadRequest()
        {
            // Arrange
            var badtmConfig = new tmConfig()
            {
                description = "tmConfig 3", sku = "3456789", weight_ounces = 15, is_active = 1,
            };
            var client = _factory.CreateClient();
            var json = JsonConvert.SerializeObject(badtmConfig);
            var postRequest = new HttpRequestMessage(HttpMethod.Post, "api/tmConfigs/");
            postRequest.Content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.SendAsync(postRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async void Add_ValidObjectPassed_ReturnsCreatedResponse()
        {
            // Arrange
            var tmConfig = new tmConfig()
            {
                description = "tmConfig 4",
                name = "tmConfig 4",
                sku = "3456789",
                weight_ounces = 15,
                is_active = 1,
            };
            var client = _factory.CreateClient();
            var json = JsonConvert.SerializeObject(tmConfig);
            var postRequest = new HttpRequestMessage(HttpMethod.Post, "api/tmConfigs/");
            postRequest.Content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.SendAsync(postRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Fact]
        public async void Add_ValidObjectPassed_ReturnedResponseHasCreatedItem()
        {
            // Arrange
            var tmConfig = new tmConfig()
            {
                description = "tmConfig 4",
                name = "tmConfig 4",
                sku = "3456789",
                weight_ounces = 15,
                is_active = 1,
            };
            var client = _factory.CreateClient();
            var json = JsonConvert.SerializeObject(tmConfig);
            var postRequest = new HttpRequestMessage(HttpMethod.Post, "api/tmConfigs/");
            postRequest.Content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.SendAsync(postRequest);

            var responseString = await response.Content.ReadAsStringAsync();
            var tmConfigFromResponse = JsonConvert.DeserializeObject<tmConfig>(responseString);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            tmConfigFromResponse.Should().BeEquivalentTo(tmConfig,
                o => o.Excluding(p => p.id));
        }

        [Fact]
        public async void Update_NotExistingGuidPassed_ReturnsNotFoundResponse()
        {
            // Arrange
            var notExistingId = int.MinValue;
            var tmConfig = new tmConfig()
            {
                id = notExistingId,
                description = "tmConfig 4",
                name = "tmConfig 4",
                sku = "3456789",
                weight_ounces = 15,
                is_active = 1,
            };
            var client = _factory.CreateClient();
            var json = JsonConvert.SerializeObject(tmConfig);
            var putRequest = new HttpRequestMessage(HttpMethod.Put, "api/tmConfigs/");
            putRequest.Content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.SendAsync(putRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async void Update_ExistingGuidPassed_ReturnsNoContentResult()
        {
            // Arrange
            var tmConfig = _snowflake.tmConfigs.First();
            tmConfig.name = "tmConfig X";
            var client = _factory.CreateClient();
            var json = JsonConvert.SerializeObject(tmConfig);
            var putRequest = new HttpRequestMessage(HttpMethod.Put, $"api/tmConfigs");
            putRequest.Content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.SendAsync(putRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }*/
        public void Dispose()
        {
            
        }
    }
}