using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Environment = fun.Core.Environment;

namespace fun.IO
{
    public sealed class EnvironmentXmlWriter
    {
        public EnvironmentXmlWriter()
        {

        }

        public void Save(Stream output, Environment environment, string[] libs = null)
        {
            var doc = new XmlDocument();

            var xmlenv = doc.CreateElement("Environment");
            foreach (var lib in libs)
            {
                var xmllib = doc.CreateElement("Lib");
                xmllib.SetAttribute("Path", lib);
            }

            foreach (var entity in environment.Entities)
            {
                var xmlentity = doc.CreateElement("Entity");
                xmlentity.SetAttribute("Name", entity.Name);
                foreach (var element in entity.Elements)
                {
                    var xmlelement = doc.CreateElement("Element");
                    var elemtype = element.GetType();
                    foreach (var property in elemtype.GetProperties())
                    {
                        var xmlproperty = doc.CreateElement("Property");
                        xmlproperty.SetAttribute("Name", property.Name);

                        /*
                        Well I got a pretty big problem here
                        gonna describe it in its issue
                        I really do not know what to do
                        */

                        xmlelement.AppendChild(xmlproperty);
                    }
                    xmlentity.AppendChild(xmlelement);
                }
                xmlenv.AppendChild(xmlentity);
            }

            doc.Save(output);
        }
    }
}
