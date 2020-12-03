using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Roulette.Repository.Contract
{
    public interface IUnitOfWork
    {
        IUserRepository UserRepository { get; }

        Task Commit();
    }
}
