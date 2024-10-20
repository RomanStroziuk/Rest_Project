﻿using Domain.Brands;
using Domain.Сategories;

namespace Domain.Sneakers;

public class Sneaker
{

    public SneakerId Id { get; }
    
    public string Model  { get; private set; }
    
    public int Size { get; private set; }
    
    public int Price { get; private set; }
    
    public BrandId BrandId { get;}
    
    public Brand? Brand { get; }
    
    
    public CategoryId CategoryId { get; }
    
    public Category? Category { get; }



    private Sneaker(SneakerId id, string model, int size, int price, BrandId brandId, CategoryId categoryId)
    {
        Id = id;
        Model = model;
        Size = size;
        Price = price;
        BrandId = brandId;
        CategoryId = categoryId;
        
    }
        
    public  static Sneaker New(SneakerId id, string model, int size, int price, BrandId brandId, CategoryId categoryId) 
    => new(id, model, size, price, brandId, categoryId);

    public void UpdateDetails(string model, int size, int price)
    {
        Model = model;
        Size = size;
        Price = price;
    }

}