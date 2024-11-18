using System.Reflection;
using Domain.Sneakers;
using Domain.Brands;
using Domain.OrderItems;
using Domain.Orders;
using Domain.Roles;
using Domain.SneakerWarehouses;
using Domain.Statuses;
using Domain.Users;
using Domain.Warehouses;
using Domain.Сategories;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Sneaker> Sneakers { get; set; }
    public DbSet<SneakerImage> SneakerImages { get; set; }
    public DbSet<Brand> Brands { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Warehouse> Warehouses { get; set; }
    public DbSet<SneakerWarehouse> SneakerWarehouses { get; set; }
    public DbSet<Status> Statuses { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(builder);
    }
}