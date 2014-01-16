using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Gwen.Control;

namespace Glitchey.Menu.Controls
{
    class CommandTextbox : TextBox
    {
        public CommandTextbox(Base parent)
            :base(parent)
        {
        }

        public bool SubmitFix = false;
        public override void Blur()
        {
            if (SubmitFix)
            {
                this.Focus();
                SubmitFix = false;
            }
            else
                base.Blur(); 
        }

    }
}
