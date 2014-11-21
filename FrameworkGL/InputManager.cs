using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Input;

namespace FrameworkGL
{
    public enum Actions
    {
        Up, Down, Left, Right,
        
    }

    public enum TriggerValues{

    }

    class InputManager
    {
        private Dictionary<Buttons, Actions> gamepadBinding;
        private Dictionary<Key, Actions> keyboardBinding;

        public InputManager() {
            gamepadBinding = new Dictionary<Buttons, Actions>();
            keyboardBinding = new Dictionary<Key, Actions>();

            // WASD
            keyboardBinding.Add(Key.W, Actions.Up);
            keyboardBinding.Add(Key.S, Actions.Down);
            keyboardBinding.Add(Key.A, Actions.Left);
            keyboardBinding.Add(Key.D, Actions.Right);
            
            // Arrows
            keyboardBinding.Add(Key.Up, Actions.Up);
            keyboardBinding.Add(Key.Down, Actions.Down);
            keyboardBinding.Add(Key.Left, Actions.Left);
            keyboardBinding.Add(Key.Right, Actions.Right);
        }
    }
}
