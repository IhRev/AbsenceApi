using Absence.Application.Common.Interfaces;
using Absence.Application.UseCases.AbsenceTypes.Commands;
using Absence.Application.UseCases.AbsenceTypes.Handlers;
using Absence.Domain.Entities;
using Absence.Domain.Interfaces;
using Absence.Domain.Specifications;
using AutoMapper;
using NSubstitute;
using Shouldly;

namespace Absence.Application.Tests.UseCases.AbsenceTypes;

public class CreateAbsenceTypeHandlerTests
{
    private readonly CreateAbsenceTypeCommand _command;
    private readonly IRepository<UserOrganizationRoleEntity> _userOrganizationRoleRepository;
    private readonly IRepository<AbsenceTypeEntity> _absenceTypesRepository;
    private readonly IUser _user;
    private readonly IMapper _mapper;
    private readonly CreateAbsenceTypeHandler _sut;

    public CreateAbsenceTypeHandlerTests()
    {
        _command = new(1, new() 
        { 
            Code = "code", 
            Name = "name", 
            CountsTowardAnnualLeave = true, 
            RequiresApproval = true 
        });
        _userOrganizationRoleRepository = Substitute.For<IRepository<UserOrganizationRoleEntity>>();
        _absenceTypesRepository = Substitute.For<IRepository<AbsenceTypeEntity>>();
        _user = Substitute.For<IUser>();
        _mapper = Substitute.For<IMapper>();
        _sut = new CreateAbsenceTypeHandler(
            _userOrganizationRoleRepository, 
            _absenceTypesRepository, 
            _user, 
            _mapper
        );
    }

    [Fact]
    public async Task Handle_ReturnsAccessDenied_WhenUserHasNoPermission()
    {
        //Arrange
        _userOrganizationRoleRepository.AnyAsync(
            Arg.Any<HasPermissionSpec>(), 
            Arg.Any<CancellationToken>()
        ).Returns(false);

        //Act
        var actual = await _sut.Handle(_command, CancellationToken.None);

        //Assert
        actual.IsT1.ShouldBeTrue();
        _absenceTypesRepository.Received(0);
    }

    [Fact]
    public async Task Handle_ReturnsSuccess_WhenUserHasPermission()
    {
        //Arrange
        _userOrganizationRoleRepository.AnyAsync(
            Arg.Any<HasPermissionSpec>(),
            Arg.Any<CancellationToken>()
        ).Returns(true);

        var entity = new AbsenceTypeEntity() { Code = "code", Name = "name" };
        _mapper.Map<AbsenceTypeEntity>(_command.AbsenceType).Returns(entity);

        //Act
        var actual = await _sut.Handle(_command, CancellationToken.None);

        //Assert
        actual.IsT0.ShouldBeTrue();
        await _absenceTypesRepository
            .Received(1)
            .InsertAsync(entity);
        entity.OrganizationId.ShouldBe(_command.OrganizationId);
        await _absenceTypesRepository
            .Received(1)
            .SaveAsync();
    }
}