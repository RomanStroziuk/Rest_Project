using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Api.Dtos.OrderItemDtos;
using Domain.Brands;
using Domain.OrderItems;
using Domain.Orders;
using Domain.Roles;
using Domain.Sneakers;
using Domain.SneakerWarehouses;
using Domain.Statuses;
using Domain.Users;
using Domain.Warehouses;
using Domain.Сategories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Tests.Common;
using Tests.Data;
using Xunit;

namespace Api.Tests.Integration.OrderItems;

public class OrderItemControllerTests : BaseIntegrationTest, IAsyncLifetime
{
    private readonly Role _adminRole;
    private readonly User _adminUser;
    private readonly Order _mainOrder;
    private readonly SneakerWarehouse _mainSneakerWarehouse;
    private readonly OrderItem _mainOrderItem;
    private readonly Warehouse _warehouse;
    private readonly Sneaker _sneaker;
    private readonly Brand _brand;
    private readonly Category _category;
    private readonly Status _mainStatus;

    public OrderItemControllerTests(IntegrationTestWebFactory factory) : base(factory)
    {
        _warehouse = WarehousesData.MainWarehouse;
        _brand = BrandsData.MainBrand();
        _category = CategoriesData.MainCategory();
        _sneaker = SneakersData.MainSneaker(_category.Id, _brand.Id);
        _mainStatus = StatusData.MainStatus();
        _adminRole = RoleData.AdminRole();
        _adminUser = UsersData.AdminUser(_adminRole.Id);
        _mainOrder = OrdersData.MainOrder(_adminUser.Id, _mainStatus.Id);
        _mainSneakerWarehouse = SneakerWarehousesData.MainSneakerWarehouse(_sneaker.Id, _warehouse.Id);
        _mainOrderItem = OrderItemData.MainOrderItem(_mainOrder.Id, _mainSneakerWarehouse.Id);
    }
    
    [Fact]
    public async Task ShouldCreateOrderItem()
    {
        //Arrange
        var authToken = await GenerateAuthTokenAsync(_adminUser.Email, _adminUser.Password);
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
        var request = new OrderItemDto(
            Id: null,
            SneakerWarehouseId: _mainSneakerWarehouse.Id.Value,
            SneakerWarehouse: null,
            OrderId: _mainOrder.Id.Value,
            Order: null,
            Quantity: 1,
            TotalPrice: 1
            );
        
        //Act
        var response = await Client.PostAsJsonAsync("/order-item/create", request);
        
        //Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        
        var orderItemFromResponse = await response.ToResponseModel<OrderItemDto>();
        var orderItemId = new OrderItemId(orderItemFromResponse.Id!.Value);
        
        var orderItemFromDatabase = await Context.OrderItems.FirstOrDefaultAsync(x => x.Id == orderItemId);
        orderItemFromDatabase.Should().NotBeNull();
    }
    
    [Fact]
    public async Task ShouldUpdateOrderItem()
    {
        //Arrange
        var authToken = await GenerateAuthTokenAsync(_adminUser.Email, _adminUser.Password);
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
        var request = new OrderItemDto(
            Id: _mainOrderItem.Id!.Value,
            SneakerWarehouseId: _mainSneakerWarehouse.Id.Value,
            SneakerWarehouse: null,
            OrderId: _mainOrder.Id.Value,
            Order: null,
            Quantity: 2,
            TotalPrice: 2
            );
        
        //Act
        var response = await Client.PutAsJsonAsync("/order-item/update", request);
        
        //Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        
        var orderItemFromResponse = await response.ToResponseModel<OrderItemDto>();
        var orderItemId = new OrderItemId(orderItemFromResponse.Id!.Value);
        
        var orderItemFromDatabase = await Context.OrderItems.FirstOrDefaultAsync(x => x.Id == orderItemId);
        orderItemFromDatabase.Should().NotBeNull();
    }
    
