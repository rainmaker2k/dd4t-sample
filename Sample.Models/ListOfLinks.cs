using System.Collections.Generic;
using System;

namespace Sample.Models
{
    public class ListOfLinks<T>
    {
        [Obsolete("Use TitleLink", true)]
        public string Heading { get; set; }
        public Link TitleLink { get; set; }
        public string Summary { get; set; }
        public IEnumerable<T> List { get; set; }
    }
}