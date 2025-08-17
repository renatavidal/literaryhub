using System.Web.Http;
using Newtonsoft.Json.Serialization;

public static class WebApiConfig
{
    public static void Register(HttpConfiguration config)
    {

        config.MapHttpAttributeRoutes();

        config.Routes.MapHttpRoute(
            name: "UIEndpoint",
            routeTemplate: "api/{controller}/{id}",
            defaults: new { id = RouteParameter.Optional }
        );

        // Solo JSON y camelCase
        config.Formatters.Remove(config.Formatters.XmlFormatter);
        var json = config.Formatters.JsonFormatter;
        json.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

    }
}
