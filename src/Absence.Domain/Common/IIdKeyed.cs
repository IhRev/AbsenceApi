namespace Absence.Domain.Common;

public interface IIdKeyed<TId>
{
    TId Id { get; set; }
}