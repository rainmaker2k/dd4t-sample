using System.Collections.Generic;
using System.Linq;
using Sample.Website.Mapping.Tridion.Helpers;
using Sample.Models;
using DD4T.ContentModel;

namespace Sample.Website.Mapping.Tridion.Builders
{
    public class ParagraphBuilder
    {
        readonly RichTextHelper richTextHelper;

        public ParagraphBuilder(RichTextHelper richTextHelper)
        {
            this.richTextHelper = richTextHelper;
        }

        public IEnumerable<Paragraph> Create(IComponent component)
        {
            return component.Fields.Field("Paragraph", this.Create);
        }
        
        public IEnumerable<Paragraph> Create(IField field)
        {
            return field.EmbeddedValues.Select(this.Create);
        }
        
        public Paragraph Create(IFieldSet fs)
        {
            return new Paragraph
            {
                BodyText = fs.Field("BodyText", fld => richTextHelper.ResolveRichText(fld.Value)) ?? string.Empty,
                SubTitle = fs.Field("SubTitle"),
            };
        }
    }
}