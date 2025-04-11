using System;
using Bogus;
using ColumnWiseSearch.DataLibrary;
using ColumnWiseSearch.Helpers.Extensions;
using ColumnWiseSearch.Models;

namespace ColumnWiseSearch.Service;
// Repository implementation
public interface IProductRepository
{
    Task<PagedResult<Product>> SearchProductsAsync(SearchRequest request);
}

public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _context;

    public ProductRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<Product>> SearchProductsAsync(SearchRequest request)
    {
        await SeedLargeProductDatasetAsync();
        var query = _context.Products.AsQueryable();

        // Apply filters and sorting
        query = query.ApplySearch(request);

        // Return paginated result
        return await PagedResult<Product>.CreateAsync(
            query,
            request.Page,
            request.PageSize);
    }

    public async Task SeedLargeProductDatasetAsync(int count = 10000)
    {

        // Configure Bogus faker for products
        var faker = new Faker<Product>()
            // Ensure consistent data when running multiple times
            .UseSeed(8675309)
            // Basic product properties
            .RuleFor(p => p.Name, f => f.Commerce.ProductName())
            .RuleFor(p => p.Description, f => f.Commerce.ProductDescription())
            .RuleFor(p => p.Category, f => f.Commerce.Categories(1)[0])
            .RuleFor(p => p.Price, f => Math.Round(decimal.Parse(f.Commerce.Price()), 2))
            .RuleFor(p => p.IsAvailable, f => f.Random.Bool(0.8f)) // 80% of products are available
            .RuleFor(p => p.CreatedDate, f => f.Date.Past(2)); // Products created within the last 2 years

        // Generate the products
        var products = faker.Generate(count);

        // Add in batches to improve performance
        const int batchSize = 500;
        for (int i = 0; i < count; i += batchSize)
        {
            var batch = products.Skip(i).Take(batchSize);
            await _context.Products.AddRangeAsync(batch);
            await _context.SaveChangesAsync();

            // Optional: Output progress
            Console.WriteLine($"Added {Math.Min(i + batchSize, count)} of {count} products");
        }

        Console.WriteLine("Database seeding completed successfully.");
    }
}
