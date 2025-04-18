﻿namespace Absence.Application.Common.DTOs;

public class EditAbsenceDTO
{
    public required int Id { get; set; }
    public required string Name { get; set; } = null!;
    public required int Type { get; set; }
    public required DateTimeOffset StartDate { get; set; }
    public required DateTimeOffset EndDate { get; set; }
}