using System.Collections.Generic;

namespace Sample.Models
{
    public class GeneralDetail
    {
        public string Heading { get; set; }
        public string Summary { get; set; }
        public Link Link { get; set; }
        public string Image { get; set; }
        public string TextImage { get; set; }
        public bool BannerImage { get; set; }
        public IEnumerable<Paragraph> Paragraphs { get; set; }
        public string Template { get; set; }
        public ListOfLinks<SingleLink> ListOfLinks { get; set; }
    }

}