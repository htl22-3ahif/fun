using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fun.Editor
{
    internal sealed class Commands
    {
        private CommandParser[] parsers;

        public Commands(params CommandParser[] parsers)
        {
            this.parsers = parsers;
        }

        public void Parse(string[] args)
        {
            foreach (var parser in parsers)
                if (parser.TryParse(args))
                    parser.Parse(args);
        }
    }
}
