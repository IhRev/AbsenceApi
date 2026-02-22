using Absence.Application.Common.Interfaces;
using Absence.Application.UseCases.AbsenceTypes.Commands;
using Absence.Application.UseCases.AbsenceTypes.Handlers;
using Absence.Domain.Entities;
using Absence.Domain.Interfaces;
using Absence.Domain.Specifications;
using AutoMapper;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Shouldly;

namespace Absence.Application.Tests.UseCases.AbsenceTypes;

public class UpdateAbsenceTypeHandlerTests
{
    private readonly UpdateAbsenceTypeCommand _command;
    private readonly IRepository<UserOrganizationRoleEntity> _userOrganizationRoleRepository;
    private readonly IRepository<AbsenceTypeEntity> _absenceTypesRepository;
    private readonly IUser _user;
    private readonly IMapper _mapper;
    private readonly UpdateAbsenceTypeHandler _sut;

    public UpdateAbsenceTypeHandlerTests()
    {
        _command = new(1, new()
        {
            Id = 2,
            Code = "code",
            Name = "name",
            CountsTowardAnnualLeave = true,
            RequiresApproval = true
        });
        _userOrganizationRoleRepository = Substitute.For<IRepository<UserOrganizationRoleEntity>>();
        _absenceTypesRepository = Substitute.For<IRepository<AbsenceTypeEntity>>();
        _user = Substitute.For<IUser>();
        _mapper = Substitute.For<IMapper>();
        _sut = new UpdateAbsenceTypeHandler(
            _userOrganizationRoleRepository,
            _absenceTypesRepository,
            _user,
            _mapper
        );
    }

    [Fact]
    public async Task Handle_ReturnsNotFound_WhenNoAbsenceTypeFound()
    {
        //Arrange
        _absenceTypesRepository.GetByIdAsync(_command.AbsenceType.Id).ReturnsNull();

        //Act
        var actual = await _sut.Handle(_command, CancellationToken.None);

        //Assert
        actual.IsT1.ShouldBeTrue();
        _absenceTypesRepository.Received(0).Update(Arg.Any<AbsenceTypeEntity>());
        await _absenceTypesRepository.Received(0).SaveAsync();
        await _userOrganizationRoleRepository.Received(0).AnyAsync(Arg.Any<PermissionSpec>());
    }

    [Fact]
    public async Task Handle_ReturnsAccessDenied_WhenUserHasNoPermission()
    {
        //Arrange
        var absenceType = new AbsenceTypeEntity() { Code = "c", Name = "n" };
        _absenceTypesRepository.GetByIdAsync(_command.AbsenceType.Id).Returns(absenceType);

        _userOrganizationRoleRepository.AnyAsync(Arg.Any<PermissionSpec>()).Returns(false);

        //Act
        var actual = await _sut.Handle(_command, CancellationToken.None);

        //Assert
        actual.IsT2.ShouldBeTrue();
        _absenceTypesRepository.Received(0).Update(absenceType);
        await _absenceTypesRepository.Received(0).SaveAsync();
    }

    [Fact]
    public async Task Handle_ReturnsSuccess_WhenUserHasPermission()
    {
        //Arrange
        var absenceType = new AbsenceTypeEntity() { Code = "c", Name = "n" };
        _absenceTypesRepository.GetByIdAsync(_command.AbsenceType.Id).Returns(absenceType);

        _userOrganizationRoleRepository.AnyAsync(Arg.Any<PermissionSpec>()).Returns(true);

        //Act
        var actual = await _sut.Handle(_command, CancellationToken.None);

        //Assert
        actual.IsT0.ShouldBeTrue();
        _mapper.Received(1).Map(_command.AbsenceType, absenceType);
        _absenceTypesRepository.Received(1).Update(absenceType);
        await _absenceTypesRepository.Received(1).SaveAsync();
    }
}