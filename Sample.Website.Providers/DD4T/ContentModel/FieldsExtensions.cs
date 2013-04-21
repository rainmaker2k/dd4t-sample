using System;
using System.Collections.Generic;
using System.Linq;

namespace DD4T.ContentModel
{
    /// <summary>
    /// Shortcuts for accessing component fields.
    /// </summary>
    public static class FieldsExtensions
    {
        public static IEnumerable<T> Field<T>(this IFieldSet fields, string name, Func<IField, IEnumerable<T>> func)
        {
            return null != fields && fields.ContainsKey(name) ? func(fields[name]) : Enumerable.Empty<T>();
        }
        
        public static T Field<T>(this IFieldSet fields, string name, Func<IField, T> func) {
            return null != fields && fields.ContainsKey(name) ? func(fields[name]) : default(T);
        }

        public static string Field(this IFieldSet fields, string name)
        {
            return fields.Field(name, fld => fld.Value);
        }

        public static string Image(this IFieldSet fields, string name = "Image")
        {
            return fields.Field(name,
                fld => fld.LinkedComponentValues
                    .Select(c => c.Multimedia.Url)
                    .FirstOrDefault()
            );
        }

        public static string FieldOrEmpty(this IFieldSet fields, string name)
        {
            return Field(fields, name) ?? string.Empty;
        }
    }
}