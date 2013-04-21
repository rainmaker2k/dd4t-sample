using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.Mvc;
using DD4T.ContentModel;
using DD4T.Factories;
using DD4T.Mvc.Providers;
using DD4T.ContentModel.Factories;
namespace Sample.Website.Utils
{
    public static class ControllerUtils
    {
        #region "Publication Helper"
        
        /// <summary>
        /// Read the PublicationId from the RouteData. The PublicationId is set by the global ActionAttribute 'LanguageDependentAttribute'
        /// </summary>
        /// <param name="routeData"></param>
        /// <returns></returns>
        public static int GetPublicationId(RouteData routeData)
        {
            if (routeData != null && routeData.Values != null && routeData.Values.ContainsKey("PublicationId"))
            {
                return Int32.Parse(routeData.Values["PublicationId"].ToString());
            }

            return 0;
        }

        public static string GetPublicationLanguage(RouteData routeData)
        {
            if (routeData != null && routeData.Values != null && routeData.Values.ContainsKey("language"))
            {
                return routeData.Values["language"].ToString();
            }

            return "it_en";
        }
        #endregion

        #region "SiteMap Helper"
        public static string GetTitle(TridionSiteMapNode node, DD4T.ContentModel.Factories.IPageFactory pageFactory, IComponentFactory cmpFactory)
        {
            string title = null;

            string pageUri = null;
            if (node.Attributes["type"].Equals("64"))
            {
                pageUri = node.Attributes["id"];
            }
            else
            {
                var landingPageNode = node.ChildNodes
                    .Cast<TridionSiteMapNode>()
                    .FirstOrDefault(tn => tn.Attributes["type"].Equals("64") && tn.Title.StartsWith("000 "));

                if (landingPageNode != null)
                {
                    pageUri = landingPageNode.Attributes["id"];
                }
            }

            if (!String.IsNullOrEmpty(pageUri))
            {
                IPage landingPage;
                if (pageFactory.TryGetPage(pageUri, out landingPage))
                {
                    var landingCp = landingPage.ComponentPresentations.FirstOrDefault();
                    if (landingCp != null)
                    {
                        var component = cmpFactory.GetComponent(landingCp.Component.Id, landingCp.ComponentTemplate.Id);
                        title = component.Fields["Heading"].Value;
                    }
                }
            }

            if (String.IsNullOrEmpty(title))
            {
                title = node.Title.Remove(0, 4);
            }
            return title;
        }
        #endregion

    }
}