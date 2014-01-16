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
    class MenuScreen: BaseScreen, IDisposable
    {

        Main _main;
        public MenuScreen(Main main)
        {
            _main = main;

            Load();
            GameRenderer.SetupViewportMenu(_main);
        }

        public override void Load()
        {
            _menu = new MainMenu(_main);
            oldKs = Keyboard.GetState();
            GameEvents.onStateChanged += GameEvents_onStateChanged;
        }

        void GameEvents_onStateChanged(object sender, StateChangedEventArgs e)
        {
            if (e.State == ScreenState.Menu)
            {
                oldKs = Keyboard.GetState();
                GameRenderer.SetupViewportMenu(_main);
                _menu.Enable();
            }
            else
            {
                _menu.Disable();
            }
        }



        BaseMenu _menu;
        MouseState oldMs;
        KeyboardState oldKs;
        public override void Update()
        {
            KeyboardState ks = Keyboard.GetState();
            MouseState ms = OpenTK.Input.Mouse.GetState();

            _menu.ShowBackground = !_main.IsGameStarted;
            
            //Escape
            if (_main.IsGameStarted && ks.IsKeyDown(Key.Escape) && oldKs.IsKeyUp(Key.Escape))
                GameActions.ChangeScreenState(ScreenState.Game);

            oldKs = ks;
            oldMs = ms;
        }

        public override void Render()
        {
            _menu.Render();
        }



        public void Dispose()
        {
            _menu.Dispose();
        }
    }
}
