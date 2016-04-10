using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fun.Editor
{
    internal abstract class CommandParser
    {
        public abstract string Keyword { get; }
        public abstract CommandParser[] SubCommands { get; }

        public bool TryParse(string[] args)
        {
            if (args.Length < 1)
                return false;

            if (args[0] != Keyword)
                return false;

            if (args.Length > 1)
                if (SubCommands != null)
                    if (!SubCommands.Any(c => c.TryParse(args.Skip(1).ToArray())))
                        return false;

            return true;
        }

        public void Parse(string[] args)
        {
            if (args.Length == 1)
                Do(new string[0]);

            if (SubCommands != null)
                foreach (var c in SubCommands)
                    if (c.Keyword == args[1])
                        c.Parse(args.Skip(1).ToArray());

            var myargs = args.Skip(1).ToArray();
            Do(myargs);
        }

        protected abstract void Do(string[] args);
    }
}
