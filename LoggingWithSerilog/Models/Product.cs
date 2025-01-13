﻿using System.ComponentModel.DataAnnotations.Schema;

namespace LoggingWithSerilog.Models;

public class Product
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public decimal? Price { get; set; }

    // Parameterless constructor for EF Core
    private Product() { }

    public Product(string name, string description, decimal price)
    {
        Id = Guid.NewGuid();
        Name = name;
        Description = description;
        Price = price;
    }
}