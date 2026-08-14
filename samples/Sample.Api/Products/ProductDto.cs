namespace Sample.Api.Products;

public sealed record ProductDto(
    int Id,
    string Name,
    decimal Price);