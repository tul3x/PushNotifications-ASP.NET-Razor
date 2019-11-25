using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Lib.Net.Http.WebPush;
using Demo.AspNetCore.PushNotifications.Model;
using Demo.AspNetCore.PushNotifications.Services.Abstractions;
using Microsoft.AspNetCore.Http;
using System;
using System.Net.Http;
using Demo.AspNetCore.PushNotifications.Services;

namespace Demo.AspNetCore.PushNotifications.Controllers
{
    [Route("push-notifications-api")]
    public class PushNotificationsApiController : Controller
    {
        private readonly IPushSubscriptionStore _subscriptionStore;
        private readonly IPushNotificationService _notificationService;
        private readonly IPushNotificationsQueue _pushNotificationsQueue;

        public PushNotificationsApiController(IPushSubscriptionStore subscriptionStore, IPushNotificationService notificationService, IPushNotificationsQueue pushNotificationsQueue)
        {
            _subscriptionStore = subscriptionStore;
            _notificationService = notificationService;
            _pushNotificationsQueue = pushNotificationsQueue;
        }

        // GET push-notifications-api/public-key
        [HttpGet("public-key")]
        public ContentResult GetPublicKey()
        {
            return Content(_notificationService.PublicKey, "text/plain");
        }

        // POST push-notifications-api/subscriptions
        [HttpPost("subscriptions")]
        public async Task<IActionResult> StoreSubscription([FromBody]PushSubscription subscription)
        {
            await _subscriptionStore.StoreSubscriptionAsync(subscription);

            var option = new CookieOptions();
            option.Expires = DateTime.Now.AddYears(10);
            Response.Cookies.Append("auth", subscription.Keys["auth"], option);

            return NoContent();
        }

        // DELETE push-notifications-api/subscriptions?endpoint={endpoint}
        [HttpDelete("subscriptions")]
        public async Task<IActionResult> DiscardSubscription(string endpoint)
        {
            await _subscriptionStore.DiscardSubscriptionAsync(endpoint);

            return NoContent();
        }

        // POST push-notifications-api/notifications
        [HttpPost("notifications")]
        public IActionResult SendNotification([FromBody]PushMessageViewModel message)
        {
            string cookie = Request.Cookies["auth"];

            _pushNotificationsQueue.Enqueue(new PushMessageAuth(message.Notification, cookie));

            return NoContent();
        }
    }

}
