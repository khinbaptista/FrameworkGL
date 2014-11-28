using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Input;

namespace FrameworkGL
{
    public enum Input
    {
        Up, Down, Left, Right,
        A, X, B, Y,
        LeftBumper, RightBumper,
        LeftTrigger, RightTrigger,
        Start, Back
    }

    class InputManager
    {
        #region Attributes

        private KeyboardState last_keyboard;
        private GamePadState last_gamepad;

        private KeyboardState keyboard;
        private GamePadState gamepad;

        private List<Input> inputs;
        private List<Input> last_inputs;

        private int playerIndex;
        private readonly float threshold;

        #endregion

        #region Properties

        public KeyboardState FromKeyboard {
            get { return keyboard; }
        }

        public GamePadState FromGamepad {
            get { return gamepad; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates a new instance of an input handler.
        /// </summary>
        /// <param name="playerIndex">Index of the player to update</param>
        /// <param name="buttonThreshold">Threshold value for triggers and thumbsticks</param>
        public InputManager(int playerIndex = 0, float buttonThreshold = 0.1f) {
            if (playerIndex < 0 || playerIndex >= 4)
                throw new ArgumentException("Player Index must be between 0 and 3");
            if (buttonThreshold < 0 || buttonThreshold > 1)
                throw new ArgumentException("Button Threshold must be between 0 and 1");

            this.playerIndex = playerIndex;
            threshold = buttonThreshold;

            inputs = new List<Input>();
        }

        public virtual void Update() {
            last_keyboard = keyboard;
            last_gamepad = gamepad;

            keyboard = Keyboard.GetState();
            gamepad = GamePad.GetState(playerIndex);

            last_inputs = new List<Input>();
            foreach (Input input in inputs)
                last_inputs.Add(input);

            inputs = new List<Input>();

            if (gamepad.DPad.IsUp || gamepad.ThumbSticks.Left.Y >= threshold || keyboard.IsKeyDown(Key.W) || keyboard.IsKeyDown(Key.Up))
                inputs.Add(Input.Up);
            if (gamepad.DPad.IsDown || gamepad.ThumbSticks.Left.Y <= -threshold || keyboard.IsKeyDown(Key.S) || keyboard.IsKeyDown(Key.Down))
                inputs.Add(Input.Down);
            if (gamepad.DPad.IsLeft || gamepad.ThumbSticks.Left.X <= -threshold || keyboard.IsKeyDown(Key.A) || keyboard.IsKeyDown(Key.Left))
                inputs.Add(Input.Left);
            if (gamepad.DPad.IsRight || gamepad.ThumbSticks.Left.X >= threshold || keyboard.IsKeyDown(Key.D) || keyboard.IsKeyDown(Key.Right))
                inputs.Add(Input.Right);

            if (gamepad.Buttons.A == ButtonState.Pressed)
                inputs.Add(Input.A);
            if (gamepad.Buttons.B == ButtonState.Pressed)
                inputs.Add(Input.B);
            if (gamepad.Buttons.X == ButtonState.Pressed)
                inputs.Add(Input.X);
            if (gamepad.Buttons.Y == ButtonState.Pressed)
                inputs.Add(Input.Y);

            if (gamepad.Buttons.LeftShoulder == ButtonState.Pressed)
                inputs.Add(Input.LeftBumper);
            if (gamepad.Buttons.RightShoulder == ButtonState.Pressed)
                inputs.Add(Input.RightBumper);

            if (gamepad.Triggers.Left >= threshold)
                inputs.Add(Input.LeftTrigger);
            if (gamepad.Triggers.Right >= threshold)
                inputs.Add(Input.RightTrigger);

            if (gamepad.Buttons.Start == ButtonState.Pressed)
                inputs.Add(Input.Start);
            if (gamepad.Buttons.Back == ButtonState.Pressed)
                inputs.Add(Input.Back);
        }

        public bool Pressed(Input input) {
            return inputs.Contains(input) && !last_inputs.Contains(input);
        }

        public bool Contains(Input input) {
            return inputs.Contains(input);
        }

        public Vector2 Movement() {
            Vector2 movement = new Vector2(0, 0);

            if (inputs.Contains(Input.Up) || inputs.Contains(Input.Down))
                if (gamepad.ThumbSticks.Left.Y != 0)
                    movement.Y = gamepad.ThumbSticks.Left.Y;
                else
                    movement.Y = 1;

            if (inputs.Contains(Input.Left) || inputs.Contains(Input.Right))
                if (gamepad.ThumbSticks.Left.X != 0)
                    movement.X = gamepad.ThumbSticks.Left.X;
                else
                    movement.X = 1;

            return movement;
        }

        #endregion
    }
}
