using Absence.Application.UseCases.Departments.Commands;
using Absence.Application.UseCases.Departments.Handlers;
using Absence.Domain.Entities;
using Absence.Domain.Interfaces;
using AutoMapper;
using NSubstitute;
using Shouldly;

namespace Absence.Application.Tests.UseCases.Departments;

public class AddDepartmentHandlerTests
{
    private AddDepartmentCommand _command;
    private IRepository<DepartmentEntity> _departmentRepository;
    private IMapper _mapper;
    private AddDepartmentHandler _sut;

    public AddDepartmentHandlerTests()
    {
        _command = new(1, new() { Name = "name" });
        _departmentRepository = Substitute.For<IRepository<DepartmentEntity>>();
        _mapper = Substitute.For<IMapper>();
        _sut = new AddDepartmentHandler(_departmentRepository, _mapper);
    }

    [Fact]
    public async Task Handle_ShouldReturnId()
    {
        // Arrange
        var entity = new DepartmentEntity { Id = 1, Name = "name" };
        _mapper.Map<DepartmentEntity>(_command.Department).Returns(entity);

        // Act
        var actual = await _sut.Handle(_command);

        // Assert
        actual.ShouldBe(entity.Id);
        await _departmentRepository.Received(1).InsertAsync(entity);
        await _departmentRepository.Received(1).SaveAsync();
    }
}