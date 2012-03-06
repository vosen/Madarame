<%@ Application Inherits="MsdnWeb.Global" Language="C#" %>
<script Language="C#" RunAt="server">

  protected void Application_Start(Object sender, EventArgs e) {
      RouteTable.Routes.IgnoreRoute("api/{*pathInfo}");
      new Web.CompletionHost().Init();
      base.Start();
  }

</script>

