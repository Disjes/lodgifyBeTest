using System.Collections.Generic;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Infrastructure.Models;
using Infrastructure.Repositories;
using Shouldly;
using Xunit;

namespace SuperPanel.Tests
{
    public class UserRepositoryTests
    {
        [Fact]
        public async void QueryAll_ShouldReturnEverything()
        {
            var r = new UserRepository(Options.Create<DataOptions>(new DataOptions()
            {
                JsonFilePath = "./../../../../data/users.json"
            }));

            var all = await r.QueryAll();

            Assert.Equal(5000, all.Count());
        }
        
        [Fact]
        public async Task FindById_ReturnsCorrectUser()
        {
            // Arrange
            var repo = new UserRepository(Options.Create<DataOptions>(new DataOptions()
            {
                JsonFilePath = "./../../../../data/users.json"
            }));

            // Act
            var user = await repo.FindById(14648);

            // Assert
            Assert.NotNull(user);
            Assert.Equal(14648, user.Id);
            Assert.Equal("Grover", user.FirstName);
        }
        
        [Fact]
        public async Task QueryAll_ReturnsListOfUsers()
        {
            // Arrange
            var repo = new UserRepository(Options.Create<DataOptions>(new DataOptions()
            {
                JsonFilePath = "./../../../../data/users.json"
            }));

            // Act
            var result = await repo.QueryAll();

            // Assert
            result.Should().BeOfType<List<User>>();
            result.Should().HaveCount((await repo.QueryAll()).Count());
        }
        
        [Fact]
        public async Task QueryAll_WithPagination_ReturnsCorrectPage()
        {
            // Arrange
            var repo = new UserRepository(Options.Create<DataOptions>(new DataOptions()
            {
                JsonFilePath = "./../../../../data/users.json"
            }));

            // Act
            var result = await repo.QueryAll(2, 1);

            // Assert
            Assert.Single(result);
        }
        
        [Fact]
        public async void Remove_DeletesUser_WhenUserExists()
        {
            // Arrange
            var repo = new UserRepository(Options.Create<DataOptions>(new DataOptions()
            {
                JsonFilePath = "./../../../../data/users.json"
            }));
            var firstId = (await repo.QueryAll()).FirstOrDefault().Id;

            // Act
            var wasRemoved = repo.Remove(firstId);

            // Assert
            Assert.True(wasRemoved);
            Assert.Null((await repo.QueryAll()).FirstOrDefault(u => u.Id == firstId));
        }
        
        [Fact]
        public void Remove_ReturnsFalse_WhenUserDoesNotExist()
        {
            // Arrange
            var repo = new UserRepository(Options.Create<DataOptions>(new DataOptions()
            {
                JsonFilePath = "./../../../../data/users.json"
            }));

            // Act
            var wasRemoved = repo.Remove(int.MaxValue);

            // Assert
            Assert.False(wasRemoved);
        }
    }
}
