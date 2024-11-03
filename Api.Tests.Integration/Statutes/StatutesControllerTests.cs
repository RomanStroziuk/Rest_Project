using System.Net;
using System.Net.Http.Json;
using Api.Dtos;
using Domain.Statuses;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Tests.Common;
using Tests.Data;
using Xunit;

namespace Api.Tests.Integration.Statutes;

public class StatusControllerTests(IntegrationTestWebFactory factory)
    : BaseIntegrationTest(factory), IAsyncLifetime
{
    private readonly Status _mainStatus = StatusData.MainStatus;

    

    [Fact]
    public async Task ShouldCreateStatus()
    {
        // Arrange
        var statusName = "Main Status";
        var request = new StatusDto(
            Id: null,
            Title: statusName);

        // Act
        var response = await Client.PostAsJsonAsync("status", request);

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
        var newStatusName = "New Status Title";
        var request = new StatusDto(
            Id: _mainStatus.Id.Value,
            Title: newStatusName);

        // Act
        var response = await Client.PutAsJsonAsync("status", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var statusFromResponse = await response.ToResponseModel<StatusDto>();

        var statusFromDatabase = await Context.Statuses
            .FirstOrDefaultAsync(x => x.Id == new StatusId(statusFromResponse.Id!.Value));

        statusFromDatabase.Should().NotBeNull();
        statusFromDatabase!.Title.Should().Be(newStatusName); 
    }

    [Fact]
    public async Task ShouldNotCreateStatusBecauseNameDuplicated()
    {
        // Arrange
        var request = new StatusDto(
            Id: null,
            Title: _mainStatus.Title);

        // Act
        var response = await Client.PostAsJsonAsync("status", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task ShouldNotUpdateStatusBecauseStatusNotFound()
    {
        // Arrange
        var request = new StatusDto(
            Id: Guid.NewGuid(),
            Title: "New StatusName");

        // Act
        var response = await Client.PutAsJsonAsync("status", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    public async Task InitializeAsync()
    {
        // This method is called before each test method is executed
        await Context.Statuses.AddAsync(_mainStatus);
        await SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        // This method is called after each test method is executed
        Context.Statuses.RemoveRange(Context.Statuses);
        await SaveChangesAsync();
    }
}
