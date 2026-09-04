namespace Absence.Infrastructure.Common;

public interface IIdKeyed<TId>
{
    TId Id { get; set; }
}