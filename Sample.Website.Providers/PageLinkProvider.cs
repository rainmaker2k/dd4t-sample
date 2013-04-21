using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sample.Website.Providers.Contracts;
using DD4T.ContentModel;
using Tridion.ContentDelivery.Web.Linking;

namespace Sample.Website.Providers
{
    public class PageLinkProvider : IPageLinkProvider
    {
        public string ResolvePageLink(string tcmUri)
        {
            TcmUri pageUri = new TcmUri(tcmUri);
            using (var pageLink = new PageLink(pageUri.PublicationId))
            {
                var link = pageLink.GetLink(pageUri.ItemId);
                if (link != null)
                {
                    return link.Url;
                }
                else
                {
                    return String.Empty;
                }
            }
        }
 
    }
}
