using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace FrameworkGL
{
    class Sprite
    {
        #region Attributes

        protected Texture texture;
        protected Model canvas;
        protected Vector2 dimensions;

        protected readonly Vector2[] originalUVs;
        protected List<Vector2> UVs;

        #endregion


        #region Properties

        public Vector2 Position
        {
            get { return canvas.Position.Xy; }
            set { canvas.Position = new Vector3(value.X, value.Y, canvas.Position.Z); }
        }

        public float Width
        {
            get { return this.dimensions.X; }
            set { this.dimensions.X = value; this.UpdateCanvas(); }
        }

        public float Height
        {
            get { return this.dimensions.Y; }
            set { this.dimensions.Y = value; this.UpdateCanvas(); }
        }

        public float DepthLayer
        {
            get { return canvas.Position.Z; }
            set { canvas.Position = new Vector3(Position.X, Position.Y, value); }
        }

        public float Rotation
        {
            get { return canvas.Rotation.Z; }
            set { canvas.Rotation = Quaternion.FromAxisAngle(Vector3.UnitZ, value); }
        }

        public float Scale {
            get { return canvas.Scale; }
            set { canvas.Scale = value; }
        }

        public Texture Texture
        {
            get { return this.texture; }
            set { this.texture = value; }
        }

        public Matrix4 ModelMatrix {
            get { return canvas.ModelMatrix; }
        }

        #endregion


        #region Methods
        
        public Sprite(Rectangle position, int layer = 2)
        {
            originalUVs = new Vector2[4];
            originalUVs[0] = new Vector2(0, 0);
            originalUVs[1] = new Vector2(1, 0);
            originalUVs[2] = new Vector2(0, 1);
            originalUVs[3] = new Vector2(1, 1);

            UVs = new List<Vector2>(4);
            foreach (Vector2 v in originalUVs)
                UVs.Add(v);

            this.dimensions = new Vector2(position.Width, position.Height);

            Mesh mesh = new Mesh();
            mesh.AddVertex(new Vector3(0, 0, layer));
            mesh.AddTexCoord(UVs[0]);
            mesh.AddVertex(new Vector3(dimensions.X, 0, layer));
            mesh.AddTexCoord(UVs[1]);
            mesh.AddVertex(new Vector3(0, -dimensions.Y, layer));
            mesh.AddTexCoord(UVs[2]);
            mesh.AddVertex(new Vector3(dimensions.X, -dimensions.Y, layer));
            mesh.AddTexCoord(UVs[3]);
            mesh.AddIndices(new uint[] { 0, 1, 2, 2, 1, 3 });
            mesh.SetUp();

            canvas = new Model(mesh);

            this.Position = new Vector2(position.X, position.Y);
            this.DepthLayer = layer;
        }

        private void UpdateCanvas()
        {
            if (canvas == null)
                throw new ArgumentNullException("canvas", "The mesh for this sprite is not initialized, somehow. This should never happen.");

            Vector3 position = canvas.Position;

            Mesh mesh = new Mesh();

            mesh = new Mesh();

            mesh.AddVertex(new Vector3(0, 0, position.Z));
            mesh.AddTexCoord(UVs[0]);

            mesh.AddVertex(new Vector3(dimensions.X, 0, position.Z));
            mesh.AddTexCoord(UVs[1]);

            mesh.AddVertex(new Vector3(0, -dimensions.Y, position.Z));
            mesh.AddTexCoord(UVs[2]);

            mesh.AddVertex(new Vector3(dimensions.X, -dimensions.Y, position.Z));
            mesh.AddTexCoord(UVs[3]);

            mesh.AddIndices(new uint[] { 0, 1, 2, 2, 1, 3 });
            mesh.SetUp();

            canvas = new Model(mesh);
            canvas.Position = position;

            this.Update();
        }

        public void ChangeClipper(Rectangle clipper)
        {
            ChangeClipper(clipper.X, clipper.X + clipper.Width, clipper.Y, clipper.Y + clipper.Height);
        }

        public void ChangeClipper(int left, int right, int bottom, int top)
        {
            float newX;
            float newY;

            UVs = new List<Vector2>(4);

            newX = (float)left / (float)texture.Width;
            newY = (float)bottom / (float)texture.Height;
            UVs.Add(new Vector2(newX, newY));

            newX = (float)right / (float)texture.Width;
            UVs.Add(new Vector2(newX, newY));

            newX = (float)left / (float)texture.Width;
            newY = (float)top / (float)texture.Height;
            UVs.Add(new Vector2(newX, newY));

            newX = (float)right / (float)texture.Width;
            UVs.Add(new Vector2(newX, newY));

            UpdateCanvas();
        }

        public void ChangeClipperFull()
        {
            UVs = new List<Vector2>(4);
            foreach (Vector2 v in originalUVs)
                UVs.Add(v);

            UpdateCanvas();
        }

        public virtual void Update() { }

        public virtual void Draw()
        {
            if (texture == null)
                throw new ArgumentNullException("texture", "No texture has been assigned to this sprite.");

            GL.BindTexture(TextureTarget.Texture2D, texture);
            canvas.Draw();
        }

        #endregion
    }
}
