using System;
using System.Collections.Generic;
using System.Web.Mvc;
using DD4T.ContentModel;
using DD4T.ContentModel.Factories;
using DD4T.Mvc.Controllers;

namespace DD4T.Web.Mvc.Controllers
{
    public partial class ComponentController : TridionControllerBase
    {
        readonly IModelFactory ModelFactory;
        
        public ComponentController(IModelFactory ModelFactory)
        {
            this.ModelFactory = ModelFactory;
        }
        
        protected override ViewResult GetView(IComponentPresentation componentPresentation)
        {
            var cp = (ComponentPresentation) componentPresentation;
            cp.Component = (Component) ComponentFactory.GetComponent(componentPresentation.Component.Id,
                                                                            componentPresentation.ComponentTemplate.Id);

            var viewResult = base.GetView(cp);
            string viewName = viewResult.ViewName;
            
            String language = this.ControllerContext.RouteData.Values["language"].ToString().Substring(3,2);
            // todo: implement ModelFactory
            
            //ModelFactory.Language = language;
            
            object model;
            return ModelFactory.TryCreateModel(viewName, cp, out model)
                ? View(viewName, model)
                : viewResult;
        }

        public ActionResult ArticleSummary(object model)
        {
            ViewBag.Column = this.RouteData.Values["column"];
            ViewBag.ShowFeatureItemImage = this.RouteData.Values["showFeature"];
            return View(model);
        }

        public ActionResult Query()
        {
            List<IComponent> components = new List<IComponent>();
            IComponentPresentation cp = this.GetComponentPresentation();

            ExtendedQueryParameters eqp = new ExtendedQueryParameters();
            if (cp.Component.Fields.ContainsKey("Schema"))
            {
                string schemaName = cp.Component.Fields["Schema"].Value;
                eqp.QuerySchemas = new string[] { schemaName };
            }
            // todo: add 'last XXX days' field
            eqp.LastPublishedDate = DateTime.Now.AddMonths(-3); // search for everything in the last 3 months

            // run the query
            ViewBag.Results = ComponentFactory.FindComponents(eqp);
            return View(cp.Component);
        }

    }
}
