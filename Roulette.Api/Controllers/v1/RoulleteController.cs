using AutoMapper;
using ge.singular.roulette;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Roulette.Data.Models.Request;
using Roulette.Data.Models.Response;
using Roulette.Repository.Contract;
using Roulette.Entity;
using Roulette.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Roulette.Api.Controllers.v1
{
    public class RouletteController : ControllerBaseEx
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRouletteRepository _RouletteRepository;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private readonly RoleManager<Role> _roleManager;

        public RouletteController(UserManager<User> userManager,
                              SignInManager<User> signInManager,
                              IConfiguration configuration,
                              RoleManager<Role> roleManager,
                              IUserRepository userRepository,
                              IUnitOfWork unitOfWork,
                              IRouletteRepository RouletteRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _RouletteRepository = RouletteRepository;
            _configuration = configuration;
            _roleManager = roleManager;
        }

        [HttpPost(nameof(Bet))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Bet([FromBody] BetRequestModel request, CancellationToken cancellationToken)
        {
            var userId = GetUserIdFromTokenAsync(await HttpContext.GetTokenAsync(Const.access_token));
            var response = await _RouletteRepository.BetIsValidHandlerAsync(request, userId, cancellationToken);

            return StatusCode(response.StatusCode, response);
        }

        [HttpGet(nameof(GameHistory))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GameHistory(CancellationToken cancellationToken)
        {
            var token = await HttpContext.GetTokenAsync(Const.access_token);
            var response = await _RouletteRepository.GetGameHistoryByUser(GetUserIdFromTokenAsync(token), cancellationToken);

            return StatusCode(response.StatusCode, response);
        }

        [HttpGet(nameof(JackPot))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> JackPot(CancellationToken cancellationToken)
        {
            var response = await _RouletteRepository.GetJackpotSum(cancellationToken);

            return StatusCode(response.StatusCode, response.Data);
        }

        [HttpGet("User/UserBalance")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UserBalance(CancellationToken cancellationToken)
        {
            var token = await HttpContext.GetTokenAsync(Const.access_token);
            var response = await _RouletteRepository.GetUserBalance(GetUserIdFromTokenAsync(token), cancellationToken);

            return StatusCode(response.StatusCode, response);
        }

        #region BetIsValidOperations
        private async Task<IActionResult> BetIsValidHandler(BetRequestModel request, IsBetValidResponse ibvr, CancellationToken cancellationToken)
        {
            var token = await HttpContext.GetTokenAsync(Const.access_token);
            var betAmount = ibvr.getBetAmount();
            var userId = GetUserIdFromTokenAsync(token);

            var userBalance = await _RouletteRepository.GetUserBalance(userId, cancellationToken);
            if (betAmount > userBalance.Data.Balance)
                return BadRequest(new Response<BetResponseModel>() { Message = "bet amount is more than balance" });

            var winNum = new Random().Next(0, 36);
            var estWin = CheckBets.EstimateWin(request.Bet, winNum);
            if (estWin > 0)
                await _RouletteRepository.AddWinToUserBalance(userId, estWin, cancellationToken);

            await _RouletteRepository.SubstractBetFromBalance(userId, betAmount, cancellationToken);

            cancellationToken.ThrowIfCancellationRequested();

            var betId = await _RouletteRepository.CreateBet(
                        new CreateBetRequestModel()
                        {
                            BetString = request.Bet,
                            BetAmount = betAmount,
                            UserId = userId
                        }, cancellationToken);

            _ = await _RouletteRepository.CreateWinnings(
                        new CreateWinningsRequestModel()
                        {
                            WinningNumber = winNum,
                            WonAmount = estWin,
                            UserId = userId,
                            BetId = betId
                        }, cancellationToken);

            _ = await _RouletteRepository.CreateJackPot(
                       new CreateJackpotRequestModel()
                       {
                           Amount = (decimal)(betAmount * 0.01),
                           BetId = betId
                       }, cancellationToken);


            return Ok(new Response<BetResponseModel>()
            {
                Data = new BetResponseModel()
                {
                    BetAmount = betAmount,
                    WinningAmount = estWin
                }
            });
        }
        #endregion
    }
}
