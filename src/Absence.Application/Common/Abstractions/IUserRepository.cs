using Absence.Domain.Entities;

namespace Absence.Application.Common.Abstractions;

public interface IUserRepository
{
    Task CreateUserAsync(UserEntity user, CancellationToken cancellationToken = default);
}