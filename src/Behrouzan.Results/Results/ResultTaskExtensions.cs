namespace Behrouzan.Results;

/// <summary>
/// Provides asynchronous composition operations for tasks that produce results.
/// </summary>
public static class ResultTaskExtensions
{
    /// <summary>
    /// Asynchronously transforms the successful value of a result-producing task.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the current result value.
    /// </typeparam>
    /// <typeparam name="TNewValue">
    /// The type of the transformed value.
    /// </typeparam>
    /// <param name="resultTask">
    /// The task that produces the current result.
    /// </param>
    /// <param name="mapper">
    /// The asynchronous function used to transform the successful value.
    /// </param>
    /// <returns>
    /// A task containing the mapped result.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="resultTask"/> or
    /// <paramref name="mapper"/> is <see langword="null"/>.
    /// </exception>
    public static async Task<Result<TNewValue>> MapAsync<T, TNewValue>(
        this Task<Result<T>> resultTask,
        Func<T, Task<TNewValue>> mapper)
    {
        ArgumentNullException.ThrowIfNull(resultTask);
        ArgumentNullException.ThrowIfNull(mapper);

        var result =
            await resultTask;

        return await result.MapAsync(mapper);
    }

    /// <summary>
    /// Asynchronously chains a result-producing task to another asynchronous
    /// operation that returns a result.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the current result value.
    /// </typeparam>
    /// <typeparam name="TNewValue">
    /// The type of value returned by the next operation.
    /// </typeparam>
    /// <param name="resultTask">
    /// The task that produces the current result.
    /// </param>
    /// <param name="binder">
    /// The asynchronous function executed when the result is successful.
    /// </param>
    /// <returns>
    /// A task containing the result of the chained operation.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="resultTask"/> or
    /// <paramref name="binder"/> is <see langword="null"/>.
    /// </exception>
    public static async Task<Result<TNewValue>> BindAsync<T, TNewValue>(
        this Task<Result<T>> resultTask,
        Func<T, Task<Result<TNewValue>>> binder)
    {
        ArgumentNullException.ThrowIfNull(resultTask);
        ArgumentNullException.ThrowIfNull(binder);

        var result =
            await resultTask;

        return await result.BindAsync(binder);
    }

    /// <summary>
    /// Ensures that the successful value produced by a result task
    /// satisfies the specified condition.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the result value.
    /// </typeparam>
    /// <param name="resultTask">
    /// The task that produces the current result.
    /// </param>
    /// <param name="predicate">
    /// The condition that the successful value must satisfy.
    /// </param>
    /// <param name="error">
    /// The error returned when the condition is not satisfied.
    /// </param>
    /// <returns>
    /// A task containing the ensured result.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="resultTask"/>,
    /// <paramref name="predicate"/>, or
    /// <paramref name="error"/> is <see langword="null"/>.
    /// </exception>
    public static async Task<Result<T>> Ensure<T>(
        this Task<Result<T>> resultTask,
        Func<T, bool> predicate,
        Error error)
    {
        ArgumentNullException.ThrowIfNull(resultTask);
        ArgumentNullException.ThrowIfNull(predicate);
        ArgumentNullException.ThrowIfNull(error);

        var result =
            await resultTask;

        return result.Ensure(
            predicate,
            error);
    }

    /// <summary>
    /// Transforms the successful value produced by a result task.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the current result value.
    /// </typeparam>
    /// <typeparam name="TNewValue">
    /// The type of the transformed value.
    /// </typeparam>
    /// <param name="resultTask">
    /// The task that produces the current result.
    /// </param>
    /// <param name="mapper">
    /// The function used to transform the successful value.
    /// </param>
    /// <returns>
    /// A task containing the mapped result.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="resultTask"/> or
    /// <paramref name="mapper"/> is <see langword="null"/>.
    /// </exception>
    public static async Task<Result<TNewValue>> Map<T, TNewValue>(
        this Task<Result<T>> resultTask,
        Func<T, TNewValue> mapper)
    {
        ArgumentNullException.ThrowIfNull(resultTask);
        ArgumentNullException.ThrowIfNull(mapper);

        var result =
            await resultTask;

        return result.Map(mapper);
    }

    /// <summary>
    /// Chains a result-producing task to another operation that returns a result.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the current result value.
    /// </typeparam>
    /// <typeparam name="TNewValue">
    /// The type of value returned by the next operation.
    /// </typeparam>
    /// <param name="resultTask">
    /// The task that produces the current result.
    /// </param>
    /// <param name="binder">
    /// The function executed when the result is successful.
    /// </param>
    /// <returns>
    /// A task containing the result of the chained operation.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="resultTask"/> or
    /// <paramref name="binder"/> is <see langword="null"/>.
    /// </exception>
    public static async Task<Result<TNewValue>> Bind<T, TNewValue>(
        this Task<Result<T>> resultTask,
        Func<T, Result<TNewValue>> binder)
    {
        ArgumentNullException.ThrowIfNull(resultTask);
        ArgumentNullException.ThrowIfNull(binder);

        var result =
            await resultTask;

        return result.Bind(binder);
    }

    /// <summary>
    /// Executes an action using the successful value produced by a result task
    /// without changing the result.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the result value.
    /// </typeparam>
    /// <param name="resultTask">
    /// The task that produces the current result.
    /// </param>
    /// <param name="action">
    /// The action to execute when the result is successful.
    /// </param>
    /// <returns>
    /// A task containing the original result unchanged.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="resultTask"/> or
    /// <paramref name="action"/> is <see langword="null"/>.
    /// </exception>
    public static async Task<Result<T>> Tap<T>(
        this Task<Result<T>> resultTask,
        Action<T> action)
    {
        ArgumentNullException.ThrowIfNull(resultTask);
        ArgumentNullException.ThrowIfNull(action);

        var result =
            await resultTask;

        return result.Tap(action);
    }

    /// <summary>
    /// Asynchronously executes an action using the successful value produced
    /// by a result task without changing the result.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the result value.
    /// </typeparam>
    /// <param name="resultTask">
    /// The task that produces the current result.
    /// </param>
    /// <param name="action">
    /// The asynchronous action to execute when the result is successful.
    /// </param>
    /// <returns>
    /// A task containing the original result unchanged.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="resultTask"/> or
    /// <paramref name="action"/> is <see langword="null"/>.
    /// </exception>
    public static async Task<Result<T>> TapAsync<T>(
        this Task<Result<T>> resultTask,
        Func<T, Task> action)
    {
        ArgumentNullException.ThrowIfNull(resultTask);
        ArgumentNullException.ThrowIfNull(action);

        var result =
            await resultTask;

        return await result.TapAsync(action);
    }
}