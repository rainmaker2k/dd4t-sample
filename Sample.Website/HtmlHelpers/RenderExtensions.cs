using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using DD4T.ContentModel;
using DD4T.ContentModel.Logging;
using DD4T.Utils;

namespace Sample.Website.HtmlHelpers
{
    public static class RenderExtensions
    {
        public static MvcHtmlString RenderComponentPresentation(this HtmlHelper htmlHelper, IComponentPresentation cp)
        {
            string controller = ConfigurationHelper.ComponentPresentationController;
            string action = ConfigurationHelper.ComponentPresentationAction;

            
            if (cp.ComponentTemplate.MetadataFields != null && cp.ComponentTemplate.MetadataFields.ContainsKey("controller"))
            {
                controller = cp.ComponentTemplate.MetadataFields["controller"].Value;
            }
            if (cp.ComponentTemplate.MetadataFields != null && cp.ComponentTemplate.MetadataFields.ContainsKey("action"))
            {
                action = cp.ComponentTemplate.MetadataFields["action"].Value;
            }


            LoggerService.Debug("about to render component presentation with controller {0} and action {1}", LoggingCategory.Performance, controller, action);
            //return ChildActionExtensions.Action(htmlHelper, action, controller, new { componentPresentation = ((ComponentPresentation)cp) });
             MvcHtmlString result = htmlHelper.Action(action, controller, new { componentPresentation = ((ComponentPresentation)cp) });
            LoggerService.Debug("finished rendering component presentation with controller {0} and action {1}", LoggingCategory.Performance, controller, action);
            return result;
        }

    }
}
