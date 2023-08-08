using AutoBogus;
using Infrastructure.Repositories;
using Moq;
using Services.Models;
using Services.Services;
using Xunit;
using FluentAssertions;
using Services.Exceptions;
using User = Infrastructure.Models.User;

namespace Services.Tests.Unit;

public class ContactsServiceTests
{
    private Mock<IContactsApiClient> _contactsApiClient;
    private Mock<IUserRepository> _userRepository;

    public ContactsServiceTests()
    {
        _contactsApiClient = new Mock<IContactsApiClient>();
        _userRepository = new Mock<IUserRepository>();
    }

    [Fact]
    public async void GetContactByIdAsync_WhenCalled_ReturnsContact()
    {
        //Arrange
        var contact = new AutoFaker<Contact>().Generate();
        _contactsApiClient.Setup(ca => ca.GetUserContactByIdAsync(It.IsAny<long>())).
            Returns(Task.FromResult(contact));
        var service = new ContactsService(_contactsApiClient.Object, _userRepository.Object);

        //Act
        var result = await service.GetContactByIdAsync(long.MaxValue);
        
        //Assert
        result.Should().BeEquivalentTo(contact);
    }
    
    [Fact]
    public async void GetContactByEmailAsync_WhenCalled_ReturnsContact()
    {
        //Arrange
        var contact = new AutoFaker<Contact>().Generate();
        _contactsApiClient.Setup(ca => ca.GetUserContactByEmailAsync(It.IsAny<string>())).
            Returns(Task.FromResult(contact));
        var service = new ContactsService(_contactsApiClient.Object, _userRepository.Object);
        var email = "mycoolemail@hotmail.com";

        //Act
        var result = await service.GetContactByEmailAsync(email);
        
        //Assert
        result.Should().BeEquivalentTo(contact);
    }
    
    [Fact]
    public async void DeleteContactAsync_WhenCalled_DoesNotThrow()
    {
        //Arrange
        var contact = new AutoFaker<Contact>().Generate();
        var user = new AutoFaker<User>().Generate();
        _userRepository.Setup(ur => ur.FindById(It.IsAny<int>())).
            Returns(Task.FromResult<User>(user));
        _userRepository.Setup(ur => ur.Remove(It.IsAny<int>())).
            Returns(true);
        _contactsApiClient.Setup(ca => ca.GetUserContactByEmailAsync(It.IsAny<string>())).
            Returns(Task.FromResult(contact));
        _contactsApiClient.Setup(ca => ca.GDPRRequest(It.IsAny<long>())).
            Returns(Task.FromResult(contact));
        var service = new ContactsService(_contactsApiClient.Object, _userRepository.Object);

        //Act
        //Assert
        await service.Invoking(s => s.DeleteContact(int.MaxValue)).Should().NotThrowAsync<NotFoundException>();
    }
    
    [Fact]
    public async void DeleteContactAsync_WhenCalled_ThrowsNotFound()
    {
        //Arrange
        var contact = new AutoFaker<Contact>().Generate();
        _userRepository.Setup(ur => ur.FindById(It.IsAny<int>())).
            Returns(Task.FromResult<User>(null));
        _contactsApiClient.Setup(ca => ca.GetUserContactByEmailAsync(It.IsAny<string>())).
            Returns(Task.FromResult(contact));
        _contactsApiClient.Setup(ca => ca.GDPRRequest(It.IsAny<long>())).
            Returns(Task.FromResult(contact));
        var service = new ContactsService(_contactsApiClient.Object, _userRepository.Object);

        //Act
        //Assert
        await service.Invoking(s => s.DeleteContact(int.MaxValue)).Should().ThrowAsync<NotFoundException>()
            .WithMessage("User does not exist.");
    }
}