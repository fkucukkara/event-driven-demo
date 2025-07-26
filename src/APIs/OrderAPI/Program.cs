using EventDrivenDemo.Shared.Extensions;
using Microsoft.EntityFrameworkCore;
using OrderAPI.Data;
using OrderAPI.DTOs;
using OrderAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Order API", Version = "v1" });
});

// Add Entity Framework
builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseInMemoryDatabase("OrderDb"));

// Add application services
builder.Services.AddScoped<OrderService>();

// Add event publishing
builder.Services.AddEventPublishing(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Map minimal API endpoints
var ordersGroup = app.MapGroup("/api/orders")
    .WithTags("Orders")
    .WithOpenApi();

// Create a new order
ordersGroup.MapPost("/", async (CreateOrderRequest request, OrderService orderService, CancellationToken cancellationToken) =>
{
    var order = await orderService.CreateOrderAsync(request, cancellationToken);
    return Results.Created($"/api/orders/{order.Id}", order);
})
.WithName("CreateOrder")
.WithSummary("Creates a new order")
.Produces(201)
.Produces(400);

// Get an order by ID
ordersGroup.MapGet("/{id:guid}", async (Guid id, OrderService orderService, CancellationToken cancellationToken) =>
{
    var order = await orderService.GetOrderAsync(id, cancellationToken);
    return order is not null ? Results.Ok(order) : Results.NotFound();
})
.WithName("GetOrder")
.WithSummary("Gets an order by ID")
.Produces(200)
.Produces(404);

// Get all orders
ordersGroup.MapGet("/", async (OrderService orderService, CancellationToken cancellationToken) =>
{
    var orders = await orderService.GetOrdersAsync(cancellationToken);
    return Results.Ok(orders);
})
.WithName("GetOrders")
.WithSummary("Gets all orders")
.Produces(200);

// Update an order status
ordersGroup.MapPut("/{id:guid}", async (Guid id, UpdateOrderRequest request, OrderService orderService, CancellationToken cancellationToken) =>
{
    var order = await orderService.UpdateOrderAsync(id, request, cancellationToken);
    return order is not null ? Results.Ok(order) : Results.NotFound();
})
.WithName("UpdateOrder")
.WithSummary("Updates an order status")
.Produces(200)
.Produces(404);

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
    context.Database.EnsureCreated();
}

app.Run();
