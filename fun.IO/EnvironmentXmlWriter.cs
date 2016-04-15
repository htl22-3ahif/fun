using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using fun.Core;
using Environment = fun.Core.Environment;
using System.Runtime.Serialization;
using System.Reflection.Emit;

namespace fun.IO
{
    public sealed class EnvironmentXmlWriter
    {
        public EnvironmentXmlWriter()
        {

        }

        public void Save(Stream output, Environment environment, params string[] libs)
        {
            var doc = new XmlDocument();
            doc.InsertBefore(doc.CreateXmlDeclaration("1.0", "UTF-8", null), doc.DocumentElement);

            var xmlenv = doc.CreateElement("Environment");
            foreach (var lib in libs)
            {
                var xmllib = doc.CreateElement("Lib");
                xmllib.SetAttribute("Path", lib);
                xmlenv.AppendChild(xmllib);
            }

            foreach (var entity in environment.Entities)
            {
                var xmlentity = doc.CreateElement("Entity");
                xmlentity.SetAttribute("Name", entity.Name);
                foreach (var element in entity.Elements)
                {
                    var xmlelement = doc.CreateElement("Element");
                    xmlelement.SetAttribute("Type", element.GetType().Name);

                    var elemtype = element.GetType();
                    foreach (var field in elemtype.GetFields(BindingFlags.Instance | BindingFlags.Public))
                    {
                        var xmlfield = doc.CreateElement("Field");
                        xmlfield.SetAttribute("Name", field.Name);

                        if (field.FieldType.IsPrimitive || field.FieldType == typeof(string))
                            PrimitiveOrStringTypeHandler(doc, field, xmlfield, element);
                        else if (field.FieldType.IsArray)
                            ArrayTypeHandler(doc, field, xmlfield, element);
                        else if (field.FieldType.IsClass || field.FieldType.IsValueType)
                            ClassOrStructTypeHandler(doc, field, xmlfield, element);

                        xmlelement.AppendChild(xmlfield);
                    }
                    xmlentity.AppendChild(xmlelement);
                }
                xmlenv.AppendChild(xmlentity);
            }
            doc.AppendChild(xmlenv);

            doc.Save(output);
        }

        private void ClassOrStructTypeHandler(XmlDocument doc, FieldInfo field, XmlElement xmlfield, object receiver)
        {
            var fields = field.FieldType.GetFields(
                BindingFlags.Instance | BindingFlags.Public);

            foreach (var _field in fields)
            {
                var _xmlfield = doc.CreateElement("Field");
                _xmlfield.SetAttribute("Name", _field.Name);

                var _receiver = field.GetValue(receiver);

                if (_field.FieldType.IsPrimitive || _field.FieldType == typeof(string))
                    PrimitiveOrStringTypeHandler(doc, _field, _xmlfield, _receiver);
                else if (_field.FieldType.IsArray)
                    ArrayTypeHandler(doc, _field, _xmlfield, _receiver);
                else if (_field.FieldType.IsClass || _field.FieldType.IsValueType)
                    ClassOrStructTypeHandler(doc, _field, _xmlfield, _receiver);

                xmlfield.AppendChild(_xmlfield);
            }
        }

        private void ArrayTypeHandler(XmlDocument doc, FieldInfo field, XmlElement xmlfield, object receiver)
        {
            var array = field.GetValue(receiver) as Array;

            var an = new AssemblyName("dynass");
            var ab = AppDomain.CurrentDomain.DefineDynamicAssembly(an, AssemblyBuilderAccess.Run);
            var mb = ab.DefineDynamicModule("dynmod");
            var tb = mb.DefineType("ArrayElement");

            for (int i = 0; i < array.Length; i++)
            {
                var fb = tb.DefineField("_" + i, field.FieldType.GetElementType(), FieldAttributes.Public);
            }

            var _receiver = FormatterServices.GetUninitializedObject(tb.CreateType());

            for (int i = 0; i < array.Length; i++)
            {
                _receiver.GetType().GetField("_" + i).SetValue(_receiver, array.GetValue(i));
            }

            for (int i = 0; i < array.Length; i++)
            {
                var _xmlfield = doc.CreateElement("Field");
                _xmlfield.SetAttribute("Name", "_" + i);

                if (field.FieldType.GetElementType().IsPrimitive || field.FieldType.GetElementType() == typeof(string))
                    PrimitiveOrStringTypeHandler(doc, 
                        _receiver.GetType().GetField("_" + i, BindingFlags.Instance | BindingFlags.Public), 
                        _xmlfield, _receiver);
                else if (field.FieldType.GetElementType().IsArray)
                    ArrayTypeHandler(doc, _receiver.GetType().GetField("_" + i, BindingFlags.Instance | BindingFlags.Public), 
                        _xmlfield, _receiver);
                else if (field.FieldType.GetElementType().IsClass || field.FieldType.GetElementType().IsValueType)
                    ClassOrStructTypeHandler(doc, _receiver.GetType().GetField("_" + i, BindingFlags.Instance | BindingFlags.Public), 
                        _xmlfield, _receiver);

                xmlfield.AppendChild(_xmlfield);
            }
        }

        private void PrimitiveOrStringTypeHandler(XmlDocument doc, FieldInfo field, XmlElement xmlfield, object receiver)
        {
            xmlfield.InnerText = field.GetValue(receiver).ToString();
        }
    }
}
