using Absence.Application.UseCases.AbsenceTypes.Commands;
using Absence.Application.UseCases.AbsenceTypes.Handlers;
using Absence.Domain.Entities;
using Absence.Domain.Interfaces;
using AutoMapper;
using NSubstitute;
using Shouldly;

namespace Absence.Application.Tests.UseCases.AbsenceTypes;

public class CreateAbsenceTypeHandlerTests
{
    private readonly CreateAbsenceTypeCommand _command;
    private readonly IRepository<AbsenceTypeEntity> _absenceTypesRepository;
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
        _absenceTypesRepository = Substitute.For<IRepository<AbsenceTypeEntity>>();
        _mapper = Substitute.For<IMapper>();
        _sut = new(_absenceTypesRepository, _mapper);
    }

    [Fact]
    public async Task Handle_ReturnsId()
    {
        //Arrange
        var entity = new AbsenceTypeEntity() { Id = 2, Code = "code", Name = "name" };
        _mapper.Map<AbsenceTypeEntity>(_command.AbsenceType).Returns(entity);

        //Act
        var actual = await _sut.Handle(_command, CancellationToken.None);

        //Assert
        actual.ShouldBe(entity.Id);
        await _absenceTypesRepository
            .Received(1)
            .InsertAsync(entity);
        entity.OrganizationId.ShouldBe(_command.OrganizationId);
        await _absenceTypesRepository
            .Received(1)
            .SaveAsync();
    }
}