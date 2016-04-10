using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fun.Editor
{
    internal abstract class CommandParser
    {
        protected CommandParser[] subCommands;

        public abstract string Keyword { get; }
        public CommandParser[] SubCommands { get { return subCommands; } }

        public bool TryParse(string[] args)
        {
            if (args.Length < 1)
                return false;

            if (args[0] != Keyword)
                return false;

            return true;

            //if (args.Length > 1)
            //    if (subCommands != null)
            //    {
            //        for (int i = 0; i < args.Length; i++)
            //            if (subCommands.Any(c => c.TryParse(args.Skip(i).ToArray())))
            //                return true;
            //    }
            //else
            //    {
            //        return true;
            //    }
            //        //if (!SubCommands.Any(c => c.TryParse(args.Skip(1).ToArray())))
            //        //    return false;

            //return false;
        }

        public void Parse(string[] args)
        {
            args = args.Skip(1).ToArray();

            if (args.Length == 0)
                Do(new string[0]);

            if (subCommands != null)
                foreach (var c in subCommands)
                    if (c.Keyword == args[0])
                        c.Parse(args);

            Do(args);
            //for (int i = 0; i < args.Length; i++)
            //{
            //    foreach (var c in subCommands)
            //    {
            //        if (c.Keyword == args[i])
            //        {
            //            if (args.Skip(i).Count() != 0)
            //                Do(args.Take(i).ToArray());

            //            c.Parse(args.Skip(i).ToArray());

            //            i = args.Length;
            //            break;
            //        }
            //    }
            //}
        }

        protected abstract void Do(string[] args);
    }
}
