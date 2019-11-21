using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Lib.Net.Http.WebPush;
using Demo.AspNetCore.PushNotifications.Services.Abstractions;
using System.Linq;

namespace Demo.AspNetCore.PushNotifications.Services.Sql
{
    internal class SqlPushSubscriptionStore : IPushSubscriptionStore
    {
        private readonly PushSubscriptionContext _context;

        public SqlPushSubscriptionStore(PushSubscriptionContext context)
        {
            _context = context;
        }

        public Task StoreSubscriptionAsync(PushSubscription subscription)
        {
            _context.Subscriptions.Add(new PushSubscriptionContext.PushSubscription(subscription));

            return _context.SaveChangesAsync();
        }

        public async Task DiscardSubscriptionAsync(string endpoint)
        {
            PushSubscriptionContext.PushSubscription subscription = await _context.Subscriptions.FindAsync(endpoint);

            _context.Subscriptions.Remove(subscription);

            await _context.SaveChangesAsync();
        }

        public Task ForEachSubscriptionAsync(Action<PushSubscription> action)
        {
            return this.ForEachSubscriptionAsync(action, CancellationToken.None);
        }

        public Task ForEachSubscriptionAsync(Action<PushSubscription> action, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() =>
            {
                PushSubscriptionContext.PushSubscription sub = _context.Subscriptions.AsNoTracking().FirstOrDefault(s => s.Auth == "4dhi8p0TtqOCJmUiHKjEug");

                action(sub);
            });
        }
    }
}
