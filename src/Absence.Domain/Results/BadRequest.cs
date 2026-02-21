namespace Absence.Domain.Results;

public struct BadRequest(string message)
{
    public string Message { get; } = message;
}