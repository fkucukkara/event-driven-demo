# PowerShell script to start the Event-Driven Architecture Demo

Write-Host "Starting Event-Driven Architecture Demo..." -ForegroundColor Green

# Check if Docker is running
try {
    docker version | Out-Null
    Write-Host "✓ Docker is running" -ForegroundColor Green
} catch {
    Write-Host "✗ Docker is not running. Please start Docker Desktop." -ForegroundColor Red
    exit 1
}

# Check if Docker Compose is available
try {
    docker-compose version | Out-Null
    Write-Host "✓ Docker Compose is available" -ForegroundColor Green
} catch {
    Write-Host "✗ Docker Compose is not available." -ForegroundColor Red
    exit 1
}

Write-Host "Building and starting services..." -ForegroundColor Yellow
docker-compose up --build -d

Write-Host ""
Write-Host "Services are starting up. Please wait a moment for all services to be ready..." -ForegroundColor Yellow
Start-Sleep -Seconds 10

Write-Host ""
Write-Host "Event-Driven Architecture Demo is now running!" -ForegroundColor Green
Write-Host ""
Write-Host "Access the APIs:" -ForegroundColor Cyan
Write-Host "  Order API:        http://localhost:5001/swagger" -ForegroundColor White
Write-Host "  Inventory API:    http://localhost:5002/swagger" -ForegroundColor White
Write-Host "  Notification API: http://localhost:5003/swagger" -ForegroundColor White
Write-Host "  RabbitMQ Admin:   http://localhost:15672 (guest/guest)" -ForegroundColor White
Write-Host ""
Write-Host "To test the event flow, use the test-requests.http file or the Swagger UIs." -ForegroundColor Yellow
Write-Host ""
Write-Host "To stop the services, run: docker-compose down" -ForegroundColor Gray
