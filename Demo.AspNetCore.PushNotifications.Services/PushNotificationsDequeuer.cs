﻿using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Lib.Net.Http.WebPush;
using Demo.AspNetCore.PushNotifications.Services.Abstractions;
using System.IO;
using System;

namespace Demo.AspNetCore.PushNotifications.Services
{
    internal class PushNotificationsDequeuer : IHostedService
    {
        private readonly IPushSubscriptionStoreAccessorProvider _subscriptionStoreAccessorProvider;
        private readonly IPushNotificationsQueue _messagesQueue;
        private readonly IPushNotificationService _notificationService;
        private readonly CancellationTokenSource _stopTokenSource = new CancellationTokenSource();

        private Task _dequeueMessagesTask;

        public PushNotificationsDequeuer(IPushNotificationsQueue messagesQueue, IPushSubscriptionStoreAccessorProvider subscriptionStoreAccessorProvider, IPushNotificationService notificationService)
        {
            _subscriptionStoreAccessorProvider = subscriptionStoreAccessorProvider;
            _messagesQueue = messagesQueue;
            _notificationService = notificationService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _dequeueMessagesTask = Task.Run(DequeueMessagesAsync);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _stopTokenSource.Cancel();

            return Task.WhenAny(_dequeueMessagesTask, Task.Delay(Timeout.Infinite, cancellationToken));
        }

        private async Task DequeueMessagesAsync()
        {
            while (!_stopTokenSource.IsCancellationRequested)
            {
                PushMessageAuth message = await _messagesQueue.DequeueAsync(_stopTokenSource.Token);

                if(message != null)
                {
                    string now = DateTime.Now.ToString("dd.MM.yyy-HH:mm:ss");
                    File.AppendAllText("log.txt", now + ": Nachricht erfolgreich Dequeued: " + message.Content + Environment.NewLine);
                }

                if (!_stopTokenSource.IsCancellationRequested)
                {
                    using (IPushSubscriptionStoreAccessor subscriptionStoreAccessor = _subscriptionStoreAccessorProvider.GetPushSubscriptionStoreAccessor())
                    {
                        await subscriptionStoreAccessor.PushSubscriptionStore.PerformSubscriptionAsync((PushSubscription subscription) =>
                        {
                            // Fire-and-forget 
                            _notificationService.SendNotificationAsync(subscription, message, _stopTokenSource.Token);
                        }, _stopTokenSource.Token, message.Auth);
                    }

                }
            }

        }
    }
}
