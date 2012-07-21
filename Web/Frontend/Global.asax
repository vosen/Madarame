<%@ Application Inherits="MsdnWeb.Global" Language="C#" %>
<script Language="C#" RunAt="server">

  protected void Application_Start(Object sender, EventArgs e) 
  {
      var container = Web.CompletionHost.Start();
      ControllerBuilder.Current.SetControllerFactory(new Web.GlobalFunqControllerFactory(container));
      var recommender = Vosen.Madarame.TitleRecommender.Load(System.Web.Configuration.WebConfigurationManager.AppSettings["MatrixPath"]);
      string dbPath = System.Web.Configuration.WebConfigurationManager.ConnectionStrings[0].ConnectionString;
      container.Register(cont => new MsdnWeb.Controllers.RecommendController(recommender, dbPath)).ReusedWithin(Funq.ReuseScope.None);
      base.Start();
  }

</script>

