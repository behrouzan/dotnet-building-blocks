# Behrouzan.Results.AspNetCore

ASP.NET Core integration for `Behrouzan.Results`.

This package converts application results into ASP.NET Core HTTP responses while keeping the core Result model independent from HTTP concerns.

## Features

- `Result<T>` to `IResult` conversion for Minimal APIs
- `Result<T>` to `IActionResult` conversion for controller-based APIs
- Non-generic `Result` support
- Automatic HTTP status code mapping
- Problem Details responses
- Structured error details
- Validation error support
- Trace identifiers
- Configurable HTTP status mappings
- Configurable problem type base
- Shared response contract for Minimal APIs and controller-based APIs

## Installation

```bash
dotnet add package Behrouzan.Results.AspNetCore
```

`Behrouzan.Results` is installed automatically as a dependency.

## Registration

Register the integration during application startup:

```csharp
builder.Services.AddBehrouzanResultHttp();
```

## Basic Usage

A `Result<T>` can be converted directly to an ASP.NET Core `IResult`:

```csharp
app.MapGet("/products/{id:int}", (int id) =>
{
    Result<Product> result = GetProduct(id);

    return result.ToHttpResult();
});
```

A successful result returns the contained value.

For example:

```json
{
  "id": 1,
  "name": "Laptop",
  "price": 1500
}
```



## Controller-based APIs

For ASP.NET Core controller-based APIs, convert a result to an `IActionResult` using `ToActionResult()`.

```csharp
[ApiController]
[Route("api/[controller]")]
public sealed class ProductsController : ControllerBase
{
    private readonly ProductService _service;

    public ProductsController(ProductService service)
    {
        _service = service;
    }

    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
    {
        return _service
            .GetById(id)
            .ToActionResult();
    }
}
```

Successful generic results return HTTP `200` with the contained value.

Failed results use the same Problem Details contract and HTTP status mapping as `ToHttpResult()`.

Non-generic successful results return HTTP `204 No Content`:

```csharp
[HttpDelete("{id:int}")]
public IActionResult Delete(int id)
{
    return _service
        .Delete(id)
        .ToActionResult();
}
```

### Minimal API vs Controller API

Use:

```csharp
result.ToHttpResult();
```

for Minimal APIs.

Use:

```csharp
result.ToActionResult();
```

for controller-based APIs.

Both paths use the same error mapping, Problem Details format, configuration, and trace identifier behavior.

## Failure Responses

Application errors are automatically converted into Problem Details responses.

For example:

```csharp
return Result<Product>.Failure(
    Error.NotFound(
        "Product.NotFound",
        "Product was not found."));
```

produces an HTTP `404` response similar to:

```json
{
  "type": "urn:behrouzan:problem:not-found",
  "title": "Resource not found",
  "status": 404,
  "detail": "Product was not found.",
  "errors": [
    {
      "code": "Product.NotFound",
      "message": "Product was not found.",
      "type": "NotFound",
      "propertyPath": null,
      "metadata": {}
    }
  ],
  "traceId": "..."
}
```

Validation errors are mapped to HTTP `400`.

## Default HTTP Mappings

The default mappings are:

| Error Type | HTTP Status |
| --- | ---: |
| `Failure` | 500 |
| `Validation` | 400 |
| `Unauthorized` | 401 |
| `Forbidden` | 403 |
| `NotFound` | 404 |
| `Conflict` | 409 |
| `RateLimit` | 429 |
| `Unavailable` | 503 |
| `Timeout` | 504 |

## Custom Status Mapping

Mappings can be overridden during registration:

```csharp
builder.Services.AddBehrouzanResultHttp(options =>
{
    options.MapStatusCode(
        ErrorType.Failure,
        StatusCodes.Status422UnprocessableEntity);
});
```

## Custom Problem Type Base

The default problem type base is:

```text
urn:behrouzan:problem
```

It can be customized:

```csharp
builder.Services.AddBehrouzanResultHttp(options =>
{
    options.ProblemTypeBase =
        "https://api.example.com/problems";
});
```

A not-found error could then produce a type such as:

```text
https://api.example.com/problems/not-found
```

## Non-Generic Results

Non-generic results can also be converted:

```csharp
Result result = DeleteProduct(id);

return result.ToHttpResult();
```

A successful non-generic result returns HTTP `204 No Content`.

Failures are converted to the corresponding Problem Details response.

## Structured Errors

The original application errors are included in the HTTP response.

This preserves information such as:

- Error code
- Message
- Error type
- Property path
- Metadata

Clients can therefore process errors programmatically instead of depending only on human-readable messages.

## Trace Identifiers

Problem responses include the current request trace identifier:

```json
{
  "traceId": "..."
}
```

This can be used to correlate client-side errors with server logs and diagnostics.

## Dependency

This package depends on:

```text
Behrouzan.Results
```

The core package remains independent from ASP.NET Core and HTTP concerns.

## License

Licensed under the MIT License.