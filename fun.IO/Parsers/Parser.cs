using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Environment = fun.Core.Environment;

namespace fun.IO.Parsers
{
    internal abstract class Parser
    {
        protected Parser[] parsers;

        public abstract bool TryParse(XmlNode node);
        public abstract void Parse(XmlNode node);
    }
}
