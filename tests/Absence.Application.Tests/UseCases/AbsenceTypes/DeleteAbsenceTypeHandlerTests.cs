using Absence.Application.Common.Interfaces;
using Absence.Application.UseCases.AbsenceTypes.Commands;
using Absence.Application.UseCases.AbsenceTypes.Handlers;
using Absence.Domain.Entities;
using Absence.Domain.Interfaces;
using Absence.Domain.Specifications;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Shouldly;

namespace Absence.Application.Tests.UseCases.AbsenceTypes;

public class DeleteAbsenceTypeHandlerTests
{
    private int _absenceTypeId = 1;
    private IRepository<UserOrganizationRoleEntity> _userOrganizationRoleRepository;
    private IRepository<AbsenceTypeEntity> _absenceTypesRepository;
    private IUser _user;
    private DeleteAbsenceTypeHandler _sut;

    public DeleteAbsenceTypeHandlerTests()
    {
        _userOrganizationRoleRepository = Substitute.For<IRepository<UserOrganizationRoleEntity>>();
        _absenceTypesRepository = Substitute.For<IRepository<AbsenceTypeEntity>>();
        _user = Substitute.For<IUser>();
        _sut = new DeleteAbsenceTypeHandler(
            _userOrganizationRoleRepository,
            _absenceTypesRepository,
            _user
        );
    }

    [Fact]
    public async Task Handle_ReturnsNotFound_WhenNoAbsenceTypeFound()
    {
        //Arrange
        _absenceTypesRepository.GetByIdAsync(_absenceTypeId).ReturnsNull();

        //Act
        var actual = await _sut.Handle(new DeleteAbsenceTypeCommand(_absenceTypeId), CancellationToken.None);

        //Assert
        actual.IsT1.ShouldBeTrue();
        _userOrganizationRoleRepository.Received(0);
        _absenceTypesRepository.Received(0).Delete(Arg.Any<AbsenceTypeEntity>());
        await _absenceTypesRepository.Received(0).SaveAsync();
    }

    [Fact]
    public async Task Handle_ReturnsAccessDenied_WhenUserHasntPermission()
    {
        //Arrange
        var absenceType = new AbsenceTypeEntity() { Code = "code", Name = "name" };
        _absenceTypesRepository.GetByIdAsync(_absenceTypeId).Returns(absenceType);

        _userOrganizationRoleRepository.AnyAsync(Arg.Any<PermissionSpec>()).Returns(false);

        //Act
        var actual = await _sut.Handle(new DeleteAbsenceTypeCommand(_absenceTypeId), CancellationToken.None);

        //Assert
        actual.IsT2.ShouldBeTrue();
        _absenceTypesRepository.Received(0).Delete(absenceType);
        await _absenceTypesRepository.Received(0).SaveAsync();
    }

    [Fact]
    public async Task Handle_ReturnsSuccess_WhenUserHasPermission()
    {
        //Arrange
        var absenceType = new AbsenceTypeEntity() { Code = "code", Name = "name" };
        _absenceTypesRepository.GetByIdAsync(_absenceTypeId).Returns(absenceType);

        _userOrganizationRoleRepository.AnyAsync(Arg.Any<PermissionSpec>()).Returns(true);

        //Act
        var actual = await _sut.Handle(new DeleteAbsenceTypeCommand(_absenceTypeId), CancellationToken.None);

        //Assert
        actual.IsT0.ShouldBeTrue();
        _absenceTypesRepository.Received(1).Delete(absenceType);
        await _absenceTypesRepository.Received(1).SaveAsync();
    }
}