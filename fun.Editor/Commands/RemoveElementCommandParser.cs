using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fun.Core;
using Environment = fun.Core.Environment;
using System.IO;
using fun.IO;
using System.Reflection;

namespace fun.Editor.Commands
{
    internal sealed class RemoveElementCommandParser : CommandParser
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
                err.WriteLine("There are no arguments");
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
                err.WriteLine("File could not be found");
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

            env.GetEntity(args[1]).RemoveElement(type);
            Console.WriteLine("Element \"{0}\" in Entity \"{1}\" from Environment \"{2}\" removed!", args[2], args[1], args[0]);

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
