using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Gwen.Control;
using System.Drawing;

namespace Glitchey.Menu.Controls
{
    class ConsoleWindow : Gwen.Control.WindowControl, IDisposable
    {
        public ConsoleWindow(Base parent, Rectangle bounds)
            :base(parent,"Console")
        {
            SetBounds(bounds);
            LoadSubControls();
        }

        private bool allowInput = true;
        public bool AllowInput
        {
            get { return allowInput; }
            set
            {
                allowInput = value;
                txtCommand.KeyboardInputEnabled = value;
                txtConsole.KeyboardInputEnabled = value;

                if (value)
                    txtCommand.Focus();
                else
                    txtCommand.Blur();

            }
        }

        

        protected override void OnResized(Base control, EventArgs args)
        {
            SetTextBoxSize();
        }

        private void SetTextBoxSize()
        {
            int commandHeight = 60;
            int offset = 12;
            txtCommand.SetBounds(0, Height - commandHeight, Width - offset, commandHeight/2  - offset/2);

            txtConsole.SetBounds(0, 0, Width - offset, Height - commandHeight - offset);
        }


        public void WriteLine(string line)
        {
            
        }

        CommandTextbox txtCommand;
        ConsoleTextbox txtConsole;
        private void LoadSubControls()
        {
            txtCommand = new CommandTextbox(this);
            txtCommand.TextColorOverride = Color.Black;
            txtCommand.SubmitPressed += txtCommand_SubmitPressed;
            

            txtConsole = new ConsoleTextbox(this);
            txtConsole.KeyboardInputEnabled = false;

            SetTextBoxSize();
            
        }

        void txtCommand_SubmitPressed(Base sender, EventArgs arguments)
        {
            CommandTextbox tb = (CommandTextbox)sender;
            string line = tb.Text;
            GameConsole.log(line);
            tb.DeleteText(0, tb.Text.Length);
            tb.Focus();
            tb.SubmitFix = true;
        }





    }
}
