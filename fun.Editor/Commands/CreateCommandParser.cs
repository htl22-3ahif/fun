using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Environment = fun.Core.Environment;
using fun.IO;

namespace fun.Editor.Commands
{
    internal sealed class CreateCommandParser : CommandParser
    {
        public override string Keyword
        {
            get
            {
                return "create";
            }
        }

        public CreateCommandParser()
        {
        }

        protected override void Do(string[] args)
        {
            var env = new Environment();
            using (var file = new FileStream(args[0], FileMode.Create, FileAccess.Write))
            {
                new EnvironmentXmlWriter().Save(file, env, args.Skip(1).ToArray());
            }

            Console.WriteLine("created!");
        }
    }
}
