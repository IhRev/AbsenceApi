namespace Absence.Domain.Interfaces;

public interface IIdKeyed<TId>
{
    TId Id { get; set; }
}