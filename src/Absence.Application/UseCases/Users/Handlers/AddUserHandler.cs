using Absence.Application.UseCases.Users.Commands;
using Absence.Domain.Entities;
using Absence.Domain.Interfaces;
using AutoMapper;
using MediatR;

namespace Absence.Application.UseCases.Users.Handlers;

internal class AddUserHandler(
    IUserService userRepository,
    IMapper mapper
) : IRequestHandler<AddUserCommand, string>
{
    private readonly IUserService _userRepository = userRepository;
    private readonly IMapper _mapper = mapper;

    public async Task<string> Handle(AddUserCommand request, CancellationToken cancellationToken)
    {
        var user = new UserEntity()
        {
            FirstName = request.User.FirstName,
            SecondName = request.User.SecondName,
            Email = request.User.Credentials.Email,
            UserName = request.User.Credentials.Email,
            OrganizationId = 1
        };
        var a = await _userRepository.CreateAsync(user, request.User.Credentials.Password);
        return user.Id;
    }
}