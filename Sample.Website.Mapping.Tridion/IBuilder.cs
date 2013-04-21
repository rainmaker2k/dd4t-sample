// -----------------------------------------------------------------------
// <copyright file="IBuilder.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Sample.Website.Mapping.Tridion
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using DD4T.ContentModel;

    public interface IBuilder
    {
        T Create<T>(IComponentPresentation componentPresentation,string language);
        T Create<T>(IComponentPresentation componentPresentation);
        T Create<T>(IPage page);
    }
}
