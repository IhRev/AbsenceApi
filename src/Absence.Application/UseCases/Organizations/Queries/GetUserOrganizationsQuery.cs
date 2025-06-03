using Absence.Application.UseCases.Organizations.DTOs;
using MediatR;

namespace Absence.Application.UseCases.Organizations.Queries;

public class GetUserOrganizationsQuery : IRequest<IEnumerable<OrganizationDTO>>;