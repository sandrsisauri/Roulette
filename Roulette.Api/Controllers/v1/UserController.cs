using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Roulette.Data.Models.Request;
using Roulette.Repository.Contract;
using Roulette.Entity;
using System.Threading;
using System.Threading.Tasks;
using Roulette.Helper;

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

        [HttpPost]
        [Authorize(Roles = Const.Admin)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Register([FromBody] CreateUserRequestModel input, CancellationToken cancellationToken)
        {
           var response = await _userRepository.CreateUser(input, cancellationToken);

            return StatusCode(response.StatusCode, response);
        }

        [AllowAnonymous]
        [HttpPost(nameof(RequestToken))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RequestToken([FromBody] LoginUserRequestModel input, CancellationToken cancellationToken)
        {
            var response = await _userRepository.GetToken(input, cancellationToken);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("{username}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUser([FromRoute]string username, CancellationToken cancellationToken)
        {
            var response = await _userRepository.GetUserByName(username, cancellationToken);
            return StatusCode(response.StatusCode, response);
        }
    }
}
