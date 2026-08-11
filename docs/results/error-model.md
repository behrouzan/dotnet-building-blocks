# Error Model

`Error` represents a structured application error.

It is designed to be independent of ASP.NET Core, Angular, React, Next.js, Flutter, or any other UI/framework.

## Properties

### Code

A machine-readable identifier for the error.

Example:

```text
User.Email.AlreadyExists
```

Use `Code` for:

- Client-side logic
- Localization
- Logging
- Analytics
- Error handling

### Message

A human-readable description of the error.

Example:

```text
This email is already registered.
```

### Type

Defines the semantic category of the error.

Supported values:

```text
Failure
Validation
NotFound
Conflict
Unauthorized
Forbidden
Unavailable
Timeout
RateLimit
```

Example:

```csharp
ErrorType.Validation
```

### PropertyPath

An optional path to the input or property associated with the error.

Examples:

```text
email
address.postalCode
items[2].quantity
```

If `PropertyPath` is `null`, the error is not associated with a specific field.

This allows different clients such as Angular, React, Next.js, Flutter, or other applications to map server errors to their own input fields without coupling the Core library to a specific UI framework.

### Severity

Represents the importance level of the error.

Supported values:

```text
Error
Warning
Info
```

The default severity is `Error`.

Example:

```csharp
var error = Error
    .Failure(
        "Profile.Incomplete",
        "Your profile is incomplete.")
    .WithSeverity(ErrorSeverity.Warning);
```

### Metadata

Contains optional additional structured information related to the error.

Example:

```csharp
var error = Error
    .Validation(
        "Product.Quantity.Invalid",
        "Quantity must be between 1 and 10.",
        "quantity")
    .WithMetadata("minimum", 1)
    .WithMetadata("maximum", 10)
    .WithMetadata("attemptedValue", 25);
```

Metadata is useful for additional machine-readable information that does not justify adding another dedicated property to `Error`.

## Creating Errors

Factory methods provide a simple and readable way to create common error types.

### Validation

```csharp
var error = Error.Validation(
    "User.Email.Invalid",
    "Email address is invalid.",
    "email");
```

### Not Found

```csharp
var error = Error.NotFound(
    "Product.NotFound",
    "Product was not found.");
```

### Conflict

```csharp
var error = Error.Conflict(
    "User.Email.AlreadyExists",
    "This email is already registered.",
    "email");
```

### Unauthorized

```csharp
var error = Error.Unauthorized(
    "Authentication.Required",
    "Authentication is required.");
```

### Forbidden

```csharp
var error = Error.Forbidden(
    "Order.AccessDenied",
    "You are not allowed to access this order.");
```

### Unavailable

```csharp
var error = Error.Unavailable(
    "Payment.Unavailable",
    "The payment service is currently unavailable.");
```

### Timeout

```csharp
var error = Error.Timeout(
    "Operation.Timeout",
    "The operation timed out.");
```

### Rate Limit

```csharp
var error = Error.RateLimit(
    "Requests.RateLimit",
    "Too many requests.");
```

## Immutability

`Error` is designed to behave as an immutable value.

Methods such as:

```csharp
WithMetadata(...)
WithSeverity(...)
```

return a new `Error` rather than modifying the existing error.

Example:

```csharp
var original = Error.Validation(
    "Product.Quantity.Invalid",
    "Quantity is invalid.",
    "quantity");

var detailed = original
    .WithMetadata("minimum", 1)
    .WithMetadata("maximum", 10);
```

`original` remains unchanged.

## Framework Independence

The Core error model has no dependency on:

- ASP.NET Core
- HTTP
- Angular
- React
- Next.js
- Flutter
- FluentValidation

Framework-specific conversions and integrations should be implemented separately.

For example, mapping an `Error` to an HTTP response will belong to the ASP.NET Core integration layer rather than the Core package.