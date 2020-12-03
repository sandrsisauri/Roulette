using AutoMapper;
using Roulette.Data.Models.Request;
using Roulette.Data.Models.Response;
using Roulette.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Roulette.Data.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserResponseModel>();
            CreateMap<UserResponseModel, User>();
            CreateMap<Winning, CreateWinningsRequestModel>();
            CreateMap<CreateWinningsRequestModel, Winning>();
            CreateMap<Bet, CreateBetRequestModel>();
            CreateMap<CreateBetRequestModel, Bet>();
            CreateMap<GameHistoryResponseModel, GameHistory>();
            CreateMap<GameHistory, GameHistoryResponseModel>();
            CreateMap<IEnumerable<GameHistory>, IEnumerable<GameHistoryResponseModel>>();
            CreateMap<IEnumerable<GameHistoryResponseModel>, IEnumerable<GameHistory>>();
            CreateMap<CreateJackpotRequestModel, Jackpot>();
            CreateMap<Jackpot, CreateJackpotRequestModel>();
        }
    }
}
