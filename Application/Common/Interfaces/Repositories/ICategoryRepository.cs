﻿using Domain.Сategories;
using Optional;
namespace Application.Common.Interfaces.Repositories;

public interface ICategoryRepository
{
    Task<Option<Category>> GetById(CategoryId id, CancellationToken cancellationToken);
    Task<Category> Add(Category category, CancellationToken cancellationToken);
    Task<Option<Category>> SearchByName(string name, CancellationToken cancellationToken);
    Task<Category> Update(Category category, CancellationToken cancellationToken);
    Task<Category> Delete(Category category, CancellationToken cancellationToken);
}