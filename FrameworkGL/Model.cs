using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace FrameworkGL
{
    class Model
    {
        public Mesh Mesh;
        public Vector3 Position;
        public Quaternion Rotation;
        public float Scale;
        public Material Material;

        public Matrix4 ModelMatrix {
            get {
                return
                    Matrix4.CreateFromQuaternion(Rotation) *
                    Matrix4.CreateScale(Scale) *
                    Matrix4.CreateTranslation(Position);
            }
        }

        public Model(Mesh mesh) {
            Mesh = mesh;

            InitializeValues();
        }

        public Model(string filepath, bool fast = true) {
            InitializeValues();
            Mesh = Mesh.FromFile(filepath, fast);
        }

        private void InitializeValues() {
            Position = Vector3.Zero;
            Rotation = Quaternion.Identity;
            Scale = 1.0f;
        }

        public void Draw() {
            bool hasTexture = false;

            if (Material != null)
                if (Material.Texture != null)
                    hasTexture = true;

            if (hasTexture) Material.Texture.Bind();
            Mesh.Draw();
            if (hasTexture) Material.Texture.Unbind();

        }

        public void TogglePoints() {
            Mesh.DrawAsPoints = !Mesh.DrawAsPoints;
        }
    }
}
