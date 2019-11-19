﻿using Microsoft.AspNetCore.Builder;
using Demo.AspNetCore.PushNotifications.Services.Sql;

namespace Demo.AspNetCore.PushNotifications.Services
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UsePushSubscriptionStore(this IApplicationBuilder app)
        {
            app.UseSqlPushSubscriptionStore();

            return app;
        }
    }
}
