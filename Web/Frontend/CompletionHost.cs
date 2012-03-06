using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServiceStack.WebHost.Endpoints;
using Vosen.Madarame;

namespace Web
{
    public class CompletionHost : AppHostBase
    {
        public CompletionHost()
            : base("Test", typeof(NameCompletionService).Assembly)
        { }

        public override void Configure(Funq.Container container)
        {
            container.Register(c => new NameCompletionService("Server=127.0.0.1;Port=5432;Database=mal-utf8;User Id=vosen;Password=postgres;"));
        }
    }
}