using System.Collections.Generic;
using Sample.Website.Providers;
using Sample.Models;
using DD4T.Providers.SDLTridion2011sp1;

namespace Sample.Website.Mapping.Tridion.Helpers
{
    public static class QueryBroker
    {


        public static IList<DD4T.ContentModel.ComponentPresentation> GetComponentPresentationsByCategory(
            string category, string keyword, string sortField)
        {
            IList<DD4T.ContentModel.ComponentPresentation> cmpToReturn =
                new List<DD4T.ContentModel.ComponentPresentation>();

            var query = new ExtendedQueryParameters();
            IList<DD4T.Providers.SDLTridion2011sp1.KeywordItem> keyToFilter = new List<KeywordItem>();
            keyToFilter.Add(new KeywordItem(category, keyword));

            query.KeywordValues = keyToFilter;
            query.QuerySortField = sortField;

            //Component pippo = new Component();
            //ComponentTemplate ppluto = new ComponentTemplate();
            //ComponentPresentation paperino = new ComponentPresentation();


            return cmpToReturn;


        }

    }
}