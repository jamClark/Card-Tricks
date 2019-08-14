using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace CardTricks.Utils
{
    /// <summary>
    /// Used to generate xaml elements in code-behind.
    /// </summary>
    public static class XamlTool
    {
        public static DataTemplate CreateDataTemplate(Type viewModelType, Type viewType)
        {
            const string xamlTemplate = "<DataTemplate DataType=\"{{x:Type vm:{0}}}\"><v:{1} /></DataTemplate>";
            var xaml = String.Format(xamlTemplate, viewModelType.Name, viewType.Name, viewModelType.Namespace, viewType.Namespace);

            var context = new ParserContext();

            context.XamlTypeMapper = new XamlTypeMapper(new string[0]);
            context.XamlTypeMapper.AddMappingProcessingInstruction("vm", viewModelType.Namespace, viewModelType.Assembly.FullName);
            context.XamlTypeMapper.AddMappingProcessingInstruction("v", viewType.Namespace, viewType.Assembly.FullName);

            context.XmlnsDictionary.Add("", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
            context.XmlnsDictionary.Add("x", "http://schemas.microsoft.com/winfx/2006/xaml");
            context.XmlnsDictionary.Add("vm", "vm");
            context.XmlnsDictionary.Add("v", "v");

            var template = (DataTemplate)XamlReader.Parse(xaml, context);
            return template;
        }

        /// <summary>
        /// Performs rudimentary parsing of xaml ControlTemplates. It cannot access any
        /// application classes in order to bind with them.
        /// </summary>
        /// <param name="xaml"></param>
        /// <returns></returns>
        public static ControlTemplate CreateControlTemplate(string xaml)
        {
            var context = new ParserContext();

            context.XmlnsDictionary.Add("", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
            context.XmlnsDictionary.Add("x", "http://schemas.microsoft.com/winfx/2006/xaml");

            ControlTemplate template = (ControlTemplate)XamlReader.Parse(xaml, context);

            //var key = template.VisualTree.Name;
            //Application.Current.Resources.Add(key, template);
            return template;
        }

        /// <summary>
        /// Performs rudimentary parsing of xaml Styles. It cannot access any
        /// application classes in order to bind with them.
        /// </summary>
        /// <param name="xaml"></param>
        /// <returns></returns>
        public static Style CreateControlStyle(string xaml)
        {
            var context = new ParserContext();

            context.XmlnsDictionary.Add("", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
            context.XmlnsDictionary.Add("x", "http://schemas.microsoft.com/winfx/2006/xaml");

            Style template = (Style)XamlReader.Parse(xaml, context);

            //var key = template.VisualTree.Name;
            //Application.Current.Resources.Add(key, template);
            return template;
        }
    }
}
