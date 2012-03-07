using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServiceStack.WebHost.Endpoints;
using Vosen.Madarame;
using System.Configuration;
using ServiceStack.ServiceHost;

namespace Web
{
    public class CompletionHost : AppHostBase
    {
        public CompletionHost()
            : base("Title completion", typeof(NameCompletionService).Assembly)
        { }

        public override void Configure(Funq.Container container)
        {
            string connString = ConfigurationManager.ConnectionStrings[0].ConnectionString;
            container.Register(c => new NameCompletionService(connString));
#if !DEBUG
            SetConfig(new EndpointHostConfig()
            {
                DebugMode = false,
                EnableFeatures = Feature.Json,
                AllowJsonpRequests = false
            });
#endif
        }
    }
}