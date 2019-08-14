using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace CardTricks.Utils
{
    public static class FontHelper
    {
        public static readonly string[] InstalledFamilies;
        public static readonly string[] InstalledTypefaces;

        static FontHelper()
        {
            InstalledFamilies = GetAllFontFamilies();
            InstalledTypefaces = GetAllFontTypefaces();
        }

        private static string[] GetAllFontFamilies()
        {
            List<string> fams = new List<string>(5);
            foreach (FontFamily fam in Fonts.SystemFontFamilies)
            {
                fams.Add(fam.Source);
            }

            return fams.ToArray();
        }

        private static string[] GetAllFontTypefaces()
        {
            List<string> fams = new List<string>(5);
            foreach (Typeface fam in Fonts.SystemTypefaces)
            {
                IEnumerator<KeyValuePair<System.Windows.Markup.XmlLanguage,string>> faces = fam.FaceNames.GetEnumerator();
                faces.MoveNext();
                string faceName = faces.Current.Value;
                fams.Add(fam.FontFamily + " " + faceName);
            }

            return fams.ToArray();
        }
        
    }
}
