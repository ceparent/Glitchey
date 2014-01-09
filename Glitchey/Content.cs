using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Glitchey.Rendering;

using System.Drawing;
using System.IO;
using QuickFont;

namespace Glitchey
{
    static class Content
    {
        static Content()
        {
            _fonts = new Dictionary<Font, QFont>();

            _textures = new Dictionary<string, int>();
            _textures.Add("default", LoadTexture("textures/surface.png"));
        }

        private static Dictionary<string, int> _textures;

        public static int LoadTexture(string path)
        {
            return LoadTexture(path, null);
        }
        public static int LoadTexture(string path, Color? invisibleColor)
        {
            if (_textures.ContainsKey(path))
                return _textures[path];
            
            if (!File.Exists("Content/" + path))
                return _textures["default"];

            int id = DrawHelper.LoadTexture("Content/" + path, invisibleColor);
            _textures.Add(path, id);

            return id;
        }

        private static Dictionary<Font, QFont> _fonts;
        public static QFont LoadFont(Font pfont)
        {
            if (_fonts.ContainsKey(pfont))
                return _fonts[pfont];

            QFont font = new QFont(pfont);
            _fonts.Add(pfont, font);

            return font;
        }

        


    }
}
