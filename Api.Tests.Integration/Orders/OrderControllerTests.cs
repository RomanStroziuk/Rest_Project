using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Api.Dtos.OrderDtos;
using Domain.Orders;
using Domain.Roles;
using Domain.Statuses;
using Domain.Users;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Tests.Common;
using Tests.Data;
using Xunit;

namespace Api.Tests.Integration.Orders;

public class OrderControllerTests : BaseIntegrationTest, IAsyncLifetime
{
    private readonly Role _adminRole;
    private readonly User _adminUser;
    private readonly Order _mainOrder;
    private readonly Status _mainStatus;
    
    public OrderControllerTests(IntegrationTestWebFactory factory) : base(factory)
    {
        _mainStatus = StatusData.MainStatus();
        _adminRole = RoleData.AdminRole();
        _adminUser = UsersData.AdminUser(_adminRole.Id);
        _mainOrder = OrdersData.MainOrder(_adminUser.Id, _mainStatus.Id);
    }

    [Fact]
    public async Task ShouldCreateOrder()
    {
        //Arrange
        var authToken = await GenerateAuthTokenAsync(_adminUser.Email, _adminUser.Password);
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
        var request = new OrderDto(
            Id: null,
            OrderDate: DateTime.UtcNow,
            UserId: _adminUser.Id.Value,
            User: null,
            StatusId: _mainStatus.Id.Value,
            Status: null
        );
        
        //Act
        var response = await Client.PostAsJsonAsync("/orders/create", request);
        
        //Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        
        var orderFromResponse = await response.ToResponseModel<OrderDto>();
        var orderId = new OrderId(orderFromResponse.Id!.Value);
        
        var orderFromDatabase = await Context.Orders.FirstOrDefaultAsync(x => x.Id == orderId);
        orderFromDatabase.Should().NotBeNull();
        orderFromDatabase!.OrderDate.Should().BeCloseTo(request.OrderDate, precision: TimeSpan.FromSeconds(1));
    }
    
    [Fact]
    public async Task ShouldUpdateOrder()
    {
        //Arrange
        var authToken = await GenerateAuthTokenAsync(_adminUser.Email, _adminUser.Password);
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
        var request = new SetStatusOrderDto(
            OrderId: _mainOrder.Id.Value,
            StatusId: _mainStatus.Id.Value
        );
        
        //Act
        var response = await Client.PutAsJsonAsync($"orders/setStatus", request);
        
        //Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        
        var orderFromResponse = await response.ToResponseModel<SetStatusOrderDto>();
        var orderId = new OrderId(orderFromResponse.OrderId);
        
        var orderFromDatabase = await Context.Orders.FirstAsync(x => x.Id == orderId);
        orderFromDatabase.Should().NotBeNull();
    }

    [Fact]
    public async Task ShouldDeleteOrder()
    {
        //Arrange
        var authToken = await GenerateAuthTokenAsync(_adminUser.Email, _adminUser.Password);
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
        var orderId = _mainOrder.Id.Value;
        
        //Act
        var response = await Client.DeleteAsync($"orders/delete/{orderId}");
        
        //Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        
        var orderFromResponse = await response.ToResponseModel<OrderDto>();
        var orderIdResponse = new OrderId(orderFromResponse.Id!.Value);
        
        var orderFromDatabase = await Context.Orders.FirstOrDefaultAsync(x => x.Id == orderIdResponse);
        orderFromDatabase.Should().BeNull();
    }

    [Fact]
    public async Task ShouldNotDeleteOrderBecauseOrderNotFound()
    {
        //Arrange
        var authToken = await GenerateAuthTokenAsync(_adminUser.Email, _adminUser.Password);
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
        var orderId = Guid.NewGuid();
        
        //Act
        var response = await Client.DeleteAsync($"orders/delete/{orderId}");
        
        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task ShouldNotUpdateOrderBecauseOrderNotFound()
    {
        //Arrange
        var authToken = await GenerateAuthTokenAsync(_adminUser.Email, _adminUser.Password);
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
        var request = new OrderDto(
            Id: Guid.NewGuid(),
            OrderDate: DateTime.Now,
            UserId: _adminUser.Id.Value,
            User: null,
            StatusId: _mainStatus.Id.Value,
            Status: null
        );
        
        //Act
        var response = await Client.PutAsJsonAsync($"orders/setStatus", request);
        
        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task ShouldNotCreateOrderBecauseUserNotFound()
    {
        //Arrange
        var authToken = await GenerateAuthTokenAsync(_adminUser.Email, _adminUser.Password);
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
        var request = new OrderDto(
            Id: null,
            OrderDate: DateTime.Now,
            UserId: Guid.NewGuid(),
            User: null,
            StatusId: _mainStatus.Id.Value,
            Status: null
        );
        
        //Act
        var response = await Client.PostAsJsonAsync("/orders/create", request);
        
        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    public async Task InitializeAsync()
    {
        await Context.Roles.AddRangeAsync(_adminRole);
        await Context.Users.AddRangeAsync(_adminUser);
        await Context.Statuses.AddRangeAsync(_mainStatus);
        
        await Context.Orders.AddAsync(_mainOrder);
        
        await SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        Context.Users.RemoveRange(Context.Users);
        Context.Roles.RemoveRange(Context.Roles);
        Context.Statuses.RemoveRange(Context.Statuses);
        Context.Orders.RemoveRange(Context.Orders);
        await SaveChangesAsync();
    }
}