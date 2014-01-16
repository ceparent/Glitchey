using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glitchey
{
    static class GameActions
    {
        public static event EventHandler ExitAction;
        public static void Exit()
        {
            if (ExitAction != null)
                ExitAction(null, EventArgs.Empty);
        }

        public static void ChangeScreenState(ScreenState state)
        {
            GameEvents.ChangeScreenState(state);
        }

    }
}
