using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Api.Dtos.SneakerDtos;
using Domain.Brands;
using Domain.Roles;
using Domain.Sneakers;
using Domain.Users;
using Domain.Сategories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Tests.Common;
using Tests.Data;
using Xunit;

namespace Api.Tests.Integration.Sneakers;

public class SneakerControllerTests : BaseIntegrationTest, IAsyncLifetime
{
    private readonly Role _adminRole;
    private readonly User _adminUser;
    private readonly Brand _brand;
    private readonly Category _category;
    private readonly Sneaker _mainSneaker;
    
    public SneakerControllerTests(IntegrationTestWebFactory factory) : base(factory)
    {
        _category = CategoriesData.MainCategory();
        _brand = BrandsData.MainBrand();
        _adminRole = RoleData.AdminRole();
        _adminUser = UsersData.AdminUser(_adminRole.Id);
        _mainSneaker = SneakersData.MainSneaker(_category.Id, _brand.Id);
    }

    [Fact]
    public async Task ShouldCreateSneaker()
    {
        //Arrange
        var authToken = await GenerateAuthTokenAsync(_adminUser.Email, _adminUser.Password);
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
        
        var request = new SneakerDto(
            Id: null,
            model: "New Sneaker Name",
            size: 40,
            price: 1000,
            CategoryId: _category.Id.Value,
            Category: null,
            BrandId: _brand.Id.Value,
            Brand: null
        );
        
        //Act
        var response = await Client.PostAsJsonAsync("/sneaker/create", request);
        
        //Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        
        var sneakerFromResponse = await response.ToResponseModel<SneakerDto>();
        var sneakerId = new SneakerId(sneakerFromResponse.Id!.Value);
        
        var sneakerFromDatabase = await Context.Sneakers.FirstOrDefaultAsync(x => x.Id == sneakerId);
        sneakerFromDatabase.Should().NotBeNull();
        
        sneakerFromDatabase!.Model.Should().Be(request.model);
        sneakerFromDatabase!.Size.Should().Be(request.size);
        sneakerFromDatabase!.Price.Should().Be(request.price);
    }

    [Fact]
    public async Task ShouldUpdateSneaker()
    {
        //Arrange
        var authToken = await GenerateAuthTokenAsync(_adminUser.Email, _adminUser.Password);
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
        var id = _mainSneaker.Id.Value;
        var request = new SneakerDto(
            Id: id,
            model: "Updated Sneaker Name",
            size: 40,
            price: 1000,
            CategoryId: _category.Id.Value,
            Category: null,
            BrandId: _brand.Id.Value,
            Brand: null
        );
        
        //Act
        var response = await Client.PutAsJsonAsync($"/sneaker/update/{id}", request);
        
        //Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        
        var sneakerFromResponse = await response.ToResponseModel<SneakerDto>();
        var sneakerId = new SneakerId(sneakerFromResponse.Id!.Value);
        
        var sneakerFromDatabase = await Context.Sneakers.FirstOrDefaultAsync(x => x.Id == sneakerId);
        sneakerFromDatabase.Should().NotBeNull();
        
        sneakerFromDatabase!.Model.Should().Be(request.model);
        sneakerFromDatabase!.Size.Should().Be(request.size);
        sneakerFromDatabase!.Price.Should().Be(request.price);
    }
    
    [Fact]
    public async Task ShouldDeleteSneaker()
    {
        //Arrange
        var authToken = await GenerateAuthTokenAsync(_adminUser.Email, _adminUser.Password);
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
        var sneakerId = _mainSneaker.Id.Value;
        
        //Act
        var response = await Client.DeleteAsync($"/sneaker/delete/{sneakerId}");
        
        //Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        
        var sneakerFromResponse = await response.ToResponseModel<SneakerDto>();
        var sneakerIdResponse = new SneakerId(sneakerFromResponse.Id!.Value);
        
        var sneakerFromDatabase = await Context.Sneakers.FirstOrDefaultAsync(x => x.Id == sneakerIdResponse);
        sneakerFromDatabase.Should().BeNull();
    }

    [Fact]
    public async Task ShouldNotDeleteSneakerBecauseSneakerNotFound()
    {
        //Arrange
        var authToken = await GenerateAuthTokenAsync(_adminUser.Email, _adminUser.Password);
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
        var sneakerId = Guid.NewGuid();
        
        //Act
        var response = await Client.DeleteAsync($"/sneaker/delete/{sneakerId}");
        
        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task ShouldNotUpdateSneakerBecauseSneakerNotFound()
    {
        //Arrange
        var authToken = await GenerateAuthTokenAsync(_adminUser.Email, _adminUser.Password);
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
        var request = new SneakerDto(
            Id: Guid.NewGuid(),
            model: "Updated Sneaker Name",
            size: 40,
            price: 1000,
            CategoryId: _category.Id.Value,
            Category: null,
            BrandId: _brand.Id.Value,
            Brand: null
        );
        
        //Act
        var response = await Client.PutAsJsonAsync("/sneaker/update", request);
        
        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task ShouldNotCreateSneakerBecauseCategoryAndBrandNotFound()
    {
        //Arrange
        var authToken = await GenerateAuthTokenAsync(_adminUser.Email, _adminUser.Password);
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
        var request = new SneakerDto(
            Id: null, 
            model: "New Sneaker Name",
            size: 40,
            price: 1000,
            CategoryId: Guid.NewGuid(),
            Category: null,
            BrandId: Guid.NewGuid(),
            Brand: null
        );
        
        //Act
        var response = await Client.PostAsJsonAsync("/sneaker/create", request);
        
        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    public async Task InitializeAsync()
    {
        await Context.Categories.AddAsync(_category);
        await Context.Brands.AddAsync(_brand);
        await Context.Roles.AddRangeAsync(_adminRole);
        await Context.Users.AddRangeAsync(_adminUser);
        
        await Context.Sneakers.AddAsync(_mainSneaker);
        
        await SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        Context.Users.RemoveRange(Context.Users);
        Context.Roles.RemoveRange(Context.Roles);
        Context.Brands.RemoveRange(Context.Brands);
        Context.Categories.RemoveRange(Context.Categories);
        Context.Sneakers.RemoveRange(Context.Sneakers);
        await SaveChangesAsync();
    }
}