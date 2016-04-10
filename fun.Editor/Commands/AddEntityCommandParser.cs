using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fun.Core;

namespace fun.Editor.Commands
{
    internal sealed class AddEntityCommandParser : CommandParser
    {
        private GetCommandParser get;

        public override string Keyword
        {
            get
            {
                return "entity";
            }
        }

        public AddEntityCommandParser(GetCommandParser get)
        {
            this.get = get;
        }

        protected override void Do(string[] args)
        {
            get.Environment.AddEntity(new Entity(args[0], get.Environment));
            Console.WriteLine("Entity \"{0}\" added", args[0]);
        }
    }
}
