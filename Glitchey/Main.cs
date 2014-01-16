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


        public bool IsGameStarted
        {
            get { return _gameScreen != null; }
        }


        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            GameOptions.ValueChanged += GameOptions_ValueChanged;

            GameEvents.onStateChanged += GameEvents_onStateChanged;

            GameActions.ExitAction += GameActions_ExitAction;

            WindowBorder = WindowBorder.Fixed;
            WindowState = (WindowState)GameOptions.GetVariable("w_state");
            GameRenderer.SetResolution(this);

            LoadScreen();


        }

        void GameOptions_ValueChanged(object sender, OptionsChangedArgs e)
        {
            bool doViewport = false;
            switch (e.Variable)
            {
                case "w_width":
                case "w_height":
                    doViewport = true;
                    break;
                case "w_state":
                    WindowState = (WindowState)e.Value;
                    doViewport = true;
                    break;
            }


            if (doViewport)
            {
                GameRenderer.SetResolution(this);
                if (_screenState == ScreenState.Menu)
                    GameRenderer.SetupViewportMenu(this);
                else if (_screenState == ScreenState.Game)
                    GameRenderer.SetupViewportGame();
            }
        }

        void GameActions_ExitAction(object sender, EventArgs e)
        {
            Exit();
        }

        protected override void OnClosed(EventArgs e)
        {
            GameEvents.Exit();
            if (_gameScreen != null)
                Disconnect();
        }

        void GameEvents_onStateChanged(object sender, StateChangedEventArgs e)
        {
            ChangeScreen(e.State);
        }


        private void LoadScreen()
        {
            _menuScreen = new MenuScreen(this);
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

            if (GL.GetError() != ErrorCode.NoError)
                throw new Exception();
            switch (_screenState)
            {
                case ScreenState.Menu:
                    if (_gameScreen != null)
                    {
                        GameRenderer.SetupViewportGame();
                        _gameScreen.Render();
                        GameRenderer.SetupViewportMenu(this);
                    }
                        
                    _menuScreen.Render();
                    break;
                case ScreenState.Game:
                    _gameScreen.Render();
                    break;
            }

            if (GL.GetError() != ErrorCode.NoError)
                throw new Exception();

            SwapBuffers();


            GameVariables.fps = ((float)(1 / e.Time) * 0.1f) + (GameVariables.fps * 0.9f);
        }


        BaseScreen _actualScreen;
        GameScreen _gameScreen;
        MenuScreen _menuScreen;
        ScreenState _screenState;
        private void ChangeScreen(ScreenState screen)
        {
            _screenState = screen;
            switch (screen)
            {
                case ScreenState.Menu:
                    CursorVisible = true;
                    _actualScreen = _menuScreen;
                    break;
                case ScreenState.Game:
                    CursorVisible = false;
                    if (_gameScreen == null)
                        StartGame("glitch2");
                    _actualScreen = _gameScreen;
                    break;
                default:
                    throw new InvalidOperationException("This screen doesn't exist");
            }
        }

        public  void StartGame(string mapName)
        {
            if (_gameScreen != null)
                Disconnect();

            _gameScreen = new GameScreen(mapName);
        }

        public void Disconnect()
        {
            _gameScreen.Dispose();
            _gameScreen = null;
            GameActions.ChangeScreenState(ScreenState.Menu);
        }

        public void Resume()
        {
            GameActions.ChangeScreenState(ScreenState.Game);
        }

        public override void Dispose()
        {
            base.Dispose();
            if (_menuScreen != null)
                _menuScreen.Dispose();
        }


    }
}
