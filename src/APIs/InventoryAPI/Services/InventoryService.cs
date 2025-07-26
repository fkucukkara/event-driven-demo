using InventoryAPI.Data;
using InventoryAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryAPI.Services;

/// <summary>
/// Service for managing inventory operations
/// </summary>
public class InventoryService
{
    private readonly InventoryDbContext _context;
    private readonly ILogger<InventoryService> _logger;

    public InventoryService(InventoryDbContext context, ILogger<InventoryService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> ReserveInventoryAsync(Guid orderId, List<(string ProductId, int Quantity)> items, CancellationToken cancellationToken = default)
    {
        //using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        
        try
        {
            foreach (var (productId, quantity) in items)
            {
                var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId, cancellationToken);
                
                if (product == null)
                {
                    _logger.LogWarning("Product {ProductId} not found for order {OrderId}", productId, orderId);
                    return false;
                }

                if (product.AvailableQuantity < quantity)
                {
                    _logger.LogWarning("Insufficient inventory for product {ProductId}. Available: {Available}, Requested: {Requested}", 
                        productId, product.AvailableQuantity, quantity);
                    return false;
                }

                product.ReservedQuantity += quantity;
                product.LastUpdated = DateTime.UtcNow;

                // Log transaction
                var inventoryTransaction = new InventoryTransaction
                {
                    ProductId = productId,
                    TransactionType = "Reserve",
                    Quantity = quantity,
                    OrderId = orderId,
                    Reason = $"Reserved for order {orderId}"
                };

                _context.InventoryTransactions.Add(inventoryTransaction);
            }

            await _context.SaveChangesAsync(cancellationToken);
            //await transaction.CommitAsync(cancellationToken);

            _logger.LogInformation("Successfully reserved inventory for order {OrderId}", orderId);
            return true;
        }
        catch (Exception ex)
        {
            //await transaction.RollbackAsync(cancellationToken);
            _logger.LogError(ex, "Failed to reserve inventory for order {OrderId}", orderId);
            return false;
        }
    }

    public async Task ReleaseInventoryAsync(Guid orderId, List<(string ProductId, int Quantity)> items, CancellationToken cancellationToken = default)
    {
        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        
        try
        {
            foreach (var (productId, quantity) in items)
            {
                var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId, cancellationToken);
                
                if (product != null)
                {
                    product.ReservedQuantity = Math.Max(0, product.ReservedQuantity - quantity);
                    product.LastUpdated = DateTime.UtcNow;

                    // Log transaction
                    var inventoryTransaction = new InventoryTransaction
                    {
                        ProductId = productId,
                        TransactionType = "Release",
                        Quantity = quantity,
                        OrderId = orderId,
                        Reason = $"Released from cancelled order {orderId}"
                    };

                    _context.InventoryTransactions.Add(inventoryTransaction);
                }
            }

            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            _logger.LogInformation("Successfully released inventory for order {OrderId}", orderId);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            _logger.LogError(ex, "Failed to release inventory for order {OrderId}", orderId);
        }
    }

    public async Task<List<Product>> GetProductsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Products.ToListAsync(cancellationToken);
    }

    public async Task<Product?> GetProductAsync(string productId, CancellationToken cancellationToken = default)
    {
        return await _context.Products.FirstOrDefaultAsync(p => p.Id == productId, cancellationToken);
    }
}
