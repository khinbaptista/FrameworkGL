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
    class Mesh
    {
        #region Attributes

        private int id;
        protected MeshData data;

        private bool hasNormals;
        private bool hasTexCoords;
        private bool hasColors;
        private bool isIndexed;

        private int positionBuffer;
        private int normalBuffer;
        private int texCoordBuffer;
        private int colorBuffer;
        private int indexBuffer;

        protected List<Vector3> vertices;
        protected List<Vector3> normals;
        protected List<Vector4> colors;
        protected List<Vector2> texCoords;
        protected List<uint> indices;

        #endregion

        #region Properties

        public static implicit operator int(Mesh mesh) {
            return mesh.id;
        }

        public List<Vector3> Vertices { get { return vertices; } }

        public List<Vector3> Normals { get { return normals; } }

        public List<Vector4> Colors { get { return colors; } }

        public List<Vector2> TexCoords { get { return texCoords; } }

        public List<uint> Indices { get { return indices; } }

        #endregion

        #region Methods

        public Mesh() {
            id = -1;

            vertices = new List<Vector3>();
            normals = new List<Vector3>();
            colors = new List<Vector4>();
            texCoords = new List<Vector2>();
            indices = new List<uint>();
        }

        public Mesh(MeshData data) {
            this.data = data;
            this.LoadData();
        }

        protected void LoadData() {
            foreach (VertexUnit unit in data.Vertices) {
                vertices.Add(unit.position);

                if (unit.normal != Vector3.Zero)
                    normals.Add(unit.normal);

                if (unit.texCoord != new Vector2(-1, -1))
                    texCoords.Add(unit.texCoord);
            }

            foreach (uint index in data.Indices)
                indices.Add(index);
        }

        #region Add data manually

        public void AddVertex(Vector3 vertex) {
            vertices.Add(vertex);
        }

        public void AddNormal(Vector3 normal) {
            normals.Add(normal);
        }

        public void AddColor(Vector4 color) {
            colors.Add(color);
        }

        public void AddColor(Color color) {
            this.AddColor(new Vector4((float)color.R / 255.0f, (float)color.G / 255.0f, (float)color.B / 255.0f, (float)color.A / 255.0f));
        }

        public void AddTexCoord(Vector2 texCoord) {
            texCoords.Add(texCoord);
        }

        public void AddIndex(uint index) {
            indices.Add(index);
        }

        #endregion

        public void SetUp() {
            if (colors.Count > 0) hasColors = true;
            if (normals.Count > 0) hasNormals = true;
            if (texCoords.Count > 0) hasTexCoords = true;
            if (indices.Count > 0) isIndexed = true;

            positionBuffer = GL.GenBuffer();
            LoadArrayBuffer(positionBuffer, vertices.ToArray());

            if (hasColors) {
                colorBuffer = GL.GenBuffer();
                LoadArrayBuffer(colorBuffer, colors.ToArray());
            }

            if (hasNormals) {
                normalBuffer = GL.GenBuffer();
                LoadArrayBuffer(normalBuffer, normals.ToArray());
            }

            if (hasTexCoords) {
                texCoordBuffer = GL.GenBuffer();
                LoadArrayBuffer(texCoordBuffer, texCoords.ToArray());
            }

            if (isIndexed) {
                indexBuffer = GL.GenBuffer();
                LoadIndices(indexBuffer, indices.ToArray());
            }

            id = GL.GenVertexArray();
        }

        #region Load buffers

        private void LoadArrayBuffer(int buffer, Vector2[] data) {
            GL.BindBuffer(BufferTarget.ArrayBuffer, buffer);
            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(data.Length * Vector2.SizeInBytes), data, BufferUsageHint.StaticDraw);

            // Clean up
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        private void LoadArrayBuffer(int buffer, Vector3[] data) {
            GL.BindBuffer(BufferTarget.ArrayBuffer, buffer);
            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(data.Length * Vector3.SizeInBytes), data, BufferUsageHint.StaticDraw);

            // Clean up
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        private void LoadArrayBuffer(int buffer, Vector4[] data) {
            GL.BindBuffer(BufferTarget.ArrayBuffer, buffer);
            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(data.Length * Vector4.SizeInBytes), data, BufferUsageHint.StaticDraw);

            // Clean up
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        private void LoadIndices(int buffer, uint[] data) {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, buffer);
            GL.BufferData<uint>(BufferTarget.ElementArrayBuffer, new IntPtr(data.Length * sizeof(uint)), data, BufferUsageHint.StaticDraw);

            // Clean up
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }

        #endregion

        #region Draw

        public void Draw() {
            GL.BindVertexArray(id);
            this.BindBuffers();

            GL.EnableVertexAttribArray((int)Shader.ArrayIndex.VertexPosition);
            if (hasColors) GL.EnableVertexAttribArray((int)Shader.ArrayIndex.VertexColor);
            if (hasNormals) GL.EnableVertexAttribArray((int)Shader.ArrayIndex.VertexNormal);
            if (hasTexCoords) GL.EnableVertexAttribArray((int)Shader.ArrayIndex.VertexTexCoord);

            if (isIndexed)
                GL.DrawElements(BeginMode.Triangles, indices.Count, DrawElementsType.UnsignedInt, 0);
            else
                GL.DrawArrays(PrimitiveType.Triangles, 0, vertices.Count);

            GL.DisableVertexAttribArray(0);
            GL.DisableVertexAttribArray(1);
            GL.DisableVertexAttribArray(2);
            GL.DisableVertexAttribArray(3);
        }

        public void BindBuffers() {
            GL.BindBuffer(BufferTarget.ArrayBuffer, positionBuffer);
            GL.VertexAttribPointer((int)Shader.ArrayIndex.VertexPosition, 3, VertexAttribPointerType.Float, false, 0, 0);

            if (hasColors) {
                GL.BindBuffer(BufferTarget.ArrayBuffer, colorBuffer);
                GL.VertexAttribPointer((int)Shader.ArrayIndex.VertexColor, 4, VertexAttribPointerType.Float, false, 0, 0);
            }

            if (hasNormals) {
                GL.BindBuffer(BufferTarget.ArrayBuffer, normalBuffer);
                GL.VertexAttribPointer((int)Shader.ArrayIndex.VertexNormal, 3, VertexAttribPointerType.Float, false, 0, 0);
            }

            if (hasTexCoords) {
                GL.BindBuffer(BufferTarget.ArrayBuffer, texCoordBuffer);
                GL.VertexAttribPointer((int)Shader.ArrayIndex.VertexTexCoord, 2, VertexAttribPointerType.Float, false, 0, 0);
            }

            if (isIndexed)
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer);
        }

        #endregion

        #endregion
    }
}
