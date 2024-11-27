using System.Net;
using System.Net.Http.Json;
using Api.Dtos;
using Api.Dtos.CategoryDtos;
using Domain.Сategories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Tests.Common;
using Tests.Data;
using Xunit;

namespace Api.Tests.Integration.Categories;

public class CategoriesControllerTests(IntegrationTestWebFactory factory)
    : BaseIntegrationTest(factory), IAsyncLifetime
{
    private readonly Category _mainCategory = CategoriesData.MainCategory;

    [Fact]
    public async Task ShouldCreateCategory()
    {
        // Arrange
        var categoryName = "From Test Category";
        var request = new CategoryDto(
            Id: null,
            Name: categoryName);

        // Act
        var response = await Client.PostAsJsonAsync("categories/create", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var categoryFromResponse = await response.ToResponseModel<CategoryDto>();
        var categoryId = new CategoryId(categoryFromResponse.Id!.Value);

        var categoryFromDataBase = await Context.Categories.FirstOrDefaultAsync(x => x.Id == categoryId);
        categoryFromDataBase.Should().NotBeNull();

        categoryFromDataBase!.Name.Should().Be(categoryName);
    }

    [Fact]
    public async Task ShouldUpdateCategory()
    {
        // Arrange
        var newCategoryName = "New Category Name";
        var request = new CategoryDto(
            Id: _mainCategory.Id.Value,
            Name: newCategoryName);

        // Act
        var response = await Client.PutAsJsonAsync("categories/update", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var categoryFromResponse = await response.ToResponseModel<CategoryDto>();

        var categoryFromDataBase = await Context.Categories
            .FirstOrDefaultAsync(x => x.Id == new CategoryId(categoryFromResponse.Id!.Value));

        categoryFromDataBase.Should().NotBeNull();

        categoryFromDataBase!.Name.Should().Be(newCategoryName);
    }

    [Fact]
    public async Task ShouldNotCreateCategoryBecauseNameDuplicated()
    {
        // Arrange
        var request = new CategoryDto(
            Id: null,
            Name: _mainCategory.Name);

        // Act
        var response = await Client.PostAsJsonAsync("categories/create", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task ShouldNotUpdateCategoryBecauseCategoryNotFound()
    {
        // Arrange
        var request = new CategoryDto(
            Id: Guid.NewGuid(),
            Name: "New Category Name");

        // Act
        var response = await Client.PutAsJsonAsync("categories/update", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task ShouldDeleteCategory()
    {
        // Arrange
        var categoryId = _mainCategory.Id.Value;
        
        // Act
        var response = await Client.DeleteAsync($"categories/delete/{categoryId}");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        
        var categoryFromResponse = await response.ToResponseModel<CategoryDto>();
        var categoryIdResponse = new CategoryId(categoryFromResponse.Id!.Value);

        var categoryFromDataBase = await Context.Categories.FirstOrDefaultAsync(x => x.Id == categoryIdResponse);
        categoryFromDataBase.Should().BeNull();
    }
    
    [Fact]
    public async Task ShouldNotDeleteCategoryBecauseCategoryNotFound()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        
        // Act
        var response = await Client.DeleteAsync($"categories/delete/{categoryId}");

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    public async Task InitializeAsync()
    {
        await Context.Categories.AddAsync(_mainCategory);

        await SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        Context.Categories.RemoveRange(Context.Categories);

        await SaveChangesAsync();
    }
    
    
    
}