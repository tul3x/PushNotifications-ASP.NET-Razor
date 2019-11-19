using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Demo.AspNetCore.PushNotifications.Services.Sql
{
    public static class SqlApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseSqlPushSubscriptionStore(this IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                PushSubscriptionContext context = serviceScope.ServiceProvider.GetService<PushSubscriptionContext>();
                context.Database.EnsureCreated();
            }

            return app;
        }
    }
}