    [Fact]
    public async Task ShouldDeleteOrderItem()
    {
        //Arrange
        var authToken = await GenerateAuthTokenAsync(_adminUser.Email, _adminUser.Password);
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
        var orderItemId = _mainOrderItem.Id!.Value;
        
        //Act
        var response = await Client.DeleteAsync($"/order-item/delete/{orderItemId}");
        
        //Assert        
        response.IsSuccessStatusCode.Should().BeTrue();
        
        var orderItemFromResponse = await response.ToResponseModel<OrderItemDto>();
        var orderItemIdResponse = new OrderItemId(orderItemFromResponse.Id!.Value);
        
        var orderItemFromDatabase = await Context.OrderItems.FirstOrDefaultAsync(x => x.Id == orderItemIdResponse);
        orderItemFromDatabase.Should().BeNull();
    }
    
    [Fact]
    public async Task ShouldNotDeleteOrderItemBecauseOrderItemNotFound()
    {
        //Arrange
        var authToken = await GenerateAuthTokenAsync(_adminUser.Email, _adminUser.Password);
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
        var orderItemId = Guid.NewGuid();
        
        //Act
        var response = await Client.DeleteAsync($"/order-item/delete/{orderItemId}");
        
        //Assert        
        response.IsSuccessStatusCode.Should().BeFalse();
    }
    
    [Fact]
    public async Task ShouldNotCreateOrderItemBecauseSneakerWarehouseNotFound()
    {
        //Arrange
        var authToken = await GenerateAuthTokenAsync(_adminUser.Email, _adminUser.Password);
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
        var request = new OrderItemDto(
            Id: null,
            SneakerWarehouseId: Guid.NewGuid(),
            SneakerWarehouse: null,
            OrderId: _mainOrder.Id.Value,
            Order: null,
            Quantity: 1,
            TotalPrice: 1
            );
        
        //Act
        var response = await Client.PostAsJsonAsync("/order-item/create", request);
        
        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ShouldNotUpdateOrderItemBecauseOrderItemNotFound()
    {
        //Arrange
        var authToken = await GenerateAuthTokenAsync(_adminUser.Email, _adminUser.Password);
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
        var request = new OrderItemDto(
            Id: Guid.NewGuid(),
            SneakerWarehouseId: _mainSneakerWarehouse.Id.Value,
            SneakerWarehouse: null,
            OrderId: _mainOrder.Id.Value,
            Order: null,
            Quantity: 1,
            TotalPrice: 1
        );

        //Act
        var response = await Client.PutAsJsonAsync("/order-item/update", request);

        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    public async Task InitializeAsync()
    {
        await Context.Warehouses.AddAsync(_warehouse);
        await Context.SneakerWarehouses.AddAsync(_mainSneakerWarehouse);
        await Context.Brands.AddAsync(_brand);
        await Context.Categories.AddAsync(_category);
        await Context.Sneakers.AddAsync(_sneaker);
        await Context.Roles.AddRangeAsync(_adminRole);
        await Context.Users.AddRangeAsync(_adminUser);
        await Context.Statuses.AddRangeAsync(_mainStatus);
        await Context.Orders.AddAsync(_mainOrder);
        
        await Context.OrderItems.AddAsync(_mainOrderItem);
        
        await SaveChangesAsync();
        
    }

    public async Task DisposeAsync()
    {
        Context.Statuses.RemoveRange(Context.Statuses);
        Context.Warehouses.RemoveRange(Context.Warehouses);
        Context.SneakerWarehouses.RemoveRange(Context.SneakerWarehouses);
        Context.Brands.RemoveRange(Context.Brands);
        Context.Categories.RemoveRange(Context.Categories);
        Context.Sneakers.RemoveRange(Context.Sneakers);
        Context.Users.RemoveRange(Context.Users);
        Context.Statuses.RemoveRange(Context.Statuses);
        Context.Orders.RemoveRange(Context.Orders);
        Context.OrderItems.RemoveRange(Context.OrderItems);
        await SaveChangesAsync();
    }
}