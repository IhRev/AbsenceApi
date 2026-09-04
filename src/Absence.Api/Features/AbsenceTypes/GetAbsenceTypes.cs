using Absence.Infrastructure.Database.Repositories;
using Absence.Infrastructure.Entities;
using AutoMapper;
using MediatR;

namespace Absence.Api.Features.AbsenceTypes;

public class AbsenceTypeDTO
{
    public required int Id { get; set; }
    public required string Name { get; set; }
}

public static class GetAbsenceTypes
{
    public sealed class Query : IRequest<IEnumerable<AbsenceTypeDTO>>;

    internal sealed class Handler(IRepository<AbsenceTypeEntity> absenceTypeRepository, IMapper mapper)
        : IRequestHandler<Query, IEnumerable<AbsenceTypeDTO>>
    {
        private readonly IRepository<AbsenceTypeEntity> _absenceTypeRepository = absenceTypeRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<IEnumerable<AbsenceTypeDTO>> Handle(Query request, CancellationToken cancellationToken = default)
        {
            var absenceTypes = await _absenceTypeRepository.GetAsync(cancellationToken: cancellationToken);
            return _mapper.Map<IEnumerable<AbsenceTypeDTO>>(absenceTypes);
        }
    }
}
