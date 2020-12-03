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
        private readonly IRouletteRepository _rouletteRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RouletteController(IRouletteRepository rouletteRepository,
                                  IUnitOfWork unitOfWork)
        {
            this._rouletteRepository = rouletteRepository;
            _unitOfWork = unitOfWork;
        }

        [HttpPost(nameof(Bet))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Bet([FromBody] BetRequestModel request, CancellationToken cancellationToken)
        {
            var token = await HttpContext.GetTokenAsync(Const.access_token);

            var response = await _unitOfWork.RouletteRepository.BetIsValidHandlerAsync(request, GetUserIdFromTokenAsync(token), cancellationToken);
            await _unitOfWork.Commit();

            return StatusCode(response.StatusCode, response);
        }

        [HttpGet(nameof(GameHistory))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GameHistory(CancellationToken cancellationToken)
        {
            var token = await HttpContext.GetTokenAsync(Const.access_token);
            var response = await _rouletteRepository.GetGameHistoryByUserAsync(GetUserIdFromTokenAsync(token), cancellationToken);

            return StatusCode(response.StatusCode, response);
        }

        [HttpGet(nameof(JackPot))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> JackPot(CancellationToken cancellationToken)
        {
            var response = await _rouletteRepository.GetJackpotSumAsync(cancellationToken);

            return StatusCode(response.StatusCode, response.Data);
        }

        [HttpGet("User/Balance")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UserBalance(CancellationToken cancellationToken)
        {
            var token = await HttpContext.GetTokenAsync(Const.access_token);
            var response = await _rouletteRepository.GetUserBalanceAsync(GetUserIdFromTokenAsync(token), cancellationToken);

            return StatusCode(response.StatusCode, response);
        }
    }
}
