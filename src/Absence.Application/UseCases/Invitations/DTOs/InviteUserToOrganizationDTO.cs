﻿using System.ComponentModel.DataAnnotations;

namespace Absence.Application.UseCases.Invitations.DTOs;

public class InviteUserToOrganizationDTO
{
    [Required(AllowEmptyStrings = false)]
    [EmailAddress]
    public required string UserEmail { get; set; }
    [Required]
    public required int OrganizationId { get; set; }
} 