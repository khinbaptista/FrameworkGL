using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using OpenTK;

namespace FrameworkGL
{
    class Camera
    {
        #region Attributes

        private Matrix4 _projection;
        private Matrix4 _view;

        private Vector3 _position;
        private Vector3 _target;
        private Vector3 _up;

        private float _angularSpeed;
        private float _movementSpeed;
        //private Vector3 _movementDirection;

        #endregion


        #region Properties

        /// <summary>
        /// Gets or sets the projection matrix for this camera
        /// </summary>
        public Matrix4 ProjectionMatrix {
            get { return _projection; }
            set { _projection = value; }
        }

        /// <summary>
        /// Gets the view matrix for this camera
        /// </summary>
        public Matrix4 ViewMatrix {
            get { return _view; }
        }

        public Matrix4 CameraMatrix {
            get { return _projection * _view; }
        }

        /// <summary>
        /// Gets or sets the position of this camera
        /// </summary>
        public Vector3 Position {
            get { return _position; }

            set {
                _target += value - _position;
                _position = value;
                _view = Matrix4.LookAt(_position, _target, _up);
            }
        }

        /// <summary>
        /// Gets or sets the normalized vector representing the direction of the camera, regardless of its position or target
        /// </summary>
        public Vector3 Direction {
            get { return (_target - _position).Normalized(); }

            set {
                _target = _position + value;
                _view = Matrix4.LookAt(_position, _target, _up);
            }
        }

        /// <summary>
        /// Sets the target of this camera
        /// </summary>
        public Vector3 Target {
            get { return _target; }

            set {
                _target = value;
                _view = Matrix4.LookAt(_position, _target, _up);
            }
        }

        /// <summary>
        /// Gets or sets the distance between the camera and the target (the direction vector is a unit-lenght vector)
        /// You can either set a Target, or set a Direction and the DistanceToTarget
        /// </summary>
        public float DistanceToTarget {
            get { return (_target - _position).Length; }

            set {
                Target = _position + Direction * value;
            }
        }

        /// <summary>
        /// Gets or sets the up direction of this camera
        /// </summary>
        public Vector3 Up {
            get { return _up; }

            set {
                _up = value;
                _view = Matrix4.LookAt(_position, _target, _up);
            }
        }

        /// <summary>
        /// Gets or sets the angular speed of this camera, in degrees
        /// </summary>
        public float AngularSpeed {
            get { return MathHelper.RadiansToDegrees(_angularSpeed); }
            set { _angularSpeed = MathHelper.DegreesToRadians(value); }
        }

        /// <summary>
        /// Gets or sets the movement speed of this camera
        /// </summary>
        public float MovementSpeed {
            get { return _movementSpeed; }
            set { _movementSpeed = value; }
        }

        #endregion


        #region Methods

        public Camera(Vector3 position, Vector3 target, Vector3 up) {
            _position = position;
            _target = target;
            _up = up;

            //_movementDirection = target;
            _movementSpeed = 0;
            _angularSpeed = 0;

            _projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45), 16 / 9, 0.1f, 100);
            _view = Matrix4.LookAt(_position, _target, _up);
        }

        public Camera(float posX, float posY, float posZ, float dirX, float dirY, float dirZ, float upX, float upY, float upZ)
            : this(new Vector3(posX, posY, posX), new Vector3(dirX, dirY, dirZ), new Vector3(upX, upY, upZ)) { }

        /// <summary>
        /// Returns a 2D camera to work in window coordinates, where the bottom-left corner is (0, 0)
        /// </summary>
        /// <param name="viewport">The native window to work with</param>
        /// <param name="layer">Layer depth to place this camera</param>
        /// <returns></returns>
        public static Camera New2D(Rectangle viewport, int layer = 5) {
            Camera camera2d = new Camera(new Vector3(0, 0, layer), -Vector3.UnitZ, Vector3.UnitY);
            camera2d.Position = new Vector3(viewport.Width / 2, viewport.Height / 2, layer);
            camera2d.ProjectionMatrix = Matrix4.CreateOrthographic(viewport.Width, viewport.Height, 0.1f, 100f);

            return camera2d;
        }

        public Matrix4 ModelViewProjection(Matrix4 model) {
            return _projection * _view * model;
        }

        /// <summary>
        /// Rotates the camera around itself (Y axis) with the camera angular speed
        /// </summary>
        /// <param name="deltaTime">Time scaling paramenter</param>
        /// <param name="clockwise">Direction of the rotation</param>
        public void Rotate(float deltaTime = 1, bool clockwise = false) {
            int direction = clockwise ? -1 : 1;
            Direction = Vector3.TransformVector(Direction, Matrix4.CreateRotationY(_angularSpeed * deltaTime * direction));
        }

        /// <summary>
        /// Rotates the camera around itself (Y axis) with the specified angular speed
        /// </summary>
        /// <param name="angularSpeed">Angular speed to this rotation, in radians</param>
        /// <param name="deltaTime">Time scaling parameter</param>
        /// <param name="clockwise">Direction of the rotation</param>
        public void Rotate(float angularSpeed, float deltaTime = 1, bool clockwise = false) {
            int direction = clockwise ? -1 : 1;
            Direction = Vector3.TransformVector(Direction, Matrix4.CreateRotationY(angularSpeed * deltaTime * direction));
        }

        /// <summary>
        /// Rotates the camera around the target (Y axis) using the camera angular speed
        /// </summary>
        /// <param name="deltaTime">Time scaling parameter</param>
        /// <param name="clockwise">Direction of the rotation</param>
        public void RotateAroundTarget(float deltaTime = 1, bool clockwise = false) {
            int direction = clockwise ? -1 : 1;
            Vector3 buffer = _position - _target;

            _position = _target + Vector3.TransformVector(buffer, Matrix4.CreateRotationY(_angularSpeed * deltaTime * direction));
            Direction = _target - _position;
        }

        /// <summary>
        /// Rotates the camera around the target (Y axis) using the specified angular speed
        /// </summary>
        /// <param name="angularSpeed">Angular speed to this rotation</param>
        /// <param name="deltaTime">Time scaling parameter</param>
        /// <param name="clockwise">Direction of the rotation</param>
        public void RotateAroundTarget(float angularSpeed, float deltaTime = 1, bool clockwise = false) {
            int direction = clockwise ? -1 : 1;
            Vector3 buffer = _position - _target;

            float angle = MathHelper.DegreesToRadians(angularSpeed * deltaTime * direction);

            _position = _target + Vector3.TransformVector(buffer, Matrix4.CreateRotationY(angle));
            Direction = _target - _position;
        }

        #endregion
    }
}
