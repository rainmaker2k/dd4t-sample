// -----------------------------------------------------------------------
// <copyright file="BuilderFactory.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System.Configuration;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;

namespace Sample.Website.Mapping.Tridion
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Reflection;
    using Sample.Website.Mapping.Tridion;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class BuilderFactory
    {
        private static readonly Dictionary<string, IBuilder> BuilderCache = new Dictionary<string, IBuilder>();

        private static IUnityContainer container;

        public static IBuilder GetBuilder(string viewName)
        {
            IBuilder builder = null;

            if (!BuilderCache.TryGetValue(viewName, out builder))
            {

                Type builderType = typeof(BuilderFactory).Assembly.GetTypes().FirstOrDefault(t => typeof(IBuilder).IsAssignableFrom(t) && t.Name.StartsWith(viewName));
                if (builderType != null)
                {
                    ConstructorInfo ctor = builderType.GetConstructor(Type.EmptyTypes);
                    if (ctor != null)
                    {
                        object instance = ctor.Invoke(null);
                        builder = GetContainer().BuildUp(builderType, instance) as IBuilder;
                        if (builder != null)
                            BuilderCache.Add(viewName, builder);
                    }
                }
            }

            return builder;
        }

        private static IUnityContainer GetContainer()
        {
            if (container != null)
                return container;

            container = new UnityContainer();

            var section = (UnityConfigurationSection)ConfigurationManager.GetSection("unity");
            section.Configure(container, "main");
            return container;
        }

        public static IBuilder GetBuilder(Type interfaceType)
        {
            return GetBuilder(interfaceType.IsInterface ? interfaceType.Name.Substring(1) : interfaceType.Name);
        }

        #region MapToEnum
        internal static bool TryMapToEnum<TSource, TEnum>(string value, out TEnum enumType) where TEnum : struct
        {
            System.Reflection.FieldInfo info = typeof(TSource).GetFields().FirstOrDefault(x => x.GetRawConstantValue().Equals(value));
            if (info != null)
            {
                if (Enum.TryParse<TEnum>(info.Name, out enumType))
                    return true;
            }
            enumType = default(TEnum);
            return false;
        }

        internal static TEnum MapToEnum<TSource, TEnum>(string value) where TEnum : struct
        {
            TEnum enumType;
            if (TryMapToEnum<TSource, TEnum>(value, out enumType))
                return enumType;
            else
            {
                System.Diagnostics.Trace.TraceWarning("Could not map '{0}' value '{1}' to enumeration '{2}'", typeof(TSource).Name, value, typeof(TEnum).Name);
                return default(TEnum);
            }
        }
        #endregion MapToEnum
    }
}
