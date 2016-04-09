using fun.IO.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Runtime.Serialization;

namespace fun.IO.XmlParsers
{
    internal sealed class FieldXmlParser : XmlParser
    {
        IFieldDataStore data;

        public FieldXmlParser(IFieldDataStore data)
        {
            this.data = data;
            this.parsers = new XmlParser[]
            {
                //coming soon (ArrayElementXmlParser)
            };
        }

        public override bool TryParse(XmlNode node)
        {
            return node.Name == "Field";
        }

        public override void Parse(XmlNode node)
        {
            var name = node.Attributes["Name"].Value;
            var field = data.Receiver.GetType().GetField(name, 
                BindingFlags.Instance | BindingFlags.Public);
            var value = node.InnerText;

            if (field.FieldType.IsPrimitive || field.FieldType == typeof(string))
                PrimitiveOrStringTypeHandler(node, name, field, value);
            else if (field.FieldType.IsClass || field.FieldType.IsValueType)
                ClassOrStructTypeHandler(node, name, field, value);
            else if (field.FieldType.IsArray)
                ArrayTypeHandler(node, name, field, value);
        }

        private void PrimitiveOrStringTypeHandler(XmlNode node, string name, FieldInfo field, object value)
        {
            field.SetValue(data.Receiver, Convert.ChangeType(value, field.FieldType));
        }

        private void ClassOrStructTypeHandler(XmlNode node, string name, FieldInfo field, object value)
        {
            MyDataStore mydata = new MyDataStore();
            mydata.Receiver = FormatterServices.GetUninitializedObject(field.FieldType);

            var parser = new FieldXmlParser(mydata);
            foreach (XmlNode _node in node.ChildNodes)
                if (parser.TryParse(_node))
                    parser.Parse(_node);

            field.SetValue(data.Receiver, mydata.Receiver);
        }

        private void ArrayTypeHandler(XmlNode node, string name, FieldInfo field, string value)
        {
            throw new NotImplementedException();
        }

        private class MyDataStore : IFieldDataStore
        {
            public object Receiver { get; set; }
        }
    }
}
