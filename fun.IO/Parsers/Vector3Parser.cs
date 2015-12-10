﻿using fun.IO.Data;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace fun.IO.Parsers
{
    internal sealed class Vector3Parser : Parser
    {
        private IElementPropertyDataStore data;

        public Vector3Parser(IElementPropertyDataStore data)
        {
            this.data = data;
            this.parsers = null;
        }

        public override bool TryParse(XmlNode node)
        {
            var type = data.Element.GetType();
            return type.GetProperty(node.Name).PropertyType == typeof(Vector3);
        }

        public override void Parse(XmlNode node)
        {
            var parts = node.Value.Split('/');

            var propInfo = data.Element.GetType().GetProperty(node.Name);
            propInfo.SetValue(data.Element, new Vector3(
                float.Parse(parts[0]), 
                float.Parse(parts[1]), 
                float.Parse(parts[2])));
        }
    }
}
