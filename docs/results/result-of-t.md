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

## Match

`Match` provides a concise way to handle both successful and failed results.

Instead of manually checking `IsSuccess` or `IsFailure`, two functions can be provided:

- `onSuccess` is executed when the result is successful.
- `onFailure` is executed when the result has failed.

Example:

```csharp
var result = Result<string>.Success("Product");

var output = result.Match(
    onSuccess: value => $"Success: {value}",
    onFailure: errors => "Failed");
```

The result is:

```text
Success: Product
```

For a failed result:

```csharp
var result = Result<string>.Failure(
    Error.NotFound(
        "Product.NotFound",
        "Product was not found."));

var output = result.Match(
    onSuccess: value => $"Success: {value}",
    onFailure: errors => $"Failed: {errors[0].Code}");
```

The result is:

```text
Failed: Product.NotFound
```

`Match` returns the value produced by the selected function.

Both functions must return the same result type.

For example:

```csharp
string text = result.Match(
    onSuccess: value => value,
    onFailure: errors => errors[0].Message);
```

Passing `null` as either function is not allowed and throws an `ArgumentNullException`.


## Map

`Map` transforms the value of a successful `Result<T>` into another value while preserving failures.

Conceptually:

```text
Result<T>
   |
   | Map(T -> TNewValue)
   v
Result<TNewValue>
```

If the result is successful, the mapper function is executed.

Example:

```csharp
var result = Result<string>.Success("Product");

var mapped = result.Map(
    value => value.Length);
```

The result is:

```text
Result<int>
IsSuccess = true
Value = 7
```

`Map` is especially useful when converting one successful value into another type.

For example:

```csharp
Result<Product> result = GetProduct();

var dtoResult = result.Map(product =>
    new ProductDto
    {
        Id = product.Id,
        Name = product.Name
    });
```

This transforms:

```text
Result<Product>
```

into:

```text
Result<ProductDto>
```

If the original result is a failure, the mapper is not executed and the existing errors are preserved.

```csharp
var result = Result<string>.Failure(
    Error.NotFound(
        "Product.NotFound",
        "Product was not found."));

var mapped = result.Map(
    value => value.Length);
```

The resulting `Result<int>` remains a failure and contains the same errors.

Passing a `null` mapper is not allowed and throws an `ArgumentNullException`.

## MapAsync

`MapAsync` is the asynchronous counterpart of `Map`.

It transforms the value of a successful `Result<T>` using an asynchronous operation.

Conceptually:

```text
Result<T>
   |
   | MapAsync(T -> Task<TNewValue>)
   v
Task<Result<TNewValue>>
```

Example:

```csharp
Result<Product> result = GetProduct();

var dtoResult = await result.MapAsync(
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

If the original result is a failure, the asynchronous mapper is not executed and the existing errors are preserved.

Passing a `null` mapper is not allowed and throws an `ArgumentNullException`.

## Bind

`Bind` chains a successful `Result<T>` to another operation that itself returns a `Result`.

Conceptually:

```text
Result<T>
   |
   | Bind(T -> Result<TNewValue>)
   v
Result<TNewValue>
```

Unlike `Map`, which transforms a value directly, `Bind` is used when the next operation can also succeed or fail.

Example:

```csharp
Result<User> userResult = GetUser(userId);

Result<Order> orderResult = userResult.Bind(
    user => CreateOrder(user));
```

If `userResult` is successful, `CreateOrder` is executed with the returned user.

```text
GetUser()
   ↓
Success<User>
   ↓
CreateOrder(user)
   ↓
Result<Order>
```

If `userResult` has failed, `CreateOrder` is not executed and the existing errors are preserved.

```text
GetUser()
   ↓
Failure<User>
   ↓
CreateOrder is skipped
   ↓
Failure<Order>
```

This avoids manually checking the result between operations:

```csharp
if (userResult.IsFailure)
{
    return Result<Order>.Failure(userResult.Errors);
}

return CreateOrder(userResult.Value);
```

The same operation can instead be written as:

```csharp
return userResult.Bind(
    user => CreateOrder(user));
```

Passing a `null` binder is not allowed and throws an `ArgumentNullException`.

## BindAsync

`BindAsync` is the asynchronous counterpart of `Bind`.

It chains a successful `Result<T>` to an asynchronous operation that itself returns a result.

Conceptually:

```text
Result<T>
   |
   | BindAsync(T -> Task<Result<TNewValue>>)
   v
