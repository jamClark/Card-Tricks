using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CardTricks.Utils
{
    public class AlignmentPair
    {
        public AlignmentPair(string key, TextAlignment value)
        {
            this.Key = key;
            this.Value = value;
        }

        public string Key { get; set; }
        public TextAlignment Value { get; set; }
    };
}
