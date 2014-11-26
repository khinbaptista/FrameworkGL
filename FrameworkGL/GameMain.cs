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
        #region Attributes

        public static Rectangle Window { get; protected set; }
        public static Camera ActiveCamera { get; protected set; }
        public static float DeltaTime { get; protected set; }

        Camera camera2d;
        Sprite sprite;
        Font2D font;
        Shader shader;
        Mesh triangle;
        Mesh floor;
        InputManager input;

        #endregion


        #region Methods

        public GameMain()
            : base(1600, 900, GraphicsMode.Default, "ROD14465894 - Graphics (CGP2012M) - Assessment 01") {
                VSync = VSyncMode.Adaptive;
                CursorVisible = false;
        }

        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);

            GL.ClearColor(Color.CornflowerBlue);

            Window = new Rectangle(Location.X, Location.Y, Width, Height);
            DeltaTime = 0.0f;
            ActiveCamera = new Camera(new Vector3(0, 0.5f, 1f), -Vector3.UnitZ, Vector3.UnitY);
            ActiveCamera.ProjectionMatrix = Matrix4.Identity;

            input = new InputManager();

            shader = new Shader();
            shader.AddShaderFile(ShaderType.VertexShader, @"GLSL\vs_mvp.glsl");
            shader.AddShaderFile(ShaderType.FragmentShader, @"GLSL\fs_color.glsl");
            shader.Link();

            //shader.TransformationMatrix = ActiveCamera.CameraMatrix;
            //shader.TransformationMatrix = Matrix4.Identity;
            shader.CameraMatrix = ActiveCamera.CameraMatrix;
            shader.ModelMatrix = Matrix4.Identity;
            
            GL.PointSize(5f);
            
            triangle = new Mesh();
            triangle.AddVertex(new Vector3(-0.3f, -0.2f, 0.0f));
            triangle.AddVertex(new Vector3(0.0f, 0.3f, 0.0f));
            triangle.AddVertex(new Vector3(0.3f, -0.2f, 0.0f));
            triangle.AddColor(Color.Brown);
            triangle.AddColor(Color.BurlyWood);
            triangle.AddColor(Color.Chocolate);
            triangle.SetUp();

            floor = Mesh.CreateDotFloorXZ(new Vector3(0, 0, 0), new Vector2(20, 20), 0.5f, Color.Beige);
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e) {
            base.OnKeyDown(e);

            if (e.Key == Key.Escape)
                Exit();
            else if (e.Key == Key.Q)
                CursorVisible = !CursorVisible;
            else if (e.Key == Key.P)
                triangle.DrawAsPoints = !triangle.DrawAsPoints;
        }

        protected override void OnUpdateFrame(FrameEventArgs e) {
            base.OnUpdateFrame(e);

            DeltaTime = (float)e.Time;
            
        }

        protected override void OnRenderFrame(FrameEventArgs e) {
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            shader.Activate();
            triangle.Draw();
            floor.Draw();
            shader.Deactivate();
            
            SwapBuffers();
        }

        protected override void OnResize(EventArgs e) {
            base.OnResize(e);
        }

        #endregion


        [STAThread]
        static void Main(string[] args) {
            using (var game = new GameMain()) {
                game.Run(60.0);
            }
        }
    }
}
