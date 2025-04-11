namespace Absence.Application.Common.Results;

public struct BadRequest(string message)
{
    public string Message { get; } = message;
}