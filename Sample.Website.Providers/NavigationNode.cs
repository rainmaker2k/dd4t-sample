using System.Collections;
using System.Collections.Specialized;
using System.Web;
using DD4T.Mvc.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sample.Website.Providers
{
    public class NavigationNode : TridionSiteMapNode
    {
        public NavigationNode(SiteMapProvider provider, string key, string uri, string url, string title, string description, IList roles, NameValueCollection attributes, NameValueCollection explicitResourceKeys, string implicitResourceKey) : 
            base(provider, key, uri, url, title, description, roles, attributes, explicitResourceKeys, implicitResourceKey)
        {
        }

        /// <summary>
        /// Number of items in this group when shown in a dropdown
        /// </summary>
        public int NumberOfItemsInGroup { get; set; }

        /// <summary>
        /// Number of items in one column in this groups dropdown
        /// </summary>

        public int NumberOfItemsInColumn { get; set; }

        public override bool IsDescendantOf(SiteMapNode node)
        {
            var parentNode = this.ParentNode;

            bool parentNodeFound = false;

            while (parentNode != null && !parentNodeFound)
            {
                if (parentNode.Key == node.Key)
                    parentNodeFound = true;
                else
                    parentNode = parentNode.ParentNode;
            }

            return parentNodeFound;
        }
    }
}
