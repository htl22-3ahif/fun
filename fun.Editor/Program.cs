using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Collections.Specialized;
using System.Reflection;
using System.IO;
using fun.Editor.Commands;
using fun.IO;
using Environment = fun.Core.Environment;

namespace fun.Editor
{
    class Program
    {
        static void Main(string[] args)
        {
            CommandManager commands = new CommandManager(
                new CreateCommandParser(),
                new GetCommandParser());

            commands.Parse(args);
        }
    }
}
