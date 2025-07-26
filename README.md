# Event-Driven Architecture Demo with .NET 9

This project demonstrates an event-driven architecture using .NET 9 Minimal APIs, RabbitMQ message broker, and Docker Compose.

> **🤖 Created by Augment Agent**
> This project has been fully prepared by **Augment Agent**, an AI-powered coding assistant developed by Augment Code.
> This is an educational demonstration project and is **not intended for commercial use**.
> Perfect for learning event-driven architecture patterns and modern .NET development practices.

## Architecture Overview

The system consists of three microservices that communicate through events:

1. **Order API** (Port 5001) - Manages orders and publishes order events
2. **Inventory API** (Port 5002) - Manages inventory and subscribes to order events
3. **Notification API** (Port 5003) - Sends notifications and subscribes to order events

## Event Flow

```
Order API → RabbitMQ → Inventory API (reserves stock)
         ↘          ↗ Notification API (sends emails)
```

### Events Published:
- `OrderCreatedEvent` - When a new order is created
- `OrderUpdatedEvent` - When an order status is updated
- `OrderCancelledEvent` - When an order is cancelled

## Getting Started

### Prerequisites
- Docker and Docker Compose
- .NET 9.0 SDK (for local development)

### Running with Docker Compose

1. Clone the repository
2. Run the entire system:
```bash
docker-compose up --build
```

3. Access the APIs:
   - Order API: http://localhost:5001/swagger
   - Inventory API: http://localhost:5002/swagger
   - Notification API: http://localhost:5003/swagger
   - RabbitMQ Management: http://localhost:15672 (guest/guest)

### 🐛 Debugging in Visual Studio

For Visual Studio users, the solution includes Docker Compose orchestration support:

1. **Open the solution** in Visual Studio 2022
2. **Set docker-compose as startup project**:
   - Right-click on `docker-compose` project in Solution Explorer
   - Select "Set as Startup Project"
3. **Start debugging** (F5) or run without debugging (Ctrl+F5)
4. **Visual Studio will**:
   - Build all Docker images
   - Start all services with debugging support
   - Open browser to Order API Swagger UI
   - Enable breakpoint debugging in containerized services

#### 🔧 Debug Configuration Features:
- **Breakpoint debugging** in all APIs
- **Hot reload** support for code changes
- **Volume mounting** for real-time file updates
- **Integrated logging** in Visual Studio output
- **Service dependency management** with health checks

### Testing the Event Flow

1. **Create an Order** (POST to Order API):
```json
{
  "customerEmail": "customer@example.com",
  "items": [
    {
      "productId": "LAPTOP001",
      "productName": "Gaming Laptop",
      "quantity": 1,
      "unitPrice": 999.99
    }
  ]
}
```

2. **Check Inventory** (GET from Inventory API):
   - View reserved quantities for products

3. **Check Notifications** (GET from Notification API):
   - View sent notifications for the customer

## Key Features

- **Event-Driven Communication**: Loose coupling between services
- **Message Broker**: RabbitMQ for reliable message delivery
- **Event Sourcing**: All events are logged and traceable
- **Microservices**: Independent, scalable services
- **Minimal APIs**: Modern, lightweight API endpoints
- **Docker Compose**: Easy deployment and orchestration
- **Swagger Documentation**: Interactive API documentation

## Technologies Used

- .NET 9.0
- ASP.NET Core Minimal APIs
- Entity Framework Core (In-Memory)
- RabbitMQ
- Docker & Docker Compose
- Swagger/OpenAPI

---

## 📚 Educational Purpose & Attribution

### 🎓 **Learning Objectives**
This project serves as a comprehensive educational resource for:
- **Event-Driven Architecture** patterns and best practices
- **Microservices** design and implementation
- **.NET 9 Minimal APIs** modern development approach
- **Message Brokers** (RabbitMQ) integration
- **Docker containerization** and orchestration
- **Asynchronous communication** between services

### 🤖 **Created by Augment Agent**
This entire project was designed and implemented by **Augment Agent**, showcasing:
- Advanced code generation capabilities
- Architectural design expertise
- Best practices implementation
- Complete solution delivery from concept to deployment

### ⚖️ **License & Usage**
- **Educational Use Only**: This project is intended for learning and educational purposes
- **Not for Commercial Use**: Please do not use this code in commercial applications without proper review and modifications
- **Open Source**: Feel free to study, modify, and learn from this codebase
- **Attribution**: When using this project for educational purposes, please credit Augment Agent

### 🔗 **Learn More**
- **Augment Code**: [https://www.augmentcode.com](https://www.augmentcode.com)
- **Event-Driven Architecture**: Study the patterns demonstrated in this project
- **Microservices**: Explore the service boundaries and communication patterns
