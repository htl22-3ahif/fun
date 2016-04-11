using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fun.Editor.Commands
{
    internal sealed class AddCommandParser : CommandParser
    {
        public override string Keyword
        {
            get
            {
                return "add";
            }
        }

        public AddCommandParser()
        {
            subCommands = new CommandParser[]
            {
                new AddEntityCommandParser(),
                new AddElementCommandParser()
            };
        }

        protected override void Do(string[] args)
        {

        }
    }
}
