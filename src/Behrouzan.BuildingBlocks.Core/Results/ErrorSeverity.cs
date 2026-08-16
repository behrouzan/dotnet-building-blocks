namespace Behrouzan.BuildingBlocks.Core.Results;

/// <summary>
/// Defines the severity level of an application error.
/// </summary>
public enum ErrorSeverity
{
    /// <summary>
    /// Represents an error that indicates a failed or invalid operation.
    /// </summary>
    Error = 0,

    /// <summary>
    /// Represents a warning that may require attention but is not necessarily fatal.
    /// </summary>
    Warning = 1,

    /// <summary>
    /// Represents informational feedback associated with an operation.
    /// </summary>
    Info = 2
}