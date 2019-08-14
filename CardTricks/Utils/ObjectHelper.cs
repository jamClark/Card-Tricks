using System;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Markup;
using System.Xml;

namespace CardTricks.Utils
{
    /// <summary>
    /// Reference Article http://www.codeproject.com/KB/tips/SerializedObjectCloner.aspx
    /// Provides a method for performing a deep copy of an object.
    /// Binary Serialization is used to perform the copy.
    /// </summary>
    public static class ObjectHelper
    {
        /// <summary>
        /// Perform a deep Copy of the object using serialization if possible.
        /// </summary>
        /// <typeparam name="T">The type of object being copied.</typeparam>
        /// <param name="source">The object instance to copy.</param>
        /// <returns>The copied object.</returns>
        public static T Clone<T>(T source)
        {
            if (!typeof(T).IsSerializable)
            {
                throw new ArgumentException("The type must be serializable.", "source");
            }

            // Don't serialize a null object, simply return the default for that object
            if (Object.ReferenceEquals(source, null))
            {
                return default(T);
            }

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new MemoryStream();
            using (stream)
            {
                formatter.Serialize(stream, source);
                stream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(stream);
            }
        }

        /// <summary>
        /// Attempts to perform a deep copy of a UIElement using XML serialization.
        /// UIElement by default do not truely support serialization so this method
        /// cannot check specifically for that capacity within the passed control.
        /// In some cases this may cause the deep copy to fail.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="element"></param>
        /// <returns></returns>
        public static T DeepCopyUIElement<T>(T element)
        {
            
            // Don't serialize a null object, simply return the default for that object
            if (Object.ReferenceEquals(element, null))
            {
                return default(T);
            }

            var xaml = XamlWriter.Save(element);
            var xamlString = new StringReader(xaml);
            var xmlTextReader = new XmlTextReader(xamlString);
            var deepCopyObject = (T)XamlReader.Load(xmlTextReader);

            return deepCopyObject;

        }

        /// <summary>
        /// Gets the FieldInfo for an instance of a variable.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expr"></param>
        public static FieldInfo Check<T>(Expression<Func<T>> expr)
        {
            var body = ((MemberExpression)expr.Body);
            return (FieldInfo)body.Member;
        }
    }
}