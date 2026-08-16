namespace Behrouzan.BuildingBlocks.Core.Results;

/// <summary>
/// Represents a structured application error containing a machine-readable code,
/// human-readable message, semantic type, optional property path, severity,
/// and additional metadata.
/// </summary>
public sealed record Error
{
    /// <summary>
    /// Gets the machine-readable identifier of the error.
    /// </summary>
    public string Code { get; }

    /// <summary>
    /// Gets the human-readable description of the error.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Gets the semantic category of the error.
    /// </summary>
    public ErrorType Type { get; }

    /// <summary>
    /// Gets the optional path of the property or input associated with the error.
    /// </summary>
    /// <remarks>
    /// Examples include <c>email</c>, <c>address.postalCode</c>,
    /// and <c>items[2].quantity</c>.
    /// </remarks>
    public string? PropertyPath { get; }

    /// <summary>
    /// Gets the severity level of the error.
    /// </summary>
    public ErrorSeverity Severity { get; }

    /// <summary>
    /// Gets additional machine-readable information associated with the error.
    /// </summary>
    public IReadOnlyDictionary<string, object?> Metadata { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Error"/> type.
    /// </summary>
    /// <param name="code">The machine-readable error code.</param>
    /// <param name="message">The human-readable error message.</param>
    /// <param name="type">The semantic category of the error.</param>
    /// <param name="propertyPath">
    /// The optional property or input path associated with the error.
    /// </param>
    /// <param name="severity">The severity level of the error.</param>
    /// <param name="metadata">
    /// Optional additional machine-readable information associated with the error.
    /// </param>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="code"/> or <paramref name="message"/> is null,
    /// empty, or consists only of white-space characters.
    /// </exception>
    public Error(
        string code,
        string message,
        ErrorType type = ErrorType.Failure,
        string? propertyPath = null,
        ErrorSeverity severity = ErrorSeverity.Error,
        IReadOnlyDictionary<string, object?>? metadata = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        ArgumentException.ThrowIfNullOrWhiteSpace(message);

        Code = code;
        Message = message;
        Type = type;
        PropertyPath = propertyPath;
        Severity = severity;

        Metadata = metadata is null
            ? new Dictionary<string, object?>()
            : new Dictionary<string, object?>(metadata);
    }

    /// <summary>
    /// Creates a general failure error.
    /// </summary>
    /// <param name="code">The machine-readable error code.</param>
    /// <param name="message">The human-readable error message.</param>
    /// <param name="propertyPath">
    /// The optional property or input path associated with the error.
    /// </param>
    /// <returns>A failure <see cref="Error"/>.</returns>
    public static Error Failure(
        string code,
        string message,
        string? propertyPath = null) =>
        new(
            code,
            message,
            ErrorType.Failure,
            propertyPath);

    /// <summary>
    /// Creates a validation error.
    /// </summary>
    /// <param name="code">The machine-readable error code.</param>
    /// <param name="message">The human-readable error message.</param>
    /// <param name="propertyPath">
    /// The optional property or input path associated with the validation error.
    /// </param>
    /// <returns>A validation <see cref="Error"/>.</returns>
    public static Error Validation(
        string code,
        string message,
        string? propertyPath = null) =>
        new(
            code,
            message,
            ErrorType.Validation,
            propertyPath);

    /// <summary>
    /// Creates a not-found error.
    /// </summary>
    /// <param name="code">The machine-readable error code.</param>
    /// <param name="message">The human-readable error message.</param>
    /// <param name="propertyPath">
    /// The optional property or input path associated with the error.
    /// </param>
    /// <returns>A not-found <see cref="Error"/>.</returns>
    public static Error NotFound(
        string code,
        string message,
        string? propertyPath = null) =>
        new(
            code,
            message,
            ErrorType.NotFound,
            propertyPath);

    /// <summary>
    /// Creates a conflict error.
    /// </summary>
    /// <param name="code">The machine-readable error code.</param>
    /// <param name="message">The human-readable error message.</param>
    /// <param name="propertyPath">
    /// The optional property or input path associated with the conflict.
    /// </param>
    /// <returns>A conflict <see cref="Error"/>.</returns>
    public static Error Conflict(
        string code,
        string message,
        string? propertyPath = null) =>
        new(
            code,
            message,
            ErrorType.Conflict,
            propertyPath);

    /// <summary>
    /// Creates an unauthorized error.
    /// </summary>
    /// <param name="code">The machine-readable error code.</param>
    /// <param name="message">The human-readable error message.</param>
    /// <returns>An unauthorized <see cref="Error"/>.</returns>
    public static Error Unauthorized(
        string code,
        string message) =>
        new(
            code,
            message,
            ErrorType.Unauthorized);

    /// <summary>
    /// Creates a forbidden error.
    /// </summary>
    /// <param name="code">The machine-readable error code.</param>
    /// <param name="message">The human-readable error message.</param>
    /// <returns>A forbidden <see cref="Error"/>.</returns>
    public static Error Forbidden(
        string code,
        string message) =>
        new(
            code,
            message,
            ErrorType.Forbidden);

    /// <summary>
    /// Creates an unavailable error.
    /// </summary>
    /// <param name="code">The machine-readable error code.</param>
    /// <param name="message">The human-readable error message.</param>
    /// <returns>An unavailable <see cref="Error"/>.</returns>
    public static Error Unavailable(
        string code,
        string message) =>
        new(
            code,
            message,
            ErrorType.Unavailable);

    /// <summary>
    /// Creates a timeout error.
    /// </summary>
    /// <param name="code">The machine-readable error code.</param>
    /// <param name="message">The human-readable error message.</param>
    /// <returns>A timeout <see cref="Error"/>.</returns>
    public static Error Timeout(
        string code,
        string message) =>
        new(
            code,
            message,
            ErrorType.Timeout);

    /// <summary>
    /// Creates a rate-limit error.
    /// </summary>
    /// <param name="code">The machine-readable error code.</param>
    /// <param name="message">The human-readable error message.</param>
    /// <returns>A rate-limit <see cref="Error"/>.</returns>
    public static Error RateLimit(
        string code,
        string message) =>
        new(
            code,
            message,
            ErrorType.RateLimit);

    /// <summary>
    /// Returns a new error containing the specified metadata entry.
    /// </summary>
    /// <param name="key">The metadata key.</param>
    /// <param name="value">The metadata value.</param>
    /// <returns>
    /// A new <see cref="Error"/> containing the existing metadata and the new entry.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="key"/> is null, empty, or consists only of
    /// white-space characters.
    /// </exception>
    public Error WithMetadata(
        string key,
        object? value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        var metadata = new Dictionary<string, object?>(Metadata)
        {
            [key] = value
        };

        return new Error(
            Code,
            Message,
            Type,
            PropertyPath,
            Severity,
            metadata);
    }

    /// <summary>
    /// Returns a new error with the specified severity.
    /// </summary>
    /// <param name="severity">The new severity level.</param>
    /// <returns>
    /// A new <see cref="Error"/> with the specified severity.
    /// </returns>
    public Error WithSeverity(ErrorSeverity severity) =>
        new(
            Code,
            Message,
            Type,
            PropertyPath,
            severity,
            Metadata);
}