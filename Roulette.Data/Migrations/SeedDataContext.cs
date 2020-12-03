using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Roulette.Data.Models;
using Roulette.Entity;
using Roulette.Helper.Statics;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Roulette.Data.Migrations
{
    public class SeedDataContext
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IEnumerable<SeedUserModel> seedUserModel;
        private readonly IEnumerable<SeedRoleModel> seedRoleModel;

        public SeedDataContext(IConfiguration configuration,
            UserManager<User> signInManager,
            RoleManager<Role> roleManager)
        {
            _userManager = signInManager;
            _roleManager = roleManager;
            Configuration = configuration;
            seedUserModel = Configuration.GetSection("UserSeed").Get<IEnumerable<SeedUserModel>>();
            seedRoleModel = Configuration.GetSection("RoleSeed").Get<IEnumerable<SeedRoleModel>>();
        }

        public IConfiguration Configuration { get; }
        public async Task Seed()
        {
            await SeedRole();
            await SeedUser();
        }
        private async Task SeedRole()
        {
            foreach (var role in seedRoleModel)
            {
                if (!await _roleManager.RoleExistsAsync(role.Name))
                {
                    await _roleManager.CreateAsync(new Role() { Name = role.Name, NormalizedName = role.Name.ToUpper() });
                }
            }
        }
        private async Task SeedUser()
        {
            foreach (var seededuser in seedUserModel)
            {
                if (await _userManager.FindByNameAsync(seededuser.UserName) == null)
                {
                    var user = new User()
                    {
                        UserName = seededuser.UserName,
                        PasswordHash = Crypto.HashPassword(seededuser.Password)
                    };

                    await _userManager.CreateAsync(user);

                    await _userManager.AddToRoleAsync(user, "Admin");
                }
            }
        }
    }
}
