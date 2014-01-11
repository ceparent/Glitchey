using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Glitchey.Rendering;

using System.Drawing;
using System.Drawing.Text;
using System.IO;
using QuickFont;

namespace Glitchey
{
    static class Content
    {
        private static PrivateFontCollection _fontCollection;
        static Content()
        {
            _fontCollection = new PrivateFontCollection();
            _fonts = new Dictionary<KeyValuePair<int, string>, QFont>();

            _textures = new Dictionary<string, int>();
            _textures.Add("default", LoadTexture("textures/missing_texture.jpg"));
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

        
        private static Dictionary<KeyValuePair<int, string>, QFont> _fonts;
        public static QFont LoadFont(string fontname, int size)
        {
            KeyValuePair<int, string> pair = new KeyValuePair<int, string>(size, fontname);
            if (_fonts.ContainsKey(pair))
                return _fonts[pair];

            string path = "Content/fonts/" + fontname + ".ttf";
            Font newFont;
            if (File.Exists(path))
            {
                _fontCollection.AddFontFile(path);
                newFont = new Font(_fontCollection.Families[0], size);
            }
            else
            {
                newFont = new Font(fontname, size);
            }
            
            QFont qfont = new QFont(newFont);
            _fonts.Add(pair, qfont);

            return qfont;
        }

        


    }
}
