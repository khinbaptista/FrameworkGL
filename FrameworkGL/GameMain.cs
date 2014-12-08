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
        public static Camera HudCamera { get; protected set; }
        public static float DeltaTime { get; protected set; }
        public static readonly bool useMouse = true;

        InputManager input;
        Shader shader2d;
        Sprite sprite;

        Shader shader;
        LightSource light;
        Material material;
        Model wall;

        #endregion

        #region Methods

        public GameMain()
            : base(1600, 900, GraphicsMode.Default, "ROD14465894 - Graphics (CGP2012M) - Assessment 01") {
                VSync = VSyncMode.Adaptive;
        }

        #region Initialization

        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);

            Initialize();
            CreateShader();
            InitializeModel();
        }

        private void Initialize() {
            WindowBorder = WindowBorder.Hidden;
            Viewport = new Rectangle(Location.X, Location.Y, Width, Height);
            DeltaTime = 0.0f;

            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace); GL.FrontFace(FrontFaceDirection.Cw); GL.CullFace(CullFaceMode.Back);
            GL.Enable(EnableCap.Blend); GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.ClearColor(Color.WhiteSmoke);
            GL.PointSize(2.0f);

            ActiveCamera = new Camera(new Vector3(0, 0, 5), new Vector3(0, 0, -1), Vector3.UnitY);
            ActiveCamera.LinearSpeed = 3.0f;
            HudCamera = Camera.New2D();

            input = new InputManager(useMouse);
            CursorVisible = !useMouse;
        }

        private void CreateShader() {
            shader = new Shader();
            shader.AddShaderFile(ShaderType.VertexShader, @"GLSL\vs_mvp_texture_normal.glsl");
            shader.AddShaderFile(ShaderType.FragmentShader, @"GLSL\fs_phong_texture.glsl");
            //shader.AddShaderFile(ShaderType.FragmentShader, @"GLSL\fs_phong.glsl");
            shader.Link();

            shader.TransformationMatrix = ActiveCamera.CameraMatrix;

            shader2d = Shader.Textured;
        }

        private void InitializeModel() {
            Mesh model = new Mesh();
            model.AddVertex(new Vector3(-1.0f, 0.0f, 0.0f));
            model.AddVertex(new Vector3(-1.0f, 2.0f, 0.0f));
            model.AddVertex(new Vector3(1f, 0.0f, 0.0f));
            model.AddVertex(new Vector3(1.0f, 2.0f, 0.0f));
            model.AddNormal(Vector3.UnitZ);
            model.AddNormal(Vector3.UnitZ);
            model.AddNormal(Vector3.UnitZ);
            model.AddNormal(Vector3.UnitZ);
            model.AddTexCoord(new Vector2(0.0f, 1.0f));
            model.AddTexCoord(new Vector2(0.0f, 0.0f));
            model.AddTexCoord(new Vector2(1.0f, 1.0f));
            model.AddTexCoord(new Vector2(1.0f, 0.0f));

            model.AddIndices(new uint[] { 0, 1, 2, 1, 3, 2 });
            model.SetUp();
            wall = new Model(model);

            light = new LightSource(new Vector3(5, 5, 5), new Vector3(1f, 1f, 1f), new Vector3(0.5f, 0.5f, 0.5f));
            material = new Material();
            material.Alpha = 1.0f;
            material.Texture = new Texture(@"img\gradientGB.png");
            //material.Diffuse = new Vector3(0.0f, 0.8f, 0.0f);
            material.Shininness = 3f;
            material.Ambient = new Vector3(0.3f, 0.3f, 0.3f);
            material.Specular = new Vector3(0.5f, 0.5f, 0.5f);
            wall.Material = material;
            
            shader.TextureAlpha = 1.0f;
            shader.Material = wall.Material;
            shader.Light = light;

            sprite = new Sprite(new Rectangle(0, Height, 256, 256));
            sprite.Texture = new Texture(@"img\Courier.png");
            shader2d.Texture = sprite.Texture;
        }

        #endregion

        #region Game Loop

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

            shader.TransformationMatrix = wall.ModelMatrix * ActiveCamera.CameraMatrix;
            shader.CameraPosition = ActiveCamera.Position;

            shader2d.TransformationMatrix = sprite.ModelMatrix * HudCamera.CameraMatrix;
        }

        protected override void OnRenderFrame(FrameEventArgs e) {
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            
            shader.Activate();
            wall.Draw();
            shader.Deactivate();

            shader2d.Activate();
            sprite.Draw();
            shader2d.Deactivate();
            
            SwapBuffers();
        }

        #endregion

        #region Events

        protected override void OnKeyDown(KeyboardKeyEventArgs e) {
            base.OnKeyDown(e);

            if (e.Key == Key.Escape)
                Exit();

            //if (e.Key == Key.P)
            //dragon.TogglePoints();
        }

        protected override void OnResize(EventArgs e) {
            base.OnResize(e);

            ClientRectangle = new Rectangle(0, 0, Width, Height);

            GL.Viewport(0, 0, Width, Height);
            ActiveCamera.ProjectionMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45), (float)Width / (float)Height, 0.1f, 100.0f);
        }

        #endregion

        #endregion

        // Entry point
        [STAThread]
        static void Main(string[] args) {
            using (var game = new GameMain()) {
                game.Run(60.0);
            }
        }
    }
}
