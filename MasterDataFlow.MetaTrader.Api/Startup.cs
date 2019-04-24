using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using Newtonsoft.Json.Serialization;
using Owin;

namespace MasterDataFlow.MetaTrader.Api
{
    public class Startup
    {
        // This code configures Web API. The Startup class is specified as a type
        // parameter in the WebApp.Start method.
        public void Configuration(IAppBuilder appBuilder)
        {
            var config = CreateHttpConfiguration();
            appBuilder.UseWebApi(config);
        }

        private static HttpConfiguration CreateHttpConfiguration()
        {
            var configuration = new HttpConfiguration();
            configuration.MapHttpAttributeRoutes();
            //configuration.EnableCors(new EnableCorsAttribute("*", "*", "*"));

            configuration.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional, action = RouteParameter.Optional }
            );

            var jsonFormatter = configuration.Formatters.OfType<JsonMediaTypeFormatter>().First();
            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            //configuration.Services.Add(typeof(IExceptionLogger), new NLogExceptionLogger());

            return configuration;
        }
    }
}
