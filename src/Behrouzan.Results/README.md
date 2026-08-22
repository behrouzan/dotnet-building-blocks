# Behrouzan.Results

A lightweight, framework-independent Result pattern library for .NET.

`Behrouzan.Results` provides structured representations of successful operations and expected application failures without coupling application logic to HTTP, ASP.NET Core, or frontend frameworks.

## Installation

```bash
dotnet add package Behrouzan.Results
```

## Features

- `Result`
- `Result<T>`
- Structured `Error`
- Multiple errors
- Multiple validation errors
- Property-specific errors
- Nested property paths
- Machine-readable error codes
- Error types
- Custom metadata
- Safe value access
- `Match`
- `Map` and `MapAsync`
- `Bind` and `BindAsync`
- Async composition over `Task<Result<T>>`
- `Ensure`
- `Tap` and `TapAsync`
- `Combine`

## Basic Usage

### Success without a value

```csharp
Result result = Result.Success();
```

### Failure without a value

```csharp
Result result = Result.Failure(
    Error.NotFound(
        "Product.NotFound",
        "Product was not found."));
```

### Success with a value

```csharp
Result<Product> result =
    Result<Product>.Success(product);
```

### Failure with a value

```csharp
Result<Product> result =
    Result<Product>.Failure(
        Error.NotFound(
            "Product.NotFound",
            "Product was not found."));
```

## Validation Errors

Multiple validation errors can be returned together:

```csharp
return Result.Failure(
    Error.Validation(
        "User.Email.Invalid",
        "Email is invalid.",
        "email"),

    Error.Validation(
        "User.Password.TooShort",
        "Password must contain at least 8 characters.",
        "password"));
```

`PropertyPath` can identify simple or nested inputs:

```text
email
address.postalCode
items[2].quantity
```

This allows API clients to associate errors with their own form fields or UI elements.

## Error Types

Built-in error types include:

- `Failure`
- `Validation`
- `NotFound`
- `Conflict`
- `Unauthorized`
- `Forbidden`
- `Unavailable`
- `Timeout`
- `RateLimit`

Example:

```csharp
var error = Error.Conflict(
    "Order.AlreadyProcessed",
    "The order has already been processed.");
```

## Metadata

Errors can contain additional machine-readable metadata:

```csharp
var error = Error
    .Validation(
        "Product.Quantity.Invalid",
        "The requested quantity is invalid.",
        "quantity")
    .WithMetadata("minimum", 1)
    .WithMetadata("maximum", 10);
```

## Match

Handle success and failure explicitly:

```csharp
var message = result.Match(
    value => $"Product: {value.Name}",
    errors => $"Operation failed with {errors.Count} error(s).");
```

## Map

Transform the value of a successful result:

```csharp
var dtoResult = productResult.Map(
    product => new ProductDto(
        product.Id,
        product.Name));
```

When the original result has failed, the mapper is not executed and the existing errors are propagated.

## MapAsync

Asynchronously transform the value of a successful result:

```csharp
var dtoResult = await productResult.MapAsync(
    async product =>
    {
        var category =
            await categoryService.GetAsync(product.CategoryId);

        return new ProductDto(
            product.Id,
            product.Name,
            category.Name);
    });
```

When the original result has failed, the mapper is not executed and the existing errors are propagated.

## Bind

Chain operations that themselves return results:

```csharp
var orderResult = productResult.Bind(
    product => CreateOrder(product));
```

When the original result has failed, the next operation is not executed.

## BindAsync

Chain asynchronous operations that themselves return results:

```csharp
var orderResult = await productResult.BindAsync(
    async product =>
        await CreateOrderAsync(product));
```

When the original result has failed, the next operation is not executed and the existing errors are propagated.

## Async Composition

Operations can also be chained directly from `Task<Result<T>>`.

This avoids repeatedly awaiting intermediate results:

```csharp
var result = await GetProductAsync(id)
    .Ensure(
        product => product.Stock > 0,
        Error.Conflict(
            "Product.OutOfStock",
            "Product is out of stock."))
    .Map(product =>
        $"{product.Name} is available.");
```

Both synchronous and asynchronous operations can participate in an asynchronous chain:

```csharp
var result = await GetProductAsync(id)
    .BindAsync(async product =>
        await CreateOrderAsync(product))
    .Map(order =>
        order.Id);
```

## Ensure

Validate a successful value without leaving the result pipeline:

```csharp
var result = productResult.Ensure(
    product => product.Stock > 0,
    Error.Conflict(
        "Product.OutOfStock",
        "Product is out of stock."));
```

If the result has already failed, the predicate is not evaluated.

If the predicate returns `false`, the result becomes a failure containing the specified error.

## Tap

Execute a side effect for a successful result without changing its value:

```csharp
var result = productResult
    .Tap(product =>
        logger.LogInformation(
            "Processing product {ProductId}.",
            product.Id))
    .Map(product =>
        product.Name);
```

`Tap` is useful for side effects such as logging, metrics, telemetry, and other operations that should not transform the result value.

The action is not executed when the result has failed.

## TapAsync

Asynchronous side effects are supported with `TapAsync`:

```csharp
var result = await GetProductAsync(id)
    .TapAsync(async product =>
        await auditService.RecordAsync(product.Id))
    .Map(product =>
        product.Name);
```

Exceptions thrown by `Tap` or `TapAsync` actions are not converted into result failures. They propagate normally to the caller.

## Combine

Multiple non-generic results can be combined:

```csharp
var result = Result.Combine(
    validateName,
    validateEmail,
    validatePassword);
```

The combined result succeeds when all results succeed.

If one or more results fail, their errors are collected into the resulting failure.

## Framework Independence

`Behrouzan.Results` has no dependency on:

- ASP.NET Core
- HTTP
- FluentValidation
- Angular
- React
- Next.js
- Flutter

It can be used in application services, domain logic, worker services, desktop applications, APIs, and other .NET projects.

For ASP.NET Core HTTP integration, use `Behrouzan.Results.AspNetCore`.

## License

Licensed under the MIT License.
