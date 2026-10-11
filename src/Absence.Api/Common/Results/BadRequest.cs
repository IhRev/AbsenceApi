namespace Absence.Api.Common.Results;

public struct BadRequest(string message)
{
    public string Message { get; } = message;
}