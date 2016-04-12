using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fun.Editor.Commands
{
    internal sealed class RemoveCommandParser : CommandParser
    {
        public override string Keyword
        {
            get
            {
                return "rm";
            }
        }

        public RemoveCommandParser()
        {
            this.subCommands = new CommandParser[]
            {
                new RemoveEntityCommandParser(),
                new RemoveElementCommandParser()
            };
        }

        protected override void Do(string[] args)
        {

        }
    }
}
