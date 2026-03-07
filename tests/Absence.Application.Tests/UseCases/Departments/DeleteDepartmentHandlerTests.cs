using Absence.Application.UseCases.Departments.Commands;
using Absence.Application.UseCases.Departments.Handlers;
using Absence.Domain.Entities;
using Absence.Domain.Interfaces;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Shouldly;

namespace Absence.Application.Tests.UseCases.Departments;

public class DeleteDepartmentHandlerTests
{
    private DeleteDepartmentCommand _command;
    private IRepository<DepartmentEntity> _departmentRepository;
    private DeleteDepartmentHandler _sut;

    public DeleteDepartmentHandlerTests()
    {
        _command = new(1, 2);
        _departmentRepository = Substitute.For<IRepository<DepartmentEntity>>();
        _sut = new(_departmentRepository);
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
        _departmentRepository.Received(0).Delete(Arg.Any<DepartmentEntity>());
        await _departmentRepository.Received(0).SaveAsync();
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenDepartmentDeleted()
    {
        // Arrange
        var @event = new DepartmentEntity { Name = "name" };
        _departmentRepository.GetByIdAsync(Arg.Any<int>()).Returns(@event);

        // Act
        var actual = await _sut.Handle(_command);

        // Assert
        actual.IsT0.ShouldBeTrue();
        _departmentRepository.Received(1).Delete(@event);
        await _departmentRepository.Received(1).SaveAsync();
    }
}