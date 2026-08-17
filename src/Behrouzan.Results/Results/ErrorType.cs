namespace Behrouzan.Results;

/// <summary>
/// Defines the semantic category of an application error.
/// </summary>
public enum ErrorType
{
    /// <summary>
    /// Represents a general application failure that does not belong to a more specific category.
    /// </summary>
    Failure = 0,

    /// <summary>
    /// Represents a validation failure caused by invalid input or data.
    /// </summary>
    Validation = 1,

    /// <summary>
    /// Represents a failure caused by a requested resource not being found.
    /// </summary>
    NotFound = 2,

    /// <summary>
    /// Represents a conflict with the current state of a resource or operation.
    /// </summary>
    Conflict = 3,

    /// <summary>
    /// Represents a failure caused by missing or invalid authentication.
    /// </summary>
    Unauthorized = 4,

    /// <summary>
    /// Represents a failure caused by insufficient permission to perform an operation.
    /// </summary>
    Forbidden = 5,

    /// <summary>
    /// Represents a failure caused by a required resource or service being temporarily unavailable.
    /// </summary>
    Unavailable = 6,

    /// <summary>
    /// Represents a failure caused by an operation exceeding its allowed time.
    /// </summary>
    Timeout = 7,

    /// <summary>
    /// Represents a failure caused by exceeding an allowed request or operation rate.
    /// </summary>
    RateLimit = 8
}