using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace FrameworkGL
{
    class Camera : GameElement
    {
        public enum ProjectionType : byte
        {
            Orthographic = 0,
            Perspective = 1
        }

        #region Attributes

        private Vector3 target;
        private Vector3 up;

        private Matrix4 view;
        private Matrix4 projection;

        #endregion


        #region Properties

        public override Vector3 Position {
            get {
                return base.Position;
            }
            set {
                base.Position = value;
                UpdateViewMatrix();
            }
        }

        public Vector3 Target {
            get { return target; }
            set {
                target = value;
                UpdateViewMatrix();
            }
        }

        public Vector3 Up {
            get { return up; }
            set {
                up = value;
                UpdateViewMatrix();
            }
        }

        public override Quaternion Rotation {
            get {
                return base.Rotation;
            }
            set {
                base.Rotation = value;
                Target = Vector3.Transform(target, value);
            }
        }

        #endregion


        #region Methods

        public Camera(Vector3 eye, Vector3 target, Vector3 up, ProjectionType projectionType = ProjectionType.Perspective) {
            this.position = eye;
            this.target = target;
            this.up = up;

            UpdateViewMatrix();

            if (projectionType == ProjectionType.Perspective)
                this.projection = Matrix4.CreatePerspectiveFieldOfView((float)MathHelper.DegreesToRadians(90.0), (float)(16 / 9), 0.1f, 100.0f);
            else if (projectionType == ProjectionType.Orthographic)
                this.projection = Matrix4.CreateOrthographic(16, 9, 0.1f, 100.0f);
        }

        public static Camera New2D(int layer = 10) {
            Camera camera2d = new Camera(new Vector3(0, 0, layer), -Vector3.UnitZ, Vector3.UnitY);
            camera2d.position = new Vector3(GameMain.Viewport.Width / 2, GameMain.Viewport.Height / 2, layer);
            camera2d.projection = Matrix4.CreateOrthographic(GameMain.Viewport.Width, GameMain.Viewport.Height, 0.1f, 100.0f);

            return camera2d;
        }

        private void UpdateViewMatrix(){
            view = Matrix4.LookAt(position, target, up);
        }

        #endregion
    }
}
