namespace Amusoft.PCR.Domain.Shared.Entities;


public static class Result
{
	public static Result<T> Success<T>(T value) => new(true, value);
	public static Result<T> Error<T>() => new(false, default);
}

public class Result<T>
{
	internal Result(bool success, T value)
	{
		Value = value;
		Success = success;
	}

	public T Value { get; }

	public bool Success { get; set; }
}