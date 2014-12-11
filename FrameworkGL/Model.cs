using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
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

        public Model(Mesh mesh, Material material) {
            Mesh = mesh;
            Material = material;

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

        public void Delete() {
            Mesh.Delete();
        }

        #region Presets

        public static Model Triangle(bool normal = true, bool texture = true, bool colour = false) {
            Mesh mesh = new Mesh();
            mesh.AddVertex(new Vector3());
            mesh.AddVertex(new Vector3());
            mesh.AddVertex(new Vector3());

            if (normal) {
                mesh.AddNormal(new Vector3());
                mesh.AddNormal(new Vector3());
                mesh.AddNormal(new Vector3());
            }

            if (texture) {
                mesh.AddTexCoord(new Vector2());
                mesh.AddTexCoord(new Vector2());
                mesh.AddTexCoord(new Vector2());
            }

            if (colour) {
                mesh.AddColor(Color.AliceBlue);
                mesh.AddColor(Color.AliceBlue);
                mesh.AddColor(Color.AliceBlue);
            }

            mesh.AddIndices(new uint[] { 0, 1, 2 });
            mesh.SetUp();

            return new Model(mesh);
        }

        public static Model Wall(bool normal = true, bool texture = true, bool colour = false) {
            Mesh mesh = new Mesh();
            mesh.AddVertex(new Vector3(-1.0f, 0.0f, 0.0f));
            mesh.AddVertex(new Vector3(-1.0f, 2.0f, 0.0f));
            mesh.AddVertex(new Vector3(1f, 0.0f, 0.0f));
            mesh.AddVertex(new Vector3(1.0f, 2.0f, 0.0f));

            if (normal) {
                mesh.AddNormal(Vector3.UnitZ);
                mesh.AddNormal(Vector3.UnitZ);
                mesh.AddNormal(Vector3.UnitZ);
                mesh.AddNormal(Vector3.UnitZ);
            }

            if (texture) {
                mesh.AddTexCoord(new Vector2(0.0f, 1.0f));
                mesh.AddTexCoord(new Vector2(0.0f, 0.0f));
                mesh.AddTexCoord(new Vector2(1.0f, 1.0f));
                mesh.AddTexCoord(new Vector2(1.0f, 0.0f));
            }

            if (colour) {
                mesh.AddColor(Color.AliceBlue);
                mesh.AddColor(Color.AliceBlue);
                mesh.AddColor(Color.AliceBlue);
                mesh.AddColor(Color.AliceBlue);
            }

            mesh.AddIndices(new uint[] { 2, 1, 0, 2, 3, 1 });
            mesh.SetUp();

            return new Model(mesh);
        }

        #endregion
    }
}
