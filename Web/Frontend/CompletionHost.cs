using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServiceStack.WebHost.Endpoints;
using Vosen.Madarame;
using System.Configuration;
using ServiceStack.ServiceHost;
using System.Web.Routing;
using System.Web.Mvc;
using Funq;

namespace Web
{
    public class CompletionHost : AppHostBase
    {
        public CompletionHost()
            : base("Title completion", typeof(NameCompletion.Service).Assembly)
        {
            RouteTable.Routes.IgnoreRoute("api/{*pathInfo}");
            RouteTable.Routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon.ico(/.*)?" });
        }

        public override void Configure(Container container)
        {
            string connString = ConfigurationManager.ConnectionStrings[0].ConnectionString;
            container.Register(c => new NameCompletion.Service(connString));
#if !DEBUG
            SetConfig(new EndpointHostConfig()
            {
                DebugMode = false,
                EnableFeatures = Feature.Json,
                AllowJsonpRequests = false
            });
#endif
        }

        public static Container Start()
        {
            new CompletionHost().Init();
            return CompletionHost.Instance.Container;
        }
    }
}