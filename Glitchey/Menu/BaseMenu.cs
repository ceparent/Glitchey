using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Gwen.Control;
using Gwen.Skin;

namespace Glitchey.Menu
{
    abstract class BaseMenu: IDisposable
    {
        protected Main _main;
        public BaseMenu(Main main)
        {
            GameOptions.ValueChanged += GameOptions_ValueChanged;
            _main = main;
            LoadGwen();
            Enable();
        }

        void GameOptions_ValueChanged(object sender, OptionsChangedArgs e)
        {
            if (e.Variable == "w_width" || e.Variable == "w_height")
            {
                renderer.Dispose();
                skin.Dispose();
                canvas.Dispose();
                DisposeMenuContent();
                LoadGwen();
                Load();

            }
        }

        protected abstract void DisposeMenuContent();
        public abstract void Load();
        public abstract void Render();
        public bool ShowBackground = true;

        public virtual void LoadGwen()
        {
            renderer = new Gwen.Renderer.OpenTK();
            skin = new TexturedBase(renderer, "Content/textures/menu/gwen.png");
            canvas = new Canvas(skin);
            input = new Gwen.Input.OpenTK(_main);
            input.Initialize(canvas);
            int width = GameOptions.GetVariable("w_width");
            int height = GameOptions.GetVariable("w_height");
            canvas.SetBounds(0, 0, width, height);
            canvas.ShouldDrawBackground = false;
        }

        public virtual void Enable()
        {
            canvas.Show();
            _main.Keyboard.KeyDown += Keyboard_KeyDown;
            _main.Keyboard.KeyUp += Keyboard_KeyUp;
            _main.Mouse.Move += Mouse_Move;
            _main.Mouse.ButtonDown += Mouse_ButtonDown;
            _main.Mouse.ButtonUp += Mouse_ButtonUp;
            _main.Mouse.WheelChanged += Mouse_WheelChanged;
        }
        public virtual void Disable()
        {
            canvas.Hide();
            _main.Keyboard.KeyDown -= Keyboard_KeyDown;
            _main.Keyboard.KeyUp -= Keyboard_KeyUp;
            _main.Mouse.Move -= Mouse_Move;
            _main.Mouse.ButtonDown -= Mouse_ButtonDown;
            _main.Mouse.ButtonUp -= Mouse_ButtonUp;
            _main.Mouse.WheelChanged -= Mouse_WheelChanged;
        }

        void Mouse_WheelChanged(object sender, OpenTK.Input.MouseWheelEventArgs e)
        {
            input.ProcessMouseMessage(e);
        }

        void Mouse_ButtonUp(object sender, OpenTK.Input.MouseButtonEventArgs e)
        {
            input.ProcessMouseMessage(e);
        }

        void Mouse_ButtonDown(object sender, OpenTK.Input.MouseButtonEventArgs e)
        {
            input.ProcessMouseMessage(e);
        }
        void Mouse_Move(object sender, OpenTK.Input.MouseMoveEventArgs e)
        {
            input.ProcessMouseMessage(e);
        }
        void Keyboard_KeyUp(object sender, OpenTK.Input.KeyboardKeyEventArgs e)
        {
            input.ProcessKeyUp(e);
        }

        void Keyboard_KeyDown(object sender, OpenTK.Input.KeyboardKeyEventArgs e)
        {
            input.ProcessKeyDown(e);
        }
        

        protected Gwen.Renderer.OpenTK renderer;
        protected Gwen.Skin.TexturedBase skin;
        protected Gwen.Control.Canvas canvas;
        protected Gwen.Input.OpenTK input;



        public virtual void Dispose()
        {
            renderer.Dispose();
            skin.Dispose();
            canvas.Dispose();
        }
    }
}
