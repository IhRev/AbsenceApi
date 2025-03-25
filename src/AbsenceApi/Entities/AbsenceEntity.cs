using AbsenceApi.Common;

namespace AbsenceApi.Entities;

public class AbsenceEntity : IIdKeyed<int>
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int AbsenceTypeId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int UserId { get; set; }
    public AbsenceTypeEntity AbsenceType { get; set; } = null!;
}