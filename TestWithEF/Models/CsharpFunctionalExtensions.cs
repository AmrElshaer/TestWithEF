#nullable enable
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using TestWithEF.Exceptions;

namespace TestWithEF.Models;

public class Result
{
    public bool Success { get; private set; }

    public Exception Error { get; private set; }

    public bool Failure => !Success;

    protected Result(bool success, Exception error)
    {
        Success = success;
        Error = error;
    }

    public static Result Fail(Exception exception)
    {
        return new Result(false, exception);
    }

    public static Result<T> Fail<T>(Exception ex)
    {
        return new Result<T>(default(T), false, ex);
    }

    public static Result Ok()
    {
        return new Result(true, null);
    }

    public static Result<T> Ok<T>(T value)
    {
        return new Result<T>(value, true, null);
    }
}

public static class ResultExtensions
{
    public static async Task<R> MatchAsync<R, T>(this Task<Result<T>> resultAsync, Func<T, R> succ, Func<Exception, R> fail)
    {
        var result = await resultAsync;

        return result.Failure
            ? fail(result.Error)
            : succ(result.Value);
    }

    // it not map output of func to another output
    public static Result<U> Bind<U, T>(this Result<T> result, Func<T, Result<U>> f) =>
        result.Failure ? result.Error : f(result.Value);

    // it map output of func to another output
    public static async Task<Result<U>> MapAsync<U, T>
        (this Result<T> result, Func<T, Task<U>> f) => result.Failure ? await Task.FromResult(Result.Fail<U>(result.Error)) : await f(result.Value);

    public static async Task<Result<U>> MapAsync<U, T>(this Task<Result<T>> resultAsync, Func<T, U> f)
    {
        var result = await resultAsync;

        return result.Failure ? await Task.FromResult(Result.Fail<U>(result.Error)) : f(result.Value);
    }

    public static Result<U> Map<U, T>(this Result<T> result, Func<T, U> f) => result.Failure ? result.Error : f(result.Value);

    public static Result<T> Apply<A, B, C, D, T>
    (
        this (Result<A>, Result<B>, Result<C>, Result<D>) items,
        Func<A, B, C, D, T> f
    )
    {
        return items.Item1.Bind(a => items.Item2)
            .Bind(b => items.Item3)
            .Bind(c => items.Item4)
            .Map(d => f(items.Item1.Value, items.Item2.Value, items.Item3.Value, items.Item4.Value));
    }

    public static Result<T> Apply<A, B, C, T>
    (
        this (Result<A>, Result<B>, Result<C>) items,
        Func<A, B, C, T> f
    )
    {
        return items.Item1.Bind(a => items.Item2).Bind(b => items.Item3)
            .Map(c => f(items.Item1.Value, items.Item2.Value, items.Item3.Value));
    }

    public static Result<T> Apply<A, B, T>
    (
        this (Result<A>, Result<B>) items,
        Func<A, B, T> f
    )
    {
        return items.Item1.Bind(a => items.Item2)
            .Map(d => f(items.Item1.Value, items.Item2.Value));
    }
}

public class Result<T> : Result
{
    public T Value { get; private set; }

    internal protected Result([AllowNull] T value, bool success, Exception error)
        : base(success, error)
    {
        Value = value;
    }

    public static implicit operator Result<T>(T value) => Ok(value);
    public static implicit operator Result<T>(Exception error) => Fail<T>(error);
}

