﻿using Absence.Domain.Common;

namespace Absence.Domain.Entities;

public class OrganizationEntity : IIdKeyed<int>
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public ICollection<UserEntity> Members { get; set; } = null!;
}