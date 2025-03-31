using Absence.Application.Common.Abstractions;
using Absence.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Absence.Infrastructure.Repositories;

internal class UserRepository(
    UserManager<UserEntity> userManager,
    IUserClaimsPrincipalFactory<UserEntity> userClaimsPrincipalFactory,
    IAuthorizationService authorizationService
) : IUserRepository
{
    private readonly UserManager<UserEntity> _userManager = userManager;
    private readonly IUserClaimsPrincipalFactory<UserEntity> _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
    private readonly IAuthorizationService _authorizationService = authorizationService;

    //public async Task<string?> GetUserNameAsync(string userId)
    //{
    //    var user = await _userManager.FindByIdAsync(userId);

    //    return user?.UserName;
    //}

    public async Task CreateUserAsync(UserEntity user, CancellationToken cancellationToken = default)
    {
        await _userManager.CreateAsync(user, "TestPassword123!");
    }

    //public async Task<bool> IsInRoleAsync(string userId, string role)
    //{
    //    var user = await _userManager.FindByIdAsync(userId);

    //    return user != null && await _userManager.IsInRoleAsync(user, role);
    //}

    //public async Task<bool> AuthorizeAsync(string userId, string policyName)
    //{
    //    var user = await _userManager.FindByIdAsync(userId);

    //    if (user == null)
    //    {
    //        return false;
    //    }

    //    var principal = await _userClaimsPrincipalFactory.CreateAsync(user);

    //    var result = await _authorizationService.AuthorizeAsync(principal, policyName);

    //    return result.Succeeded;
    //}

    //public async Task DeleteUserAsync(string userId)
    //{
    //    var user = await _userManager.FindByIdAsync(userId);
    //    if (user is null)
    //    {
    //        return;
    //    }

    //    await DeleteUserAsync(user);
    //}

    //public Task DeleteUserAsync(UserEntity user) => 
    //    _userManager.DeleteAsync(user);
}