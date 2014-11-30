using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Input;
using OpenTK.Graphics.OpenGL4;

namespace FrameworkGL
{
    class GameMain : GameWindow
    {
        #region Static attributes

        public static Rectangle Viewport { get; protected set; }
        public static Camera ActiveCamera { get; protected set; }
        public static float DeltaTime { get; protected set; }

        InputManager input;
        Shader shader;
        Shader shader2d;
        Mesh model;

        #endregion


        #region Methods

        public GameMain()
            : base(1600, 900, GraphicsMode.Default, "ROD14465894 - Graphics (CGP2012M) - Assessment 01") {
                VSync = VSyncMode.Adaptive;
        }

        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);

            GL.ClearColor(Color.Black);
            GL.Enable(EnableCap.DepthTest);// | EnableCap.CullFace);
            GL.Enable(EnableCap.Blend); GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.PointSize(10.0f);
            

            Viewport = new Rectangle(Location.X, Location.Y, Width, Height);
            DeltaTime = 0.0f;
            ActiveCamera = new Camera(new Vector3(0, 0, 10), new Vector3(0, 0, -1), Vector3.UnitY);
            ActiveCamera.LinearSpeed = 3.0f;

            input = new InputManager(true);
            CursorVisible = false;

            shader = Shader.FixedLight;
            shader.TransformationMatrix = ActiveCamera.CameraMatrix;

            shader2d = Shader.Textured;
            shader2d.TransformationMatrix = Matrix4.Identity;
            /*
            model = new Mesh();
            model.AddVertex(new Vector3(-3f, -2f, 0.0f));
            model.AddVertex(new Vector3(0.0f, 3f, 0.0f));
            model.AddVertex(new Vector3(3f, -2f, 0.0f));
            model.AddColor(Color.Brown);
            model.AddColor(Color.BurlyWood);
            model.AddColor(Color.Chocolate);
            model.SetUp();*/

            model = Mesh.FromFile(@"obj\monkey.obj");

            shader.ModelviewMatrix = ActiveCamera.ViewMatrix;
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e) {
            base.OnKeyDown(e);

            if (e.Key == Key.Escape)
                Exit();

            if (e.Key == Key.P)
                model.DrawAsPoints = !model.DrawAsPoints;
        }

        private void HandleInput() {
            float cameraSpeed = 3.0f;

            if (input.MouseMovement != Vector2.Zero)
                ActiveCamera.RotateFromMouse(input.MouseMovement * DeltaTime * 0.01f);

            if (input.FromKeyboard.IsKeyDown(Key.Q))
                ActiveCamera.AbsolutePosition += Vector3.UnitY * DeltaTime * cameraSpeed;

            if (input.FromKeyboard.IsKeyDown(Key.E))
                ActiveCamera.AbsolutePosition -= Vector3.UnitY * DeltaTime * cameraSpeed;

            if (input.FromKeyboard.IsKeyDown(Key.W))
                ActiveCamera.Move();

            if (input.FromKeyboard.IsKeyDown(Key.S))
                ActiveCamera.Move(true);

            if (input.FromKeyboard.IsKeyDown(Key.A))
                ActiveCamera.MoveSideways(true);

            if (input.FromKeyboard.IsKeyDown(Key.D))
                ActiveCamera.MoveSideways(false);

            shader.TransformationMatrix = ActiveCamera.CameraMatrix;
        }

        protected override void OnUpdateFrame(FrameEventArgs e) {
            base.OnUpdateFrame(e);

            DeltaTime = (float)e.Time;
            
            input.Update();
            HandleInput();
        }

        protected override void OnRenderFrame(FrameEventArgs e) {
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            shader.Activate();
            model.Draw();
            shader.Deactivate();

            SwapBuffers();
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
