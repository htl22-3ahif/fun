using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fun.Client
{
    static class Program
    {
        /// <summary>
        /// Main entry point for application. 
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            using (var game = new FunGame(
                int.Parse(args[0]),
                int.Parse(args[1]),
                args[2]))
                game.Run();
        }
    }
}
