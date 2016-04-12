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
            Environment env;
            var envPath = args[0];
            string[] libaries;

            using (var file = new FileStream(envPath, FileMode.Open, FileAccess.Read))
            {
                env = new EnvironmentXmlReader().Load(file, out libaries)[0];
            }

            env.AddEntity(new Entity(args[1], env));
            Console.WriteLine("Entity \"{0}\" in Environment \"{1}\" added!", args[1], args[0]);

            using (var file = new FileStream(envPath, FileMode.Create, FileAccess.Write))
            {
                new EnvironmentXmlWriter().Save(file, env, libaries);
            }
        }
    }
}
