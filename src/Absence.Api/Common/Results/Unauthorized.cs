namespace Absence.Api.Common.Results;

/// <summary>
/// The caller presented a valid token whose user no longer exists. Reachable because
/// deleting an account does not revoke already-issued access tokens.
/// </summary>
public struct Unauthorized;