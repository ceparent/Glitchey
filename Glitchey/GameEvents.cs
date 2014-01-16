using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glitchey
{
    static class GameEvents
    {
        public static event EventHandler onExit; 
        public static void Exit()
        {
            if (onExit != null)
                onExit(null, EventArgs.Empty);
        }

        
        public static event EventHandler<StateChangedEventArgs> onStateChanged;
        public static void ChangeScreenState(ScreenState state)
        {
            if (onStateChanged != null)
                onStateChanged(null, new StateChangedEventArgs(state));
        }

    }

    public class StateChangedEventArgs : EventArgs
    {
        public ScreenState State { get; set; }
        public StateChangedEventArgs(ScreenState state)
        {
            State = state;
        }
    }
}
