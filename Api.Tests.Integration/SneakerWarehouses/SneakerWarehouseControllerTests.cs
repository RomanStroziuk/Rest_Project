using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Api.Dtos.SneakerWarehouseDtos;
using Domain.Brands;
using Domain.Roles;
using Domain.Sneakers;
using Domain.SneakerWarehouses;
using Domain.Users;
using Domain.Warehouses;
using Domain.Сategories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Tests.Common;
using Tests.Data;
using Xunit;

namespace Api.Tests.Integration.SneakerWarehouses;

public class SneakerWarehouseControllerTests : BaseIntegrationTest, IAsyncLifetime
{
    private readonly Role _adminRole;
    private readonly User _adminUser;
    private readonly SneakerWarehouse _mainSneakerWarehouse;
    private readonly Sneaker _sneaker;
    private readonly Warehouse _warehouse;
    private readonly Brand _brand;
    private readonly Category _category;
    
    public SneakerWarehouseControllerTests(IntegrationTestWebFactory factory) : base(factory)
    {
        _brand = BrandsData.MainBrand();
        _category = CategoriesData.MainCategory();
        _sneaker = SneakersData.MainSneaker(_category.Id, _brand.Id);
        _warehouse = WarehousesData.MainWarehouse;

        _adminRole = RoleData.AdminRole();
        _adminUser = UsersData.AdminUser(_adminRole.Id);
        _mainSneakerWarehouse = SneakerWarehousesData.MainSneakerWarehouse(_sneaker.Id, _warehouse.Id);
    }

    [Fact]
    public async Task ShouldCreateSneakerWarehouse()
    {
        // Arrange
        var authToken = await GenerateAuthTokenAsync(_adminUser.Email, _adminUser.Password);
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
        
        var request = new SneakerWarehouseDto(
            Id: null, 
            SneakerId: _sneaker.Id.Value,
            Sneaker: null,
            WarehouseId: _warehouse.Id.Value,
            Warehouse: null,
            SneakerQuantity: 10);
        
        // Act
        var response = await Client.PostAsJsonAsync("/sneaker-warehouse/create", request);
        
        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        
        var sneakerWarehouseFromResponse = await response.ToResponseModel<SneakerWarehouseDto>();
        var sneakerWarehouseId = new SneakerWarehouseId(sneakerWarehouseFromResponse.Id!.Value);
        
        var sneakerWarehouseFromDatabase = await Context.SneakerWarehouses.FirstOrDefaultAsync(x => x.Id == sneakerWarehouseId);
        sneakerWarehouseFromDatabase.Should().NotBeNull();
        
        sneakerWarehouseFromDatabase!.SneakerId.Value.Should().Be(request.SneakerId);
        sneakerWarehouseFromDatabase!.WarehouseId.Value.Should().Be(request.WarehouseId);
        sneakerWarehouseFromDatabase!.SneakerQuantity.Should().Be(10);
    }

    [Fact]
    public async Task ShouldUpdateSneakerWarehouse()
    {
        //Arrange
        var authToken = await GenerateAuthTokenAsync(_adminUser.Email, _adminUser.Password);
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
        
        var newSneakerQuantity = 20;
        
        var request = new SneakerWarehouseDto(
            Id: _mainSneakerWarehouse.Id.Value, 
            SneakerId: _sneaker.Id.Value,
            Sneaker: null,
            WarehouseId: _warehouse.Id.Value,
            Warehouse: null,
            SneakerQuantity: newSneakerQuantity);
        
        //Act
        var response = await Client.PutAsJsonAsync("/sneaker-warehouse/update", request);
        
        //Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        
        var sneakerWarehouseFromResponse = await response.ToResponseModel<SneakerWarehouseDto>();
        var sneakerWarehouseId = new SneakerWarehouseId(sneakerWarehouseFromResponse.Id!.Value);
        
        var sneakerWarehouseFromDatabase = await Context.SneakerWarehouses.FirstOrDefaultAsync(x => x.Id == sneakerWarehouseId);
        sneakerWarehouseFromDatabase.Should().NotBeNull();
        sneakerWarehouseFromDatabase!.SneakerQuantity.Should().Be(newSneakerQuantity);
    }
    
    [Fact]
    public async Task ShouldDeleteSneakerWarehouse()
    {
        //Arrange
        var authToken = await GenerateAuthTokenAsync(_adminUser.Email, _adminUser.Password);
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
        var sneakerWarehouseId = _mainSneakerWarehouse.Id.Value;
        
        //Act
        var response = await Client.DeleteAsync($"/sneaker-warehouse/delete/{sneakerWarehouseId}");
        
        //Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        
        var warehouseFromResponse = await response.ToResponseModel<SneakerWarehouseDto>();
        var sneakerWarehouseIdResponse = new SneakerWarehouseId(warehouseFromResponse.Id!.Value);
        
        var sneakerWarehouseFromDatabase = await Context.SneakerWarehouses.FirstOrDefaultAsync(x => x.Id == sneakerWarehouseIdResponse);
        sneakerWarehouseFromDatabase.Should().BeNull();
    }
    
    [Fact]
    public async Task ShouldNotDeleteSneakerWarehouseBecauseNotFound()
    {
        //Arrange
        var authToken = await GenerateAuthTokenAsync(_adminUser.Email, _adminUser.Password);
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
        var sneakerWarehouseId = Guid.NewGuid();
        
        //Act
        var response = await Client.DeleteAsync($"sneaker-warehouse/delete/{sneakerWarehouseId}");
        
        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ShouldNotUpdateSneakerWarehouseBecauseSneakerWarehouseNotFound()
    {
        //Arrange
        var authToken = await GenerateAuthTokenAsync(_adminUser.Email, _adminUser.Password);
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
        var request = new SneakerWarehouseDto(
            Id: Guid.NewGuid(), 
            SneakerId: _sneaker.Id.Value,
            Sneaker: null,
            WarehouseId: _warehouse.Id.Value,
            Warehouse: null,
            SneakerQuantity: 10);
        
        //Act
        var response = await Client.PutAsJsonAsync("sneaker-warehouse/update", request);
        
        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ShouldNotCreateSneakerWarehouseBecauseSneakerAndWarehouseNotFound()
    {
        //Arrange
        var authToken = await GenerateAuthTokenAsync(_adminUser.Email, _adminUser.Password);
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
        var request = new SneakerWarehouseDto(
            Id: null,
            SneakerId: Guid.NewGuid(),
            Sneaker: null,
            WarehouseId: Guid.NewGuid(),
            Warehouse: null,
            SneakerQuantity: 10);

        //Act
        var response = await Client.PostAsJsonAsync($"/sneaker-warehouse/create", request);

        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    public async Task InitializeAsync()
    {
        await Context.Categories.AddAsync(_category);
        await Context.Brands.AddAsync(_brand);
        await Context.Sneakers.AddAsync(_sneaker);
        await Context.Warehouses.AddAsync(_warehouse);
        await Context.Roles.AddRangeAsync(_adminRole);
        await Context.Users.AddRangeAsync(_adminUser);
        
        await Context.SneakerWarehouses.AddAsync(_mainSneakerWarehouse);
        
        await SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        Context.Users.RemoveRange(Context.Users);
        Context.Roles.RemoveRange(Context.Roles);
        Context.SneakerWarehouses.RemoveRange(Context.SneakerWarehouses);
        Context.Sneakers.RemoveRange(Context.Sneakers);
        Context.Warehouses.RemoveRange(Context.Warehouses);
        Context.Brands.RemoveRange(Context.Brands);
        Context.Categories.RemoveRange(Context.Categories);
         
        await SaveChangesAsync();
    }
}