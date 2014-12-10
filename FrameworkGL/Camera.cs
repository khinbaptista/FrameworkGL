using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using OpenTK;

namespace FrameworkGL
{
    class Camera : GameElement
    {

        #region Attributes

        private Vector3 target;
        private Vector3 up;

        private Matrix4 view;
        private Matrix4 projection;

        #endregion


        #region Properties

        /// <summary>
        /// Gets or sets the eye position
        /// </summary>
        public override Vector3 Position {
            get {
                return base.Position;
            }
            set {
                target += value - position;
                base.Position = value;
                UpdateViewMatrix();
            }
        }

        /// <summary>
        /// Gets or sets the target of the camera
        /// </summary>
        public Vector3 Target {
            get { return target; }
            set {
                target = value;
                UpdateViewMatrix();
            }
        }

        public Vector3 AbsolutePosition {
            get { return position; }
            set {
                Vector3 moved = value - position;
                base.Position = value;
                Target += moved;
            }
        }

        /// <summary>
        /// Gets or sets the up vector of the camera
        /// </summary>
        public Vector3 Up {
            get { return up; }
            set {
                up = value;
                UpdateViewMatrix();
            }
        }

        /// <summary>
        /// Gets or sets the rotation quaternion of this camera
        /// </summary>
        public override Quaternion Rotation {
            get {
                return base.Rotation;
            }
            set {
                base.Rotation = value.Normalized();
                //up = Vector3.Transform(up, value);
                Direction = Vector3.Transform(Direction, Rotation);
            }
        }

        /// <summary>
        /// Gets or sets the unit vector direction of this camera (points at target)
        /// </summary>
        public Vector3 Direction {
            get { return (target - position).Normalized(); }
            set { Target = position + value.Normalized(); }
        }

        /// <summary>
        /// Gets the view matrix of this camera
        /// </summary>
        public Matrix4 ViewMatrix {
            get { return view; }
        }

        /// <summary>
        /// Gets or sets the projection matrix of this camera
        /// </summary>
        public Matrix4 ProjectionMatrix {
            get { return projection; }
            set { projection = value; }
        }

        /// <summary>
        /// Gets a matrix loaded with both projection and view transformations
        /// </summary>
        public Matrix4 CameraMatrix {
            get { return view * projection; }
        }

        #endregion


        #region Methods

        /// <summary>
        /// Instances a new camera
        /// </summary>
        /// <param name="eye">Position of the camera</param>
        /// <param name="target">Target of the camera</param>
        /// <param name="up">Orientation of the camera</param>
        public Camera(Vector3 eye, Vector3 target, Vector3 up) {
            this.position = eye;
            this.target = target;
            this.up = up;

            UpdateViewMatrix();
            this.projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45.0f), 16f / 9f, 0.1f, 100.0f);
        }

        /// <summary>
        /// Instances a new camera
        /// </summary>
        public Camera(float eyeX, float eyeY, float eyeZ, float targetX, float targetY, float targetZ,
            float upX, float upY, float upZ)
            : this(new Vector3(eyeX, eyeY, eyeZ), new Vector3(targetX, targetY, targetZ), new Vector3(upX, upY, upZ)) { }

        /// <summary>
        /// Creates a camera for 2D screen operations (like GUI/HUD or anything 2D) (respects OpenGL coordinate system)
        /// </summary>
        /// <param name="layer">Layer of the camera (position in the 3rd dimension)</param>
        /// <returns>Camera 2D</returns>
        public static Camera New2D(Rectangle viewport, int layer = 10) {

            Camera camera2d = new Camera(new Vector3(0, 0, layer), -Vector3.UnitZ, Vector3.UnitY);
            camera2d.Position = new Vector3(viewport.Width / 2, viewport.Height / 2, layer);
            camera2d.projection = Matrix4.CreateOrthographic(viewport.Width, viewport.Height, 0.1f, 100.0f);

            camera2d.UpdateViewMatrix();

            return camera2d;
        }

        private void UpdateViewMatrix(){
            view = Matrix4.LookAt(position, target, up);
        }

        public void RotateFromMouse(Vector2 mouseMove) {
            angleXZ += mouseMove;

            Quaternion rotation = Quaternion.FromAxisAngle(Vector3.Transform(Vector3.UnitY, this.rotation), mouseMove.X);
            rotation *= Quaternion.FromAxisAngle(Vector3.Transform(Vector3.UnitX, Matrix4.CreateRotationY(angleXZ.X)), mouseMove.Y);

            Rotation = rotation;
        }

        #endregion
    }
}
