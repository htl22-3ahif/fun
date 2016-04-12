using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace fun.Editor.Commands
{
    internal sealed class ViewCommandParser : CommandParser
    {
        public override string Keyword
        {
            get
            {
                return "view";
            }
        }

        protected override void Do(string[] args)
        {
            XmlDocument doc = new XmlDocument();
            Console.WriteLine("\"{0}\" XML file:", args[0]);
            Console.WriteLine();
            doc.Load(args[0]);
            doc.Save(Console.Out);
            Console.WriteLine();
            Console.WriteLine();
        }
    }
}
