using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Roulette.Data;
using Roulette.Repository.Contract;
using Roulette.Entity;
using Roulette.Helper.Statics;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Roulette.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private readonly RoleManager<Role> _roleManager;
        private IDbTransaction _transaction;
        private DataContext _context;
        private readonly IDbConnection _connection;

        public UserRepository(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IConfiguration configuration,
            RoleManager<Role> roleManager,
            DataContext contex,
            IDbTransaction transaction = default)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _roleManager = roleManager;
            _transaction = transaction;
            _connection = contex.Connection;
            if (_connection.State == ConnectionState.Closed) _connection.Open();
            _context = contex;
        }

        public async Task<JwtSecurityToken> GenerateToken(User user)
        {
            var signinKey = new SymmetricSecurityKey(
             Encoding.UTF8.GetBytes(_configuration["Jwt:SigningKey"]));

            int expiryInMinutes = Convert.ToInt32(_configuration["Jwt:ExpiryInMinutes"]);

            var claims = await GetValidClaims(user);
            var token = new JwtSecurityToken(
               issuer: _configuration["Jwt:Issuer"],
               audience: _configuration["Jwt:Issuer"],
               expires: DateTime.UtcNow.AddMinutes(expiryInMinutes),
               signingCredentials: new SigningCredentials(signinKey, SecurityAlgorithms.HmacSha256),
               claims: claims
             );
            return token;
        }

        private async Task<List<Claim>> GetValidClaims(User user)
        {
            IdentityOptions _options = new IdentityOptions();
            var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
            new Claim(_options.ClaimsIdentity.UserIdClaimType, user.Id.ToString()),
            new Claim(_options.ClaimsIdentity.UserNameClaimType, user.UserName),
            new Claim("UserId", user.Id.ToString())
        };
            var userClaims = await _userManager.GetClaimsAsync(user);
            var userRoles = await _userManager.GetRolesAsync(user);
            claims.AddRange(userClaims);
            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole));
                var role = await _roleManager.FindByNameAsync(userRole);
                if (role != null)
                {
                    var roleClaims = await _roleManager.GetClaimsAsync(role);
                    foreach (Claim roleClaim in roleClaims)
                    {
                        claims.Add(roleClaim);
                    }
                }
            }
            return claims;
        }

        public async Task Delete(object Id)
        {
            if (Id != null)
            {
                string sQuery = "DELETE FROM RouletteUsers"
                                    + " WHERE Id = @Id";

                await _connection.ExecuteAsync(sQuery, new { Id }, _transaction);
            }
        }
    }
}
