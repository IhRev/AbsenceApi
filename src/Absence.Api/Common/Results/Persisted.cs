namespace Absence.Api.Common.Results;

/// <summary>
/// An organization admin wrote the absence immediately. <see cref="AbsenceId"/> is the row that was
/// created, updated, or deleted.
/// </summary>
public readonly struct Persisted(int absenceId)
{
    public int AbsenceId { get; } = absenceId;
}
