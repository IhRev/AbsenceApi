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
    private readonly IRepository<AbsenceTypeEntity> _absenceTypesRepository;
    private readonly IMapper _mapper;
    private readonly GetAllAbsenceTypesHandler _sut;

    public GetAllAbsenceTypesHandlerTests()
    {
        _query = new(1);
        _absenceTypesRepository = Substitute.For<IRepository<AbsenceTypeEntity>>();
        _mapper = Substitute.For<IMapper>();
        _sut = new(_absenceTypesRepository, _mapper);
    }

    [Fact]
    public async Task Handle_ReturnsAbsenceTypes()
    {
        //Arrange
        var entities = new List<AbsenceTypeEntity>()
        {
            new() { Code = "code", Name = "name" },
            new() { Code = "code1", Name = "name1" }
        };
        _absenceTypesRepository.GetAsync(Arg.Any<AbsenceTypeSpec>()).Returns(entities);

        var dtos = new List<AbsenceTypeDTO>()
        {
            new() { Code = "code", Name = "name" }
        };
        _mapper.Map<IEnumerable<AbsenceTypeDTO>>(entities).Returns(dtos);

        //Act
        var actual = await _sut.Handle(_query);

        //Assert
        actual.ShouldBe(dtos);
    }
}