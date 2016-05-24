using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fun.Editor.Commands
{
    class AddLibCommandParser : CommandParser
    {
        public override string Keyword
        {
            get
            {
                return "lib";
            }
        }

        protected override void Do(string[] args)
        {
            throw new NotImplementedException();
        }
    }
}
