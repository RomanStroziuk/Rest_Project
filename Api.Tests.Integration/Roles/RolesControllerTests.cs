using System.Net;
using System.Net.Http.Json;
using Api.Dtos.RoleDtos;
using Domain.Users;
using Domain.Roles;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Tests.Common;
using Tests.Data;
using Xunit;
using System.Net.Http.Headers;


namespace Api.Tests.Integration.Roles;

public class RoleControllerTests : BaseIntegrationTest, IAsyncLifetime
{
    private readonly Role _adminRole;
    private readonly Role _mainRole;
    private readonly User _adminUser;
    
    
    public RoleControllerTests(IntegrationTestWebFactory factory) : base(factory)
    {
        _adminRole = RoleData.AdminRole();
        _adminUser = UsersData.AdminUser(_adminRole.Id);
        _mainRole = RoleData.MainRole();
    }
    
    [Fact]
    public async Task ShouldFailToCreateRole_WhenUnauthorized()
    {
        // Arrange
        const string roleName = "Unauthorized Role";
        var request = new RoleDto(Id: null, Title: roleName);

        // Act
        var response = await Client.PostAsJsonAsync("role/create", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }



    [Fact]
    public async Task ShouldCreateRole()
    {
        // Arrange
        var authToken = await GenerateAuthTokenAsync(_adminUser.Email, _adminUser.Password);
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
        const string roleName = "Test Role";
        var request = new RoleDto(
            Id: null,
            Title: roleName);

        // Act
        var response = await Client.PostAsJsonAsync("role/create", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var roleFromResponse = await response.ToResponseModel<RoleDto>();
        var roleId = new RoleId(roleFromResponse.Id!.Value);

        var roleFromDatabase = await Context.Roles.FirstOrDefaultAsync(x => x.Id == roleId);
        roleFromDatabase.Should().NotBeNull();
        roleFromDatabase!.Title.Should().Be(roleName);
    }
    
    [Fact]
    public async Task ShouldNotCreateRoleBecauseNameDuplicated()
    {
        // Arrange
        var authToken = await GenerateAuthTokenAsync(_adminUser.Email, _adminUser.Password);
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
        var request = new RoleDto(
            Id: null,
            Title: _mainRole.Title);

        // Act
        var response = await Client.PostAsJsonAsync("role/create", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }


    [Fact]
    public async Task ShouldUpdateRole()
    {
        // Arrange
        var authToken = await GenerateAuthTokenAsync(_adminUser.Email, _adminUser.Password);
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
        var newRoleName = "New Role Name";
        var request = new RoleDto(
            Id: _mainRole.Id.Value,
            Title: newRoleName);

        // Act
        var response = await Client.PutAsJsonAsync("role/update", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var roleFromResponse = await response.ToResponseModel<RoleDto>();

        var roleFromDatabase = await Context.Roles
            .FirstOrDefaultAsync(x => x.Id == new RoleId(roleFromResponse.Id!.Value));

        roleFromDatabase.Should().NotBeNull();
        roleFromDatabase!.Title.Should().Be(newRoleName);
    }
    
    [Fact]
    public async Task ShouldNotUpdateRoleBecauseRoleNotFound()
    {
        // Arrange
        var authToken = await GenerateAuthTokenAsync(_adminUser.Email, _adminUser.Password);
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
        var request = new RoleDto(
            Id: Guid.NewGuid(),
            Title: "New Role Name");

        // Act
        var response = await Client.PutAsJsonAsync("role/update", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    
    
    [Fact]
    public async Task ShouldDeleteRole()
    {
        // Arrange
        var authToken = await GenerateAuthTokenAsync(_adminUser.Email, _adminUser.Password);
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
        var roleId = _mainRole.Id.Value;
        
        // Act
        var response = await Client.DeleteAsync($"role/delete/{roleId}");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
       
        var roleFromResponse = await response.ToResponseModel<RoleDto>();
        var roleIdResponse = new RoleId(roleFromResponse.Id!.Value);
        
        var roleFromDatabase = await Context.Roles.FirstOrDefaultAsync(x => x.Id == roleIdResponse);
        
        roleFromDatabase.Should().BeNull();
    }
    
    [Fact]
    public async Task ShouldNotDeleteRoleBecauseRoleNotFound()
    {
        // Arrange
        var authToken = await GenerateAuthTokenAsync(_adminUser.Email, _adminUser.Password);
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
        var roleId = Guid.NewGuid();
        
        // Act
        var response = await Client.DeleteAsync($"role/delete/{roleId}");

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    
    [Fact]
    public async Task ShouldFailToDeleteRole_WhenUnauthorized()
    {
        //Arrange
        var roleId = _mainRole.Id.Value;

        // Act
        var response = await Client.DeleteAsync($"role/delete/{roleId}");

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    
    public async Task InitializeAsync()
    {
        await Context.Roles.AddRangeAsync(_mainRole);
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
