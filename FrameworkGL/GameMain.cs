using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Input;

namespace FrameworkGL
{
    class GameMain : GameWindow
    {
        #region Static attributes

        public static Vector2 Viewport { get; protected set; }
        public static Camera ActiveCamera { get; protected set; }
        public static float DeltaTime { get; protected set; }



        #endregion


        #region Methods

        public GameMain()
            : base(1600, 900, GraphicsMode.Default, "ROD14465894 - Graphics (CGP2012M) - Assessment 01") {
                VSync = VSyncMode.Adaptive;
        }

        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);

            Viewport = new Vector2(Width, Height);
            DeltaTime = 0.0f;
            ActiveCamera = new Camera(new Vector3(0, 0, 10), new Vector3(0, 0, -1), Vector3.UnitY);
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e) {
            base.OnKeyDown(e);

            if (e.Key == Key.W)
                ActiveCamera.Rotate(Vector3.UnitY, true);
        }

        protected override void OnUpdateFrame(FrameEventArgs e) {
            base.OnUpdateFrame(e);

            DeltaTime = (float)e.Time;
        }

        protected override void OnRenderFrame(FrameEventArgs e) {
            base.OnRenderFrame(e);
        }

        protected override void OnResize(EventArgs e) {
            base.OnResize(e);
        }

        #endregion

        static void Main(string[] args) {
            using (var game = new GameMain()) {
                game.Run(60.0);
            }
        }
    }
}
