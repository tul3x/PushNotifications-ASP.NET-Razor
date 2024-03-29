﻿using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Lib.Net.Http.WebPush;
using Demo.AspNetCore.PushNotifications.Services.Abstractions;
using System.IO;

namespace Demo.AspNetCore.PushNotifications.Services.PushService
{
    internal class PushServicePushNotificationService : IPushNotificationService
    {
        private readonly PushServiceClient _pushClient;
        private readonly IPushSubscriptionStoreAccessorProvider _subscriptionStoreAccessorProvider;

        private readonly ILogger _logger;

        public string PublicKey { get { return _pushClient.DefaultAuthentication.PublicKey; } }

        public PushServicePushNotificationService(PushServiceClient pushClient, IPushSubscriptionStoreAccessorProvider subscriptionStoreAccessorProvider, ILogger<PushServicePushNotificationService> logger)
        {
            _pushClient = pushClient;

            _subscriptionStoreAccessorProvider = subscriptionStoreAccessorProvider;
            _logger = logger;
        }

        public Task SendNotificationAsync(PushSubscription subscription, PushMessageAuth message)
        {
            return SendNotificationAsync(subscription, message, CancellationToken.None);
        }

        public async Task SendNotificationAsync(PushSubscription subscription, PushMessageAuth message, CancellationToken cancellationToken)
        {
            try
            {
                await _pushClient.RequestPushMessageDeliveryAsync(subscription, message, cancellationToken);
                File.AppendAllText("log.txt", "Nachricht erfolgreich an die Push-Dienst geleitet" + Environment.NewLine);
            }
            catch (Exception ex)
            {
                await HandlePushMessageDeliveryExceptionAsync(ex, subscription);
                File.AppendAllText("log.txt", "Fehler beim Senden der Nachricht: " + ex.Message + " - " + ex.InnerException + Environment.NewLine);
            }
        }

        private async Task HandlePushMessageDeliveryExceptionAsync(Exception exception, PushSubscription subscription)
        {
            PushServiceClientException pushServiceClientException = exception as PushServiceClientException;

            if (pushServiceClientException is null)
            {
                _logger?.LogError(exception, "Failed requesting push message delivery to {0}.", subscription.Endpoint);
            }
            else
            {
                if ((pushServiceClientException.StatusCode == HttpStatusCode.NotFound) || (pushServiceClientException.StatusCode == HttpStatusCode.Gone))
                {
                    using (IPushSubscriptionStoreAccessor subscriptionStoreAccessor = _subscriptionStoreAccessorProvider.GetPushSubscriptionStoreAccessor())
                    {
                        await subscriptionStoreAccessor.PushSubscriptionStore.DiscardSubscriptionAsync(subscription.Endpoint);
                    }
                    _logger?.LogInformation("Subscription has expired or is no longer valid and has been removed.");
                }
            }
        }
    }
}
