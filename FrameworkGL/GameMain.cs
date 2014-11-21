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

        public static Vector2 Viewport { get; protected set; }
        public static Camera ActiveCamera { get; protected set; }
        public static float DeltaTime { get; protected set; }

        Shader shader;
        Mesh triangle;

        #endregion


        #region Methods

        public GameMain()
            : base(1600, 900, GraphicsMode.Default, "ROD14465894 - Graphics (CGP2012M) - Assessment 01") {
                VSync = VSyncMode.Adaptive;
        }

        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);

            GL.ClearColor(Color.Black);

            Viewport = new Vector2(Width, Height);
            DeltaTime = 0.0f;
            ActiveCamera = new Camera(new Vector3(0, 0, 10), new Vector3(0, 0, -1), Vector3.UnitY);

            shader = new Shader();
            shader.AddShaderFile(ShaderType.VertexShader, @"GLSL\vs_mvp.glsl");
            shader.AddShaderFile(ShaderType.FragmentShader, @"GLSL\fs_color.glsl");
            shader.Link();

            shader.TransformationMatrix = Matrix4.Identity;

            triangle = new Mesh();
            triangle.AddVertex(new Vector3(-0.3f, -0.2f, 0.0f));
            triangle.AddVertex(new Vector3(0.0f, 0.3f, 0.0f));
            triangle.AddVertex(new Vector3(0.3f, -0.2f, 0.0f));
            triangle.AddColor(Color.Brown);
            triangle.AddColor(Color.BurlyWood);
            triangle.AddColor(Color.Chocolate);
            triangle.SetUp();
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e) {
            base.OnKeyDown(e);

            if (e.Key == Key.Escape)
                Exit();
            else if (e.Key == Key.D)
                shader.TransformationMatrix *= Matrix4.CreateTranslation(0.5f * DeltaTime, 0.0f, 0.0f);
            else if (e.Key == Key.A)
                shader.TransformationMatrix *= Matrix4.CreateTranslation(-0.5f * DeltaTime, 0.0f, 0.0f);
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
