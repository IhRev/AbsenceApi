namespace Absence.Domain;

public interface IIdKeyed<TId>
{
    TId Id { get; set; }
}
