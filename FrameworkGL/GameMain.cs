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
        public static readonly bool useMouse = false;

        float fps;

        InputManager input;
        HUD hud;
        Shader shader;
        Shader shaderTextured;
        Model ground;

        bool goPositive;
        Model movingMonkey;

        Model rotatingMonkey;
        Model wall;
        Model dragon;
        Model pyramid;

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
            WindowBorder = WindowBorder.Resizable;
            DeltaTime = 0.0f;
            Viewport = ClientRectangle;
            fps = 0.0f;

            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace); GL.FrontFace(FrontFaceDirection.Ccw); GL.CullFace(CullFaceMode.Back);
            GL.Enable(EnableCap.Blend); GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.ClearColor(Color.WhiteSmoke);
            GL.PointSize(2.0f);
            
            ActiveCamera = new Camera(new Vector3(0, 0, 5), new Vector3(0, 0, -1), Vector3.UnitY);
            ActiveCamera.LinearSpeed = 3.0f;
            
            hud = new HUD(ClientRectangle);
            
            input = new InputManager(useMouse);
            CursorVisible = !useMouse;
        }

        private void CreateShader() {
            shader = Shader.Phong;

            shader.TransformationMatrix = ActiveCamera.CameraMatrix;

            shaderTextured = Shader.PhongTextured;
        }

        private void InitializeModel() {
            movingMonkey = new Model(@"obj\monkey.obj");
            dragon = new Model(@"obj\dragonFix.obj");

            Material material = new Material();
            material.Alpha = 1.0f;
            //material.Texture = new Texture(@"img\gradientGB.png");
            material.Diffuse = new Vector3(0.75f, 0.1f, 0.1f);
            material.Shininness = 50f;
            material.Ambient = new Vector3(0.01f, 0.01f, 0.01f);
            material.Specular = new Vector3(0.1f, 0.1f, 0.1f);
            movingMonkey.Material = material;

            dragon.Material = material;
            dragon.Position = new Vector3(15, -1, -10);

            pyramid = new Model(@"obj\pyramid.obj");
            pyramid.Material = material;
            pyramid.Position = new Vector3(-3, 0, 0);

            rotatingMonkey = new Model(movingMonkey.Mesh);
            rotatingMonkey.Material = material;
            rotatingMonkey.Position = new Vector3(5, 0, 0);

            //shader.TextureAlpha = 1.0f;
            shader.Material = movingMonkey.Material;
            shader.Light = new LightSource(new Vector3(20, 20, 20), new Vector3(0.7f, 0.7f, 0.7f), new Vector3(0.5f, 0.5f, 0.5f));

            wall = Model.Wall();
            wall.Position = new Vector3(0, 0, -5);
            wall.Material = material;
            wall.Material.Texture = new Texture(@"img\gradientGB.png");

            shaderTextured.TransformationMatrix = ActiveCamera.CameraMatrix;
            shaderTextured.Material = wall.Material;
            shaderTextured.Light = new LightSource(new Vector3(20, 20, 20), new Vector3(0.7f, 0.7f, 0.7f), new Vector3(0.5f, 0.5f, 0.5f));

            ground = Model.Wall();
            ground.Rotation = Quaternion.FromAxisAngle(Vector3.UnitX, MathHelper.DegreesToRadians(270f));
            ground.Scale = 100;
            ground.Position = new Vector3(0, -1, 40);
            ground.Material = wall.Material;
        }

        #endregion

        #region Game Loop

        private void HandleInput() {
            const float cameraSpeed = 3.0f;
            const float meshSpeed = 5.0f;

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

            if (input.FromKeyboard.IsKeyDown(Key.Up))
                dragon.Position -= Vector3.UnitZ * DeltaTime * meshSpeed;

            if (input.FromKeyboard.IsKeyDown(Key.Down))
                dragon.Position += Vector3.UnitZ * DeltaTime * meshSpeed;

            if (input.FromKeyboard.IsKeyDown(Key.Right))
                dragon.Position += Vector3.UnitX * DeltaTime * meshSpeed;

            if (input.FromKeyboard.IsKeyDown(Key.Left))
                dragon.Position -= Vector3.UnitX * DeltaTime * meshSpeed;
        }

        protected override void OnUpdateFrame(FrameEventArgs e) {
            base.OnUpdateFrame(e);
            
            DeltaTime = (float)e.Time;
            
            input.Update();
            HandleInput();

            if (goPositive) {
                movingMonkey.Position += new Vector3(0, 1 * DeltaTime, 0);
                if (movingMonkey.Position.Y > 5)
                    goPositive = false;
            }
            else {
                movingMonkey.Position -= new Vector3(0, 1 * DeltaTime, 0);
                if (movingMonkey.Position.Y < 0)
                    goPositive = true;
            }
            movingMonkey.Rotation *= Quaternion.FromAxisAngle(new Vector3(0, 1, 0), DeltaTime * -0.75f);

            rotatingMonkey.Rotation *= Quaternion.FromAxisAngle(Vector3.UnitY, DeltaTime * 0.5f);
        }

        protected override void OnRenderFrame(FrameEventArgs e) {
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            shader.TransformationMatrix = movingMonkey.ModelMatrix * ActiveCamera.CameraMatrix;
            shader.Activate();
            movingMonkey.Draw();
            shader.Deactivate();

            shader.TransformationMatrix = dragon.ModelMatrix * ActiveCamera.CameraMatrix;
            shader.Activate();
            dragon.Draw();
            shader.Deactivate();

            shader.TransformationMatrix = rotatingMonkey.ModelMatrix * ActiveCamera.CameraMatrix;
            shader.Activate();
            rotatingMonkey.Draw();
            shader.Deactivate();

            GL.FrontFace(FrontFaceDirection.Cw);
            shader.TransformationMatrix = pyramid.ModelMatrix * ActiveCamera.CameraMatrix;
            shader.Activate();
            pyramid.Draw();
            shader.Deactivate();
            GL.FrontFace(FrontFaceDirection.Ccw);

            shaderTextured.TransformationMatrix = wall.ModelMatrix * ActiveCamera.CameraMatrix;
            shaderTextured.Activate();
            wall.Draw();
            shaderTextured.Deactivate();

            shaderTextured.TransformationMatrix = ground.ModelMatrix * ActiveCamera.CameraMatrix;
            shaderTextured.Activate();
            ground.Draw();
            shaderTextured.Deactivate();

            hud.Write("FPS: " + fps);
            hud.Draw();
            
            SwapBuffers();
        }

        #endregion

        #region Events

        protected override void OnKeyDown(KeyboardKeyEventArgs e) {
            base.OnKeyDown(e);

            if (e.Key == Key.Escape)
                Exit();

            if (e.Key == Key.F)
                fps = (float)RenderFrequency;

            if (e.Key == Key.R){
                ActiveCamera = new Camera(new Vector3(0, 0, 5), new Vector3(0, 0, -1), Vector3.UnitY);
                ActiveCamera.LinearSpeed = 3.0f;
            }
        }

        protected override void OnResize(EventArgs e) {
            base.OnResize(e);

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
