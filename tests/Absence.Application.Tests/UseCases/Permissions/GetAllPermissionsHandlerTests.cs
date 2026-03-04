using Absence.Application.UseCases.Permissions.DTOs;
using Absence.Application.UseCases.Permissions.Handlers;
using Absence.Application.UseCases.Permissions.Queries;
using Absence.Domain.Entities;
using Absence.Domain.Interfaces;
using AutoMapper;
using NSubstitute;
using Shouldly;

namespace Absence.Application.Tests.UseCases.Permissions;

public class GetAllPermissionsHandlerTests
{
    private readonly IRepository<PermissionEntity> _permissionRepository;
    private readonly IMapper _mapper;
    private readonly GetAllPermissionsHandler _sut;

    public GetAllPermissionsHandlerTests()
    {
        _permissionRepository = Substitute.For<IRepository<PermissionEntity>>();
        _mapper = Substitute.For<IMapper>();
        _sut = new GetAllPermissionsHandler(_permissionRepository, _mapper);
    }

    [Fact]
    public async Task Handle_ShouldReturnPermissions()
    {
        // Arrange
        var permissionEntities = new List<PermissionEntity>
        {
            new() { Id = 1, Name = "permission1" },
            new() { Id = 2, Name = "permission2" }
        };
        _permissionRepository.GetAsync().Returns(permissionEntities);

        var expected = new List<PermissionDTO>
        {
            new() { Id = 1, Name = "permission1" },
            new() { Id = 2, Name = "permission2" }
        };
        _mapper.Map<IEnumerable<PermissionDTO>>(permissionEntities).Returns(expected);

        // Act
        var actual = await _sut.Handle(new GetAllPermissionsQuery(5));

        // Assert
        actual.ShouldBe(expected);
    }
}