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
            var err = Console.Error;
            if (args == null || args.Length < 3)
            {
                err.WriteLine("There are no Arguments");
                err.WriteLine("Arguments: <environment_file> <entity_name> <element_type>");
                return;
            }

            Environment env;
            var envPath = args[0];
            string[] libaries;

            try
            {
                using (var file = new FileStream(envPath, FileMode.Open, FileAccess.Read))
                {
                    env = new EnvironmentXmlReader().Load(file, out libaries)[0];
                }
            }
            catch (Exception)
            {
                err.WriteLine("File could not bee found");
                return;
            }

            var assemblies = new Assembly[libaries.Length];

            try
            {
                for (int i = 0; i < libaries.Length; i++)
                    assemblies[i] = Assembly.LoadFrom(libaries[i]);
            }
            catch (Exception)
            {
                err.WriteLine("Assembly could not be load");
                return;
            }

            Type type;
            try
            {
                type = assemblies
                    .First(a => a.ExportedTypes.Any(t => t.Name == args[2]))
                    .ExportedTypes
                    .First(t => t.Name == args[2]);
            }
            catch (Exception)
            {
                err.WriteLine("Element could not be found");
                return;
            }

            env.GetEntity(args[1]).AddElement(type);
            Console.WriteLine("Element \"{2}\" in Entity \"{1}\" from Environment \"{0}\" added!", args[2], args[1], args[0]);

            try
            {
                using (var file = new FileStream(envPath, FileMode.Create, FileAccess.Write))
                {
                    new EnvironmentXmlWriter().Save(file, env, libaries);
                }
            }
            catch (Exception)
            {
                err.WriteLine("Writing in path {0} failed", envPath);
                return;
            }
        }
    }
}
