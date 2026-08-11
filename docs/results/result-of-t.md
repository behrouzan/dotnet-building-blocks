# Result<T>

`Result<T>` represents the outcome of an operation that returns a value when successful.

For example:

```csharp
Result<ProductDto>
```

represents an operation that either successfully returns a `ProductDto` or fails with one or more errors.

Conceptually:

```text
Result<ProductDto>
│
├── Success
│   ├── Value = ProductDto
│   └── Errors = empty
│
└── Failure
    ├── Value = unavailable
    └── Errors = one or more
```

## Success

Create a successful result using:

```csharp
return Result<ProductDto>.Success(productDto);
```

The returned value is available through:

```csharp
result.Value
```

Example:

```csharp
if (result.IsSuccess)
{
    ProductDto product = result.Value;
}
```

## Failure

Create a failed result using:

```csharp
return Result<ProductDto>.Failure(
    Error.NotFound(
        "Product.NotFound",
        "Product was not found."));
```

A failed `Result<T>` does not contain a usable value.

## Safe Value Access

`Value` can only be accessed when the result is successful.

Correct:

```csharp
if (result.IsSuccess)
{
    var product = result.Value;
}
```

Or:

```csharp
if (result.IsFailure)
{
    // Handle errors
    return;
}

var product = result.Value;
```

Incorrect:

```csharp
var result = GetProduct();

var product = result.Value;
```

If `result` represents a failure, accessing `Value` throws an `InvalidOperationException`.

This prevents accidentally using a value from a failed operation.

## Complete Example

```csharp
public async Task<Result<ProductDto>> GetProductAsync(int id)
{
    var product = await db.Products.FindAsync(id);

    if (product is null)
    {
        return Result<ProductDto>.Failure(
            Error.NotFound(
                "Product.NotFound",
                "Product was not found."));
    }

    var dto = new ProductDto
    {
        Id = product.Id,
        Name = product.Name
    };

    return Result<ProductDto>.Success(dto);
}
```

Consumer:

```csharp
var result = await GetProductAsync(10);

if (result.IsFailure)
{
    foreach (var error in result.Errors)
    {
        // Handle error
    }

    return;
}

var product = result.Value;
```

## Multiple Errors

A failed `Result<T>` may contain multiple errors.

```csharp
return Result<UserDto>.Failure(
    Error.Validation(
        "User.Email.Invalid",
        "Email is invalid.",
        "email"),

    Error.Validation(
        "User.Password.Invalid",
        "Password is invalid.",
        "password"));
```

## How IsSuccess Is Determined

`Result<T>` does not inspect the database or automatically detect whether an operation succeeded.

The application explicitly creates either a successful or failed result.

For example:

```csharp
if (product is null)
{
    return Result<ProductDto>.Failure(
        Error.NotFound(
            "Product.NotFound",
            "Product was not found."));
}

return Result<ProductDto>.Success(productDto);
```

Calling `Success(...)` creates a result with:

```text
IsSuccess = true
```

Calling `Failure(...)` creates a result with:

```text
IsSuccess = false
```

The application therefore decides whether an expected operation succeeded or failed.

## Result vs Result<T>

Use `Result` when no value needs to be returned.

Use `Result<T>` when a successful operation produces a value.

Examples:

```text
DeleteProduct   -> Result

SendEmail       -> Result

CancelOrder     -> Result

GetProduct      -> Result<ProductDto>

CreateUser      -> Result<UserDto>

CalculatePrice  -> Result<decimal>
```

## Exceptions

`Result<T>` does not automatically catch exceptions.

For example:

```csharp
var product = await db.Products.FindAsync(id);
```

may still throw an exception if the database connection fails.

Expected application failures should normally use `Result<T>.Failure(...)`.

Unexpected exceptions can be handled by higher application layers, such as an ASP.NET Core global exception handler.