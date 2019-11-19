using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Demo.AspNetCore.PushNotifications.Services.Abstractions;

namespace Demo.AspNetCore.PushNotifications.Services.Sql
{
    public static class SqlServiceCollectionExtensions
    {
        private const string SQL_CONNECTIONSTRING = "TestSQLServer";

        public static IServiceCollection AddSqlPushSubscription(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<PushSubscriptionContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString(SQL_CONNECTIONSTRING)));

            services.AddTransient<IPushSubscriptionStore, SqlPushSubscriptionStore>();
            services.AddHttpContextAccessor();
            services.AddSingleton<IPushSubscriptionStoreAccessorProvider, SqlPushSubscriptionStoreAccessorProvider>();

            return services;
        }
    }
}
