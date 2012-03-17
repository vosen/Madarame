<%@ Application Inherits="MsdnWeb.Global" Language="C#" %>
<script Language="C#" RunAt="server">

  protected void Application_Start(Object sender, EventArgs e) 
  {
      new Web.CompletionHost().Init();
      base.Start();
      ControllerBuilder.Current.SetControllerFactory(new ServiceStack.Mvc.FunqControllerFactory(new Funq.Container()));
  }

</script>

