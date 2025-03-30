using Absence.Application.Common.Abstractions;
using Absence.Application.UseCases.Users.Commands;
using Absence.Domain.Entities;
using AutoMapper;
using MediatR;

namespace Absence.Application.UseCases.Users.Handlers;

internal class AddUserHandler(
    IUserRepository userRepository,
    IMapper mapper
) : IRequestHandler<AddUserCommand, string>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IMapper _mapper = mapper;

    public async Task<string> Handle(AddUserCommand request, CancellationToken cancellationToken)
    {
        var user = _mapper.Map<UserEntity>(request.User);
        await _userRepository.CreateUserAsync(user, cancellationToken);
        return user.Id;
    }
}