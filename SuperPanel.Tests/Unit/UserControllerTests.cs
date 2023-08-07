using System.Collections.Generic;
using System.Threading.Tasks;
using AutoBogus;
using Infrastructure.Models;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Services.Services;
using SuperPanel.App.Controllers;
using SuperPanel.App.Models;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Services.Exceptions;
using X.PagedList;
using Xunit;

namespace SuperPanel.Tests.Unit
{
    public class UserControllerTests
    {
        private Mock<ILogger<UsersController>> _logger;
        private Mock<IUserRepository> _userRepository;
        private Mock<IContactsService> _contactsService;
        private Mock<IOptions<Misc>> _miscConfig;

        public UserControllerTests()
        {
            _logger = new Mock<ILogger<UsersController>>();
            _userRepository = new Mock<IUserRepository>();
            _contactsService = new Mock<IContactsService>();
            _miscConfig = new Mock<IOptions<Misc>>();
        }

        [Fact]
        public async void UsersIndex_ReturnsOkResult()
        {
            // Arrange
            var usersList = new AutoFaker<User>()
                .Generate(1);

            int pageNumber = 1;  // The desired page number
            int pageSize = usersList.Count;  // Number of items per page, in this case, we use the total count of generated users
            var pagedList = new StaticPagedList<User>(usersList, pageNumber, pageSize, usersList.Count);
            _userRepository.Setup(ur => ur.QueryAll(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(Task.FromResult<IPagedList<User>>(pagedList));

            var controller = new UsersController(
                _logger.Object,
                _userRepository.Object,
                _contactsService.Object,
                _miscConfig.Object
                );

            // Act
            var actionResult = await controller.Index(1);

            // Assert
            var result = actionResult as ObjectResult;
            result?.StatusCode.Should().Be(StatusCodes.Status200OK);
        }

        [Fact]
        public async void UsersIndex_ReturnsList()
        {
            // Arrange
            var usersList = new AutoFaker<User>()
                .Generate(5);

            int pageNumber = 1;  // The desired page number
            int pageSize = usersList.Count;  // Number of items per page, in this case, we use the total count of generated users
            var pagedList = new StaticPagedList<User>(usersList, pageNumber, pageSize, usersList.Count);
            _userRepository.Setup(ur => ur.QueryAll(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(Task.FromResult<IPagedList<User>>(pagedList));
            
            var controller = new UsersController(
                _logger.Object,
                _userRepository.Object,
                _contactsService.Object,
                _miscConfig.Object
            );

            // Act8
            var actionResult = (await controller.Index(1)) as ObjectResult;

            // Assert
            var result = actionResult?.Value as IEnumerable<User>;
            result.Should().BeOfType<List<User>>()
                .Which.Should().HaveCount(5);
            result.Should().BeEquivalentTo(usersList);
        }
        
        [Fact]
        public async void UsersIndex_ReturnsPaginationCorrectly()
        {
            // Arrange
            var usersList = new AutoFaker<User>()
                .Generate(5);

            int pageNumber = 1;  // The desired page number
            int pageSize = usersList.Count;  // Number of items per page, in this case, we use the total count of generated users
            var pagedList = new StaticPagedList<User>(usersList, pageNumber, pageSize, usersList.Count);
            _userRepository.Setup(ur => ur.QueryAll(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(Task.FromResult<IPagedList<User>>(pagedList));

            var controller = new UsersController(
                _logger.Object,
                _userRepository.Object,
                _contactsService.Object,
                _miscConfig.Object
            );

            // Act
            var actionResult = (await controller.Index(1)) as ObjectResult;

            // Assert
            var result = actionResult?.Value as IEnumerable<User>;
            result.Should().BeOfType<List<User>>()
                .Which.Should().HaveCount(5);
            result.Should().BeEquivalentTo(usersList);
        }
        
        [Fact]
        public async void InputFieldGetList_ReturnsList()
        {
            // Arrange
            var usersList = new AutoFaker<User>()
                .Generate(25);

            int pageNumber = 1;  // The desired page number
            int pageSize = usersList.Count;  // Number of items per page, in this case, we use the total count of generated users
            var pagedList = new StaticPagedList<User>(usersList, pageNumber, pageSize, usersList.Count);
            _userRepository.Setup(ur => ur.QueryAll(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(Task.FromResult<IPagedList<User>>(pagedList));

            var controller = new UsersController(
                _logger.Object,
                _userRepository.Object,
                _contactsService.Object,
                _miscConfig.Object
            );

            // Act
            var actionResultFirstPage = (await controller.Index(1)) as ObjectResult;
            var actionResultSecondPage = (await controller.Index(2)) as ObjectResult;
            var actionResultThirdPage = (await controller.Index(3)) as ObjectResult;

            // Assert
            var resultFirstPage = actionResultFirstPage?.Value as IPagedList<User>;
            var resultSecondPage = actionResultSecondPage?.Value as IPagedList<User>;
            var resultThirdPage = actionResultThirdPage?.Value as IPagedList<User>;
            resultFirstPage.Should().BeOfType<IPagedList<User>>()
                .Which.Should().HaveCount(10);
            resultSecondPage.Should().BeOfType<IPagedList<User>>()
                .Which.Should().HaveCount(10);
            resultThirdPage.Should().BeOfType<IPagedList<User>>()
                .Which.Should().HaveCount(5);
        }

        [Fact]
        public void UsersDelete_WhenOk_ReturnsNoContent()
        {
            // Arrange
            _contactsService.Setup(ur => ur.DeleteContact(It.IsAny<int>()))
                .Returns(Task.CompletedTask);

            var controller = new UsersController(
                _logger.Object,
                _userRepository.Object,
                _contactsService.Object,
                _miscConfig.Object
            );

            // Act
            var actionResult = controller.Delete(long.MinValue).Result as ObjectResult;

            // Assert
            actionResult?.StatusCode.Should().Be(StatusCodes.Status204NoContent);
        }
        
        [Fact]
        public void UsersDelete_WhenUserDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            var usersList = new AutoFaker<User>()
                .Generate(5);

            _contactsService.Setup(ur => ur.DeleteContact(It.IsAny<int>()))
                .Throws(() => new NotFoundException("User does not exist."));

            var controller = new UsersController(
                _logger.Object,
                _userRepository.Object,
                _contactsService.Object,
                _miscConfig.Object
            );

            // Act
            var actionResult = controller.Delete(long.MinValue).Result as ObjectResult;

            // Assert
            actionResult?.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        }
    }
}