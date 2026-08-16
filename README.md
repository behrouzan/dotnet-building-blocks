# dotnet-building-blocks# Behzad Building Blocks

Reusable, framework-independent building blocks for .NET applications.

The project is designed to provide common application primitives that can be reused across different projects instead of implementing the same infrastructure repeatedly.

## Current Features

### Result & Error

A structured way to represent successful operations and expected application failures.

Features currently include:

- `Result`
- `Result<T>`
- Structured `Error`
- Multiple errors
- Multiple validation errors
- Field/property-specific errors
- Nested property paths
- Error codes
- Error types
- Error severity
- Custom metadata
- Safe value access
- Factory methods

## Quick Start

### Successful operation without a value

```csharp
return Result.Success();
```

### Failed operation

```csharp
return Result.Failure(
    Error.NotFound(
        "Product.NotFound",
        "Product was not found."));
```

### Successful operation with a value

```csharp
return Result<ProductDto>.Success(productDto);
```

### Failed operation with a value type

```csharp
return Result<ProductDto>.Failure(
    Error.NotFound(
        "Product.NotFound",
        "Product was not found."));
```

## Validation

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

Multiple errors may also target the same property.

```csharp
return Result.Failure(
    Error.Validation(
        "User.Password.TooShort",
        "Password must contain at least 8 characters.",
        "password"),

    Error.Validation(
        "User.Password.RequiresDigit",
        "Password must contain at least one digit.",
        "password"));
```

## Client Independence

The Core library does not depend on any frontend or application framework.

Errors can be consumed by:

- Angular
- React
- Next.js
- Flutter
- .NET MAUI
- Desktop applications
- Worker services
- Other API clients

For example, `PropertyPath` can identify the input associated with an error:

```text
email
address.postalCode
items[2].quantity
```

Each client can decide how to map these paths to its own form controls or UI.

## Framework Independence

`Behrouzan.BuildingBlocks.Core` does not depend on:

- ASP.NET Core
- HTTP
- FluentValidation
- Angular
- React
- Next.js
- Flutter

Framework-specific integrations will be implemented separately.

## Documentation

Detailed documentation is available in:

- [`docs/error-model.md`](docs/error-model.md)
- [`docs/result.md`](docs/result.md)
- [`docs/result-of-t.md`](docs/result-of-t.md)

## Project Structure

```text
src/
  Behrouzan.BuildingBlocks.Core/

samples/
  Sample.Api/

tests/
  Behrouzan.BuildingBlocks.Core.Tests/

docs/
```

## Roadmap

Planned Result features include:

- Unit tests
- `Match`
- `Map`
- `Bind`
- `Ensure`
- `Tap`
- `Combine`

Future integrations may include:

- ASP.NET Core
- Problem Details
- Global exception handling
- FluentValidation

## Status

This project is currently under active development and its public API may change before the first stable release.

## License

Licensed under the MIT License.