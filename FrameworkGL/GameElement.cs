using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace FrameworkGL
{
    abstract class GameElement
    {
        #region Attributes

        protected Vector3 position;
        protected Quaternion rotation;

        protected float linearSpeed;
        protected float angularSpeed;
        protected Vector3 movementDirection;

        #endregion


        #region Properties

        /// <summary>
        /// Gets or sets the position of this element
        /// </summary>
        public virtual Vector3 Position {
            get { return position; }
            set { position = value; }
        }

        /// <summary>
        /// Gets or sets the rotation quaternion of this element
        /// </summary>
        public virtual Quaternion Rotation {
            get { return rotation; }
            set { rotation = value; }
        }

        /// <summary>
        /// Gets or sets the speed with which this element will move linearly
        /// </summary>
        public virtual float LinearSpeed {
            get { return linearSpeed; }
            set { linearSpeed = value; }
        }

        /// <summary>
        /// Gets or sets the speed (in degrees) with which this element will move angularly
        /// </summary>
        public virtual float AngularSpeed {
            get { return angularSpeed; }
            set { angularSpeed = value; }
        }

        /// <summary>
        /// Gets or sets the direction of linear movement of this element
        /// </summary>
        public virtual Vector3 MovementDirection {
            get { return movementDirection; }
            set { movementDirection = value.Normalized(); }
        }

        #endregion


        #region Methods

        public GameElement() {
            position = Vector3.Zero;
            rotation = Quaternion.Identity;
            linearSpeed = 0.0f;
            angularSpeed = 0.0f;
            movementDirection = new Vector3(0.0f, 0.0f, -1.0f);
        }

        /// <summary>
        /// Moves this element using MovementDirection and LinearSpeed
        /// </summary>
        /// <param name="smooth">Whether or not to take delta time into account</param>
        public virtual void Move(bool smooth = true) {
            float multiplier = smooth ? GameMain.DeltaTime : 1.0f;

            Position += movementDirection * linearSpeed * multiplier;
        }

        /// <summary>
        /// Rotates this element using AngularSpeed
        /// </summary>
        /// <param name="axis">Axis around which to rotate</param>
        /// <param name="clockwise">True to rotate clockwise; false to rotate counter-clockwise</param>
        /// <param name="smooth">Whether or not to take delta time into account</param>
        public virtual void Rotate(Vector3 axis, bool clockwise, bool smooth = true) {
            float multiplier = smooth ? GameMain.DeltaTime : 1.0f;
            multiplier *= clockwise ? -1.0f : 1.0f;

            Rotation *= Quaternion.FromAxisAngle(axis, MathHelper.DegreesToRadians(angularSpeed * multiplier));
        }

        /// <summary>
        /// Rotates this element around its local axis using AngularSpeed
        /// </summary>
        /// <param name="axis">Local axis around which to rotate</param>
        /// <param name="clockwise">True to rotate clockwise; false to rotate counter-clockwise</param>
        /// <param name="smooth">Whether or not to take delta time into account</param>
        public virtual void RotateLocal(Vector3 axis, bool clockwise, bool smooth = true) {
            Rotate(Vector3.Transform(axis, rotation), clockwise, smooth);
        }

        #endregion
    }
}
