using System.Net;
using System.Net.Http.Json;
using Api.Dtos;
using Api.Dtos.RoleDtos;
using Domain.Roles;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Tests.Common;
using Tests.Data;
using Xunit;

namespace Api.Tests.Integration.Roles;

public class RoleControllerTests(IntegrationTestWebFactory factory)
    : BaseIntegrationTest(factory), IAsyncLifetime
{
    private readonly Role _mainRole = RolesData.MainRole;


    [Fact]
    public async Task ShouldCreateRole()
    {
        // Arrange
        var roleName = "From Test Role";
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
    public async Task ShouldUpdateRole()
    {
        // Arrange
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
    public async Task ShouldNotCreateRoleBecauseNameDuplicated()
    {
        // Arrange
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
    public async Task ShouldNotUpdateRoleBecauseRoleNotFound()
    {
        // Arrange
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
        var roleId = Guid.NewGuid();
        
        // Act
        var response = await Client.DeleteAsync($"role/delete/{roleId}");

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    public async Task InitializeAsync()
    {
        await Context.Roles.AddAsync(_mainRole);
        await SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        Context.Roles.RemoveRange(Context.Roles);
        await SaveChangesAsync();
    }
}
