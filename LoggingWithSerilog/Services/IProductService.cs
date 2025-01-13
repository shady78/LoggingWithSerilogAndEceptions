
using global::LoggingWithSerilog.Dtos;
using global::LoggingWithSerilog.Models;

namespace LoggingWithSerilog.Services;

public interface IProductService
{
    /// <summary>
    /// Adds a new product to the database and invalidates the cache
    /// </summary>
    /// <param name="request">The product creation data transfer object</param>
    Task Add(ProductCreationDto request);

    /// <summary>
    /// Gets a product by its ID, using cache when available
    /// </summary>
    /// <param name="id">The unique identifier of the product</param>
    /// <returns>The requested product</returns>
    Task<Product> Get(Guid id);

    /// <summary>
    /// Gets all products, using cache when available
    /// </summary>
    /// <returns>A list of all products</returns>
    Task<List<Product>> GetAll();
}