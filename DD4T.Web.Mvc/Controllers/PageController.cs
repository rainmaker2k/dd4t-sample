using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Security;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using DD4T.Mvc.Controllers;
using DD4T.Factories;

namespace DD4T.Web.Mvc.Controllers
{
    public class PageController : TridionControllerBase
    {

        #region private members
        //private Regex reDefaultPage = new Regex(@".*/[^/\.]*$");
        private Regex reDefaultPage = new Regex(@".*/[^/\.]*(/?)$");
        private string defaultPageFileName = null;
        private string pageContentModelObject = "pageContentModelObject";
        #endregion

        #region MVC
        /// <summary>
        /// Create IPage from XML in the broker and forward to the view
        /// </summary>
        /// <param name="pageId"></param>
        /// <returns></returns>
#if (!DEBUG)
        [OutputCache(CacheProfile = "ControllerCache")]
#endif
        [HandleError]
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Page(string pageId, string language)
        {
            try
            {
                string urlPage = "/" + language + "/" + pageId;
                ContentModel.IPage p = base.PageFactory.FindPage(urlPage);
                RouteData.Values.Add(pageContentModelObject, p);
                return base.Page(language + "/" + pageId);
            }
            catch (SecurityException se)
            {
                throw new HttpException(403, se.Message);
            }
        }

        ///// <summary>
        ///// Create IPage from XML and forward to the view
        ///// </summary>
        ///// <remarks>Todo: include this in framework, URL rewriting for images, JS, CSS, etc</remarks>
        ///// <returns></returns>
        //[HandleError]
        //[AcceptVerbs(HttpVerbs.Post)]
        //public System.Web.Mvc.ActionResult PreviewPage()
        //{
        //    try
        //    {
        //        using (StreamReader reader = new StreamReader(this.Request.InputStream))
        //        {
        //            string pageXml = reader.ReadToEnd();
        //            IPage model = this.PageFactory.GetIPageObject(pageXml);
        //            if (model == null)
        //            {
        //                throw new ModelNotCreatedException("--unknown--");
        //            }
        //            ViewBag.Title = model.Title;
        //            ViewBag.Renderer = ComponentPresentationRenderer;
        //            return GetView(model);
        //        }
        //    }
        //    catch (SecurityException se)
        //    {
        //        throw new HttpException(403, se.Message);
        //    }
        //}

        /// <summary>
        /// Execute search, add results to viewbag and execute standard action result
        /// </summary>
        /// <param name="pageId"></param>
        /// <returns></returns>
        [HandleError]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Search(string pageId)
        {
            using (StreamReader reader = new StreamReader(this.Request.InputStream))
            {
                NameValueCollection queryString = HttpUtility.ParseQueryString(reader.ReadToEnd());
                string query = queryString.Get("query");         
                List<string> searchResults = new List<string>();

                // ToDo: implement actual search
                if (query.ToLower().Equals("test"))
                {
                    searchResults.Add("first example result");
                    searchResults.Add("second example result");
                    searchResults.Add("third example result");
                }

                // add result to viewbag
                ViewBag.SearchResults = searchResults;
                ViewBag.SearchQuery = query;
                ViewBag.ShowSearchResults = true;

                return Page("Search/" + pageId);
            }
        }
        
        #endregion
    }
}
