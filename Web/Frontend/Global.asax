<%@ Application Inherits="MsdnWeb.Global" Language="C#" %>
<script Language="C#" RunAt="server">

  protected void Application_Start(Object sender, EventArgs e) 
  {
      new Web.CompletionHost().Init();
      base.Start();
      var container = new Funq.Container();
      container.Register(cont => new MsdnWeb.Controllers.HomeController(null)).ReusedWithin(Funq.ReuseScope.None);
      ControllerBuilder.Current.SetControllerFactory(new ServiceStack.Mvc.FunqControllerFactory(container));
  }

</script>

