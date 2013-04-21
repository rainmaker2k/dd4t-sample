using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Sample.Website.Tridion;
using Sample.Models;
using Sample.Website.Providers.Contracts;
using DD4T.ContentModel.Contracts.Caching;
using DD4T.ContentModel.Factories;
using DD4T.Factories;
using DD4T.Factories.Caching;
using DD4T.Utils;
using DD4T.ContentModel.Logging;
using System.Xml.Linq;
using System.Configuration;
using System.Web;
using Microsoft.Practices.Unity;


namespace Sample.Website.Providers
{
    public class PublicationMetadataProvider : IPublicationMetaDataProvider
    {
        [Dependency]
        public IPageFactory PageFactory { get; set; }

        private ICacheAgent _cacheAgent;
        /// <summary>
        /// Get or set the CacheAgent
        /// </summary>  
        public virtual ICacheAgent CacheAgent
        {
            get
            {
                if (_cacheAgent == null)
                {
                    _cacheAgent = new DefaultCacheAgent();
                    _cacheAgent.GetLastPublishDateCallBack =
                        (key, item) =>
                        {
                            var uri = key.Remove(0, "PublicationMetaData_".Length);
                            return PageFactory.GetLastPublishedDateByUri(uri);
                        };
                }
                return _cacheAgent;
            }
            set
            {
                _cacheAgent = value;
            }
        }


        public string RetrieveMetadataValue(string fieldName, string publicationUri)
        {
            var xDoc = GetPubMetadataDocument(publicationUri);
            LoggerService.Debug("parsed metadatapath into XDocument", LoggingCategory.Performance);
            LoggerService.Debug("created query for XDocument", LoggingCategory.Performance);
            var queryresult = from c in xDoc.Root.Element("fields").Elements()
                              where c.Attribute("key").Value == fieldName
                              select c.Value;

            return queryresult.FirstOrDefault();

        }

        private XDocument GetPubMetadataDocument(string publicationUri)
        {
            var cacheKey = String.Format("PublicationMetaData_{0}", publicationUri);
            var cacheObject = CacheAgent.Load(cacheKey);
            if (cacheObject is XDocument)
            {
                return cacheObject as XDocument;
            }

            string publicationUrl = PublicationInformation.GetPublicationUrlByUri(publicationUri);

            string publicationMetadataPage = String.Format("{0}{1}", publicationUrl,
                                                           ConfigurationManager.AppSettings["PublicationMetaDataMapPath"]);

            string pageContent;
            if (!PageFactory.TryFindPageContent(publicationMetadataPage, out pageContent))
            {
                pageContent = "<metadata><fields></fields></metadata>";
            }

            LoggerService.Debug(">>ReadPublicationMetaDataFromXml", LoggingCategory.Performance);

            XDocument xDoc = XDocument.Parse(pageContent);
            CacheAgent.Store(cacheKey, xDoc);
            return xDoc;
        }

        
    }
}
