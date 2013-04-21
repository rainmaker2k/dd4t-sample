using Sample.Models;
using DD4T.ContentModel;
using System.Linq;
using System.Collections.Generic;
using System;

namespace Sample.Website.Mapping.Tridion.Builders
{
    /// <summary>
    /// Utility methods to fill in Link-related models from a Tridion components.
    /// 
    /// To reuse the same logic for initialising subclasses, all methods made generic,
    /// accepting subclasses of the classes they're operating on.
    /// </summary>
    partial class LinkModelBuilder
    {
        internal T BuildLink<T>(IFieldSet fields, T instance)
            where T: Link
        {
            //Func<string, string> FindInHierarchy = name =>
            //    fields
            //        .Generate(fs => fs.Field("Link", fld => fld.EmbeddedValues.FirstOrDefault()))
            //        .Select(fld => fld.Field(name))
            //        .LastOrDefault(s => !string.IsNullOrEmpty(s));
            
            //var externalLink = FindInHierarchy("ExternalLink");
            //var componentLink = FindInHierarchy("ComponentLink");

            var externalLink = FindInHierarchy(fields, "ExternalLink");
            var componentLink = FindInHierarchy(fields, "ComponentLink");

            if (!string.IsNullOrEmpty(componentLink))
            {
                componentLink = LinkFactory.ResolveLink(componentLink);
            }
            
            instance.Href = componentLink ?? externalLink;
            return instance;
        }

        public T BuildLinkText<T>(IFieldSet fields, T instance)
            where T: Link
        {
            instance.Title = fields.Field("LinkTitle");
            return BuildLink(fields, instance);
        }

        public T BuildSingleLink<T>(IFieldSet fields, T instance)
            where T: SingleLink
        {
            BuildLinkText(fields, instance);
            instance.HeadingTitle = fields.Field("Heading") ?? "";
            instance.Image = fields.Image();

            string linkTitle = FindInHierarchy(fields,"LinkTitle");

            instance.Title = linkTitle ?? fields.Field("Heading");
            return instance;
        }

        private string FindInHierarchy(IFieldSet fields, string name) {
              return fields
                    .Generate(fs => fs.Field("Link", fld => fld.EmbeddedValues.FirstOrDefault()))
                    .Select(fld => fld.Field(name))
                    .LastOrDefault(s => !string.IsNullOrEmpty(s));

        }
    }
}