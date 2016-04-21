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
            var err = Console.Error;
            err.WriteLine("There is no subcommand");
            err.WriteLine("Subcommands: ");
            foreach (var subCommand in subCommands)
                err.WriteLine("- {0}", subCommand.Keyword);
        }
    }
}
