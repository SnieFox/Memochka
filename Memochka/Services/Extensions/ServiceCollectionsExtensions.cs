using Memochka.Models.Entities;
using Memochka.Models.MemochkaDbContext;
using Memochka.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Memochka.Services.Extensions
{
    public static class ServiceCollectionsExtensions
    {
        public static IServiceCollection AddDbServices(this IServiceCollection services, string connectionString)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentException("Connection string must be not null or empty", nameof(connectionString));
            }

            services.AddDbContext<MemochkaContext>(db =>
                db.UseSqlServer(connectionString));

            services.AddTransient<IUser<User>, UserService>();
            services.AddTransient<IMeme, MemeService>();
            services.AddTransient<IArticle, ArticleService>();
            services.AddTransient<IAuthorization, AuthorizationService>();

            return services;
        }
    }
}
