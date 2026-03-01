using Absence.Application.UseCases.AbsenceTypes.Commands;
using Absence.Application.UseCases.AbsenceTypes.Handlers;
using Absence.Domain.Entities;
using Absence.Domain.Interfaces;
using AutoMapper;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Shouldly;

namespace Absence.Application.Tests.UseCases.AbsenceTypes;

public class UpdateAbsenceTypeHandlerTests
{
    private readonly UpdateAbsenceTypeCommand _command;
    private readonly IRepository<AbsenceTypeEntity> _absenceTypesRepository;
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
        _absenceTypesRepository = Substitute.For<IRepository<AbsenceTypeEntity>>();
        _mapper = Substitute.For<IMapper>();
        _sut = new(_absenceTypesRepository, _mapper);
    }

    [Fact]
    public async Task Handle_ReturnsNotFound_WhenNoAbsenceTypeFound()
    {
        //Arrange
        _absenceTypesRepository.GetByIdAsync(_command.AbsenceType.Id).ReturnsNull();

        //Act
        var actual = await _sut.Handle(_command);

        //Assert
        actual.IsT1.ShouldBeTrue();
        _absenceTypesRepository.Received(0).Update(Arg.Any<AbsenceTypeEntity>());
        await _absenceTypesRepository.Received(0).SaveAsync();
    }

    [Fact]
    public async Task Handle_ReturnsNotFound_WhenAbsenceTypeIsDeleted()
    {
        //Arrange
        _absenceTypesRepository
            .GetByIdAsync(_command.AbsenceType.Id)
            .Returns(new AbsenceTypeEntity() { Code = "code", Name = "name", IsDeleted = true });

        //Act
        var actual = await _sut.Handle(_command);

        //Assert
        actual.IsT1.ShouldBeTrue();
        _absenceTypesRepository.Received(0).Update(Arg.Any<AbsenceTypeEntity>());
        await _absenceTypesRepository.Received(0).SaveAsync();
    }

    [Fact]
    public async Task Handle_ReturnsSuccess_WhenUpdatedSuccessfully()
    {
        //Arrange
        var absenceType = new AbsenceTypeEntity() { Code = "code", Name = "name" };
        _absenceTypesRepository.GetByIdAsync(_command.AbsenceType.Id).Returns(absenceType);

        //Act
        var actual = await _sut.Handle(_command);

        //Assert
        actual.IsT0.ShouldBeTrue();
        _mapper.Received(1).Map(_command.AbsenceType, absenceType);
        _absenceTypesRepository.Received(1).Update(absenceType);
        await _absenceTypesRepository.Received(1).SaveAsync();
    }
}