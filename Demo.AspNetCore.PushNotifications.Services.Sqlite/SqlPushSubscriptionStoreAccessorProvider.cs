using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Demo.AspNetCore.PushNotifications.Services.Abstractions;

namespace Demo.AspNetCore.PushNotifications.Services.Sql
{
    internal class SqlPushSubscriptionStoreAccessorProvider : IPushSubscriptionStoreAccessorProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IServiceProvider _serviceProvider;

        public SqlPushSubscriptionStoreAccessorProvider(IHttpContextAccessor httpContextAccessor, IServiceProvider serviceProvider)
        {
            _httpContextAccessor = httpContextAccessor;
            _serviceProvider = serviceProvider;
        }

        public IPushSubscriptionStoreAccessor GetPushSubscriptionStoreAccessor()
        {
            if (_httpContextAccessor.HttpContext is null)
            {
                return new SqlPushSubscriptionStoreAccessor(_serviceProvider.CreateScope());
            }
            else
            {
                return new SqlPushSubscriptionStoreAccessor(_httpContextAccessor.HttpContext.RequestServices.GetRequiredService<IPushSubscriptionStore>());
            }
        }
    }
}
