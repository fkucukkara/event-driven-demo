using EventDrivenDemo.Shared.Events;
using EventDrivenDemo.Shared.Extensions;
using EventDrivenDemo.Shared.Messaging;
using InventoryAPI.Data;
using InventoryAPI.EventHandlers;
using InventoryAPI.Models;
using InventoryAPI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Inventory API", Version = "v1" });
});

// Add Entity Framework
builder.Services.AddDbContext<InventoryDbContext>(options =>
    options.UseInMemoryDatabase("InventoryDb"));

// Add application services
builder.Services.AddScoped<InventoryService>();

// Add event handling
builder.Services.AddEventHandler<OrderCreatedEventHandler, OrderCreatedEvent>();
builder.Services.AddEventHandler<OrderCancelledEventHandler, OrderCancelledEvent>();

// Add event consuming
builder.Services.Configure<RabbitMqOptions>(builder.Configuration.GetSection(RabbitMqOptions.SectionName));
builder.Services.AddHostedService<RabbitMqEventConsumer>();

// Set service name for queue naming
Environment.SetEnvironmentVariable("SERVICE_NAME", "inventory");

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Map minimal API endpoints
var inventoryGroup = app.MapGroup("/api/inventory")
    .WithTags("Inventory")
    .WithOpenApi();

// Get all products and their inventory levels
inventoryGroup.MapGet("/products", async (InventoryService inventoryService, CancellationToken cancellationToken) =>
{
    var products = await inventoryService.GetProductsAsync(cancellationToken);
    return Results.Ok(products);
})
.WithName("GetProducts")
.WithSummary("Gets all products and their inventory levels")
.Produces<List<Product>>(200);

// Get a specific product and its inventory level
inventoryGroup.MapGet("/products/{productId}", async (string productId, InventoryService inventoryService, CancellationToken cancellationToken) =>
{
    var product = await inventoryService.GetProductAsync(productId, cancellationToken);
    return product is not null ? Results.Ok(product) : Results.NotFound();
})
.WithName("GetProduct")
.WithSummary("Gets a specific product and its inventory level")
.Produces<Product>(200)
.Produces(404);

// Seed initial data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();
    context.Database.EnsureCreated();

    // Seed some sample products
    if (!context.Products.Any())
    {
        context.Products.AddRange(
            new Product { Id = "LAPTOP001", Name = "Gaming Laptop", StockQuantity = 50, ReservedQuantity = 0 },
            new Product { Id = "MOUSE001", Name = "Wireless Mouse", StockQuantity = 100, ReservedQuantity = 0 },
            new Product { Id = "KEYBOARD001", Name = "Mechanical Keyboard", StockQuantity = 75, ReservedQuantity = 0 }
        );
        context.SaveChanges();
    }
}

app.Run();
