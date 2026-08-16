namespace Behrouzan.BuildingBlocks.AspNetCore.Results;

/// <summary>
/// Represents an application error in an HTTP response.
/// </summary>
public sealed record HttpError(
    string Code,
    string Message,
    string Type,
    string? PropertyPath,
    string Severity,
    IReadOnlyDictionary<string, object?> Metadata);