namespace AbsenceApi.Common;

public interface IIdKeyed<TId>
{
    TId Id { get; set; }
}