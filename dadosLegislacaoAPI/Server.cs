using Owin;
using System.Web.Http;

namespace dadosLegislacaoAPI
{

    public class Server
    {
    public void Configuration(IAppBuilder app)
    {
        HttpConfiguration config = new HttpConfiguration();
        config.Routes.MapHttpRoute(
            name: "Dados Servidores API",
            routeTemplate: "api/{controller}/{txPesquisa}",
            defaults: new
            {
                txPesquisa = RouteParameter.Optional,
            }
        );
        //config.Filters.Add(new BasicAuthenticationAttribute());
        app.UseWebApi(config);
    }
    }
    

}
