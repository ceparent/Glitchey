using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Gwen;
using Gwen.Control;

namespace Glitchey.Menu.Controls
{
    class ConsoleTextbox : Base
    {

        private ScrollControl _scroll;
        public ConsoleTextbox(Base parent)
            :base(parent)
        {

            _scroll = new ScrollControl(this);
            _scroll.Dock = Pos.Fill;
            _scroll.EnableScroll(true, true);
            _scroll.AutoHideBars = true;
            _scroll.Margin = new Margin(1,1,1,1);
            m_InnerPanel = _scroll;


        }


    }
}
