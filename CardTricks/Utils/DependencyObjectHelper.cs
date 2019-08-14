using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup.Primitives;

namespace CardTricks.Utils
{
    public static class DependencyObjectHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static List<DependencyProperty> GetDependencyProperties(Object element)
        {
            List<DependencyProperty> properties = new List<DependencyProperty>();
            MarkupObject markupObject = MarkupWriter.GetMarkupObjectFor(element);
            
            if (markupObject != null)
            {
                foreach (MarkupProperty mp in markupObject.Properties)
                {
                    if (mp.DependencyProperty != null) properties.Add(mp.DependencyProperty);
                }
            }
            return properties;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static DependencyProperty GetDependencyProperty(object element, string name)
        {
            foreach (DependencyProperty prop in GetDependencyProperties(element))
            {
                if (prop.Name.Equals(name)) return prop;
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static List<DependencyProperty> GetAttachedProperties(Object element)
        {
            List<DependencyProperty> attachedProperties = new List<DependencyProperty>();
            MarkupObject markupObject = MarkupWriter.GetMarkupObjectFor(element);

            if (markupObject != null)
            {
                foreach (MarkupProperty mp in markupObject.Properties)
                {
                    if (mp.IsAttached) attachedProperties.Add(mp.DependencyProperty);
                }
            }
            return attachedProperties;
        }

        /// <summary>
        /// Helper for finding all DependecyProperties of a type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<FieldInfo> GetDependencyProperties(Type type)
        {
            var properties = type.GetFields(BindingFlags.Static | BindingFlags.Public)
                                 .Where(f => f.FieldType == typeof(DependencyProperty));
            if (type.BaseType != null)
                properties = properties.Union(GetDependencyProperties(type.BaseType));
            return properties;
        }

        /// <summary>
        /// Helper for finding a named DependencyProperty of a type.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static DependencyProperty GetDependencyProperty(Type type, string name)
        {
            FieldInfo fieldInfo = type.GetField(name, BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Static);
            return (fieldInfo != null) ? (DependencyProperty)fieldInfo.GetValue(null) : null;
        }

    }

}
