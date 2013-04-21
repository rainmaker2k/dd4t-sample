// -----------------------------------------------------------------------
// <copyright file="BuilderBase.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using DD4T.ContentModel;
using DD4T.ContentModel.Factories;
using Sample.Website.Mapping.Tridion.Helpers;

namespace Sample.Website.Mapping.Tridion
{
    using System;
    using System.Reflection;
    using Helpers;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public abstract class BuilderBase : IBuilder
    {
        public BuilderBase()
        {
            RichTextHelper = new RichTextHelper();
        }

        private object CreateModel(IComponentPresentation componentPresentation)
        {
            return GetType().GetMethod("Create", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(IComponentPresentation) }, null).Invoke(this, new object[] { componentPresentation });
        }

        private object CreateModel(IComponentPresentation componentPresentation,string language)
        {
            return GetType().GetMethod("Create", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(IComponentPresentation) }, null).Invoke(this, new object[] { componentPresentation, language });
        }


        public bool TryCreate<T>(IComponentPresentation componentPresentation, out T model)
        {
            try
            {
                model = (T)CreateModel(componentPresentation);
                return true;
            }
            catch (Exception exception)
            {
                throw exception;
                model = default(T);
                return false;
            }
        }

        public bool TryCreate<T>(IComponentPresentation componentPresentation, string language, out T model)
        {
            try
            {
                model = (T)CreateModel(componentPresentation,language);
                return true;
            }
            catch
            {
                throw;
                model = default(T);
                return false;
            }
        }

        public virtual I Create<I>(IComponentPresentation componentPresentation)
        {
            I model;
            if (TryCreate<I>(componentPresentation, out model))
                return model;
            else
            {
                System.Diagnostics.Trace.TraceWarning("Could not create model for '{0}'", typeof(I).Name);
                return default(I);
            }
        }

        public virtual I Create<I>(IComponentPresentation componentPresentation,string language)
        {
            I model;
            if (TryCreate<I>(componentPresentation, language, out model))
                return model;
            else
            {
                System.Diagnostics.Trace.TraceWarning("Could not create model for '{0}'", typeof(I).Name);
                return default(I);
            }
        }

        public I Create<I>(IPage page)
        {
            return (I)GetType().GetMethod("Create", BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(IPage) }, null).Invoke(this, new object[] { page });
        }


        #region MapToEnum
        protected bool TryMapToEnum<TSource, TEnum>(string value, out TEnum enumType) where TEnum : struct
        {
            return BuilderFactory.TryMapToEnum<TSource, TEnum>(value, out enumType);
        }

        protected TEnum MapToEnum<TSource, TEnum>(string value) where TEnum : struct
        {
            return BuilderFactory.MapToEnum<TSource, TEnum>(value);
        }

        //protected SchemaTypes MapSchemaType(string value)
        //{
        //    return MapToEnum<SchemaNames, SchemaTypes>(value);
        //}

        #endregion MapToEnum

        #region Properties
        private ILinkFactory _linkFactory;
        public ILinkFactory LinkFactory
        {
            get
            {
                return _linkFactory;
            }
            set
            {
                this.RichTextHelper.LinkFactory = value;
                _linkFactory = value;
            }
        }

        protected RichTextHelper RichTextHelper { get; set; }

        #endregion Properties
    }
}
