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
using Roulette.Helper.Statics;
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
            this._RouletteRepository = RouletteRepository;
            _configuration = configuration;
            _roleManager = roleManager;
        }

        [HttpPost("Bet")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Bet([FromBody] BetRequestModel request, CancellationToken cancellationToken)
        {
            if(string.IsNullOrEmpty(request.Bet))
            return BadRequest(nameof(Bet));

            var token = await HttpContext.GetTokenAsync(Const.access_token);

            var ibvr = CheckBets.IsValid(request.Bet);
            if (ibvr.getIsValid())
                return await BetIsValidHandler(request, token, ibvr, cancellationToken);

            return BadRequest(new Response<BetResponseModel>() { Message = "Oops something went wrong ..." });
        }

        [HttpGet("GameHistory")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GameHistory(CancellationToken cancellationToken)
        {
            var token = await HttpContext.GetTokenAsync(Const.access_token);
            var userid = GetUserIdFromToken(token);

            cancellationToken.ThrowIfCancellationRequested();
            var gameHistory = await _RouletteRepository.GetGameHistoryByUser(userid);
            if (!gameHistory.Any())
                return NotFound(nameof(gameHistory));

            return Ok(new Response<IEnumerable<GameHistoryResponseModel>>()
            {
                Data = Mapper.Map<IEnumerable<GameHistoryResponseModel>>(gameHistory)
            });
        }
        
        [HttpGet("Jackpot")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> JackPot(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var jacpotAmount = await _RouletteRepository.GetJackpotSum();

            return Ok(new Response<JackpotSumResponseModel>()
            {
                Data = new JackpotSumResponseModel()
                {
                    JackpotAmount = jacpotAmount.JackpotAmount,
                    LastModifietAt = jacpotAmount.LastModifietAt
                }
            });
        }

        [HttpGet("User/UserBalance")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UserBalance(CancellationToken cancellationToken)
        {
            var token = await HttpContext.GetTokenAsync(Const.access_token);
            var userid = GetUserIdFromToken(token);

            cancellationToken.ThrowIfCancellationRequested();
            var userBalance = await _RouletteRepository.GetUserBalance(userid);

            return Ok(new Response<UserBalanceResponseModel>()
            {
                Data = new UserBalanceResponseModel()
                {
                    Balance = userBalance.Balance,
                    UserId = userBalance.UserId,
                    UserName = userBalance.UserName
                }
            });
        }

        #region BetIsValidOperations
        private async Task<IActionResult> BetIsValidHandler(BetRequestModel request, string token, IsBetValidResponse ibvr, CancellationToken cancellationToken)
        {
            var userId = GetUserIdFromToken(token);
            var betAmount = ibvr.getBetAmount();

            var userBalance = await _RouletteRepository.GetUserBalance(userId);
            if (betAmount > userBalance.Balance)
                return BadRequest(new Response<BetResponseModel>() { Message = "bet amount is more than balance" });

            var winNum = new Random().Next(0, 36);
            var estWin = CheckBets.EstimateWin(request.Bet, winNum);
            if (estWin > 0)
                await _RouletteRepository.AddWinToUserBalance(userId, estWin);

            await _RouletteRepository.SubstractBetFromBalance(userId, betAmount);

            cancellationToken.ThrowIfCancellationRequested();

            var betId = await _RouletteRepository.CreateBet(
                        new CreateBetRequestModel()
                        {
                            BetString = request.Bet,
                            BetAmount = betAmount,
                            UserId = userId
                        });

            _ = await _RouletteRepository.CreateWinnings(
                        new CreateWinningsRequestModel()
                        {
                            WinningNumber = winNum,
                            WonAmount = estWin,
                            UserId = userId,
                            BetId = betId
                        });

            _ = await _RouletteRepository.CreateJackPot(
                       new CreateJackpotRequestModel()
                       {
                           Amount = (decimal)(betAmount * 0.01),
                           BetId = betId
                       });


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
