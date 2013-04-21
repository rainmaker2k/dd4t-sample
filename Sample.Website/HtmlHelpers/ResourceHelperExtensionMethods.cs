using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using System.Configuration;

using DD4T.ContentModel;
using DD4T.ContentModel.Contracts.Providers;
using DD4T.ContentModel.Factories;

namespace Sample.Website.HtmlHelpers
{
    public static class ResourceHelperExtensionMethods
    {
        public static string Resource(this HtmlHelper htmlHelper, string resourceName)
        {
            return (string)Resource(htmlHelper.ViewContext.HttpContext, resourceName);
        }

        public static string Resource<T>(this HtmlHelper<T> htmlHelper, string resourceName)
        {
            if (htmlHelper.ViewContext.RouteData.Values.ContainsKey("language"))
            {
                string language = (string)htmlHelper.ViewContext.RouteData.Values["language"];
                return (string)Resource(language, resourceName);
            }
            return String.Format("[{0}]", resourceName);
        }

        public static object Resource(this HttpContextBase httpContext, string resourceName)
        {
            return httpContext.GetGlobalResourceObject(CultureInfo.CurrentUICulture.ToString(), resourceName);
        }

        public static object Resource(string language, string resourceName)
        {
            //Todo: Clean

            // Get the cache
            Cache cache = HttpContext.Current.Cache;

            if (cache["Resources"] == null)
            {
                cache.Insert("Resources", new Dictionary<string, Dictionary<string, string>>());
            }

            var resources = (Dictionary<string, Dictionary<string, string>>)cache.Get("Resources");

            Dictionary<string, string> languageSpecificResources;

            if (!resources.ContainsKey(language))
            {
                // create the whole resources dictionary for the specified language
                string resourceFile = ConfigurationManager.AppSettings["ResourcePath"];

                string resourcePagePath = String.Format("/{0}/{1}", language, resourceFile);

                IPageProvider provider = DependencyResolver.Current.GetService<IPageProvider>();
                IPageFactory factory = DependencyResolver.Current.GetService<IPageFactory>();
                factory.PageProvider = provider;

                var resourcesPage = factory.FindPage(resourcePagePath);

                languageSpecificResources = new Dictionary<string, string>();

                foreach (IComponentPresentation componentPresentation in resourcesPage.ComponentPresentations)
                {
                    foreach (IFieldSet fieldSet in componentPresentation.Component.Fields["Resource"].EmbeddedValues)
                    {
                        if (languageSpecificResources.ContainsKey(fieldSet["Key"].Value))
                        {
                            throw new DuplicateNameException(String.Format("Duplicate key found for key name: {0}", fieldSet["Key"].Value));
                        }

                        languageSpecificResources.Add(fieldSet["Key"].Value, fieldSet["Value"].Value);
                    }
                }

                // add the dictionary to the main resources dictionary in the cache
                resources.Add(language, languageSpecificResources);
            }
            else
            {
                languageSpecificResources = resources[language];
            }

            if (languageSpecificResources.ContainsKey(resourceName))
            {
                return languageSpecificResources[resourceName];
            }

            return String.Format("[{0}]", resourceName);
        }

        public static Dictionary<string, Dictionary<string, object>> CreateResourcesDictionary()
        {
            return new Dictionary<string, Dictionary<string, object>>();
        }
    }
}