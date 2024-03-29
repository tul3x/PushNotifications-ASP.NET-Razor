﻿using System.Threading;
using System.Threading.Tasks;
using Lib.Net.Http.WebPush;

namespace Demo.AspNetCore.PushNotifications.Services.Abstractions
{
    public interface IPushNotificationService
    {
        string PublicKey { get; }

        Task SendNotificationAsync(PushSubscription subscription, PushMessageAuth message);

        Task SendNotificationAsync(PushSubscription subscription, PushMessageAuth message, CancellationToken cancellationToken);
    }
}
