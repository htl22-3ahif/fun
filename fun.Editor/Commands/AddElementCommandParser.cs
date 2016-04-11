using fun.Core;
using fun.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Environment = fun.Core.Environment;

namespace fun.Editor.Commands
{
    internal sealed class AddElementCommandParser : CommandParser
    {
        public override string Keyword
        {
            get
            {
                return "element";
            }
        }

        protected override void Do(string[] args)
        {
            Environment env;
            var envPath = args[0];
            string[] libaries;

            using (var file = new FileStream(envPath, FileMode.Open, FileAccess.Read))
            {
                env = new EnvironmentXmlReader().Load(file, out libaries)[0];
            }

            var assemblies = new Assembly[libaries.Length];

            for (int i = 0; i < libaries.Length; i++)
                assemblies[i] = Assembly.LoadFrom(libaries[i]);

            var type = assemblies
                .First(a => a.ExportedTypes.Any(t => t.Name == args[2]))
                .ExportedTypes
                .First(t => t.Name == args[2]);

            env.GetEntity(args[1]).AddElement(type);
            Console.WriteLine("Element \"{0}\" in Entity \"{0}\" from Environment \"{0}\" added", args[2], args[1], args[0]);

            using (var file = new FileStream(envPath, FileMode.Open, FileAccess.Write))
            {
                new EnvironmentXmlWriter().Save(file, env, libaries);
            }
        }
    }
}
