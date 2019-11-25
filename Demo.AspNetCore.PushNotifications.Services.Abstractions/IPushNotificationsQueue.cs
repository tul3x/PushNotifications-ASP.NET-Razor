using System.Threading;
using System.Threading.Tasks;
using Lib.Net.Http.WebPush;

namespace Demo.AspNetCore.PushNotifications.Services.Abstractions
{
    public interface IPushNotificationsQueue
    {
        void Enqueue(PushMessageAuth message);

        Task<PushMessageAuth> DequeueAsync(CancellationToken cancellationToken);
    }
}
