using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Api.Dtos;
using Api.Dtos.StatusDtos;
using Domain.Statuses;
using Domain.Roles;
using Domain.Users;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Tests.Common;
using Tests.Data;
using Xunit;

namespace Api.Tests.Integration.Statutes;

public class StatusControllerTests : BaseIntegrationTest, IAsyncLifetime
{
    private readonly Role _adminRole;
    private readonly User _adminUser;
    private readonly Status _mainStatus;
    
    public StatusControllerTests(IntegrationTestWebFactory factory) : base(factory)
    {
        _adminRole = RoleData.AdminRole();
        _mainStatus = StatusData.MainStatus();
        _adminUser = UsersData.AdminUser(_adminRole.Id);
    }
    
    [Fact]
    public async Task ShouldCreateStatus()
    {
        // Arrange
        var authToken = await GenerateAuthTokenAsync(_adminUser.Email, _adminUser.Password);
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
        var statusName = "Main Status";
        var request = new StatusDto(
            Id: null,
            Title: statusName);

        // Act
        var response = await Client.PostAsJsonAsync("status/create", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var statusFromResponse = await response.ToResponseModel<StatusDto>();
        var statusId = new StatusId(statusFromResponse.Id!.Value);

        var statusFromDatabase = await Context.Statuses.FirstOrDefaultAsync(x => x.Id == statusId);
        statusFromDatabase.Should().NotBeNull();
        statusFromDatabase!.Title.Should().Be(statusName);
    }

    [Fact]
    public async Task ShouldUpdateStatus()
    {
        // Arrange
        var authToken = await GenerateAuthTokenAsync(_adminUser.Email, _adminUser.Password);
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
        var newStatusName = "New Status Title";
        var request = new StatusDto(
            Id: _mainStatus.Id.Value,
            Title: newStatusName);

        // Act
        var response = await Client.PutAsJsonAsync("status/update", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var statusFromResponse = await response.ToResponseModel<StatusDto>();

        var statusFromDatabase = await Context.Statuses
            .FirstOrDefaultAsync(x => x.Id == new StatusId(statusFromResponse.Id!.Value));

        statusFromDatabase.Should().NotBeNull();
        statusFromDatabase!.Title.Should().Be(newStatusName); 
    }
    
    [Fact]
    public async Task ShouldFailToCreateStatus_WhenUnauthorized()
    {
        // Arrange
        const string statusName = "Unauthorized Role";
        var request = new StatusDto(Id: null, Title: statusName);

        // Act
        var response = await Client.PostAsJsonAsync("status/create", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    
    [Fact]
    public async Task ShouldFailToDeleteStatus_WhenUnauthorized()
    {
        //Arrange
        var statusId = _mainStatus.Id.Value;

        // Act
        var response = await Client.DeleteAsync($"status/delete/{statusId}");

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ShouldNotCreateStatusBecauseNameDuplicated()
    {
        // Arrange
        var authToken = await GenerateAuthTokenAsync(_adminUser.Email, _adminUser.Password);
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
        var request = new StatusDto(
            Id: null,
            Title: _mainStatus.Title);

        // Act
        var response = await Client.PostAsJsonAsync("status/create", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task ShouldNotUpdateStatusBecauseStatusNotFound()
    {
        // Arrange
        var authToken = await GenerateAuthTokenAsync(_adminUser.Email, _adminUser.Password);
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
        var request = new StatusDto(
            Id: Guid.NewGuid(),
            Title: "New StatusName");

        // Act
        var response = await Client.PutAsJsonAsync("status/update", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task ShouldDeleteStatus()
    {
        // Arrange
        var authToken = await GenerateAuthTokenAsync(_adminUser.Email, _adminUser.Password);
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
        var statusId = _mainStatus.Id.Value;
        
        // Act
        var response = await Client.DeleteAsync($"status/delete/{statusId}");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        
        var statusFromResponse = await response.ToResponseModel<StatusDto>();
        var statusIdResponse = new StatusId(statusFromResponse.Id!.Value);
        
        var statusFromDatabase = await Context.Statuses.FirstOrDefaultAsync(x => x.Id == statusIdResponse);
        statusFromDatabase.Should().BeNull();
    }
    
    [Fact]
    public async Task ShouldNotDeleteStatusBecauseStatusNotFound()
    {
        // Arrange
        var authToken = await GenerateAuthTokenAsync(_adminUser.Email, _adminUser.Password);
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
        var statusId = Guid.NewGuid();
        
        // Act
        var response = await Client.DeleteAsync($"status/delete/{statusId}");

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
     public async Task InitializeAsync()
           {
               await Context.Statuses.AddAsync(_mainStatus);
              await Context.Roles.AddRangeAsync(_adminRole);
        await Context.Users.AddRangeAsync(_adminUser);

        await SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        Context.Statuses.RemoveRange(Context.Statuses);
        Context.Users.RemoveRange(Context.Users);
        Context.Roles.RemoveRange(Context.Roles);

        await SaveChangesAsync();
    }
}
