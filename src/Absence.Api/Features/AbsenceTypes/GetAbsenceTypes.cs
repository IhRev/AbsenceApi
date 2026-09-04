using Absence.Infrastructure.Database.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Absence.Api.Features.AbsenceTypes;

public class AbsenceTypeDTO
{
    public required int Id { get; set; }
    public required string Name { get; set; }
}

public static class GetAbsenceTypes
{
    public sealed class Query : IRequest<IEnumerable<AbsenceTypeDTO>>;

    internal sealed class Handler(AbsenceContext db)
        : IRequestHandler<Query, IEnumerable<AbsenceTypeDTO>>
    {
        public async Task<IEnumerable<AbsenceTypeDTO>> Handle(Query request, CancellationToken cancellationToken = default)
        {
            return await db.AbsenceTypes
                .Select(_ => new AbsenceTypeDTO
                {
                    Id = _.Id,
                    Name = _.Name
                })
                .ToListAsync(cancellationToken);
        }
    }
}
