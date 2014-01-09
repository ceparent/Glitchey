using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Glitchey.Rendering;

using QuickFont;
using System.Drawing;
using OpenTK;

namespace Glitchey.Menu.Controls
{
    class TextLink : Control
    {
        public event EventHandler onClick;

        public TextLink(QFont font, string text, Color color, Color hoverColor, Vector2 position)
        {
            _font = font;
            Text = text;
            Color = color;
            Position = position;
            HoverColor = hoverColor;
        }

        private SizeF _size;
        private string _text;
        public string Text
        {
            get { return _text; }
            set 
            {
                _text = value;
                _size = _font.Measure(_text);
            }
        }

        private Vector2 _position;
        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        private Color _color;
        public Color Color
        {
            get { return _color; }
            set { _color = value; }
        }

        private Color _hoverColor;
        public Color HoverColor
        {
            get { return _hoverColor; }
            set { _hoverColor = value; }
        }

        private Color _actualColor;
        public override void Update(bool clicked, float x, float y)
        {
            RectangleF rec = new RectangleF(Position.X - _size.Width / 2, Position.Y - _size.Height / 2, _size.Width, _size.Height);
            if (rec.Contains(x, y))
            {
                if (clicked && onClick != null)
                    onClick(this, EventArgs.Empty);

                _actualColor = HoverColor;
            }
            else
                _actualColor = Color;
                
        }

        QFont _font;
        public override void Render()
        {
            DrawHelper.DrawString(Text, _font, _actualColor, Position, true);
        }
    }
}
