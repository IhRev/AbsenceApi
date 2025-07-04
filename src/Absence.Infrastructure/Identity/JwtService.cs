﻿using Absence.Application.Common.Constants;
using Absence.Application.Identity;
using Absence.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Absence.Infrastructure.Identity;

internal class JwtService(IOptions<JwtConfiguration> jwtConfiguration) : IJwtService
{
    private const string ALGORITHM = SecurityAlgorithms.HmacSha256;
    private readonly JwtConfiguration _jwtConfiguration = jwtConfiguration.Value;

    public string GenerateToken(UserEntity user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(CustomClaimTypes.ShortId, user.ShortId.ToString())
        };

        var key = GetSigningKey();
        var credentials = new SigningCredentials(key, ALGORITHM);

        var token = new JwtSecurityToken(
            issuer: _jwtConfiguration.Issuer,
            audience: _jwtConfiguration.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtConfiguration.JwtTokenExpireTimeInMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = GetSigningKey()
        };

        var principal = new JwtSecurityTokenHandler().ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
           !jwtSecurityToken.Header.Alg.Equals(ALGORITHM, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token");
        }

        return principal;
    }

    private SymmetricSecurityKey GetSigningKey() => 
        new(Encoding.UTF8.GetBytes(_jwtConfiguration.Secret));
}