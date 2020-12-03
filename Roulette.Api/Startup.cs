using AspNetCore.Identity.Dapper;
using AutoMapper;
using FluentMigrator.Runner;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using NSwag;
using NSwag.Generation.Processors.Security;
using Roulette.Api.Exstensions;
using Roulette.Data;
using Roulette.Data.Mapper;
using Roulette.Data.Migrations;
using Roulette.Repository;
using Roulette.Repository.Contract;
using Roulette.Entity;
using Roulette.Helper;
using System.Text;

namespace Roulette.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }


        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            AddIdentity(services);
            AddSwagger(services);
            services.AddControllers();
            AddFluentMigration(services);
            AddAuthentication(services);
            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddHealthChecks();
            AddMapper();
            AddDependencies(services);
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, SeedDataContext seeder)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMiddleware(typeof(ErrorHandlingMiddleware));
            app.UseMiddleware(typeof(HttpContextMiddleware));
            app.UseMiddleware<AccessTokenMiddleware>();

            //migration and seeding
            using var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
            if (Configuration["ConnectionStrings:UseMigrationService"] == "True")
            {
                var runner = serviceScope.ServiceProvider.GetRequiredService<IMigrationRunner>();

                DatabaseHelper.CreateIfNotExists(Configuration.GetConnectionString(Const.RouletteConnectionString));

                //if you want migrate up again. remove versioninfo rows
                runner.MigrateUp();

                seeder.Seed().Wait();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseOpenApi();

            app.UseSwaggerUi3(settings =>
            {
                settings.Path = "/api";
            });

            app.UseHealthChecks("/health", new HealthCheckOptions()
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        #region Private Methods
        private void AddDependencies(IServiceCollection services)
        {
            services.AddTransient(implementationFactory: sp => new DataContext(Configuration.GetConnectionString(Const.RouletteConnectionString)));

            services.AddTransient<IUserRepository, UserRepository>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddTransient<IRouletteRepository, RouletteRepository>();

            services.AddTransient<SeedDataContext>();
        }

        private static void AddMapper()
        {
#if DEBUG
            Mapper.Reset();
#endif
            Mapper.Initialize((config) =>
            {
                config.AddProfile<MappingProfile>();
            });
        }

        private void AddAuthentication(IServiceCollection services)
        {
            services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = Configuration["Jwt:Issuer"],
                    ValidIssuer = Configuration["Jwt:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:SigningKey"]))
                };
            });
        }

        private void AddFluentMigration(IServiceCollection services)
        {
            //fluent migration services
            services.AddFluentMigratorCore().
                     ConfigureRunner(rb => rb
                    .WithGlobalConnectionString(Configuration.GetConnectionString(Const.RouletteConnectionString))
                    .AddSqlServer()
                    .WithMigrationsIn(new System.Reflection.Assembly[] { typeof(Migration_Initial).Assembly

    })
                    // Define the assembly containing the migrations
                    .ScanIn(typeof(Migration_Initial).Assembly).For.Migrations())
                // Build the service provider
                .BuildServiceProvider(false);
        }

        private static void AddSwagger(IServiceCollection services)
        {
            services.AddSwaggerDocument(config =>
            {
                config.SerializerSettings = new Newtonsoft.Json.JsonSerializerSettings()
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    Converters = { new StringEnumConverter() },
                };
                config.OperationProcessors.Add(new OperationSecurityScopeProcessor("JWT Token"));
                config.DocumentProcessors.Add(new SecurityDefinitionAppender("JWT Token", new OpenApiSecurityScheme
                {
                    Type = OpenApiSecuritySchemeType.ApiKey,
                    Name = "Authorization",
                    Description = "Type 'Bearer ' + valid JWT token into field",
                    In = OpenApiSecurityApiKeyLocation.Header
                }));

                config.PostProcess = swagger =>
                {
                    swagger.Info.Title = "Roulette API";
                    swagger.Info.Contact = new OpenApiContact
                    {
                        Name = "Aleksandre Sisauri",
                        Email = "Sisauri.Aleksandre@gmail.com",
                    };
                };
            });
        }

        private void AddIdentity(IServiceCollection services)
        {
            services.AddIdentity<User, Role>(settings =>
            {
                settings.Lockout.MaxFailedAccessAttempts = 3;
                settings.Password.RequiredLength = 3;
                settings.Password.RequireNonAlphanumeric = false;
                settings.Password.RequireUppercase = false;
                settings.Password.RequireDigit = false;
                settings.Password.RequireLowercase = false;
                settings.User.RequireUniqueEmail = false;
            })
                .AddDapperStores(new SqlServerProvider(
                    Configuration.GetConnectionString(Const.RouletteConnectionString),
                    new SqlConfiguration(new System.Data.SqlClient.SqlConnectionStringBuilder(Configuration.GetConnectionString(Const.RouletteConnectionString)).InitialCatalog, "RouletteUsers", "RouletteRoles", "RouletteUserClaims", "RouletteUserRoles", "RouletteUserLogins", "RouletteRoleClaims", "RouletteUserTokens")))
                .AddDefaultTokenProviders();
        }
        #endregion
    }
}
