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
        
        public void Dispose()
        {
            
        }
    }
}