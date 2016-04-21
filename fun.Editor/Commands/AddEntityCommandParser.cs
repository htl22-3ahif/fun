using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fun.Core;
using Environment = fun.Core.Environment;
using System.IO;
using fun.IO;

namespace fun.Editor.Commands
{
    internal sealed class AddEntityCommandParser : CommandParser
    {
        public override string Keyword
        {
            get
            {
                return "entity";
            }
        }

        protected override void Do(string[] args)
        {
            var err = Console.Error;
            if (args == null || args.Length < 2)
            {
                err.WriteLine("There are no Arguments");
                err.WriteLine("Arguments: <environment_file> <entity_name>");
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

            env.AddEntity(new Entity(args[1], env));
            Console.WriteLine("Entity \"{0}\" in Environment \"{1}\" added!", args[1], args[0]);

            try
            {
                using (var file = new FileStream(envPath, FileMode.Create, FileAccess.Write))
                {
                    new EnvironmentXmlWriter().Save(file, env, libaries);
                }
            }
            catch (Exception)
            {
                err.WriteLine("Assembly could not be load");
                return;
            }
        }
    }
}
