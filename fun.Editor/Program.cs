using System;
using fun.Editor.Commands;
using System.IO;

namespace fun.Editor
{
    class Program
    {
        static void Main(string[] args)
        {
            CommandManager commands = new CommandManager(
                new CreateCommandParser(),
                new AddCommandParser(),
                new ViewCommandParser(),
                new RemoveCommandParser(),
                new ListCommandParser());

            commands.Parse(args);
        }
    }
}
