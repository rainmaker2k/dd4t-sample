using System.Web.Mvc;
using Sample.Website.Tridion;
using DD4T.Utils;

namespace Sample.Tridion.Web.ActionAttributes
{
    /// <summary>
    /// Reads the {language} part from the requests (e.g. 'it_en') and queries the broker to find the matching PublicationId and stores this ID in
    /// a ConcurrentDictionary and sets it as a RouteValue
    /// </summary>
    public class LanguageDependentAttribute : ActionFilterAttribute
    {
        private static System.Collections.Concurrent.ConcurrentDictionary<string, int> publicationIds = new System.Collections.Concurrent.ConcurrentDictionary<string, int>();

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            const string LANGUAGE = "language";
            const string PUBLICATIONID = "PublicationId";

            //Get host from request
            var routeData = filterContext.RouteData;

            //Read {language} part from routedata
            var language = "it_en"; //TODO: Default from web.config?

            if (routeData.Values != null && routeData.Values[LANGUAGE] != null)
            {
                language = routeData.Values[LANGUAGE].ToString();
            }

            //Get publicationId based on this language
            int publicationId = 0;
            if (!publicationIds.TryGetValue(language, out publicationId))
            {
                //Read publicationId from Broker
                var pubId = PublicationInformation.GetIdFromPublicationUrl(language);

                //Add it
                if (pubId != 0)
                {
                    if (!publicationIds.TryAdd(language, pubId))
                    {
                        //TODO: Log it!
                        LoggerService.Error("Could not store publicationId in the ConcurrentDictionary!", DD4T.ContentModel.Logging.LoggingCategory.Controller, language);
                    }
                    if (!filterContext.RouteData.Values.ContainsKey(PUBLICATIONID))
                    {
                        filterContext.RouteData.Values.Add(PUBLICATIONID, pubId);
                    }
                    else
                    {
                        //Update it, because it could differ between requests
                        filterContext.RouteData.Values[PUBLICATIONID] = pubId;
                    }

                }
            }
            else
            {
                if (!filterContext.RouteData.Values.ContainsKey(PUBLICATIONID))
                {
                    filterContext.RouteData.Values.Add(PUBLICATIONID, publicationId);
                }
                else
                {
                    //Update it, because it could differ between requests
                    filterContext.RouteData.Values[PUBLICATIONID] = publicationId;                    
                }
            }

            base.OnActionExecuting(filterContext);
        }
    }
}
