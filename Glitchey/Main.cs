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
            : base(GameOptions.window_width, GameOptions.window_height)
        {
            Title = "Glitchey";
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            GameEvents.onExit +=GameConsole_onExit;
            GameEvents.onStateChanged += GameEvents_onStateChanged;

            WindowBorder = WindowBorder.Fixed;
            WindowState = GameOptions.window_state;

            LoadScreen();

        }

        void GameEvents_onStateChanged(object sender, StateChangedEventArgs e)
        {
            ChangeScreen(e.State);
        }

        private void GameConsole_onExit(object sender, EventArgs e)
        {
            this.Exit();
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
