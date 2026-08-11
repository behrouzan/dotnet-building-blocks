# Result

`Result` represents the outcome of an operation that does not need to return a value.

An operation can either succeed or fail.

## Success

Create a successful result using:

```csharp
return Result.Success();
```

A successful result has:

```text
IsSuccess = true
IsFailure = false
Errors = empty
```

A successful result cannot contain errors.

## Failure

A failed result must contain at least one `Error`.

Example:

```csharp
return Result.Failure(
    Error.NotFound(
        "Product.NotFound",
        "Product was not found."));
```

A failed result has:

```text
IsSuccess = false
IsFailure = true
Errors = one or more errors
```

## Multiple Errors

A result can contain multiple errors.

This is particularly useful for validation.

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

## Multiple Errors for the Same Property

A single property can have more than one error.

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

Clients can therefore display multiple validation messages for the same input.

## Global Errors

An error does not have to be associated with a property.

```csharp
return Result.Failure(
    Error.Conflict(
        "Order.InvalidState",
        "The order cannot be modified in its current state."));
```

In this case, `PropertyPath` is `null`.

## Accessing Errors

All errors are available through:

```csharp
result.Errors
```

The first error can be accessed through:

```csharp
result.FirstError
```

`FirstError` is `null` when the result is successful.

## Valid States

`Result` protects itself from invalid states.

The following combinations are not allowed:

```text
Success + one or more errors
Failure + zero errors
```

Therefore:

```text
Success
└── Errors = empty

Failure
└── Errors = one or more
```

## Example

```csharp
public Result DeleteProduct(int id)
{
    var product = FindProduct(id);

    if (product is null)
    {
        return Result.Failure(
            Error.NotFound(
                "Product.NotFound",
                "Product was not found."));
    }

    Delete(product);

    return Result.Success();
}
```

## When to Use Result

Use `Result` when an operation does not need to return a value.

Typical examples include:

```text
DeleteProduct
SendEmail
CancelOrder
ActivateUser
DeactivateUser
PublishArticle
```

## Expected Failures vs Exceptions

`Result` is primarily intended for expected application failures.

Examples:

```text
Validation failure
Resource not found
Business rule violation
Conflict
Unauthorized operation
Forbidden operation
```

Unexpected failures such as programming bugs or unexpected infrastructure exceptions do not automatically become `Result.Failure`.

For example, a database operation may throw an exception if the database becomes unavailable.

Exception handling and conversion to HTTP responses will be handled separately by the ASP.NET Core integration layer.