using System.Net.Http.Json;
using Api.Dtos;
using Domain.Roles;
using Domain.Users;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Tests.Common;
using Tests.Data;
using Xunit;
using System.Net;
using System.Net.Http.Headers;
using Api.Dtos.UserDtos;


namespace Api.Tests.Integration.Users;

public class UsersControllerTests : BaseIntegrationTest, IAsyncLifetime
{
    private readonly Role _adminRole;
    private readonly Role _userRole;
    private readonly User _adminUser;
    private readonly User _mainUser;
    
    public UsersControllerTests(IntegrationTestWebFactory factory) : base(factory)
    {
        _adminRole = RoleData.AdminRole();
        _userRole = RoleData.UserRole();
        _mainUser = UsersData.JustUser(_userRole.Id);
        _adminUser = UsersData.AdminUser(_adminRole.Id);
    }
[Fact]
public async Task ShouldCreateUser()
{
    // Arrange
    var firstName = "John";
    var lastName = "Doe";   
    var email = "john.doe@email.com";
    var password = "Roman@2";
    var request = new CreateUserDto(
        FirstName: firstName,
        LastName: lastName,
        Email: email,
        Password: password);

    // Act
    var response = await Client.PostAsJsonAsync("user/register", request);

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.OK);
    response.IsSuccessStatusCode.Should().BeTrue();

    var responseUser = await response.ToResponseModel<CreateUserDto>();
    responseUser.Should().NotBeNull();
    responseUser.Email.Should().Be(request.Email);

    var dbUser = await Context.Users.FirstOrDefaultAsync(x => x.Email == request.Email);
    dbUser.FirstName.Should().Be(firstName);
    dbUser.LastName.Should().Be(lastName);
    dbUser.Email.Should().Be(email);
    dbUser.Password.Should().Be(password);
}
[Fact]

public async Task ShouldFailToCreateUser_WithDuplicateEmail()
{
    // Arrange
    var request = new CreateUserDto(FirstName:"Roman", LastName:"Ivan", Email:_adminUser.Email, Password:"Roman@2");

    // Act
    var response = await Client.PostAsJsonAsync("user/register", request);

    // Assert
    response.IsSuccessStatusCode.Should().BeFalse();
    response.StatusCode.Should().Be(HttpStatusCode.Conflict);
}


   
    
    [Fact]
    public async Task ShouldFailToDeleteUser_WhenUnauthorized()
    {
        // Act
        var response = await Client.DeleteAsync($"user/delete/{_mainUser.Id}");

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
    }
    
    [Fact]
    public async Task ShouldDeleteUser()
    {
        // Arrange
        var authToken = await GenerateAuthTokenAsync(_adminUser.Email, _adminUser.Password);
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

        // Act
        var response = await Client.DeleteAsync($"user/delete/{_mainUser.Id.Value}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.IsSuccessStatusCode.Should().BeTrue();

        var deletedUser = await Context.Users.FirstOrDefaultAsync(u => u.Id == _mainUser.Id);
        deletedUser.Should().BeNull();
    }

    [Fact]
    public async Task ShouldFailToDeleteUserWhenUserDoesNotExist()
    {
        // Arrange
        var authToken = await GenerateAuthTokenAsync(_adminUser.Email, UsersData.passwordAdmin);
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
        
        var nonExistentUserId = Guid.NewGuid();
        // Act
        var response = await Client.DeleteAsync($"user/delete/{nonExistentUserId}");

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task ShouldUpdateUserEmail()
    {
        
        // Arrange
        var authToken = await GenerateAuthTokenAsync(_adminUser.Email, UsersData.passwordAdmin);
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

        var newEmail = "updatedemail@test.com";

        // Act
        var response = await Client.PutAsJsonAsync($"user/updateEmail/{_mainUser.Id.Value}", newEmail);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var updatedUser = await Context.Users.FirstOrDefaultAsync(u => u.Id == _mainUser.Id);
        updatedUser.Should().NotBeNull();
        updatedUser!.Email.Should().Be(newEmail);
    }
    
    [Fact]
    public async Task ShouldUpdateUserPassword()
    {
        
        // Arrange
        var authToken = await GenerateAuthTokenAsync(_adminUser.Email, UsersData.passwordAdmin);
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

        var newPassword = "Roman@31";

        // Act
        var response = await Client.PutAsJsonAsync($"user/updatePassword/{_mainUser.Id.Value}", newPassword);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var updatedUser = await Context.Users.FirstOrDefaultAsync(u => u.Id == _mainUser.Id);
        updatedUser.Should().NotBeNull();
        updatedUser!.Password.Should().Be(newPassword);
    }
    
    [Fact]
    public async Task ShouldFailToUpdateUserEmailWhenUnauthorized()
    {
        // Arrange
        var newEmail = "newunauthorizedemail@test.com";

        // Act
        var response = await Client.PutAsJsonAsync($"user/updateEmail/{_mainUser.Id.Value}", newEmail);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    public async Task InitializeAsync()
    {
        await Context.Roles.AddRangeAsync(_adminRole, _userRole);
        await Context.Users.AddRangeAsync(_adminUser, _mainUser);

        await SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        Context.Users.RemoveRange(Context.Users);
        Context.Roles.RemoveRange(Context.Roles);
        await Context.SaveChangesAsync();
    }
}
