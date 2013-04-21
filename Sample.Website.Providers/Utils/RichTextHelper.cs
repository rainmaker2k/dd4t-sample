using System;
using System.Xml;
using DD4T.ContentModel.Factories;
using DD4T.Factories;

namespace Sample.Website.Mapping.Tridion.Helpers
{
    public class RichTextHelper
    {

        public ILinkFactory LinkFactory { get; set; }

        /// <summary>
        /// Extension method on String to resolve rich text. 
        /// Use as: Model.Field["key"].Value.ResolveRichText()
        /// </summary>
        /// <param name="value"></param>
        /// <returns>MvcHtmlString (resolved rich text)</returns>
        public String ResolveRichText(String value)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(string.Format("<xhtml>{0}</xhtml>", value));
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
            nsmgr.AddNamespace("xhtml", "http://www.w3.org/1999/xhtml");
            nsmgr.AddNamespace("xlink", "http://www.w3.org/1999/xlink");

            // resolve links which haven't been resolved
            foreach (XmlNode link in doc.SelectNodes("//xhtml:a[@xlink:href[starts-with(string(.),'tcm:')]][@xhtml:href='' or not(@xhtml:href)]", nsmgr))
            {
                string tcmuri = link.Attributes["xlink:href"].Value;
                string linkUrl = "";
                try
                {
                    linkUrl = link.Attributes["href"].Value;
                }
                catch
                {
                    linkUrl = LinkFactory.ResolveLink(tcmuri);
                }


                if (!string.IsNullOrEmpty(linkUrl))
                {
                    // add href
                    XmlAttribute href = doc.CreateAttribute("xhtml:href");
                    href.Value = linkUrl;
                    link.Attributes.Append(href);

                    // remove all xlink attributes
                    foreach (XmlAttribute xlinkAttr in link.SelectNodes("//@xlink:*", nsmgr))
                    {
                        link.Attributes.Remove(xlinkAttr);
                    }
                }
                else
                {
                    // copy child nodes of link so we keep them
                    foreach (XmlNode child in link.ChildNodes)
                    {
                        link.ParentNode.InsertBefore(child.CloneNode(true), link);
                    }
                    // remove link node
                    link.ParentNode.RemoveChild(link);
                }
            }

            return doc.DocumentElement.InnerXml;
        }
    }
}
