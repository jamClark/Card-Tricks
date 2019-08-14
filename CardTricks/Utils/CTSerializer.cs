using CardTricks.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CardTricks.Utils
{
    public static class CTSerializer
    {

        public static void Write(XmlWriter writer, object data, string openingElementName)
        {
            if (writer == null) return;
            if (data == null) return;

            //confirm we can serialize this object
            Type type = data.GetType();
            var serAttr = type.GetCustomAttribute<SaveableObjectAttribute>();
            if (serAttr == null)    throw new Exception("Data is not flagged as serializable.");
                
            writer.WriteStartDocument();
            writer.WriteStartElement(openingElementName);

            //first thing we need to do is seek out saveable data within this serializable object
            foreach (PropertyInfo property in type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic |
                                                        BindingFlags.GetField | BindingFlags.GetProperty |
                                                        BindingFlags.Instance))
            {
                object propData = property.GetValue(data);
                WriteObject(writer, property, propData);
            }

            foreach (FieldInfo property in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic |
                                                        BindingFlags.GetField | BindingFlags.GetProperty |
                                                        BindingFlags.Instance))
            {
                object propData = property.GetValue(data);
                WriteObject(writer, property, propData);
            }

            writer.WriteEndElement();
            writer.WriteEndDocument();
        }

        private static void WriteObject(XmlWriter writer, MemberInfo field, object data)
        {
            Type type = data.GetType();
            var serAttr = type.GetCustomAttribute<SaveableObjectAttribute>();
            var saveAttr = field.GetCustomAttribute<SaveableAttribute>();

            //we give Serialization objects preference over Saveable ones
            if (saveAttr == null && serAttr == null) return;
            else if (saveAttr != null && serAttr != null)
            {
                //this field is saveable, but it is also of a type that has the SaveableObject attribute.
                //So we must give that preference over simply saving local data.
                writer.WriteStartElement(field.Name);

                foreach (PropertyInfo property in type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic |
                                                        BindingFlags.GetField | BindingFlags.GetProperty |
                                                        BindingFlags.Instance))
                {
                    object propData = property.GetValue(data);
                    WriteObject(writer, property, propData);
                }

                foreach (FieldInfo property in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic |
                                                            BindingFlags.GetField | BindingFlags.GetProperty |
                                                            BindingFlags.Instance))
                {
                    object propData = property.GetValue(data);
                    WriteObject(writer, property, propData);
                }

                writer.WriteEndElement();
            }
            else if (saveAttr != null)
            {
                string name = (saveAttr.Name == null) ? field.Name : saveAttr.Name;
                //no recursion, just write it
                writer.WriteElementString(name, data.ToString() );
            }
            
        }

        public static void Write(Stream stream, object data)
        {
            throw new Exception("Not implemented.");
        }
    }
}
