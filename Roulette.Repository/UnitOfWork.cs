using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Roulette.Data;
using Roulette.Repository.Contract;
using Roulette.Entity;
using System.Data;
using System.Threading.Tasks;

namespace Roulette.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private readonly RoleManager<Role> _roleManager;
        private DataContext _context;
        private IUserRepository _userRepository;
        private readonly IDbTransaction _transaction;

        public UnitOfWork(UserManager<User> userManager,
                          SignInManager<User> signInManager,
                          IConfiguration configuration,
                          RoleManager<Role> roleManager,
                          DataContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _roleManager = roleManager;
            _context = context;
            if (_context.Connection.State == ConnectionState.Closed) _context.Connection.Open();
            _transaction = context.Connection.BeginTransaction();
        }

        public async Task Commit()
        {
            await Task.Run(() =>
            {
                if (_transaction != null)
                {
                    try
                    {
                        _transaction.Commit();
                    }
                    catch
                    {
                        _transaction.Rollback();
                        throw;
                    }
                    finally
                    {
                        _transaction.Dispose();
                        ResetRepositories();
                    }
                }
            });

        }
        public IUserRepository UserRepository
        {
            get { return _userRepository = new UserRepository(_userManager, _signInManager, _configuration, _roleManager, _context, _transaction); }
        }
        private void ResetRepositories()
        {
            _userRepository = null;
        }

        public void Dispose()
        {
            if (_transaction != null)
            {
                _transaction.Dispose();
            }
        }
    }
}
