<%@ Application Inherits="MsdnWeb.Global" Language="C#" %>
<script Language="C#" RunAt="server">

  protected void Application_Start(Object sender, EventArgs e) 
  {
      var container = Web.CompletionHost.Start();
      ControllerBuilder.Current.SetControllerFactory(new Web.GlobalFunqControllerFactory(container));
      container.Register(cont => new MsdnWeb.Controllers.RecommendController(null)).ReusedWithin(Funq.ReuseScope.None);
      base.Start();
  }

</script>

