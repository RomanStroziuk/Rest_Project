using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Api.Dtos;
using Api.Dtos.BrandDtos;
using Domain.Brands;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Tests.Common;
using Tests.Data;
using Xunit;
using Domain.Roles;
using Domain.Users;

namespace Api.Tests.Integration.Brands;

public class BrandsControllerTests : BaseIntegrationTest, IAsyncLifetime
{
    private readonly Role _adminRole;
    private readonly User _adminUser;
    private readonly Brand _mainBrand;
    
    public BrandsControllerTests(IntegrationTestWebFactory factory) : base(factory)
    {
        _adminRole = RoleData.AdminRole();
        _mainBrand = BrandsData.MainBrand();
        _adminUser = UsersData.AdminUser(_adminRole.Id);
    }


    [Fact]
    public async Task ShouldCreateBrand()
    {
        // Arrange
        var authToken = await GenerateAuthTokenAsync(_adminUser.Email, _adminUser.Password);
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
        var brandName = "From Test Brand";
        var request = new BrandDto(
            Id: null,
            Name: brandName);

        // Act
        var response = await Client.PostAsJsonAsync("brands/create", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var brandFromResponse = await response.ToResponseModel<BrandDto>();
        var brandId = new BrandId(brandFromResponse.Id!.Value);

        var brandFromDataBase = await Context.Brands.FirstOrDefaultAsync(x => x.Id == brandId);
        brandFromDataBase.Should().NotBeNull();

        brandFromDataBase!.Name.Should().Be(brandName);
    }

    [Fact]
    public async Task ShouldUpdateBrand()
    {
        // Arrange
        var authToken = await GenerateAuthTokenAsync(_adminUser.Email, _adminUser.Password);
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
        var newBrandName = "New Brand Name";
        var request = new BrandDto(
            Id: _mainBrand.Id.Value,
            Name: newBrandName);

        // Act
        var response = await Client.PutAsJsonAsync("brands/update", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var brandFromResponse = await response.ToResponseModel<BrandDto>();

        var brandFromDataBase = await Context.Brands
            .FirstOrDefaultAsync(x => x.Id == new BrandId(brandFromResponse.Id!.Value));

        brandFromDataBase.Should().NotBeNull();

        brandFromDataBase!.Name.Should().Be(newBrandName);
    }

    [Fact]
    public async Task ShouldNotCreateBrandBecauseNameDuplicated()
    {
        // Arrange
        var authToken = await GenerateAuthTokenAsync(_adminUser.Email, _adminUser.Password);
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
        var request = new BrandDto(
            Id: null,
            Name: _mainBrand.Name);

        // Act
        var response = await Client.PostAsJsonAsync("brands/create", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
    
    [Fact]
    public async Task ShouldFailToCreateBrand_WhenUnauthorized()
    {
        // Arrange
        const string brandName = "Unauthorized Role";
        var request = new BrandDto(Id: null, Name: brandName);

        // Act
        var response = await Client.PostAsJsonAsync("brands/create", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    
    [Fact]
    public async Task ShouldFailToDeleteBrand_WhenUnauthorized()
    {
        //Arrange
        var brandId = _mainBrand.Id.Value;

        // Act
        var response = await Client.DeleteAsync($"brands/delete/{brandId}");

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }


    [Fact]
    public async Task ShouldNotUpdateBrandBecauseBrandNotFound()
    {
        // Arrange
        var authToken = await GenerateAuthTokenAsync(_adminUser.Email, _adminUser.Password);
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
        var request = new BrandDto(
            Id: Guid.NewGuid(),
            Name: "New Brand Name");

        // Act
        var response = await Client.PutAsJsonAsync("brands/update", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ShouldDeleteBrand()
    {
        // Arrange
        var authToken = await GenerateAuthTokenAsync(_adminUser.Email, _adminUser.Password);
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
        var brandId = _mainBrand.Id.Value;
        
        // Act
        var response = await Client.DeleteAsync($"brands/delete/{brandId}");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var brandFromResponse = await response.ToResponseModel<BrandDto>();
        var brandIdResponse = new BrandId(brandFromResponse.Id!.Value);

        var brandFromDataBase = await Context.Brands.FirstOrDefaultAsync(x => x.Id == brandIdResponse);
        brandFromDataBase.Should().BeNull();
    }
    
    [Fact]
    public async Task ShouldNotDeleteBrandBecauseBrandNotFound()
    {
        // Arrange
        var authToken = await GenerateAuthTokenAsync(_adminUser.Email, _adminUser.Password);
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
        var brandId = Guid.NewGuid();
        
        // Act
        var response = await Client.DeleteAsync($"brands/delete/{brandId}");

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    public async Task InitializeAsync()
    {
        await Context.Brands.AddAsync(_mainBrand);
        await Context.Roles.AddRangeAsync(_adminRole);
        await Context.Users.AddRangeAsync(_adminUser);

        await SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        Context.Brands.RemoveRange(Context.Brands);
        Context.Users.RemoveRange(Context.Users);
        Context.Roles.RemoveRange(Context.Roles);

        await SaveChangesAsync();
    }
}