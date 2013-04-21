using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DD4T.ContentModel.Factories;
using DD4T.ContentModel.Exceptions;

namespace Sample.Website.Mapping.Tridion
{
    public class ModelFactory: IModelFactory
    {
        public ILinkFactory LinkFactory { get; set; }

        public string MapComponentViewName(string viewName)
        {
            return BuilderFactory.MapToEnum<ComponentViews, ComponentViews>(viewName).ToString();
        }

        public I Create<I>(DD4T.ContentModel.IPage page)
        {
            IBuilder builder = BuilderFactory.GetBuilder(typeof(I));
            if (builder != null)
                return builder.Create<I>(page);

            throw new ModelNotCreatedException(typeof(I).Name);
        }

        public object CreateModel(string viewName, DD4T.ContentModel.IComponentPresentation componentPresentation)
        {
            object model;
            if (TryCreateModel(viewName, componentPresentation, out model))
                return model;

            throw new ModelNotCreatedException(viewName);
        }


        public bool TryCreateModel(string viewName, DD4T.ContentModel.IComponentPresentation componentPresentation, out object model)
        {
            IBuilder builder = BuilderFactory.GetBuilder(viewName);
            if (builder != null)
            {
                var baseBuilder = builder as BuilderBase;
                if (baseBuilder != null )
                {
                    var builderBase = baseBuilder;
                    builderBase.LinkFactory = LinkFactory;
                }

                model = builder.Create<object>(componentPresentation);

                return true;
            }

            model = null;
            return false;
        }





    }
}
