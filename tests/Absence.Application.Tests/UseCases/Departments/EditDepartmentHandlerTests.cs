using Absence.Application.UseCases.Departments.Commands;
using Absence.Application.UseCases.Departments.Handlers;
using Absence.Domain.Entities;
using Absence.Domain.Interfaces;
using AutoMapper;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Shouldly;

namespace Absence.Application.Tests.UseCases.Departments;

public class EditDepartmentHandlerTests
{
    private EditDepartmentCommand _command;
    private IRepository<DepartmentEntity> _departmentRepository;
    private IMapper _mapper;
    private EditDepartmentHandler _sut;

    public EditDepartmentHandlerTests()
    {
        _command = new(1, new() { Name = "name" });
        _departmentRepository = Substitute.For<IRepository<DepartmentEntity>>();
        _mapper = Substitute.For<IMapper>();
        _sut = new(_departmentRepository, _mapper);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenDepartmentDoesntExist()
    {
        // Arrange
        _departmentRepository.GetByIdAsync(Arg.Any<int>()).ReturnsNull();

        // Act
        var actual = await _sut.Handle(_command);

        // Assert
        actual.IsT1.ShouldBeTrue();
        _mapper.Received(0).Map(_command.Department, Arg.Any<DepartmentEntity>());
        _departmentRepository.Received(0).Update(Arg.Any<DepartmentEntity>());
        await _departmentRepository.Received(0).SaveAsync();
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenDepartmentUpdated()
    {
        // Arrange
        var @event = new DepartmentEntity { Name = "name" };
        _departmentRepository.GetByIdAsync(Arg.Any<int>()).Returns(@event);

        // Act
        var actual = await _sut.Handle(_command);

        // Assert
        actual.IsT0.ShouldBeTrue();
        _mapper.Received(1).Map(_command.Department, @event);
        _departmentRepository.Received(1).Update(@event);
        await _departmentRepository.Received(1).SaveAsync();
    }
}