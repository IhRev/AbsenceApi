﻿using Absence.Application.Common.DTOs;
using MediatR;

namespace Absence.Application.UseCases.Organizations.Commands;

public class AddOrganizationCommand(CreateOrganizationDTO organization) : IRequest<int>
{
    public CreateOrganizationDTO Organization { get; } = organization;
}