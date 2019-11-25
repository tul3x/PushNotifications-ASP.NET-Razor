using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Lib.Net.Http.WebPush;
using Demo.AspNetCore.PushNotifications.Services.Abstractions;

namespace Demo.AspNetCore.PushNotifications.Services
{
    internal class PushNotificationsQueue : IPushNotificationsQueue
    {
        private readonly ConcurrentQueue<PushMessageAuth> _messages = new ConcurrentQueue<PushMessageAuth>();
        private readonly SemaphoreSlim _messageEnqueuedSignal = new SemaphoreSlim(0);

        public void Enqueue(PushMessageAuth message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            _messages.Enqueue(message);

            _messageEnqueuedSignal.Release();
        }

        public async Task<PushMessageAuth> DequeueAsync(CancellationToken cancellationToken)
        {
            await _messageEnqueuedSignal.WaitAsync(cancellationToken);

            _messages.TryDequeue(out PushMessageAuth message);

            return message;
        }
    }
}
