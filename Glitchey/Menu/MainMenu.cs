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

using Gwen;
using Gwen.Control;
using Gwen.Skin;
using Gwen.Renderer;

namespace Glitchey.Menu
{
    class MainMenu : BaseMenu
    {
        public MainMenu(Main main)
            :base(main)
        {
            Load();

            GameEvents.onStateChanged += GameEvents_onStateChanged;
        }

        void GameEvents_onStateChanged(object sender, StateChangedEventArgs e)
        {
            if (e.State == ScreenState.Menu)
            {
                UpdateMenuItems();

            }
        }

        private void UpdateMenuItems()
        {
            bool started = _main.IsGameStarted;

            btnStart.IsHidden = started;
            btnDisconnect.IsHidden = !started;
            btnResume.IsHidden = !started;

        }

        Gwen.Font titlefont;
        Gwen.Font font;
        public static ConsoleWindow ConsoleWindow;
        private Button btnStart;
        private Button btnDisconnect;
        private Button btnResume;
        private Button btnOptions;
        private Button btnExit;
        public override void Load()
        {
            int w_width = GameOptions.GetVariable("w_width");
            int w_height = GameOptions.GetVariable("w_height");

            int x_offset = w_width / 8;
            int y_offset = w_height / 8;
            Rectangle rec = new Rectangle(w_width / 2 + x_offset / 2, y_offset / 2, w_width / 2 - x_offset, w_height - y_offset);
            ConsoleWindow = new ConsoleWindow(canvas, rec);

            

            titlefont = new Gwen.Font(renderer, "bitween10", 70);
            font = new Gwen.Font(renderer, "bitween10", 20);
            // Glitchey
            Button button = new Button(canvas);
            button.Text = "Game";
            button.Font = titlefont;
            button.TextColorOverride = Color.White;
            button.ShouldDrawBackground = false;
            button.SetBounds(100, w_height / 3, 400, 120);

            int offset = 3;
            int heightOffset = 50;

            int buttonwidth = 200;
            int startx = 200;
            //Start game
            btnStart = new Button(canvas);
            btnStart.Text = "Start Game";
            btnStart.Font = font;
            btnStart.TextColorOverride = Color.White;
            btnStart.ShouldDrawBackground = false;
            btnStart.SetBounds(startx, w_height / 3 + offset * heightOffset, buttonwidth, 30);
            btnStart.Clicked += startgame_click;

            btnDisconnect = new Button(canvas);
            btnDisconnect.Text = "Disconnect";
            btnDisconnect.Font = font;
            btnDisconnect.TextColorOverride = Color.White;
            btnDisconnect.ShouldDrawBackground = false;
            btnDisconnect.SetBounds(startx, w_height / 3 + offset * heightOffset, buttonwidth, 30);
            btnDisconnect.Clicked += btnDisconnect_Clicked;

            offset++;

            // Resume
            btnResume = new Button(canvas);
            btnResume.Text = "Resume";
            btnResume.Font = font;
            btnResume.TextColorOverride = Color.White;
            btnResume.ShouldDrawBackground = false;
            btnResume.SetBounds(startx, w_height / 3 + offset * heightOffset, buttonwidth, 30);
            btnResume.Clicked += startgame_click;

            offset = 6;
            // options
            btnOptions = new Button(canvas);
            btnOptions.Text = "Options";
            btnOptions.Font = font;
            btnOptions.TextColorOverride = Color.White;
            btnOptions.ShouldDrawBackground = false;
            btnOptions.SetBounds(startx, w_height / 3 + offset * heightOffset, buttonwidth, 30);
            btnOptions.Clicked += btnOptions_Clicked;


            offset = 7;
            // Exit
            btnExit = new Button(canvas);
            btnExit.Text = "Exit";
            btnExit.Font = font;
            btnExit.TextColorOverride = Color.White;
            btnExit.ShouldDrawBackground = false;
            btnExit.SetBounds(startx, w_height / 3 + offset * heightOffset, buttonwidth, 30);
            btnExit.Clicked += exit_click;
            offset++;

            if (optionsWindow != null)
                optionsWindow.Parent = canvas;

            if (newgamewindow != null)
                newgamewindow.Parent = canvas;

            UpdateMenuItems();
        }

        void btnDisconnect_Clicked(Gwen.Control.Base sender, ClickedEventArgs arguments)
        {
            _main.Disconnect();
        }

        void btnOptions_Clicked(Gwen.Control.Base sender, ClickedEventArgs arguments)
        {
            OpenOptionWindow();
        }

        OptionWindow optionsWindow;
        private void OpenOptionWindow()
        {
            if (optionsWindow == null)
                optionsWindow = new OptionWindow(canvas);
            else if (optionsWindow.IsHidden)
                optionsWindow.Show();
            else
                optionsWindow.BringToFront();
        }

        NewGameWindow newgamewindow;
        private void OpenNewGameWindow()
        {
            if (newgamewindow == null)
                newgamewindow = new NewGameWindow(canvas, _main);
            else if (newgamewindow.IsHidden)
                newgamewindow.Show();
            else
                newgamewindow.BringToFront();
        }


        public override void Enable()
        {
            if (ConsoleWindow != null)
                ConsoleWindow.AllowInput = true;
            base.Enable();
        }
        public override void Disable()
        {
            if (ConsoleWindow != null)
                ConsoleWindow.AllowInput = false;
            base.Disable();
        }


        private void startgame_click(Gwen.Control.Base sender, ClickedEventArgs arguments)
        {
            OpenNewGameWindow();
            
        }

        private void exit_click(Gwen.Control.Base sender, ClickedEventArgs arguments)
        {
            GameActions.Exit();
        }


        public void DisposeFonts()
        {
            titlefont.Dispose();
            titlefont = null;
            font.Dispose();
            font = null;
        }

        protected override void DisposeMenuContent()
        {
            DisposeFonts();
            optionsWindow = null;
            newgamewindow = null;
        }
        public override void Dispose()
        {
            DisposeMenuContent();
            base.Dispose();
        }
        
        public override void Render()
        {
            Rectangle viewport =  new Rectangle(0, 0, GameOptions.GetVariable("w_width"), GameOptions.GetVariable("w_height"));
            if (ShowBackground)
            {
                int texture = Content.LoadTexture("textures/menu/background.png");
                DrawHelper.DrawTexture(texture,viewport, 0, OpenTK.Vector2.Zero);
            }
            else
            {
                DrawHelper.DrawRectangle(viewport, Color.FromArgb(170,0,0,0), true,true);
            }
            

            canvas.RenderCanvas();
            
        }



    }
}
