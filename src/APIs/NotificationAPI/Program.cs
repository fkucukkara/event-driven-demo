using EventDrivenDemo.Shared.Events;
using EventDrivenDemo.Shared.Extensions;
using EventDrivenDemo.Shared.Messaging;
using Microsoft.EntityFrameworkCore;
using NotificationAPI.Data;
using NotificationAPI.EventHandlers;
using NotificationAPI.Models;
using NotificationAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Notification API", Version = "v1" });
});

// Add Entity Framework
builder.Services.AddDbContext<NotificationDbContext>(options =>
    options.UseInMemoryDatabase("NotificationDb"));

// Add application services
builder.Services.AddScoped<NotificationService>();

// Add event handling
builder.Services.AddEventHandler<OrderCreatedEventHandler, OrderCreatedEvent>();
builder.Services.AddEventHandler<OrderUpdatedEventHandler, OrderUpdatedEvent>();
builder.Services.AddEventHandler<OrderCancelledEventHandler, OrderCancelledEvent>();

// Add event consuming
builder.Services.Configure<RabbitMqOptions>(builder.Configuration.GetSection(RabbitMqOptions.SectionName));
builder.Services.AddHostedService<RabbitMqEventConsumer>();

// Set service name for queue naming
Environment.SetEnvironmentVariable("SERVICE_NAME", "notification");

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Map minimal API endpoints
var notificationsGroup = app.MapGroup("/api/notifications")
    .WithTags("Notifications")
    .WithOpenApi();

// Get all notifications
notificationsGroup.MapGet("/", async (NotificationService notificationService, CancellationToken cancellationToken) =>
{
    var notifications = await notificationService.GetNotificationsAsync(cancellationToken);
    return Results.Ok(notifications);
})
.WithName("GetNotifications")
.WithSummary("Gets all notifications")
.Produces<List<Notification>>(200);

// Get notifications for a specific email address
notificationsGroup.MapGet("/by-email/{email}", async (string email, NotificationService notificationService, CancellationToken cancellationToken) =>
{
    var notifications = await notificationService.GetNotificationsByEmailAsync(email, cancellationToken);
    return Results.Ok(notifications);
})
.WithName("GetNotificationsByEmail")
.WithSummary("Gets notifications for a specific email address")
.Produces<List<Notification>>(200);

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<NotificationDbContext>();
    context.Database.EnsureCreated();
}

app.Run();
