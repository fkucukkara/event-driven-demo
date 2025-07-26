#!/bin/bash

# Bash script to start the Event-Driven Architecture Demo

echo "Starting Event-Driven Architecture Demo..."

# Check if Docker is running
if ! docker version > /dev/null 2>&1; then
    echo "✗ Docker is not running. Please start Docker."
    exit 1
fi
echo "✓ Docker is running"

# Check if Docker Compose is available
if ! docker-compose version > /dev/null 2>&1; then
    echo "✗ Docker Compose is not available."
    exit 1
fi
echo "✓ Docker Compose is available"

echo "Building and starting services..."
docker-compose up --build -d

echo ""
echo "Services are starting up. Please wait a moment for all services to be ready..."
sleep 10

echo ""
echo "Event-Driven Architecture Demo is now running!"
echo ""
echo "Access the APIs:"
echo "  Order API:        http://localhost:5001/swagger"
echo "  Inventory API:    http://localhost:5002/swagger"
echo "  Notification API: http://localhost:5003/swagger"
echo "  RabbitMQ Admin:   http://localhost:15672 (guest/guest)"
echo ""
echo "To test the event flow, use the test-requests.http file or the Swagger UIs."
echo ""
echo "To stop the services, run: docker-compose down"
