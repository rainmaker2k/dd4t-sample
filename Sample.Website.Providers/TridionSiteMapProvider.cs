using Sample.Models;
using DD4T.ContentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using DD4T.ContentModel.Factories;
using System.Xml.Linq;
using System.Collections.Specialized;
using DD4T.Mvc.Providers;
using System.Collections;
using DD4T.ContentModel.Contracts.Caching;
using DD4T.Factories;
using DD4T.Factories.Caching;
using DD4T.Utils;
using DD4T.ContentModel.Logging;
using Sample.Website.Tridion;

namespace Sample.Website.Providers
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class TridionSiteMapProvider : StaticSiteMapProvider
    {

        public TridionSiteMapProvider()
        {
            PublicationNodeDictionary = new Dictionary<string, Dictionary<string, SiteMapNode>>();
        }

        // Sitemap attributes which are not considered part of route, you can add your custom attributes here.

        public const string CacheRegion = "System";
        public const string DefaultCacheKey = "SiteMapRootNode";
        public const string CacheNullValue = "NULL";
        public const string DefaultSiteMapPath = "/system/sitemap/sitemap.xml";
        public const int MaxItemsInGroup = 6;

        private SiteMapNode rootNode;

        private IPageFactory _pageFactory;
        public virtual IPageFactory PageFactory
        {
            get
            {
                if (_pageFactory == null)
                    _pageFactory = new PageFactory();
                return _pageFactory;
            }
            set
            {
                _pageFactory = value;
            }
        }

        private IBinaryFactory _binaryFactory;
        public virtual IBinaryFactory BinaryFactory
        {
            get
            {
                if (_binaryFactory == null)
                    _binaryFactory = new BinaryFactory();
                return _binaryFactory;
            }
            set
            {
                _binaryFactory = value;
            }
        }

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
                                var url = key.Remove(0, "SITEMAP_".Length);
                                return PageFactory.GetLastPublishedDateByUrl(url);
                            };
                }
                return _cacheAgent;
            }
            set
            {
                _cacheAgent = value;
            }
        }

        public Dictionary<string, Dictionary<string, SiteMapNode>> PublicationNodeDictionary { get; set; }
        public virtual string SiteMapPath
        {
            get
            {
                string path = ConfigurationHelper.SiteMapPath;
                if (string.IsNullOrEmpty(path))
                {
                    return DefaultSiteMapPath;
                }
                return path;
            }
        }


        private SiteMapNode ReadSitemapFromXml(string siteprefix, string sitemapUrl)
        {
            string cachekey = String.Format("SITEMAP_{0}", sitemapUrl);
            object result = CacheAgent.Load(cachekey);
            if (result != null)
            {
                if (!(result is SiteMapNode) && ((string)result).Equals(CacheNullValue))
                    return null;
                return result as SiteMapNode;
            }

            LoggerService.Debug(">>ReadSitemapFromXml", LoggingCategory.Performance);
            SiteMapNode rootNode = null;

            var existingNode = FindSiteMapNodeFromKey("root_" + siteprefix);
            if (existingNode != null)
            {
                RemoveNode(existingNode);
            }

            var nodeDictionary = new Dictionary<string, SiteMapNode>();
            PublicationNodeDictionary[siteprefix] = nodeDictionary;

            string sitemap;
            if (!PageFactory.TryFindPageContent(sitemapUrl, out sitemap))
            {
                sitemap = emptySiteMapString();
            }
            LoggerService.Debug(string.Format("loaded sitemap with url {0}, length {1}", sitemapUrl, sitemap.Length), LoggingCategory.Performance);

            XDocument xDoc = XDocument.Parse(sitemap);

            LoggerService.Debug("parsed sitemap into XDocument", LoggingCategory.Performance);

            XElement siteMapRoot = xDoc.Root;

            rootNode = new NavigationNode(this, "root_" + siteprefix, "root_" + siteprefix, String.Format("/{0}", siteprefix), String.Empty, String.Empty, new ArrayList(), new NameValueCollection(), new NameValueCollection(), String.Empty);
            LoggerService.Debug("created root node", LoggingCategory.Performance);

            //Fill down the hierarchy.
            
            AddChildren(rootNode, siteMapRoot.Elements("siteMapNode"), 1, nodeDictionary);

            CacheAgent.Store(cachekey, rootNode);

            LoggerService.Debug("<<ReadSitemapFromXml", LoggingCategory.Performance);
            return rootNode;
        }

        private void AddChildren(SiteMapNode rootNode, IEnumerable<XElement> siteMapNodes, int currentLevel, Dictionary<string, SiteMapNode> nodeDictionary)
        {
            LoggerService.Debug(">>AddChildren for root node {0} at level {1}", LoggingCategory.Performance, rootNode.Title, currentLevel);
            foreach (var element in siteMapNodes)
            {
                
                LoggerService.Debug(">>>for loop iteration: {0}", LoggingCategory.Performance, element.ToString());
                SiteMapNode childNode = CreateNodeFromElement(element, currentLevel, nodeDictionary);
                LoggerService.Debug("finished creating TridionSiteMapNode", LoggingCategory.Performance);

                var existing = FindSiteMapNodeFromKey(childNode.Key);
                if (existing != null)
                {
                    RemoveNode(existing);
                }

                childNode.ParentNode = rootNode;
                LoggerService.Debug("about to add TridionSiteMapNode to node dictionary", LoggingCategory.Performance);
                LoggerService.Debug("finished adding TridionSiteMapNode to node dictionary", LoggingCategory.Performance);

                //Use the SiteMapNode AddNode method to add the SiteMapNode to the ChildNodes collection
                LoggerService.Debug("about to add node to SiteMap", LoggingCategory.Performance);
                AddNode(childNode, rootNode);
                LoggerService.Debug(string.Format("finished adding node to sitemap (title={0}, parent title={1})", childNode.Title, rootNode.Title), LoggingCategory.Performance);

                // Check for children in this node.
                AddChildren(childNode, element.Elements("siteMapNode"), currentLevel + 1, nodeDictionary);
                LoggerService.Debug("<<<for loop iteration: {0}", LoggingCategory.Performance, element.ToString());
            }

            LoggerService.Debug("<<AddChildren for root node {0} at level {1}", LoggingCategory.Performance, rootNode.Title, currentLevel);
        }

        protected virtual SiteMapNode CreateNodeFromElement(XElement element, int currentLevel, Dictionary<string, SiteMapNode> nodeDictionary)
        {
            var attributes = new NameValueCollection();
            foreach (var a in element.Attributes())
            {
                attributes.Add(a.Name.ToString(), a.Value);
            }

            string uri;
            try
            {
                if (element.Attribute("uri") != null)
                    uri = element.Attribute("uri").Value;
                else if (element.Attribute("pageId") != null)
                    uri = element.Attribute("pageId").Value;
                else
                    uri = "";
            }
            catch
            {
                LoggerService.Debug("exception while retrieving uri", LoggingCategory.General);
                uri = "";
            }

            NavigationNode childNode = 
                new NavigationNode(this,
                    element.Attribute("id").Value, //key
                    uri,
                    element.Attribute("url") != null ? element.Attribute("url").Value : element.Attribute("id").Value, //url
                    element.Attribute("title").Value, //title
                    element.Attribute("description").Value, //description
                    null, //roles
                    attributes, //attributes
                    null, //explicitresourceKeys
                    null) { Level = currentLevel }; // implicitresourceKey


            if (element.Element("NumberOfItemsInGroup") != null)
                childNode.NumberOfItemsInGroup = Convert.ToInt32(element.Element("NumberOfItemsInGroup").Value);
            else
                childNode.NumberOfItemsInGroup = MaxItemsInGroup;

            if (element.Element("NumberOfItemsPerColumn") != null)
                childNode.NumberOfItemsInColumn = Convert.ToInt32(element.Element("NumberOfItemsPerColumn").Value);

            nodeDictionary.Add(childNode.Key, childNode);

            return childNode;
        }

        public virtual bool IsInitialized
        {
            get;
            private set;
        }

        public override void Initialize(string name, NameValueCollection attributes)
        {
            if (!IsInitialized)
            {
                base.Initialize(name, attributes);
                IsInitialized = true;
            }
        }

        public virtual string CacheKey
        {
            get
            {
                return DefaultCacheKey;
            }
        }


        private object lock1 = new object();
        public override SiteMapNode BuildSiteMap()
        {
            //object result = CacheAgent.Load(CacheKey);
            //if (result != null)
            //{
            //    if (result is string && ((string)result).Equals(CacheNullValue))
            //        return null;
            //    return result as SiteMapNode;
            //}

            //SiteMapNode rootNode;
            //lock (lock1)
            //{
            //    base.Clear();

            //    PublicationNodeDictionary = new Dictionary<string, Dictionary<string, SiteMapNode>>();

            //    var availPrefix = ConfigurationHelper.GetSetting("AvailablePublicationPrefixes");
            //    string[] publicationPrefixes = availPrefix != null ? availPrefix.Split(',') : new string[] { "it_en" };

            //    var allNodes = publicationPrefixes.Select(p => ReadSitemapFromXml(p, String.Format("/{0}/{1}", p, SiteMapPath)));

            //    rootNode = new NavigationNode(this, "root", "root", String.Empty, String.Empty, String.Empty, new ArrayList(), new NameValueCollection(), new NameValueCollection(), String.Empty);
            //    AddNode(rootNode);
            //    foreach (TridionSiteMapNode node in allNodes)
            //    {
            //        AddNode(node, rootNode);
            //    }

            //    // Store the root node in the cache.
            //    CacheAgent.Store(CacheKey, CacheRegion, rootNode);
            //}

            //return rootNode;


            return rootNode;
        }

        private string emptySiteMapString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<siteMap publicationid=\"tcm:0-70-1\">");
            sb.Append("<siteMapNode title=\"website\" url=\"/\">");
            sb.Append("</siteMapNode>");
            sb.Append("</siteMap>");

            return sb.ToString();
        }

        protected override SiteMapNode GetRootNodeCore()
        {
            //return BuildSiteMap();
            return RootNode;
        }

        public SiteMapNode FindSiteMapNode(IPage page)
        {
            string pubUrl = PublicationInformation.GetPublicationUrlByUri(page.Publication.Id);
            //string trimmedPubUrl = pubUrl.Trim('/');

            if (PublicationNodeDictionary.ContainsKey(pubUrl))
            {
                var nodeDictionary = PublicationNodeDictionary[pubUrl];
                if (nodeDictionary.ContainsKey(page.Id))
                {
                    var siteMapNode = nodeDictionary[page.Id];

                    return siteMapNode; 
                }
                
            }

            return null;
        }

        public SiteMapNode GetPublicationNode(IPage page)
        {
            string publicationUrl = PublicationInformation.GetPublicationUrlByUri(page.Publication.Id);

            //var publicationNode = RootNode.ChildNodes.Cast<TridionSiteMapNode>().FirstOrDefault(tsn => publicationUrl.Equals(String.Format("{0}/", tsn.Url)));

            //return publicationNode;

            return ReadSitemapFromXml(publicationUrl, String.Format("{0}{1}", publicationUrl, SiteMapPath));
        }

        public override SiteMapNode RootNode
        {
            get
            {
                //return BuildSiteMap();
                if (rootNode == null)
                    rootNode = new NavigationNode(this, "root", "root", String.Empty, String.Empty, String.Empty, new ArrayList(), new NameValueCollection(), new NameValueCollection(), String.Empty);;

                return rootNode;
            }
        }

        protected override void Clear()
        {
            lock (this)
            {
                // CacheAgent.Remove("rootNode"); // currently, CacheAgents do not support 'Remove'
                base.Clear();
            }
        }

    }
}
