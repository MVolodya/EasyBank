using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace EasyBank
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();

            ModelBinders.Binders.Add(typeof(decimal), new EasyBank.Filters.ModelBinders.DecimalModelBinder());
            ModelBinders.Binders.Add(typeof(decimal?), new EasyBank.Filters.ModelBinders.DecimalModelBinder());
        }

        protected void Application_BeginRequest()
        {
            CultureInfo cInf = new CultureInfo("uk-UA", false);
            // NOTE: change the culture name en-ZA to whatever culture suits your needs

            cInf.DateTimeFormat.DateSeparator = "-";
            cInf.DateTimeFormat.ShortDatePattern = "yyyy-MM-dd";
            cInf.DateTimeFormat.LongDatePattern = "yyyy-MM-dd hh:mm:ss tt";

            System.Threading.Thread.CurrentThread.CurrentCulture = cInf;
            System.Threading.Thread.CurrentThread.CurrentUICulture = cInf;

        }
    }
}