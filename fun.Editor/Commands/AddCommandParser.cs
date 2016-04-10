using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fun.Editor.Commands
{
    internal sealed class AddCommandParser : CommandParser
    {
        private GetCommandParser get;

        public override string Keyword
        {
            get
            {
                return "add";
            }
        }

        public AddCommandParser(GetCommandParser get)
        {
            this.get = get;

            subCommands = new CommandParser[]
            {
                new AddEntityCommandParser(get)
            };
        }

        protected override void Do(string[] args)
        {

        }
    }
}
