using System.Net;
using System.Net.Http.Json;
using Api.Dtos;
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
        var response = await Client.PostAsJsonAsync("brands", request);

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
        var response = await Client.PutAsJsonAsync("brands", request);

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
        var response = await Client.PostAsJsonAsync("brands", request);

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
        var response = await Client.PutAsJsonAsync("brands", request);

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
        Context.Brands.RemoveRange(Context.Brands);

        await SaveChangesAsync();
    }
    
    
    
}