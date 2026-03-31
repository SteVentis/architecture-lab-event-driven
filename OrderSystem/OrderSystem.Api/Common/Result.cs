namespace OrderSystem.Api.Common;

public class Result 
{
	public bool IsSuccess { get; set; }
	public bool IsFailure => !IsSuccess;
	public string ErrorMessage { get; set; } = string.Empty;

}

public class Result<T> : Result
{
	public T Value { get; set; } = default!;

	public Result(bool isSuccess, string errorMessage, T value)
	{
		IsSuccess = isSuccess;
		ErrorMessage = errorMessage;
		Value = value;
	}

	public static Result<T> Success(T value) 
		=> new Result<T>(true, string.Empty, value);

	public static Result<T> Failure(string errorMessage) 
		=> new Result<T>(false, errorMessage, default!);

}
