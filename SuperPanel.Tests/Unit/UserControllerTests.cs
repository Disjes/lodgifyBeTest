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
            var misc = new Misc() { PageSize = 10 };
            var miscOptions = Options.Create(misc);
            _miscConfig.Setup(m => m.Value).Returns(miscOptions.Value);

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
        public async void UsersIndexFirstPage_ReturnsList()
        {
            // Arrange
            var usersList = new AutoFaker<User>()
                .Generate(25);
            var misc = new Misc() { PageSize = 10 };
            var miscOptions = Options.Create(misc);
            _miscConfig.Setup(m => m.Value).Returns(miscOptions.Value);

            int pageNumber = 1;  // The desired page number
            int pageSize = _miscConfig.Object.Value.PageSize;  // Number of items per page, in this case, we use the total count of generated users
            var pagedList = new PagedList<User>(usersList, pageNumber, pageSize);
            _userRepository.Setup(ur => ur.QueryAll(pageNumber, pageSize))
                .Returns(Task.FromResult<IPagedList<User>>(pagedList));

            var controller = new UsersController(
                _logger.Object,
                _userRepository.Object,
                _contactsService.Object,
                _miscConfig.Object
            );

            // Act
            var actionResultFirstPage = (await controller.Index(1)) as ViewResult;

            // Assert
            var resultFirstPage = actionResultFirstPage?.Model as IPagedList<User>;
            resultFirstPage.Should().BeOfType<PagedList<User>>()
                .Which.Should().HaveCount(10);
        }
        
        [Fact]
        public async void UsersIndexSecondPage_ReturnsList()
        {
            // Arrange
            var usersList = new AutoFaker<User>()
                .Generate(25);
            var misc = new Misc() { PageSize = 10 };
            var miscOptions = Options.Create(misc);
            _miscConfig.Setup(m => m.Value).Returns(miscOptions.Value);

            int pageNumber = 2;  // The desired page number
            int pageSize = _miscConfig.Object.Value.PageSize;  // Number of items per page, in this case, we use the total count of generated users
            var pagedList = new PagedList<User>(usersList, pageNumber, pageSize);
            _userRepository.Setup(ur => ur.QueryAll(pageNumber, pageSize))
                .Returns(Task.FromResult<IPagedList<User>>(pagedList));

            var controller = new UsersController(
                _logger.Object,
                _userRepository.Object,
                _contactsService.Object,
                _miscConfig.Object
            );

            // Act
            var actionResultSecondPage = (await controller.Index(2)) as ViewResult;

            // Assert
            var resultSecondPage = actionResultSecondPage?.Model as IPagedList<User>;
            resultSecondPage.Should().BeOfType<PagedList<User>>()
                .Which.Should().HaveCount(10);
        }
        
        [Fact]
        public async void UsersIndexThirdPage_ReturnsList()
        {
            // Arrange
            var usersList = new AutoFaker<User>()
                .Generate(25);
            var misc = new Misc() { PageSize = 10 };
            var miscOptions = Options.Create(misc);
            _miscConfig.Setup(m => m.Value).Returns(miscOptions.Value);

            int pageNumber = 3;  // The desired page number
            int pageSize = _miscConfig.Object.Value.PageSize;  // Number of items per page, in this case, we use the total count of generated users
            var pagedList = new PagedList<User>(usersList, pageNumber, pageSize);
            _userRepository.Setup(ur => ur.QueryAll(pageNumber, pageSize))
                .Returns(Task.FromResult<IPagedList<User>>(pagedList));

            var controller = new UsersController(
                _logger.Object,
                _userRepository.Object,
                _contactsService.Object,
                _miscConfig.Object
            );

            // Act
            var actionResultThirdPage = (await controller.Index(3)) as ViewResult;

            // Assert
            var resultThirdPage = actionResultThirdPage?.Model as IPagedList<User>;
            resultThirdPage.Should().BeOfType<PagedList<User>>()
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
            var actionResult = controller.Delete(int.MinValue).Result as ObjectResult;

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
            var actionResult = controller.Delete(int.MinValue).Result as ObjectResult;

            // Assert
            actionResult?.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        }
    }
}