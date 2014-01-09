using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Glitchey.Rendering;
using Glitchey.Menu.Controls;

using System.Drawing;
using OpenTK;
using QuickFont;

namespace Glitchey.Menu
{
    class MainMenu : BaseMenu
    {
        public MainMenu()
        {
            Load();
        }

        QFont _font;
        List<Control> _controls;
        public override void Load()
        {
            _font = Content.LoadFont(new Font("bitween 10", 32));
            LoadControls();
        }

        private void LoadControls()
        {
            _controls = new List<Control>();

            TextLink l;
            l = new TextLink(_font, "Exit", Color.FromArgb(255, 30, 30, 35), Color.White, new Vector2(950 , 925));
            l.onClick += Exit_click;
            _controls.Add(l);

            l = new TextLink(_font, "Start Game", Color.FromArgb(255, 30, 30, 35), Color.White, new Vector2(925, 615));
            l.onClick += Start_Click;
            _controls.Add(l);

        }

        private void Start_Click(object sender, EventArgs e)
        {
            GameEvents.ChangeScreenState(ScreenState.Game);
        }

        private void Exit_click(object sender, EventArgs e)
        {
            GameEvents.Exit();
        }


        
        public override void Render()
        {
            int texture = Content.LoadTexture("textures/menu/background.png");
            DrawHelper.DrawTexture(texture, new Rectangle(0, 0, GameOptions.viewport_width, GameOptions.viewport_height), 0, OpenTK.Vector2.Zero);

            foreach (Control c in _controls)
            {
                c.Render();
            }
            
        }

        public override void Click(bool click, float x, float y)
        {
            foreach (Control c in _controls)
            {
                c.Update(click, x, y);   
            }
        }
    }
}
