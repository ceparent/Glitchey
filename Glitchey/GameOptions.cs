using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

using OpenTK;

namespace Glitchey
{
    class OptionsChangedArgs : EventArgs
    {
        public string Variable { get; set; }
        public int Value { get; set; }
        public OptionsChangedArgs(string variable, int value)
        {
            Variable = variable;
            Value = value;
        }
    }


    static class GameOptions
    {
        public static event EventHandler<OptionsChangedArgs> ValueChanged;

        private static Dictionary<string, int> _variables;
        static GameOptions()
        {
            _variables = new Dictionary<string, int>();
            LoadDefaultOptions();
            LoadOptionsFile("debug");
            
        }


        public static int GetVariable(string variable)
        {
            return _variables[variable];
        }

        public static void SetVariable(string variable, int value, bool disableEvent = false)
        {
            _variables[variable] = value;
            if (!disableEvent && ValueChanged != null)
                ValueChanged(null, new OptionsChangedArgs(variable, value));
        }

        private static void LoadDefaultOptions()
        {

            //window
            _variables["w_width"] = 1920;
            _variables["w_height"] = 1080;
            _variables["w_state"] = (int)WindowState.Fullscreen;

            //rendering
            _variables["r_tesselation"] = 7;

            //physic
            _variables["phys_debug"] = 0;

            //Game
            _variables["sv_gravity"] = 1200;
        }


        public static void LoadOptionsFile(string name)
        {
            using (StreamReader reader = new StreamReader("Content/cfg/" + name + ".cfg"))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine().Trim();
                    if (line.StartsWith("//"))
                        continue;
                    
                    string[] split = line.Split(' ');
                    if (split.Length < 2 || !_variables.ContainsKey(split[0]))
                        continue;

                    string key = split[0];
                    int value;
                    if (int.TryParse(split[1], out value))
                        _variables[key] = value;
                }

            }
        }
        




    }
}
