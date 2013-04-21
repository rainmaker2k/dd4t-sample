using System.Linq;
using System.Web.Mvc;
using DD4T.ContentModel;

namespace DD4T.Web.Mvc.Controllers
{
    partial class ComponentController
    {
        public ViewResult Component(string tcmUri, string viewName)
        {
            return this.GetView(new ComponentPresentation
            {
                Component = this.ComponentFactory.GetComponent(tcmUri) as Component,
                ComponentTemplate = new ComponentTemplate
                {
                    Title = viewName,
                    /// TODO: Remove after DD4T.Web.Mvc 1.25.0.0, the above line will be sufficient.
                    MetadataFields = new FieldSet {
                        { "view", new Field { Values = new[] { viewName }.ToList() } }
                    },
                },
            });
        }
    }
}