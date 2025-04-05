using Absence.Domain.Entities;
using Absence.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Absence.Infrastructure.Identity;

internal class UserService(
    UserManager<UserEntity> userManager,
    IUserClaimsPrincipalFactory<UserEntity> userClaimsPrincipalFactory,
    IAuthorizationService authorizationService
) : IUserService
{
    private readonly UserManager<UserEntity> _userManager = userManager;
    private readonly IUserClaimsPrincipalFactory<UserEntity> _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
    private readonly IAuthorizationService _authorizationService = authorizationService;

    public Task<IdentityResult> CreateAsync(UserEntity user, string password) => 
        _userManager.CreateAsync(user, password);

    public Task<UserEntity?> FindByEmailAsync(string email) =>
        _userManager.FindByEmailAsync(email);

    public Task<bool> CheckPasswordAsync(UserEntity user, string password) => 
        _userManager.CheckPasswordAsync(user, password);
}