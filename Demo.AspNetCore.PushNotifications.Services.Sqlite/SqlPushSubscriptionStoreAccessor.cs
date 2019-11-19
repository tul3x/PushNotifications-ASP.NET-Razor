using Microsoft.Extensions.DependencyInjection;
using Demo.AspNetCore.PushNotifications.Services.Abstractions;

namespace Demo.AspNetCore.PushNotifications.Services.Sql
{
    internal class SqlPushSubscriptionStoreAccessor : IPushSubscriptionStoreAccessor
    {
        private IServiceScope _serviceScope;

        public IPushSubscriptionStore PushSubscriptionStore { get; private set; }

        public SqlPushSubscriptionStoreAccessor(IPushSubscriptionStore pushSubscriptionStore)
        {
            PushSubscriptionStore = pushSubscriptionStore;
        }

        public SqlPushSubscriptionStoreAccessor(IServiceScope serviceScope)
        {
            _serviceScope = serviceScope;
            PushSubscriptionStore = _serviceScope.ServiceProvider.GetService<IPushSubscriptionStore>();
        }

        public void Dispose()
        {
            PushSubscriptionStore = null;

            if (!(_serviceScope is null))
            {
                _serviceScope.Dispose();
                _serviceScope = null;
            }
        }
    }
}
