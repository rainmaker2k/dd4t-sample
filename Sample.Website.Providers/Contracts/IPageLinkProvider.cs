using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sample.Website.Providers.Contracts
{
    public interface IPageLinkProvider
    {
        string ResolvePageLink(string tcmUri); 
    }
}
