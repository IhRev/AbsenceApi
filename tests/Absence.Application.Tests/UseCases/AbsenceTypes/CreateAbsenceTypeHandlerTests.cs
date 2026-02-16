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

public class CreateAbsenceTypeHandlerTests
{
    private readonly CreateAbsenceTypeCommand _command;
    private readonly IRepository<DepartmentEntity> _departmentRepository;
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
        _departmentRepository = Substitute.For<IRepository<DepartmentEntity>>();
        _absenceTypesRepository = Substitute.For<IRepository<AbsenceTypeEntity>>();
        _user = Substitute.For<IUser>();
        _mapper = Substitute.For<Mapper>();
        _sut = new CreateAbsenceTypeHandler(
            _departmentRepository, 
            _absenceTypesRepository, 
            _user, 
            _mapper
        );
    }

    [Fact]
    public async Task Handle_ReturnsBadRequest_WhenUserDoesntBelongToOrganization()
    {
        //Arrange
        _departmentRepository.GetFirstOrDefaultAsync(
            Arg.Any<DepartmentSpec>(), 
            Arg.Any<CancellationToken>()
        ).ReturnsNull();

        //Act
        var actual = await _sut.Handle(_command, CancellationToken.None);

        //Assert
        actual.IsT1.ShouldBeTrue();
    }
}