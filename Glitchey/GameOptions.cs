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
        //viewport
        public static int viewport_width = 1920;
        public static int viewport_height = 1080;

        //window
        /* 1366 x 768 window */
        public static int window_width = 1366;
        public static int window_height = 768;
        public static WindowState window_state = WindowState.Normal;

        //public static int window_width = 1920;
        //public static int window_height = 1080;
        //public static WindowState window_state = WindowState.Fullscreen;


        //rendering
        public static int patch_tesselation = 7;



    }
}
