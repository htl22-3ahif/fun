using fun.IO.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace fun.IO.XmlParsers
{
    internal sealed class ParamXmlParser : XmlParser
    {
        IParamDataStore data;

        public ParamXmlParser(IParamDataStore data)
        {
            this.data = data;

            //coming soon (w/ sneaky shit)
            //this.parsers = new Parser[]
            //{
            //    new ParamParser(mydata)
            //};
        }

        public override bool TryParse(XmlNode node)
        {
            return node.Name == "Param";
        }

        public override void Parse(XmlNode node)
        {
            var type = Type.GetType(node.Attributes[typeof(Type).Name].Value);
			var value = node.InnerText;

            if (type.IsPrimitive)
                data.PushParam(Convert.ChangeType(value, type));
            else if (type == typeof(string))
                data.PushParam(value.ToCharArray());
            else
                throw new NotImplementedException();
        }
    }
}
