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
    private readonly User _adminUser;
    private readonly User _mainUser;
    
    public UsersControllerTests(IntegrationTestWebFactory factory) : base(factory)
    {
        _adminRole = RoleData.AdminRole();
        _mainUser = UsersData.JustUser(_adminRole.Id);
        _adminUser = UsersData.AdminUser(_adminRole.Id);
    }

    [Fact]
    public async Task ShouldCreateUser()
    {
        // Arrange
        var authToken = await GenerateAuthTokenAsync(_adminUser.Email, _adminUser.Password);
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
        var firstName = "John";
        var lastName = "Doe";   
        var email = "john.doe@email.com";
        var password = "password";
        var request = new UserDto(
            Id: null,
            FirstName: firstName,
            LastName: lastName,
            Email: email ,
            Password: password ,
            RoleId: _adminRole.Id.Value,
            Role: null);

        // Act
        var response = await Client.PostAsJsonAsync("users", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var responseUser = await response.ToResponseModel<UserDto>();
        var userId = new UserId(responseUser.Id!.Value);

        var dbUser = await Context.Users.FirstAsync(x => x.Id == userId);
        dbUser.FirstName.Should().Be(firstName);
        dbUser.LastName.Should().Be(lastName);
        dbUser.Email.Should().Be(email);
        dbUser.Password.Should().Be(password);
        dbUser.RoleId.Value.Should().Be(_adminRole.Id.Value);
    }

   
    [Fact]
    public async Task ShouldReturnErrorWhenRoleDoesNotExist()
    {
        // Arrange
        var authToken = await GenerateAuthTokenAsync(_adminUser.Email, _adminUser.Password);
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
        var firstName = "John";
        var lastName = "Doe";   
        var email = "john.doe@email.com";
        var password = "password";
        var nonExistentFacultyId = Guid.NewGuid(); 
        var request = new UserDto(
            Id: null,
            FirstName: firstName,
            LastName: lastName,
            Email: email ,
            Password: password ,
            RoleId: nonExistentFacultyId,
            Role: null);

        // Act
        var response = await Client.PostAsJsonAsync("users", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse(); 

        response.StatusCode.Should().Be(HttpStatusCode.NotFound); 

        var errorResponse = await response.Content.ReadAsStringAsync();
        
        errorResponse.Should().Contain("not found");
        
        var userInDb = await Context.Users.FirstOrDefaultAsync(x => x.FirstName == firstName && x.LastName == lastName);
        userInDb.Should().BeNull(); 
    }

    
    [Fact]
    public async Task ShouldUpdateUserSuccessfully()
    {
        var authToken = await GenerateAuthTokenAsync(_adminUser.Email, _adminUser.Password);
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
        var firstName = "John";
        var lastName = "Doe";   
        var email = "john.doe@email.com";
        var password = "password";
        var request = new UserDto(
            Id: _mainUser.Id.Value,
            FirstName: firstName,
            LastName: lastName,
            Email: email ,
            Password: password ,
            RoleId: _adminRole.Id.Value,
            Role: null);
        
        // Arrang

        // Act
        var response = await Client.PutAsJsonAsync($"users/{_mainUser.Id.Value}", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var updatedUserFromResponse = await response.ToResponseModel<UserDto>();
        var updatedUserId = new UserId(updatedUserFromResponse.Id!.Value);

        var updatedUserFromDb = await Context.Users.FirstOrDefaultAsync(x => x.Id == updatedUserId);
        updatedUserFromDb.Should().NotBeNull();


        updatedUserFromDb!.FirstName.Should().Be(firstName);
        updatedUserFromDb.LastName.Should().Be(lastName);
        updatedUserFromDb.Email.Should().Be(email);
        updatedUserFromDb.Password.Should().Be(password);
        updatedUserFromDb.RoleId.Value.Should().Be(_adminRole.Id.Value);
    }

    
    
    [Fact]
    public async Task ShouldNotUpdateUserBecauseRoleDoesNotExist()
    {
        // Arrange
        var authToken = await GenerateAuthTokenAsync(_adminUser.Email, _adminUser.Password);
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
        var firstName = "UpdatedJohn";
        var lastName = "UpdatedDoe";
        var email = "john.doe@email.com";
        var password = "password";
        var nonExistentUserId = Guid.NewGuid(); 
        var roleId = Guid.NewGuid(); 

        var request = new UserDto(
            Id: nonExistentUserId, 
            FirstName: firstName,
            LastName: lastName,
            Email: email ,
            Password: password ,
            RoleId: roleId,
            Role: null);

        // Act
        var response = await Client.PutAsJsonAsync($"users/{nonExistentUserId}", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse(); 
        response.StatusCode.Should().Be(HttpStatusCode.NotFound); 
    }
    
    public async Task InitializeAsync()
    {
        await Context.Roles.AddRangeAsync(_adminRole);
        await Context.Users.AddRangeAsync(_adminUser);

        await SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        Context.Users.RemoveRange(Context.Users);
        Context.Roles.RemoveRange(Context.Roles);

        await SaveChangesAsync();
    }
}
