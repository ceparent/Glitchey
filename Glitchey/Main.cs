using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;

using Glitchey.Screens;
using Glitchey.Systems;
using Glitchey.Rendering;

namespace Glitchey
{
    public enum ScreenState { Menu, Game };
    class Main : GameWindow
    {
        public Main()
            : base(GameOptions.GetVariable("w_width"), GameOptions.GetVariable("w_height"))
        {
            Title = "Glitchey";
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            GameEvents.onStateChanged += GameEvents_onStateChanged;
            GameEvents.onExit += GameEvents_onExit;

            WindowBorder = WindowBorder.Fixed;
            WindowState = GameOptions.window_state;

            LoadScreen();

            
        }

        void GameEvents_onExit(object sender, EventArgs e)
        {
            Exit();
        }

        void GameEvents_onStateChanged(object sender, StateChangedEventArgs e)
        {
            ChangeScreen(e.State);
        }

        protected override void OnClosed(EventArgs e)
        {
            GameEvents.onExit -= GameEvents_onExit;
            GameEvents.Exit();
        }


        private void LoadScreen()
        {
            _menuScreen = new MenuScreen(Mouse);
            ChangeScreen(ScreenState.Menu); 
        }



        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            if(Focused)
                _actualScreen.Update();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            _actualScreen.Render();

            SwapBuffers();
            GameVariables.fps = ((float)(1 / e.Time) + GameVariables.fps) / 2;
        }


        BaseScreen _actualScreen;
        GameScreen _gameScreen;
        MenuScreen _menuScreen;
        private void ChangeScreen(ScreenState screen)
        {
            switch (screen)
            {
                case ScreenState.Menu:
                    CursorVisible = true;
                    _actualScreen = _menuScreen;
                    break;
                case ScreenState.Game:
                    CursorVisible = false;
                    if (_gameScreen == null)
                        _gameScreen = new GameScreen();
                    _actualScreen = _gameScreen;
                    break;
                default:
                    throw new InvalidOperationException("This screen doesn't exist");
            }
        }

        private void StartGame()
        {
            _gameScreen = new GameScreen();
        }

    }
}
