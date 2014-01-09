using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Input;

using Glitchey.Menu;
using Glitchey.Systems;
using Glitchey.Core;
using Glitchey.Rendering;

namespace Glitchey.Screens
{
    class MenuScreen: BaseScreen
    {


        public MenuScreen(MouseDevice md)
        {
            Mouse = md;
            Load();
            GameRenderer.SetupViewportMenu();
        }

        public override void Load()
        {
            _menu = new MainMenu();
            oldKs = Keyboard.GetState();
            GameEvents.onStateChanged += GameEvents_onStateChanged;
        }

        void GameEvents_onStateChanged(object sender, StateChangedEventArgs e)
        {
            if (e.State == ScreenState.Menu)
            {
                oldKs = Keyboard.GetState();
                GameRenderer.SetupViewportMenu();
            }
        }



        BaseMenu _menu;
        MouseDevice Mouse;
        MouseState oldMs;
        KeyboardState oldKs;
        public override void Update()
        {
            KeyboardState ks = Keyboard.GetState();
            MouseState ms = OpenTK.Input.Mouse.GetState();

            //Click
            _menu.Click(ms.LeftButton == ButtonState.Pressed && oldMs.LeftButton == ButtonState.Released,Mouse.X, Mouse.Y);
            
            //Escape
            if (ks.IsKeyDown(Key.Escape) && oldKs.IsKeyUp(Key.Escape))
                GameEvents.ChangeScreenState(ScreenState.Game);

            oldKs = ks;
            oldMs = ms;
        }

        public override void Render()
        {
            _menu.Render();
        }


    }
}
