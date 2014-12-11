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

        private Mesh canvas;
        private Texture texture;
        private Vector2[] texCoordsFull;

        private Vector2 position;
        private float rotation;
        private float scale;
        private Vector2 dimensions;
        private int layer;

        #endregion

        #region Properties

        public Vector2 Position {
            get { return position; }
            set { position = value; }
        }

        public float Scale {
            get { return scale; }
            set { scale = value; }
        }

        public float Rotation {
            get { return rotation; }
            set { rotation = value; }
        }

        public Matrix4 ModelMatrix {
            get {
                return
                    Matrix4.CreateScale(scale) *
                    Matrix4.CreateRotationZ(rotation) *
                    Matrix4.CreateTranslation(new Vector3(position.X, position.Y, 0));
            }
        }

        public Texture Image {
            get { return texture; }
            set { texture = value; }
        }

        #endregion

        #region Methods

        public Sprite(Rectangle area, Texture image, int layer = 5) {
            position = new Vector2(area.X, area.Y);
            rotation = 0.0f;
            scale = 1.0f;
            this.layer = layer;
            dimensions = new Vector2(area.Width, area.Height);

            texCoordsFull = new Vector2[4];
            texCoordsFull[0] = new Vector2(0, 1);
            texCoordsFull[1] = new Vector2(0, 0);
            texCoordsFull[2] = new Vector2(1, 1);
            texCoordsFull[3] = new Vector2(1, 0);

            ClipToFullTexture();

            texture = image;
        }

        public void ClipToFullTexture() {
            if (canvas != null)
                canvas.Delete();

            canvas = new Mesh();
            canvas.AddVertex(new Vector3(0, 0, layer));
            canvas.AddTexCoord(texCoordsFull[0]);

            canvas.AddVertex(new Vector3(0, dimensions.Y, layer));
            canvas.AddTexCoord(texCoordsFull[1]);

            canvas.AddVertex(new Vector3(dimensions.X, 0, layer));
            canvas.AddTexCoord(texCoordsFull[2]);

            canvas.AddVertex(new Vector3(dimensions.X, dimensions.Y, layer));
            canvas.AddTexCoord(texCoordsFull[3]);

            canvas.AddIndices(new uint[] { 2, 1, 0, 3, 1, 2 });
            canvas.SetUp();
        }

        /// <summary>
        /// Changes the texture clipper for this texture (will not change the position or size of the sprite)
        /// </summary>
        /// <param name="clipper">Rectangle with the origin at its top left corner, in pixels of the texture</param>
        public void Clip(Rectangle clipper) {
            this.Clip(clipper.X, clipper.X + clipper.Width, clipper.Y + clipper.Height, clipper.Y);
        }


        public void Clip(int left, int right, int bottom, int top){
            Vector2 newPos = new Vector2();
            Vector2 newSize = new Vector2();

            newPos.X = (float)left / (float)texture.Width;
            newPos.Y = (float)top / (float)texture.Height;
            newSize.X = (float)right / (float)texture.Width;
            newSize.Y = (float)bottom / (float)texture.Height;

            this.UpdateMesh(newPos, newSize);
        }

        private void UpdateMesh(Vector2 position, Vector2 size) {
            if (canvas != null)
                canvas.Delete();

            canvas = new Mesh();
            canvas.AddVertex(new Vector3(0, 0, layer));
            canvas.AddTexCoord(new Vector2(position.X, size.Y));

            canvas.AddVertex(new Vector3(0, dimensions.Y, layer));
            canvas.AddTexCoord(new Vector2(position.X, position.Y));

            canvas.AddVertex(new Vector3(dimensions.X, 0, layer));
            canvas.AddTexCoord(new Vector2(size.X, size.Y));

            canvas.AddVertex(new Vector3(dimensions.X, dimensions.Y, layer));
            canvas.AddTexCoord(new Vector2(size.X, position.Y));

            canvas.AddIndices(new uint[] { 2, 1, 0, 3, 1, 2 });
            canvas.SetUp();
        }

        public void Draw() {
            texture.Bind();
            canvas.Draw();
            texture.Unbind();
        }

        public void Delete() {
            canvas.Delete();
        }

        #endregion
    }
}
