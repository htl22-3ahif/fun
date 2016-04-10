using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Environment = fun.Core.Environment;
using fun.IO;

namespace fun.Editor.Commands
{
    internal sealed class GetCommandParser : CommandParser
    {
        public Environment Environment { get; private set; }

        public override string Keyword
        {
            get
            {
                return "get";
            }
        }

        public GetCommandParser()
        {
            Environment = null;

            subCommands = new CommandParser[]
            {
                new AddCommandParser(this)
            };
        }

        protected override void Do(string[] args)
        {
            var envPath = args[0];
            string[] libaries;

            using (var file = new FileStream(envPath, FileMode.Open, FileAccess.Read))
            {
                Environment = new EnvironmentXmlReader().Load(file, out libaries)[0];
            }

            args = args.Skip(1).ToArray();

            foreach (var c in subCommands)
                if (c.TryParse(args))
                    c.Parse(args);

            using (var file = new FileStream(envPath, FileMode.Open, FileAccess.Write))
            {
                new EnvironmentXmlWriter().Save(file, Environment, libaries);
            }
        }
    }
}
