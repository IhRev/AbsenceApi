using Absence.Application.UseCases.Departments.DTOs;
using Absence.Application.UseCases.Departments.Handlers;
using Absence.Application.UseCases.Departments.Queries;
using Absence.Domain.Entities;
using Absence.Domain.Interfaces;
using Absence.Domain.Specifications;
using AutoMapper;
using NSubstitute;
using Shouldly;

namespace Absence.Application.Tests.UseCases.Departments;

public class GetAllDepartmentsHandlerTests
{
    private GetAllDepartmentsQuery _query;
    private IRepository<DepartmentEntity> _departmentRepository;
    private IMapper _mapper;
    private GetAllDepartmentsHandler _sut;

    public GetAllDepartmentsHandlerTests()
    {
        _query = new(1);
        _departmentRepository = Substitute.For<IRepository<DepartmentEntity>>();
        _mapper = Substitute.For<IMapper>();
        _sut = new GetAllDepartmentsHandler(_departmentRepository, _mapper);
    }

    [Fact]
    public async Task Handle_ShouldReturnDepartments()
    {
        // Arrange
        var departments = new List<DepartmentEntity>()
        {
            new() { Id = 1, Name = "Department 1", OrganizationId = 1 },
            new() { Id = 2, Name = "Department 2", OrganizationId = 1 }
        };
        _departmentRepository.GetAsync(Arg.Any<DepartmentSpec>()).Returns(departments);

        var dtos = new List<DepartmentDTO>()
        {
            new() { Id = 1, Name = "Department 1" },
            new() { Id = 2, Name = "Department 2" }
        };
        _mapper.Map<IEnumerable<DepartmentDTO>>(departments).Returns(dtos);

        // Act
        var actual = await _sut.Handle(_query);

        // Assert
        actual.ShouldBe(dtos);
    }
}