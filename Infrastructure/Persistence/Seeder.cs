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
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class Seeder(ApplicationDbContext context)
{
    public async Task SeedAsync()
    {
        if (!context.Roles.Any()) await SeedRolesAsync();
        if (!context.Users.Any()) await SeedUsersAsync();
        if (!context.Categories.Any()) await SeedCategoriesAsync();
        if (!context.Brands.Any()) await SeedBrandsAsync();
        if (!context.Warehouses.Any()) await SeedWarehousesAsync();
        if (!context.Statuses.Any()) await SeedStatusesAsync();
        if (!context.Sneakers.Any()) await SeedSneakersAsync();
        if (!context.SneakerWarehouses.Any()) await SeedSneakerWarehousesAsync();
        if (!context.Orders.Any()) await SeedOrdersAsync();
    }

    private async Task SeedRolesAsync()
    {
        var roles = new List<Role>
        {
            Role.New(new RoleId(Guid.NewGuid()), "Admin"),
            Role.New(new RoleId(Guid.NewGuid()), "User")
        };
        context.Roles.AddRange(roles);
        await context.SaveChangesAsync();
    }

    private async Task SeedUsersAsync()
    {
        var adminRole = context.Roles.FirstOrDefault(r => r.Title == "Admin");
        var userRole = context.Roles.FirstOrDefault(r => r.Title == "User");

        var users = new List<User>
        {
            User.New(new UserId(Guid.NewGuid()), "Admin", "Admin", "admin@gmail.com", "password", adminRole.Id),
            User.New(new UserId(Guid.NewGuid()), "User", "User", "user@gmail.com", "password", userRole.Id)
        };
        context.Users.AddRange(users);
        await context.SaveChangesAsync();
    }

    private async Task SeedCategoriesAsync()
    {
        var categories = new List<Category>
        {
            Category.New(new CategoryId(Guid.NewGuid()), "Running"),
            Category.New(new CategoryId(Guid.NewGuid()), "Basketball"),
            Category.New(new CategoryId(Guid.NewGuid()), "Casual")
        };
        context.Categories.AddRange(categories);
        await context.SaveChangesAsync();
    }

    private async Task SeedBrandsAsync()
    {
        var brands = new List<Brand>
        {
            Brand.New(new BrandId(Guid.NewGuid()), "Nike"),
            Brand.New(new BrandId(Guid.NewGuid()), "Adidas"),
            Brand.New(new BrandId(Guid.NewGuid()), "Puma")
        };
        context.Brands.AddRange(brands);
        await context.SaveChangesAsync();
    }

    private async Task SeedWarehousesAsync()
    {
        var warehouses = new List<Warehouse>
        {
            Warehouse.New(new WarehouseId(Guid.NewGuid()), "Warehouse 1", 100),
            Warehouse.New(new WarehouseId(Guid.NewGuid()), "Warehouse 2", 200)
        };
        context.Warehouses.AddRange(warehouses);
        await context.SaveChangesAsync();
    }

    private async Task SeedStatusesAsync()
    {
        var statuses = new List<Status>
        {
            Status.New(new StatusId(Guid.NewGuid()), "Pending"),
            Status.New(new StatusId(Guid.NewGuid()), "Shipped"),
            Status.New(new StatusId(Guid.NewGuid()), "Delivered")
        };
        context.Statuses.AddRange(statuses);
        await context.SaveChangesAsync();
    }

    private async Task SeedSneakersAsync()
    {
        var brandNike = context.Brands.FirstOrDefault(b => b.Name == "Nike");
        var categoryRunning = context.Categories.FirstOrDefault(c => c.Name == "Running");

        var sneakers = new List<Sneaker>
        {
            Sneaker.New(new SneakerId(Guid.NewGuid()), "Air Zoom Pegasus", 42, 120, brandNike.Id, categoryRunning.Id),
            Sneaker.New(new SneakerId(Guid.NewGuid()), "Revolution 5", 43, 80, brandNike.Id, categoryRunning.Id)
        };
        context.Sneakers.AddRange(sneakers);
        await context.SaveChangesAsync();
    }

    private async Task SeedSneakerWarehousesAsync()
    {
        var warehouse1 = context.Warehouses.FirstOrDefault(w => w.Location == "Warehouse 1");
        var sneaker1 = context.Sneakers.FirstOrDefault(s => s.Model == "Air Zoom Pegasus");

        var sneakerWarehouses = new List<SneakerWarehouse>
        {
            SneakerWarehouse.New(new SneakerWarehouseId(Guid.NewGuid()), sneaker1.Id, warehouse1.Id, 50)
        };
        context.SneakerWarehouses.AddRange(sneakerWarehouses);
        await context.SaveChangesAsync();
    }

    private async Task SeedOrdersAsync()
    {
        var user = context.Users.FirstOrDefault(u => u.Email == "user@gmail.com");
        var status = context.Statuses.FirstOrDefault(s => s.Title == "Pending");
        if (user == null || status == null)
        {
            throw new InvalidOperationException("Required data for Orders seeding is missing. Ensure Users and Statuses are seeded first.");
        }
        var order = Order.New(new OrderId(Guid.NewGuid()), user.Id, status.Id, 0);
        var sneakerWarehouse = context.SneakerWarehouses
            .Include(sw => sw.Sneaker)
            .FirstOrDefault();
        if (sneakerWarehouse != null)
        {
            var orderItem = OrderItem.New(new OrderItemId(Guid.NewGuid()), sneakerWarehouse.Id, order.Id, 2);
            order.AddItem(orderItem);
        }
        context.Orders.Add(order);
        await context.SaveChangesAsync();
    }
}