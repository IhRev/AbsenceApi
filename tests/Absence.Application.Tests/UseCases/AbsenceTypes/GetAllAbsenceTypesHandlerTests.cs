using Absence.Application.Common.Interfaces;
using Absence.Application.UseCases.AbsenceTypes.DTOs;
using Absence.Application.UseCases.AbsenceTypes.Handlers;
using Absence.Application.UseCases.AbsenceTypes.Queries;
using Absence.Domain.Entities;
using Absence.Domain.Interfaces;
using Absence.Domain.Specifications;
using AutoMapper;
using NSubstitute;
using Shouldly;

namespace Absence.Application.Tests.UseCases.AbsenceTypes;

public class GetAllAbsenceTypesHandlerTests
{
    private readonly GetAllAbsenceTypesQuery _query;
    private readonly IRepository<UserOrganizationRoleEntity> _userOrganizationRoleRepository;
    private readonly IRepository<AbsenceTypeEntity> _absenceTypesRepository;
    private readonly IUser _user;
    private readonly IMapper _mapper;
    private readonly GetAllAbsenceTypesHandler _sut;

    public GetAllAbsenceTypesHandlerTests()
    {
        _query = new(1);
        _userOrganizationRoleRepository = Substitute.For<IRepository<UserOrganizationRoleEntity>>();
        _absenceTypesRepository = Substitute.For<IRepository<AbsenceTypeEntity>>();
        _user = Substitute.For<IUser>();
        _mapper = Substitute.For<IMapper>();
        _sut = new GetAllAbsenceTypesHandler(
            _absenceTypesRepository,
            _userOrganizationRoleRepository,
            _user,
            _mapper
        );
    }

    [Fact]
    public async Task Handle_ReturnsAccessDenied_WhenUserHasntPermission()
    {
        //Arrange
        _userOrganizationRoleRepository.AnyAsync(Arg.Any<PermissionSpec>()).Returns(false);

        //Act
        var actual = await _sut.Handle(_query, CancellationToken.None);

        //Assert
        actual.IsT1.ShouldBeTrue();
        _absenceTypesRepository.Received(0);
    }

    [Fact]
    public async Task Handle_ReturnsSuccess_WhenUserHasPermission()
    {
        //Arrange
        _userOrganizationRoleRepository.AnyAsync(Arg.Any<PermissionSpec>()).Returns(true);

        var entities = new List<AbsenceTypeEntity>();
        _absenceTypesRepository.GetAsync(Arg.Any<AbsenceTypeSpec>()).Returns(entities);

        var dtos = new List<AbsenceTypeDTO>();
        _mapper.Map<List<AbsenceTypeDTO>>(entities).Returns(dtos);

        //Act
        var actual = await _sut.Handle(_query, CancellationToken.None);

        //Assert
        actual.IsT0.ShouldBeTrue();
        actual.AsT0.Value.ShouldBe(dtos);
    }
}