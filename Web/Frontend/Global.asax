<%@ Application Inherits="MsdnWeb.Global" Language="C#" %>
<script Language="C#" RunAt="server">

  protected void Application_Start(Object sender, EventArgs e)
  {
      string appData = Server.MapPath("~/App_Data/");
      var recommender = Vosen.Juiz.FunkSVD.TitleRecommender.Load(
        System.IO.Path.Combine(appData, System.Web.Configuration.WebConfigurationManager.AppSettings["FeaturesPath"]),
        System.IO.Path.Combine(appData, System.Web.Configuration.WebConfigurationManager.AppSettings["FeatureAveragesPath"]),
        System.IO.Path.Combine(appData, System.Web.Configuration.WebConfigurationManager.AppSettings["MovieAveragesPath"]),
        System.IO.Path.Combine(appData, System.Web.Configuration.WebConfigurationManager.AppSettings["TitleMappingPath"]),
        System.IO.Path.Combine(appData, System.Web.Configuration.WebConfigurationManager.AppSettings["DocumentMappingPath"]));
      string dbPath = System.Web.Configuration.WebConfigurationManager.ConnectionStrings[0].ConnectionString;
      var container = Web.CompletionHost.Start();
      ControllerBuilder.Current.SetControllerFactory(new Web.GlobalFunqControllerFactory(container));
      container.Register(cont => new MsdnWeb.Controllers.RecommendController(recommender, dbPath)).ReusedWithin(Funq.ReuseScope.None);
      base.Start();
  }

</script>

