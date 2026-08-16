using Behrouzan.BuildingBlocks.Core.Results;

namespace Sample.Api.Products;

public sealed class ProductService
{
    public Result<ProductDto> GetById(int id)
    {
        if (id <= 0)
        {
            return Result<ProductDto>.Failure(
                Error.Validation(
                    "Product.Id.Invalid",
                    "Product id must be greater than zero.",
                    "id"));
        }

        if (id != 1)
        {
            return Result<ProductDto>.Failure(
                Error.NotFound(
                    "Product.NotFound",
                    "Product was not found."));
        }

        var product = new ProductDto(
            1,
            "Laptop",
            1500m);

        return Result<ProductDto>.Success(product);
    }

    public Result Delete(int id)
    {
        if (id <= 0)
        {
            return Result.Failure(
                Error.Validation(
                    "Product.Id.Invalid",
                    "Product id must be greater than zero.",
                    "id"));
        }

        if (id != 1)
        {
            return Result.Failure(
                Error.NotFound(
                    "Product.NotFound",
                    "Product was not found."));
        }

        return Result.Success();
    }
}