public static class Validation
{
    public static Result<string> NotEmpty(this string value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        => string.IsNullOrWhiteSpace(value) ? StringException.Empty(paramName) : value;

    public static Result<string> ValidEmail(this string value)
        => Regex.IsMatch(value, @"^(.+)@(.+)$") ? value : EmailException.InValidEmailException();
}

public readonly struct Maybe<T> : IEquatable<Maybe<T>>, IEquatable<object>, IMaybe<T>
{
    private readonly bool _isValueSet;

    private readonly T _value;

    /// <summary>
    /// Returns the inner value if there's one, otherwise throws an InvalidOperationException with <paramref name="errorMessage"/>
    /// </summary>
    /// <exception cref="InvalidOperationException">Maybe has no value.</exception>
    public T GetValueOrThrow(string errorMessage = null)
    {
        if (HasNoValue)
            throw new InvalidOperationException(errorMessage);

        return _value;
    }

    /// <summary>
    /// Returns the inner value if there's one, otherwise throws a custom exception with <paramref name="exception"/>
    /// </summary>
    /// <exception cref="Exception">Maybe has no value.</exception>
    public T GetValueOrThrow(Exception exception)
    {
        if (HasNoValue)
            throw exception;

        return _value;
    }

    public T GetValueOrDefault(T defaultValue = default)
    {
        if (HasNoValue)
            return defaultValue;

        return _value;
    }

    /// <summary>
    /// Try to use GetValueOrThrow() or GetValueOrDefault() instead for better explicitness.
    /// </summary>
    public T Value => GetValueOrThrow();

    public static Maybe<T> None => new Maybe<T>();

    public bool HasValue => _isValueSet;

    public bool HasNoValue => !HasValue;

    private Maybe(T value)
    {
        if (value == null)
        {
            _isValueSet = false;
            _value = default;

            return;
        }

        _isValueSet = true;
        _value = value;
    }

    public static implicit operator Maybe<T>(T value)
    {
        if (value is Maybe<T> m)
        {
            return m;
        }

        return Maybe.From(value);
    }

    public static implicit operator Maybe<T>(Maybe value) => None;

    public static Maybe<T> From(T obj)
    {
        return new Maybe<T>(obj);
    }

    public static bool operator ==(Maybe<T> maybe, T value)
    {
        if (value is Maybe<T>)
            return maybe.Equals(value);

        if (maybe.HasNoValue)
            return value is null;

        return maybe._value.Equals(value);
    }

    public static bool operator !=(Maybe<T> maybe, T value)
    {
        return !(maybe == value);
    }

    public static bool operator ==(Maybe<T> maybe, object other)
    {
        return maybe.Equals(other);
    }

    public static bool operator !=(Maybe<T> maybe, object other)
    {
        return !(maybe == other);
    }

    public static bool operator ==(Maybe<T> first, Maybe<T> second)
    {
        return first.Equals(second);
    }

    public static bool operator !=(Maybe<T> first, Maybe<T> second)
    {
        return !(first == second);
    }

    public override bool Equals(object obj)
    {
        if (obj is null)
            return false;

        if (obj is Maybe<T> other)
            return Equals(other);

        if (obj is T value)
            return Equals(value);

        return false;
    }

    public bool Equals(Maybe<T> other)
    {
        if (HasNoValue && other.HasNoValue)
            return true;

        if (HasNoValue || other.HasNoValue)
            return false;

        return EqualityComparer<T>.Default.Equals(_value, other._value);
    }

    public override int GetHashCode()
    {
        if (HasNoValue)
            return 0;

        return _value.GetHashCode();
    }

    public override string ToString()
    {
        if (HasNoValue)
            return "No value";

        return _value.ToString();
    }

    public Result<T> ToResult(Exception ex) => HasNoValue ? Result.Fail<T>(ex) : this.Value;
}

/// <summary>
/// Non-generic entrypoint for <see cref="Maybe{T}" /> members
/// </summary>
public readonly struct Maybe
{
    public static Maybe None => new Maybe();

    /// <summary>
    /// Creates a new <see cref="Maybe{T}" /> from the provided <paramref name="value"/>
    /// </summary>
    public static Maybe<T> From<T>(T value) => Maybe<T>.From(value);
}

/// <summary>
/// Useful in scenarios where you need to determine if a value is Maybe or not
/// </summary>
public interface IMaybe<out T>
{
    T Value { get; }

    bool HasValue { get; }

    bool HasNoValue { get; }
}
