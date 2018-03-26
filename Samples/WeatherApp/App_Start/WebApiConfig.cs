using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Web.Http;
using Microsoft.Bot.Framework.Builder.Witai;

namespace WitWeather
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Json settings
            config.Formatters.JsonFormatter.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            config.Formatters.JsonFormatter.SerializerSettings.Formatting = Formatting.Indented;
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore,
            };

            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            WitLocator.Instance.Register<IWitConfig>(new WitConfig
            {
                WitConfigDictionary = new Dictionary<CultureInfo, string>
                {
                    { new CultureInfo("fr"), ConfigurationManager.AppSettings[$"Wit.ApiKey_fr"] },
                    { new CultureInfo("en"), ConfigurationManager.AppSettings[$"Wit.ApiKey_en"] }
                }
            });
        }
    }
}
