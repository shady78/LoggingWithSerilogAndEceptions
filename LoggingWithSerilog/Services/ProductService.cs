using LoggingWithSerilog.Data;
using LoggingWithSerilog.Dtos;
using LoggingWithSerilog.Extensions;
using LoggingWithSerilog.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace LoggingWithSerilog.Services;

public class ProductService(ApplicationDbContext context, IDistributedCache cache /*IMemoryCache cache*/, ILogger<ProductService> logger) : IProductService
{
    public async Task Add(ProductCreationDto request)
    {
        var product = new Product(request.Name, request.Description, request.Price);
        await context.Products.AddAsync(product);
        await context.SaveChangesAsync();
        // invalidate cache for products, as new product is added
        var cacheKey = "products";
        logger.LogInformation("invalidating cache for key: {CacheKey} from cache.", cacheKey);
        cache.Remove(cacheKey);
    }

    public async Task<Product> Get(Guid id)
    {
        var cacheKey = $"product:{id}";
        logger.LogInformation("fetching data for key: {CacheKey} from cache.", cacheKey);
        // Distributed-Cash with Redis 
        var product = await cache.GetOrSetAsync(cacheKey,
           async () =>
           {
               logger.LogInformation("cache miss. fetching data for key: {CacheKey} from database.", cacheKey);
               return await context.Products.FindAsync(id)!;
           })!;

        // In-Memory Cash
        //if (!cache.TryGetValue(cacheKey, out Product? product))
        //{
        //    logger.LogInformation("cache miss. fetching data for key: {CacheKey} from database.", cacheKey);
        //    product = await context.Products.FindAsync(id);
        //    var cacheOptions = new MemoryCacheEntryOptions()
        //        .SetSlidingExpiration(TimeSpan.FromSeconds(30))
        //        .SetAbsoluteExpiration(TimeSpan.FromSeconds(300))
        //        .SetPriority(CacheItemPriority.Normal);
        //    logger.LogInformation("setting data for key: {CacheKey} to cache.", cacheKey);
        //    cache.Set(cacheKey, product, cacheOptions);
        //}
        //else
        //{
        //    logger.LogInformation("cache hit for key: {CacheKey}.", cacheKey);
        //}
        return product!;
    }

    public async Task<List<Product>> GetAll()
    {
        var cacheKey = "products";
        logger.LogInformation("fetching data for key: {CacheKey} from cache.", cacheKey);
        // Distributed-Cash with Redis
        var cacheOptions = new DistributedCacheEntryOptions()
               .SetAbsoluteExpiration(TimeSpan.FromMinutes(20))
               .SetSlidingExpiration(TimeSpan.FromMinutes(2));
        var products = await cache.GetOrSetAsync(
            cacheKey,
            async () =>
            {
                logger.LogInformation("cache miss. fetching data for key: {CacheKey} from database.", cacheKey);
                return await context.Products.ToListAsync();
            },
            cacheOptions)!;

        // In-Memory Cash
        //if (!cache.TryGetValue(cacheKey, out List<Product>? products))
        //{
        //    logger.LogInformation("cache miss. fetching data for key: {CacheKey} from database.", cacheKey);
        //    products = await context.Products.ToListAsync();
        //    var cacheOptions = new MemoryCacheEntryOptions()
        //        .SetSlidingExpiration(TimeSpan.FromSeconds(30))
        //        .SetAbsoluteExpiration(TimeSpan.FromSeconds(300))
        //        .SetPriority(CacheItemPriority.NeverRemove)
        //        .SetSize(2048);
        //    logger.LogInformation("setting data for key: {CacheKey} to cache.", cacheKey);
        //    cache.Set(cacheKey, products, cacheOptions);
        //}
        //else
        //{
        //    logger.LogInformation("cache hit for key: {CacheKey}.", cacheKey);
        //}
        return products!;
    }
}