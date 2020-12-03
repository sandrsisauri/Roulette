using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Roulette.Data.Models.Request;
using Roulette.Data.Models.Response;
using Roulette.Repository.Contract;
using Roulette.Entity;
using Roulette.Helper.Statics;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Roulette.Api.Controllers.v1
{
    public class UserController : ControllerBaseEx
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private readonly RoleManager<Role> _roleManager;

        public UserController(UserManager<User> userManager,
                              SignInManager<User> signInManager,
                              IConfiguration configuration,
                              RoleManager<Role> roleManager,
                              IUserRepository userRepository,
                              IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _roleManager = roleManager;
        }

        [HttpPost()]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] CreateUserRequestModel input, CancellationToken cancellationToken)
        {
            var hashedPassword = Crypto.HashPassword(input.Password);

            var user = new User() { UserName = input.UserName, PasswordHash = hashedPassword };

            cancellationToken.ThrowIfCancellationRequested();

            var result = await _userManager.CreateAsync(user);
            await _userManager.AddToRoleAsync(user, "User");

            if (result.Succeeded)
                return Created("api/user/" + nameof(Register),
                    new
                    {
                        userId = user.Id,
                        token = new JwtSecurityTokenHandler().WriteToken(await _userRepository.GenerateToken(user))
                    });

            return BadRequest(result);
        }

        [AllowAnonymous]
        [HttpPost("RequestToken")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RequestToken([FromBody] LoginUserRequestModel input, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByNameAsync(input.UserName);
            if (user == null)
                return BadRequest(nameof(user));

            if (Crypto.VerifyHashedPassword(user.PasswordHash, input.Password))
            {
                cancellationToken.ThrowIfCancellationRequested();
                JwtSecurityToken token = await _userRepository.GenerateToken(user);

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token)
                });
            }
            return BadRequest();
        }

        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetUser(string userName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
                return BadRequest(nameof(user));

            return Ok(Mapper.Map<UserResponseModel>(user));
        }

        [HttpDelete()]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(Guid userId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (userId == Guid.Empty)
                return BadRequest();

            await _unitOfWork.UserRepository.Delete(userId);
            await _unitOfWork.Commit();

            return NoContent();
        }
    }
}
