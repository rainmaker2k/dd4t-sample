using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Sample.Website.Providers;
using Sample.Models;
using DD4T.ContentModel;
using DD4T.ContentModel.Factories;
using DD4T.Mvc.Providers;
using Sample.Website.Providers.Contracts;


namespace Sample.Website.Mapping.Tridion.Helpers
{
    public class MenuHelper
    {
        private const int MaxItemsInCol = 10;

        private readonly IPageFactory pageFactory;
        private readonly IComponentFactory componentFactory;
        private readonly IPageLinkProvider pageLinkProvider;

        public MenuHelper(IPageFactory pageFactory, IComponentFactory componentFactory, IPageLinkProvider pageLinkProvider)
        {
            this.pageFactory = pageFactory;
            this.componentFactory = componentFactory;
            this.pageLinkProvider = pageLinkProvider;
        }

        public string GetTitle(TridionSiteMapNode node)
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
                        var component = componentFactory.GetComponent(landingCp.Component.Id, landingCp.ComponentTemplate.Id);
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


        private bool InNavigation(string title)
        {
            return Regex.IsMatch(title, "^\\d{3} ");
        }

        //Retrieve only structuregroups that follow the "010 bla" pattern
        public IEnumerable<TridionSiteMapNode> GetInNavigationNodes(SiteMapNode tridionSiteMapNode)
        {
            return tridionSiteMapNode.ChildNodes.Cast<TridionSiteMapNode>()
                                     .Where(cn => !cn.Title.StartsWith("000 ") && InNavigation(cn.Title));
        }


        public static List<SiteMapNode> GetPath(SiteMapNode node, SiteMapNode homePage = null)
        {
            var pathNodes = new List<SiteMapNode>();

            while (node.ParentNode != null)
            {
                pathNodes.Add(node);
                node = node.ParentNode;
            }
            if (homePage!= null)
                pathNodes.Add(homePage);

            pathNodes.Reverse();
            return pathNodes;
        }




    }
}
