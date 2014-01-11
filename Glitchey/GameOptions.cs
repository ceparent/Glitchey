using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;

namespace Glitchey
{
    static class GameOptions
    {
        private static Dictionary<string, int> _variables;
        static GameOptions()
        {
            _variables = new Dictionary<string, int>();
            LoadVariables();
        }

        private static void LoadVariables()
        {
            //viewport
            _variables["vp_width"] = 1920;
            _variables["vp_height"] = 1080;

            //window
            /* 1366 x 768 window */
            //_variables["w_width"] = 1366;
            //_variables["w_height"] = 768;

            _variables["w_width"] = 1920;
            _variables["w_height"] = 1080;

            //rendering
            _variables["r_tesselation"] = 5;
        }
        public static WindowState window_state = WindowState.Fullscreen;
        //public static WindowState window_state = WindowState.Normal;

        public static int GetVariable(string variable)
        {
            return _variables[variable];
        }

        public static void SetVariable(string variable, int value)
        {
            _variables[variable] = value;
        }

        




    }
}
