using KProject.Infrastructure.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KProject.Infrastructure;

public static class DependencyInjection
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddInfrastructure(IConfiguration configuration) => services
            .AddDatabase(configuration)
            .AddIdentity()
            .AddAuthentication()
            .AddAuthorization();

        private IServiceCollection AddDatabase(IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
            {
                var connectionString = configuration.GetConnectionString("Database");

                options.UseNpgsql(connectionString).UseSnakeCaseNamingConvention();
            });

            return services;
        }

        private IServiceCollection AddIdentity()
        {
            //IdentityCore adiciona o basico que a gente precisa
            //SignInManager porque o IdentityCore nao tem e a gente precisa pra gerar o cookie/bearer automaticamente
            //AddEntityFrameworkStores adiciona as tabelas necessarias baseadas em quem o AppDbContext herda - nesse caso, IdentityUserContext
        
            services
                .AddIdentityCore<IdentityUser<int>>()
                .AddErrorDescriber<ErrorDescriber>()
                .AddSignInManager<SignInManager<IdentityUser<int>>>()
                .AddEntityFrameworkStores<AppDbContext>();
        
            return services;
        }

        private IServiceCollection AddAuthentication()
        {
            //Precisamos usar os schemes do Identity porque eh o que o SignInManager usa pra realizar os processos
        
            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme =
                        IdentityConstants.ApplicationScheme;
                    options.DefaultChallengeScheme =
                        IdentityConstants.ApplicationScheme;
                })
                .AddCookie(IdentityConstants.ApplicationScheme, options =>
                {
                    options.LoginPath = "/users/login";
                    options.LogoutPath = "/users/logout";
                    options.AccessDeniedPath = "/users/access-denied"; 
                });

            return services;
        }
    }
}