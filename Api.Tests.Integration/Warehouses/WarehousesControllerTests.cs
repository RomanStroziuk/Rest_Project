using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Api.Dtos.WarehouseDtos;
using Domain.Warehouses;
using Domain.Roles;
using Domain.Users;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Tests.Common;
using Tests.Data;
using Xunit;

namespace Api.Tests.Integration.Warehouses;

public class WarehousesControllerTests : BaseIntegrationTest, IAsyncLifetime
{
    private readonly Role _adminRole;
    private readonly User _adminUser;
    private readonly Warehouse _mainWarehouse;
    
    public WarehousesControllerTests(IntegrationTestWebFactory factory) : base(factory)
    {
        _adminRole = RoleData.AdminRole();
        _mainWarehouse = WarehousesData.MainWarehouse;
        _adminUser = UsersData.AdminUser(_adminRole.Id);
    }

    [Fact]
    public async Task ShouldCreateWarehouse()
    {
        //Arrange
        
        var authToken = await GenerateAuthTokenAsync(_adminUser.Email, _adminUser.Password);
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
        var warehouseName = "From Test Warehouse";
        var request = new WarehouseDto(
            Id: null,
            Location: warehouseName,
            TotalQuantity: 50);
        
        //Act
        var response = await Client.PostAsJsonAsync("warehouse/create", request);
        
        //Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        
        var warehouseFromResponse = await response.ToResponseModel<WarehouseDto>();
        var warehouseId = new WarehouseId(warehouseFromResponse.Id!.Value);
        
        var warehouseFromDatabase = await Context.Warehouses.FirstOrDefaultAsync(x => x.Id == warehouseId);
        warehouseFromDatabase.Should().NotBeNull();
        
        warehouseFromDatabase!.Location.Should().Be(warehouseName);
        warehouseFromDatabase!.TotalQuantity.Should().Be(50);
    }
    
    [Fact]
    public async Task ShouldFailToCreateCategory_WhenUnauthorized()
    {
        // Arrange
        const string locationName = "Unauthorized Role";
        var request = new WarehouseDto(Id: null, Location: locationName, TotalQuantity: 20);

        // Act
        var response = await Client.PostAsJsonAsync("warehouse/create", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    
    [Fact]
    public async Task ShouldFailToDeleteCategory_WhenUnauthorized()
    {
        //Arrange
        var warehouseId = _mainWarehouse.Id.Value;

        // Act
        var response = await Client.DeleteAsync($"warehouse/delete/{warehouseId}");

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ShouldUpdateWarehouse()
    {
        //Arrange
        
        var authToken = await GenerateAuthTokenAsync(_adminUser.Email, _adminUser.Password);
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
        var newWarehouseName = "New Warehouse Name2";
        var request = new WarehouseDto(
            Id: _mainWarehouse.Id.Value,
            Location: newWarehouseName,
            TotalQuantity: 50);
        
        //Act
        var response = await Client.PutAsJsonAsync("warehouse/update", request);
        
        //Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        
        var warehouseFromResponse = await response.ToResponseModel<WarehouseDto>();
        var warehouseId = new WarehouseId(warehouseFromResponse.Id!.Value);
        
        var warehouseFromDatabase = await Context.Warehouses.FirstOrDefaultAsync(x => x.Id == warehouseId);
        warehouseFromDatabase.Should().NotBeNull();
        
        warehouseFromDatabase!.Location.Should().Be(newWarehouseName);
        warehouseFromDatabase!.TotalQuantity.Should().Be(50);
    }

    [Fact]
    public async Task ShouldNotCreateWarehouseBecauseNameDuplicated()
    {
        //Arrange
        
        var authToken = await GenerateAuthTokenAsync(_adminUser.Email, _adminUser.Password);
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
        var request = new WarehouseDto(
            Id: null,
            Location: _mainWarehouse.Location,
            TotalQuantity: 50);
        
        //Act
        var response = await Client.PostAsJsonAsync("warehouse/create", request);
        
        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task ShouldNotUpdateWarehouseBecauseNotFound()
    {
        //Arrange
        
        var authToken = await GenerateAuthTokenAsync(_adminUser.Email, _adminUser.Password);
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
        var request = new WarehouseDto(
            Id: Guid.NewGuid(),
            Location: "New Warehouse Name",
            TotalQuantity: 50);

        //Act
        var response = await Client.PutAsJsonAsync("warehouse/update", request);

        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task ShouldDeleteWarehouse()
    {
        //Arrange
        
        var authToken = await GenerateAuthTokenAsync(_adminUser.Email, _adminUser.Password);
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
        var warehouseId = _mainWarehouse.Id.Value;
        
        //Act
        var response = await Client.DeleteAsync($"warehouse/delete/{warehouseId}");
        
        //Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        
        var warehouseFromResponse = await response.ToResponseModel<WarehouseDto>();
        var warehouseIdResponse = new WarehouseId(warehouseFromResponse.Id!.Value);
        
        var warehouseFromDatabase = await Context.Warehouses.FirstOrDefaultAsync(x => x.Id == warehouseIdResponse);
        warehouseFromDatabase.Should().BeNull();
    }
    [Fact]
    public async Task ShouldNotDeleteWarehouseBecauseNotFound()
    {
        //Arrange
        
        var authToken = await GenerateAuthTokenAsync(_adminUser.Email, _adminUser.Password);
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
        var warehouseId = Guid.NewGuid();
        
        //Act
        var response = await Client.DeleteAsync($"warehouse/delete/{warehouseId}");
        
        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    public async Task InitializeAsync()
    {
        await Context.Warehouses.AddAsync(_mainWarehouse);
        await Context.Roles.AddRangeAsync(_adminRole);
        await Context.Users.AddRangeAsync(_adminUser);

        await SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        Context.Warehouses.RemoveRange(Context.Warehouses);
        Context.Users.RemoveRange(Context.Users);
        Context.Roles.RemoveRange(Context.Roles);

        await SaveChangesAsync();
    }
}