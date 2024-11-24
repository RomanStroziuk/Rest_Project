using System.Net;
using System.Net.Http.Json;
using Api.Dtos;
using Api.Dtos.BrandDtos;
using Domain.Brands;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Tests.Common;
using Tests.Data;
using Xunit;

namespace Api.Tests.Integration.Brands;

public class BrandsControllerTests(IntegrationTestWebFactory factory)
    : BaseIntegrationTest(factory), IAsyncLifetime
{
    private readonly Brand _mainBrand = BrandsData.MainBrand;

    [Fact]
    public async Task ShouldCreateBrand()
    {
        // Arrange
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
    public async Task ShouldNotUpdateBrandBecauseBrandNotFound()
    {
        // Arrange
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

        await SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        Context.SneakerWarehouses.RemoveRange(await Context.SneakerWarehouses.ToListAsync());
    
        Context.Brands.RemoveRange(await Context.Brands.ToListAsync());

        await SaveChangesAsync();
    }

    
    
}