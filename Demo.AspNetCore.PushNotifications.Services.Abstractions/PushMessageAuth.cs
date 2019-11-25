using Lib.Net.Http.WebPush;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Demo.AspNetCore.PushNotifications.Services.Abstractions
{
    public class PushMessageAuth : PushMessage
    {

        public PushMessageAuth(string content, string auth) : base(content)
        {
            this.Auth = auth;
        }

        public PushMessageAuth(HttpContent content, string auth) : base(content)
        {
            this.Auth = auth;
        }

        public string Auth { get; set; }

    }
    
}
