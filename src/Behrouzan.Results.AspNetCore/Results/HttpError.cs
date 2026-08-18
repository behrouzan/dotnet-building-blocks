namespace Behrouzan.Results.AspNetCore;

/// <summary>
/// Represents an application error in an HTTP response.
/// </summary>
internal sealed record HttpError(
    string Code,
    string Message,
    string Type,
    string? PropertyPath,
    IReadOnlyDictionary<string, object?> Metadata);