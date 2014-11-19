using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace FrameworkGL
{
    class GameElement
    {
        #region Attributes

        protected Vector3 position;
        protected Quaternion rotation;

        protected float linearSpeed;
        protected float angularSpeed;
        protected Vector3 movementDirection;

        #endregion


        #region Properties

        public virtual Vector3 Position {
            get { return position; }
            set { position = value; }
        }

        public virtual Quaternion Rotation {
            get { return rotation; }
            set { rotation = value; }
        }

        public virtual float LinearSpeed {
            get { return linearSpeed; }
            set { linearSpeed = value; }
        }

        public virtual float AngularSpeed {
            get { return angularSpeed; }
            set { angularSpeed = value; }
        }

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

        public virtual void Move(bool smooth = true) {
            float multiplier = smooth ? GameMain.DeltaTime : 1.0f;

            Position += movementDirection * linearSpeed * multiplier;
        }

        public virtual void Rotate(Vector3 axis, bool clockwise, bool smooth = true) {
            float multiplier = smooth ? GameMain.DeltaTime : 1.0f;
            multiplier *= clockwise ? -1.0f : 1.0f;

            Rotation *= Quaternion.FromAxisAngle(axis, MathHelper.DegreesToRadians(angularSpeed * multiplier));
        }

        public virtual void RotateLocal(Vector3 axis, bool clockwise, bool smooth = true) {
            Rotate(Vector3.Transform(axis, rotation), clockwise, smooth);
        }

        #endregion
    }
}
