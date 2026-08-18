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
- `Map`
- `Bind`
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

## Bind

Chain operations that themselves return results:

```csharp
var orderResult = productResult.Bind(
    product => CreateOrder(product));
```

When the original result has failed, the next operation is not executed.

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