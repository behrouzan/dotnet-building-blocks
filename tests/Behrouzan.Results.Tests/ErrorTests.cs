using Behrouzan.Results;

namespace Behrouzan.Results.Tests;

public class ErrorTests
{
    [Fact]
    public void Constructor_ShouldCreateError_WithProvidedValues()
    {
        var metadata = new Dictionary<string, object?>
        {
            ["minimum"] = 1
        };

        var error = new Error(
            "Product.Quantity.Invalid",
            "Quantity is invalid.",
            ErrorType.Validation,
            "quantity",
            ErrorSeverity.Warning,
            metadata);

        Assert.Equal("Product.Quantity.Invalid", error.Code);
        Assert.Equal("Quantity is invalid.", error.Message);
        Assert.Equal(ErrorType.Validation, error.Type);
        Assert.Equal("quantity", error.PropertyPath);
        Assert.Equal(ErrorSeverity.Warning, error.Severity);
        Assert.Equal(1, error.Metadata["minimum"]);
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenCodeIsEmpty()
    {
        Assert.Throws<ArgumentException>(() =>
            new Error(
                "",
                "Something went wrong."));
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenMessageIsEmpty()
    {
        Assert.Throws<ArgumentException>(() =>
            new Error(
                "General.Error",
                ""));
    }

    [Fact]
    public void Validation_ShouldCreateValidationError()
    {
        var error = Error.Validation(
            "User.Email.Invalid",
            "Email is invalid.",
            "email");

        Assert.Equal(ErrorType.Validation, error.Type);
        Assert.Equal("User.Email.Invalid", error.Code);
        Assert.Equal("Email is invalid.", error.Message);
        Assert.Equal("email", error.PropertyPath);
        Assert.Equal(ErrorSeverity.Error, error.Severity);
        Assert.Empty(error.Metadata);
    }

    [Fact]
    public void NotFound_ShouldCreateNotFoundError()
    {
        var error = Error.NotFound(
            "Product.NotFound",
            "Product was not found.");

        Assert.Equal(ErrorType.NotFound, error.Type);
        Assert.Null(error.PropertyPath);
    }

    [Fact]
    public void Conflict_ShouldCreateConflictError_WithPropertyPath()
    {
        var error = Error.Conflict(
            "User.Email.AlreadyExists",
            "Email already exists.",
            "email");

        Assert.Equal(ErrorType.Conflict, error.Type);
        Assert.Equal("email", error.PropertyPath);
    }

    [Fact]
    public void WithMetadata_ShouldReturnNewError_WithMetadata()
    {
        var original = Error.Validation(
            "Product.Quantity.Invalid",
            "Quantity is invalid.",
            "quantity");

        var modified = original
            .WithMetadata("minimum", 1)
            .WithMetadata("maximum", 10);

        Assert.Empty(original.Metadata);

        Assert.Equal(1, modified.Metadata["minimum"]);
        Assert.Equal(10, modified.Metadata["maximum"]);

        Assert.Equal(original.Code, modified.Code);
        Assert.Equal(original.Message, modified.Message);
        Assert.Equal(original.Type, modified.Type);
        Assert.Equal(original.PropertyPath, modified.PropertyPath);
    }

    [Fact]
    public void WithSeverity_ShouldReturnNewError_WithNewSeverity()
    {
        var original = Error.Validation(
            "User.Email.Invalid",
            "Email is invalid.",
            "email");

        var modified = original.WithSeverity(
            ErrorSeverity.Warning);

        Assert.Equal(ErrorSeverity.Error, original.Severity);
        Assert.Equal(ErrorSeverity.Warning, modified.Severity);

        Assert.Equal(original.Code, modified.Code);
        Assert.Equal(original.Message, modified.Message);
        Assert.Equal(original.Type, modified.Type);
        Assert.Equal(original.PropertyPath, modified.PropertyPath);
    }

    [Fact]
    public void WithMetadata_WithExistingKey_ShouldReplaceValue()
    {
        var error = Error
            .Validation(
                "Product.Quantity.Invalid",
                "Quantity is invalid.",
                "quantity")
            .WithMetadata("minimum", 1);

        var modified = error.WithMetadata("minimum", 5);

        Assert.Equal(1, error.Metadata["minimum"]);
        Assert.Equal(5, modified.Metadata["minimum"]);
    }
}