Task<Result<TNewValue>>
```

Example:

```csharp
Result<Product> result = GetProduct();

var orderResult = await result.BindAsync(
    async product =>
        await CreateOrderAsync(product));
```

If the original result is successful, the asynchronous binder is executed.

If the original result is a failure, the binder is not executed and the existing errors are preserved.

Passing a `null` binder is not allowed and throws an `ArgumentNullException`.

If the binder returns `null`, an `InvalidOperationException` is thrown.

## Ensure

`Ensure` validates the value of a successful `Result<T>` using a predicate.

It is useful when an operation has already succeeded, but the returned value must satisfy an additional condition before the pipeline can continue.

Conceptually:

```text
Result<T>
   |
   | Ensure(T -> bool)
   |
   ├── true  -> original Result<T>
   |
   └── false -> Failure<T>
```

Example:

```csharp
var result = productResult.Ensure(
    product => product.Stock > 0,
    Error.Conflict(
        "Product.OutOfStock",
        "Product is out of stock."));
```

If the original result is successful and the predicate returns `true`, the original successful result is returned unchanged.

If the predicate returns `false`, a failed `Result<T>` containing the specified error is returned.

If the original result has already failed, the predicate is not executed and the existing errors are preserved.

Passing a `null` predicate or error is not allowed and throws an `ArgumentNullException`.

## Tap

`Tap` executes a side effect using the value of a successful `Result<T>` without changing the result itself.

It is useful for operations such as logging, metrics, telemetry, or other side effects that should not transform the result value.

Conceptually:

```text
Result<T>
   |
   | Tap(T -> void)
   v
Result<T>
```

Example:

```csharp
var result = productResult.Tap(
    product =>
        logger.LogInformation(
            "Processing product {ProductId}.",
            product.Id));
```

If the result is successful, the action is executed and the original result is returned unchanged.

If the result has failed, the action is not executed and the existing failure is returned unchanged.

Passing a `null` action is not allowed and throws an `ArgumentNullException`.

Exceptions thrown by the action are not converted into result failures. They propagate normally to the caller.

## TapAsync

`TapAsync` is the asynchronous counterpart of `Tap`.

It executes an asynchronous side effect without changing the successful result value.

Conceptually:

```text
Result<T>
   |
   | TapAsync(T -> Task)
   v
Task<Result<T>>
```

Example:

```csharp
var result = await productResult.TapAsync(
    async product =>
        await auditService.RecordAsync(product.Id));
```

If the result is successful, the asynchronous action is executed and the original result is returned unchanged.

If the result has failed, the action is not executed and the existing failure is preserved.

Passing a `null` action is not allowed and throws an `ArgumentNullException`.

Exceptions thrown by the asynchronous action are not converted into result failures. They propagate normally to the caller.

## Async Composition

`Task<Result<T>>` can be composed directly using result operations without manually awaiting every intermediate step.

This is useful when the operation that starts a result pipeline is asynchronous.

Instead of writing:

```csharp
var result = await GetProductAsync(id);

if (result.IsFailure)
{
    return Result<string>.Failure(result.Errors);
}

var ensured = result.Ensure(
    product => product.Stock > 0,
    Error.Conflict(
        "Product.OutOfStock",
        "Product is out of stock."));

if (ensured.IsFailure)
{
    return Result<string>.Failure(ensured.Errors);
}

return ensured.Map(
    product =>
        $"{product.Name} is available.");
```

the same flow can be composed directly:

```csharp
return await GetProductAsync(id)
    .Ensure(
        product => product.Stock > 0,
        Error.Conflict(
            "Product.OutOfStock",
            "Product is out of stock."))
    .Map(
        product =>
            $"{product.Name} is available.");
```

The following operations can be used directly with `Task<Result<T>>`:

- `Map`
- `MapAsync`
- `Bind`
- `BindAsync`
- `Ensure`
- `Tap`
- `TapAsync`

Synchronous and asynchronous operations can be mixed in the same pipeline.

For example:

```csharp
return await GetProductAsync(id)
    .Ensure(
        product => product.Stock > 0,
        Error.Conflict(
            "Product.OutOfStock",
            "Product is out of stock."))
    .Tap(product =>
        logger.LogInformation(
            "Product {ProductId} is available.",
            product.Id))
    .TapAsync(async product =>
        await auditService.RecordAsync(product.Id))
    .Map(
        product =>
            $"{product.Name} is available for ordering.");
```

If any result in the pipeline is a failure, subsequent operations that require a successful value are skipped and the existing errors are propagated.

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