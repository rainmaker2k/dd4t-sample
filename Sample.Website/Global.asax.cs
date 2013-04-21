using System.Configuration;
using System.Data.EntityClient;
using System.Data.SQLite;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Routing;
using log4net.Config;
using Sample.Tridion.Web.ActionAttributes;

namespace Sample.Website
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new LanguageDependentAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            //RouteDebug.RouteDebugger.RewriteRoutesForTesting(RouteTable.Routes);
            
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
			routes.IgnoreRoute("{resource}.ico/{*pathInfo}");
            routes.IgnoreRoute("Scripts/{resource}");
            routes.IgnoreRoute("Images/{*pathInfo}");//Handled by th binaryHandler
            routes.IgnoreRoute("{resource}/Images/{*pathInfo}");//Handled by th binaryHandler
            routes.IgnoreRoute("{resource}/images/{*pathInfo}");//Handled by th binaryHandler
            routes.IgnoreRoute("_Images/{*pathInfo}"); 
            routes.IgnoreRoute("_Scripts/{*pathInfo}");
            routes.IgnoreRoute("_Styles/{*pathInfo}");
            
            //Route for serving Javascript from broker DB
            routes.MapRoute(
                "Javascript",
                "{language}/system/js/{*PageId}",
                new { controller = "Javascript", action = "Index" }, // Parameter defaults
                new { pageId = @"^(.*.js)?$", httpMethod = new HttpMethodConstraint("GET") } // Parameter constraints
            );
            
            //routes.MapRoute(
            //    "HomePage",
            //    "{controller}/{action}",
            //    new { controller = "Home", action="Index"});

            routes.MapRoute(
                "PreviewPage",
                "{*PageId}",
                new { controller = "Page", action = "PreviewPage" }, // Parameter defaults
                new { httpMethod = new HttpMethodConstraint("POST") } // Parameter constraints
            );

            routes.MapRoute("SiteSelector", "go", new { Controller = "SiteSelector", action="Index", language = "it_en" });

            var pageDefaults = new {
                language = "it_en",
                controller = "Page",
                action = "Page",
                pageId = WebConfigurationManager.AppSettings["DefaultPage"],
                parameters = UrlParameter.Optional,
            };
            
            //Route for serving partial views to booking engine (header/footer etc.)
            routes.MapRoute(
                "PartialView",
                "{language}/system/include/{Controller}/{Action}",
                pageDefaults, // Parameter defaults
                new { httpMethod = new HttpMethodConstraint("GET") } // Parameter constraints
            );
            
            routes.MapRoute(
                "TridionPage",
                "{language}/{*pageId}",
                pageDefaults, // Parameter defaults
                new { pageId = @"^(.*(\.html|/))?$" } // Parameter constraints to only catch .html pages
            );
            
            //General. For parameter use: http://localhost:1775/it_en/faq/detail?keywordUri=13-540-1024
            routes.MapRoute(
                "General",
                "{language}/{controller}/{action}/{parameters}",
                pageDefaults
            );

            routes.MapRoute(
                "Default",
                "{controller}/{action}");

        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            
            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            XmlConfigurator.Configure();

            Bootstrapper.Initialise();

            InitLocalImageDb();
        }

        /// <summary>
        /// Recreates the local caching DB if it doesn't exist.
        /// </summary>
        private static void InitLocalImageDb()
        {
            var ee = new System.Data.EntityClient.EntityConnectionStringBuilder();
            var builder = new EntityConnectionStringBuilder(ConfigurationManager.ConnectionStrings["BinariesEntities"].ConnectionString);

            using (var connection = new SQLiteConnection(builder.ProviderConnectionString))
            using (var command = connection.CreateCommand())
            {
                connection.Open(); //Automatically creates sqlite datbase if none exists
                var table = connection.GetSchema("Tables").Select("Table_Name = 'Binaries'");
                if (table.Length <= 0)
                {
                    command.CommandText =
                        "CREATE TABLE [Binaries] (" +
                        "[ComponentUri] NVARCHAR(100)  UNIQUE NOT NULL PRIMARY KEY," +
                        "[Path] NVARCHAR(512)  UNIQUE NOT NULL," +
                        "[Content] BLOB  NOT NULL," +
                        "[LastPublishedDate] TIMESTAMP  NOT NULL" +
                        ")";
                    command.ExecuteNonQuery();

                }

                var uriIndex = connection.GetSchema("Indexes").Select("Index_Name = 'IDX_BINARIES_COMPONENTURI'");
                if (uriIndex.Length <= 0)
                {
                    command.CommandText =
                        "CREATE INDEX [IDX_BINARIES_COMPONENTURI] ON [Binaries](" +
                        "[ComponentUri]  DESC" +
                        ")";
                    command.ExecuteNonQuery();
                }

                var pathIndex = connection.GetSchema("Indexes").Select("Index_Name = 'IDX_BINARIES_PATH'");
                if (pathIndex.Length <= 0)
                {
                    command.CommandText =
                        "CREATE INDEX [IDX_BINARIES_PATH] ON [Binaries](" +
                        "[Path]  DESC" +
                        ")";
                    command.ExecuteNonQuery();
                }
            }
        }

    }
}