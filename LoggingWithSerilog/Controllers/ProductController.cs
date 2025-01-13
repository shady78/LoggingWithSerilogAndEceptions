using LoggingWithSerilog.Dtos;
using LoggingWithSerilog.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LoggingWithSerilog.Controllers;
[Route("api/[controller]")]
[ApiController]
public class ProductController(IProductService _productService) : ControllerBase
{
    //    app.MapGet("/products", async(IProductService service) =>
    //{
    //    var products = await service.GetAll();
    //    return Results.Ok(products);
    //});

    //app.MapGet("/products/{id:guid}", async(Guid id, IProductService service) =>
    //{
    //    var product = await service.Get(id);
    //    return Results.Ok(product);
    //});

    //app.MapPost("/products", async (ProductCreationDto product, IProductService service) =>
    //{
    //    await service.Add(product);
    //    return Results.Created();
    //});
    [HttpPost]
    public async Task<IActionResult> Post(ProductCreationDto productCreationDto)
    {
        await _productService.Add(productCreationDto);
        return Created();
    }
    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAll()
    {
        var products = await _productService.GetAll();
        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var product = await _productService.Get(id);
        return Ok(product);
    }

}
