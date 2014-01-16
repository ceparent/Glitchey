using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Glitchey.Menu;

namespace Glitchey
{
    static class GameConsole
    {
        
        
        public static void log(string line)
        {
            MainMenu.ConsoleWindow.WriteLine(line);
        }



    }
}
