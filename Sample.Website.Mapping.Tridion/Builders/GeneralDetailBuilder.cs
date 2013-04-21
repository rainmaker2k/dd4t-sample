using Sample.Website.Mapping.Tridion.Helpers;

namespace Sample.Website.Mapping.Tridion.Builders
{
    using Sample.Models;
    using DD4T.ContentModel;
    
    public class GeneralDetailBuilder : BuilderBase
    {
        public GeneralDetail Create(IComponentPresentation componentPresentation)
        {
            var fields = componentPresentation.Component.Fields;
            return new GeneralDetail
            {
                Heading = fields.Field("Heading"),
                Summary = RichTextHelper.ResolveRichText(fields.FieldOrEmpty("Summary")),
                Link = fields.Field("Link", new LinkModelBuilder(this.LinkFactory).CreateLinkText),
                Image = fields.Image(),
                TextImage = fields.Field("TextImage"),
                Template = componentPresentation.ComponentTemplate.Title,
                Paragraphs = fields.Field("Paragraph", new ParagraphBuilder(RichTextHelper).Create),
            };
        }
    }
}