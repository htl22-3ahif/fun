using fun.IO.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Runtime.Serialization;
using System.Reflection.Emit;

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
            else if (field.FieldType.IsArray)
                ArrayTypeHandler(node, name, field, value);
            else if (field.FieldType.IsClass || field.FieldType.IsValueType)
                ClassOrStructTypeHandler(node, name, field, value);
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
            /*
            Ok, this is going to be more complex than I thought.
            So here is the problem:
            
            I want to define in an array fields, so you can add this fields to
            your array. So the way I want to do it, is to get the receiver to contain a
            certain field named "_". This field must be the type, the array contains as his elements.
            
            It is like preparing the object to be like to look like a normal object to the fieldparser
            so i do not have to make a complitely new parser. 
            
            The object must contain a field (name: _) and the field must be the type of the arrays element
            and I do not manage to get to be this type, so when you call _.gettype() you get the type of the arrays element
            
            Now after a while, I got a pretty sick solution:
            
            To build a completily new type, I have to build a new assembly and module,
            but after that, I finaly got my dynamic type, with the ElementType*/

            var count = node.ChildNodes.OfType<XmlNode>()
                .Count(n => n.Name == "Field");

            var an = new AssemblyName("dynass");
            var ab = AppDomain.CurrentDomain.DefineDynamicAssembly(an, AssemblyBuilderAccess.Run);
            var mb = ab.DefineDynamicModule("dynmod");
            var tb = mb.DefineType("ArrayElement");

            MyDataStore mydata = new MyDataStore();
            var parser = new FieldXmlParser(mydata);
            var array = Array.CreateInstance(field.FieldType.GetElementType(), count);

            for (int i = 0; i < count; i++)
            {
                var fb = tb.DefineField("_" + i, field.FieldType.GetElementType(), FieldAttributes.Public);
            }

            mydata.Receiver = FormatterServices.GetUninitializedObject(tb.CreateType());

            for (int i = 0; i < count; i++)
            {
                var _node = node.ChildNodes.OfType<XmlNode>()
                    .Where(n => n.Name == "Field").ElementAt(i);

                parser.Parse(_node);

                array.SetValue(mydata.Receiver
                    .GetType().GetField("_" + i)
                    .GetValue(mydata.Receiver), i);
            }

            field.SetValue(data.Receiver, array);
        }

        private class MyDataStore : IFieldDataStore
        {
            public object Receiver { get; set; }
        }
    }
}
