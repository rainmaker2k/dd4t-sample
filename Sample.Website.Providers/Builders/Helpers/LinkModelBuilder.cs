using Sample.Models;
using DD4T.ContentModel;
using DD4T.ContentModel.Factories;

namespace Sample.Website.Mapping.Tridion.Builders
{
    public partial class LinkModelBuilder
    {
        readonly ILinkFactory LinkFactory;

        public LinkModelBuilder(ILinkFactory factory)
        {
            this.LinkFactory = factory;
        }

        public Link CreateLinkText(IField field)
        {
            return CreateLinkText(field.EmbeddedValues[0]);
        }

        public Link CreateLinkText(IFieldSet fields)
        {
            return BuildLinkText(fields, new Link { });
        }

        public SingleLink CreateSingleLink(IFieldSet fields)
        {
            return BuildSingleLink(fields, new SingleLink { });
        }

        public static string GetLinkName(IComponent c)
        {
            return c.Title.Replace(" ", "_").ToLower();
        }

        public Link LinkToSelf(IComponent c)
        {
            return new Link
            {
                Href = this.LinkFactory.ResolveLink(c.Id),
                Title = c.Title,
                Name = GetLinkName(c),
            };
        }
    }
}