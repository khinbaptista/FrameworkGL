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

using System.Diagnostics; // Stopwatch for debugging

namespace FrameworkGL
{
    class GameMain : GameWindow
    {
        #region Static attributes

        public static Rectangle Viewport { get; protected set; }
        public static Camera ActiveCamera { get; protected set; }
        public static float DeltaTime { get; protected set; }

        public static Stopwatch stopwatch;

        InputManager input;
        Shader shader;
        Shader shader2d;
        Model dragon;
        Model monkey;

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
            GL.PointSize(5.0f);
            

            Viewport = new Rectangle(Location.X, Location.Y, Width, Height);
            DeltaTime = 0.0f;
            ActiveCamera = new Camera(new Vector3(0, 0, 10), new Vector3(0, 0, -1), Vector3.UnitY);
            ActiveCamera.LinearSpeed = 3.0f;

            input = new InputManager(true);
            CursorVisible = false;

            shader = Shader.FixedLight;
            shader.TransformationMatrix = ActiveCamera.CameraMatrix;

            shader2d = Shader.FixedLight;
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

            stopwatch = new Stopwatch();

            stopwatch.Start();
            dragon = new Model(@"obj\dragonFix.obj", true); //Mesh.FromFileFast(@"obj\dragonFix.obj");
            stopwatch.Stop();
            Console.WriteLine("Total elapsed time since loading started: " + stopwatch.Elapsed.ToString());
            dragon.Scale = 0.7f;
            
            monkey = new Model(@"obj\monkey.obj");
            monkey.Position = new Vector3(10, 0, -5);
            monkey.Scale = 1.5f;
            shader.ModelviewMatrix = ActiveCamera.ViewMatrix;
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e) {
            base.OnKeyDown(e);

            if (e.Key == Key.Escape)
                Exit();

            if (e.Key == Key.P)
                dragon.TogglePoints();
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
        }

        protected override void OnUpdateFrame(FrameEventArgs e) {
            base.OnUpdateFrame(e);

            DeltaTime = (float)e.Time;
            
            input.Update();
            HandleInput();

            //shader.ModelviewMatrix = ActiveCamera.ViewMatrix;
            monkey.Rotation *= Quaternion.FromAxisAngle(Vector3.One, DeltaTime * 0.5f);
            dragon.Rotation *= Quaternion.FromAxisAngle(Vector3.UnitY, DeltaTime * 0.5f);
        }

        protected override void OnRenderFrame(FrameEventArgs e) {
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            shader.TransformationMatrix = dragon.ModelMatrix * ActiveCamera.CameraMatrix;
            shader.Activate();
            dragon.Draw();
            shader.Deactivate();

            shader.TransformationMatrix = monkey.ModelMatrix * ActiveCamera.CameraMatrix;
            shader.Activate();
            monkey.Draw();
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
