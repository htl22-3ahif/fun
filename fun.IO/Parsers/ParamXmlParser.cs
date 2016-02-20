using fun.IO.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace fun.IO.XmlParsers
{
    internal sealed class ParamXmlParser : XmlParser
    {
        IParamDataStore data;
        MyDataStore mydata;

        public ParamXmlParser(IParamDataStore data)
        {
            this.data = data;
            this.mydata = new MyDataStore(data);
        }

        public override bool TryParse(XmlNode node)
        {
            return node.Name == "Param";
        }

        public override void Parse(XmlNode node)
        {
            var type = Type.GetType(node.Attributes[typeof(Type).Name].Value);
            var value = node.InnerText;
            
            if (type == null)
                foreach (Assembly assembly in data.Assemblys)
                    foreach (var exptype in assembly.ExportedTypes)
                        if (exptype.FullName == node.Attributes[typeof(Type).Name].Value)
                            type = exptype;
                            

            if (type.IsPrimitive)
                data.PushParam(Convert.ChangeType(value, type));
            else if (type == typeof(string))
                data.PushParam(value.ToCharArray());
            else if (type.IsArray)
            {
                var parser = new ParamXmlParser(mydata);

                foreach (var _node in node.OfType<XmlNode>())
                    if (parser.TryParse(_node))
                        parser.Parse(_node);

                Array paramObject = (Array)Activator.CreateInstance(type, mydata.Params.Length);
                for (int i = 0; i < mydata.Params.Length; i++)
                    paramObject.SetValue(mydata.Params[i], i);
                data.PushParam(paramObject);
                mydata.FlushParams();
            }
            else if (type.IsClass || type.IsValueType)
            {
                var parser = new ParamXmlParser(mydata);

                foreach (var _node in node.OfType<XmlNode>())
                    if (parser.TryParse(_node))
                        parser.Parse(_node);

                var paramObject = Activator.CreateInstance(type, mydata.Params);
                data.PushParam(paramObject);
                mydata.FlushParams();
            }
            else
                throw new NotImplementedException();
        }

        private sealed class MyDataStore : IParamDataStore
        {
            private List<object> parameters = new List<object>();
            private IParamDataStore data;

            public MyDataStore(IParamDataStore data)
            {
                this.data = data;
            }

            public Assembly[] Assemblys
            {
                get
                {
                    return data.Assemblys;
                }
            }

            public object[] Params { get { return parameters.ToArray(); } }

            public void PushParam(object param)
            {
                parameters.Add(param);
            }

            public void FlushParams()
            {
                parameters.Clear();
            }
        }
    }
}
