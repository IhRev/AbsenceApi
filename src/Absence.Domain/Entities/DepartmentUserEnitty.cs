using Absence.Domain.Common;

namespace Absence.Domain.Entities;

public class DepartmentUserEnitty : IIdKeyed<int>
{
    public int Id { get; set; }
    public int DepartmentId { get; set; }
    public int UserId { get; set; }
    public DepartmentEntity Department { get; set; } = null!;
    public UserEntity User { get; set; } = null!;
}