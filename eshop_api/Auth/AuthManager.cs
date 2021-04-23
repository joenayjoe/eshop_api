using eshop_api.DTO;
using eshop_api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace eshop_api.Auth
{
    public class AuthManager : IAuthManager
    {
        private readonly UserManager<AppUser> userManager;
        private readonly IConfiguration configuration;
        private AppUser user;

        public AuthManager(UserManager<AppUser> userManager, IConfiguration configuration)
        {
            this.userManager = userManager;
            this.configuration = configuration;
        }
        public async Task<string> CreateToken()
        {
            var signingCredentials = GetSigninCredentials();
            var claims = await GetClaims();
            var token = GenerateToken(signingCredentials, claims);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private JwtSecurityToken GenerateToken(SigningCredentials signingCredentials, IList<Claim> claims)
        {
            var token = new JwtSecurityToken(
                    issuer: configuration.GetSection("JWT:ValidIssuer").Value,
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(Double.Parse(configuration.GetSection("JWT:Expires").Value)),
                    signingCredentials: signingCredentials
                );
            return token;
        }

        private async Task<IList<Claim>> GetClaims()
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email)
            };

            var roles = await userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            return claims;
        }

        private SigningCredentials GetSigninCredentials()
        {
            var key = configuration.GetSection("JWT:Secret").Value;
            var secret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }

        public async Task<AppUser> ValidateUser(UserLoginDTO loginDTO)
        {
            user = await userManager.FindByEmailAsync(loginDTO.Email);
            if(user != null && await userManager.CheckPasswordAsync(user, loginDTO.Password))
            {
                return user;
            }
            return null;
        }
    }
}
