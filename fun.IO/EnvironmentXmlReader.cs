using fun.IO.Data;
using fun.IO.XmlParsers;
using System.IO;
using System.Xml;
using System.Linq;
using Environment = fun.Core.Environment;
using System.Reflection;

namespace fun.IO
{
    public sealed class EnvironmentXmlReader
    {
        private DataStore data;
        private XmlParser[] parsers;

        public EnvironmentXmlReader()
        {
            data = new DataStore();
            parsers = new XmlParser[]
            {
                new EnvironmentXmlParser(data)
            };
        }

        public Environment[] Load(Stream input, out string[] libaries)
        {
            var doc = new XmlDocument();
            doc.Load(input);

            foreach (var node in doc.ChildNodes.OfType<XmlNode>())
                foreach (var parser in parsers)
                    if (parser.TryParse(node))
                        parser.Parse(node);

            // a really complex way of getting the relative path 
            // (and its only working, when exe and dll is in the same directory)
            // (if not, then it takes the full path)

            var myAss = Assembly.GetExecutingAssembly();
            var pathToMyAss = myAss.CodeBase.Substring(0, myAss.CodeBase.LastIndexOf('/'));

            libaries = data.Assemblys
                .Select(a => a.CodeBase
                    .Substring(pathToMyAss == a.CodeBase
                        .Substring(0, a.CodeBase.LastIndexOf('/')) ? 
                        pathToMyAss.Length + 1 : 0))
                .ToArray();

            return data.Envionments;
        }
    }
}
