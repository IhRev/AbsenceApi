using Absence.Application.UseCases.AbsenceTypes.Handlers;
using Absence.Domain.Entities;
using Absence.Domain.Interfaces;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Shouldly;

namespace Absence.Application.Tests.UseCases.AbsenceTypes;

public class DeleteAbsenceTypeHandlerTests
{
    private readonly int _absenceTypeId = 1;
    private readonly int _organizationId = 1;
    private IRepository<AbsenceTypeEntity> _absenceTypesRepository;
    private DeleteAbsenceTypeHandler _sut;

    public DeleteAbsenceTypeHandlerTests()
    {
        _absenceTypesRepository = Substitute.For<IRepository<AbsenceTypeEntity>>();
        _sut = new(_absenceTypesRepository);
    }

    [Fact]
    public async Task Handle_ReturnsNotFound_WhenNoAbsenceTypeFound()
    {
        //Arrange
        _absenceTypesRepository.GetByIdAsync(_absenceTypeId).ReturnsNull();

        //Act
        var actual = await _sut.Handle(new(_organizationId, _absenceTypeId));

        //Assert
        actual.IsT1.ShouldBeTrue();
        _absenceTypesRepository.Received(0).Delete(Arg.Any<AbsenceTypeEntity>());
        await _absenceTypesRepository.Received(0).SaveAsync();
    }

    [Fact]
    public async Task Handle_ReturnsNotFound_WhenAbsenceTypeIsDeleted()
    {
        //Arrange
        _absenceTypesRepository
            .GetByIdAsync(_absenceTypeId)
            .Returns(new AbsenceTypeEntity() { Code = "code", Name = "name", IsDeleted = true});

        //Act
        var actual = await _sut.Handle(new(_organizationId, _absenceTypeId));

        //Assert
        actual.IsT1.ShouldBeTrue();
        _absenceTypesRepository.Received(0).Delete(Arg.Any<AbsenceTypeEntity>());
        await _absenceTypesRepository.Received(0).SaveAsync();
    }

    [Fact]
    public async Task Handle_ReturnsSuccess_WhenAbsenceTypeDeleted()
    {
        //Arrange
        var absenceType = new AbsenceTypeEntity() { Code = "code", Name = "name" };
        _absenceTypesRepository.GetByIdAsync(_absenceTypeId).Returns(absenceType);

        //Act
        var actual = await _sut.Handle(new(_organizationId, _absenceTypeId));

        //Assert
        actual.IsT0.ShouldBeTrue();
        _absenceTypesRepository.Received(1).Delete(absenceType);
        await _absenceTypesRepository.Received(1).SaveAsync();
    }
}