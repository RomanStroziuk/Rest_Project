using System.Net;
using System.Net.Http.Json;
using Api.Dtos.WarehouseDtos;
using Domain.Warehouses;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Tests.Common;
using Tests.Data;
using Xunit;

namespace Api.Tests.Integration.Warehouses;

public class WarehousesControllerTests(IntegrationTestWebFactory factory)
    : BaseIntegrationTest(factory), IAsyncLifetime
{
    private readonly Warehouse _mainWarehouse = WarehousesData.MainWarehouse;

    [Fact]
    public async Task ShouldCreateWarehouse()
    {
        //Arrange
        var warehouseName = "From Test Warehouse";
        var request = new WarehouseDto(
            Id: null,
            Location: warehouseName,
            TotalQuantity: 10);
        
        //Act
        var response = await Client.PostAsJsonAsync("warehouse/create", request);
        
        //Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        
        var warehouseFromResponse = await response.ToResponseModel<WarehouseDto>();
        var warehouseId = new WarehouseId(warehouseFromResponse.Id!.Value);
        
        var warehouseFromDatabase = await Context.Warehouses.FirstOrDefaultAsync(x => x.Id == warehouseId);
        warehouseFromDatabase.Should().NotBeNull();
        
        warehouseFromDatabase!.Location.Should().Be(warehouseName);
        warehouseFromDatabase!.TotalQuantity.Should().Be(10);
    }

    [Fact]
    public async Task ShouldUpdateWarehouse()
    {
        //Arrange
        var newWarehouseName = "New Warehouse Name";
        var request = new WarehouseDto(
            Id: _mainWarehouse.Id.Value,
            Location: newWarehouseName,
            TotalQuantity: 10);
        
        //Act
        var response = await Client.PutAsJsonAsync("warehouse/update", request);
        
        //Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        
        var warehouseFromResponse = await response.ToResponseModel<WarehouseDto>();
        var warehouseId = new WarehouseId(warehouseFromResponse.Id!.Value);
        
        var warehouseFromDatabase = await Context.Warehouses.FirstOrDefaultAsync(x => x.Id == warehouseId);
        warehouseFromDatabase.Should().NotBeNull();
        
        warehouseFromDatabase!.Location.Should().Be(newWarehouseName);
        warehouseFromDatabase!.TotalQuantity.Should().Be(10);
    }

    [Fact]
    public async Task ShouldNotCreateWarehouseBecauseNameDuplicated()
    {
        //Arrange
        var request = new WarehouseDto(
            Id: null,
            Location: _mainWarehouse.Location,
            TotalQuantity: 10);
        
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
        var request = new WarehouseDto(
            Id: Guid.NewGuid(),
            Location: "New Warehouse Name",
            TotalQuantity: 10);

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
        await SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        Context.SneakerWarehouses.RemoveRange(await Context.SneakerWarehouses.ToListAsync());
        
        Context.Warehouses.RemoveRange(Context.Warehouses);
        
        await SaveChangesAsync();
    }
}