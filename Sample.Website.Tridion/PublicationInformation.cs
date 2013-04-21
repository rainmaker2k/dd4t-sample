using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tridion.ContentDelivery.Meta;
using Sample.Models;
namespace Sample.Website.Tridion
{
    public static class PublicationInformation
    {

        public static int GetIdFromPublicationUrl(string publicationUrl)
        {
            //Prepend leading '/'
            if (!publicationUrl.StartsWith("/"))
            {
                publicationUrl = string.Concat("/", publicationUrl);
            }

            //Append '/'
            if (!publicationUrl.EndsWith("/"))
            {
                publicationUrl = string.Concat(publicationUrl, "/");
            }
            
            PublicationMetaFactory m = new PublicationMetaFactory();
            var pubMeta = m.GetMetaByPublicationUrl(publicationUrl);

            if (pubMeta != null && pubMeta.Count() > 0)
            {
                //Get the first pubmeta with this publicationUrl
                var publicationMeta = pubMeta.FirstOrDefault();

                if (publicationMeta != null)
                {
                    return publicationMeta.Id;
                }
            }

            return 0;
        }

        public static string GetPublicationUrlByUri(string pubUri)
        {
            PublicationMetaFactory m = new PublicationMetaFactory();
            var pubMeta = m.GetMeta(pubUri);

            if (pubMeta != null)
            {
                return pubMeta.PublicationUrl;
            }

            return String.Empty;
        }



    }
}
