namespace Absence.Api.Common.Exceptions;

public class MissingUserClaimException(string claim)
    : Exception($"Token is missing a usable '{claim}' claim.")
{
    public string Claim { get; } = claim;
}