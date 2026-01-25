using Absence.Application.UseCases.Organizations.DTOs;
using MediatR;

namespace Absence.Application.UseCases.Organizations.Commands;

public record AddOrganizationCommand(CreateOrganizationDTO Organization) : IRequest<int>;