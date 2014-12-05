using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
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

        public static implicit operator int(Mesh mesh) { return mesh.id; }

        public List<Vector3> Vertices { get { return vertices; } }

        public List<Vector3> Normals { get { return normals; } }

        public List<Vector4> Colors { get { return colors; } }

        public List<Vector2> TexCoords { get { return texCoords; } }

        public List<uint> Indices { get { return indices; } }

        public bool DrawAsPoints = false;

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

        public Mesh(MeshData data)
            : this() {
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

                if (unit.colour != new Vector4(-1, -1, -1, -1))
                    colors.Add(unit.colour);
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

        public void AddIndices(IEnumerable<uint> indices) {
            this.indices.AddRange(indices);
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
                GL.DrawElements((DrawAsPoints ? BeginMode.Points : BeginMode.Triangles), indices.Count, DrawElementsType.UnsignedInt, 0);
            else
                GL.DrawArrays((DrawAsPoints ? PrimitiveType.Points : PrimitiveType.Triangles), 0, vertices.Count);

            GL.DisableVertexAttribArray((int)Shader.ArrayIndex.VertexPosition);
            GL.DisableVertexAttribArray((int)Shader.ArrayIndex.VertexColor);
            GL.DisableVertexAttribArray((int)Shader.ArrayIndex.VertexNormal);
            GL.DisableVertexAttribArray((int)Shader.ArrayIndex.VertexTexCoord);
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

        #region Load from file

        public static Mesh FromFile(string filepath, bool fast = true) {
            List<Vector3> positions = new List<Vector3>();
            List<Vector3> normals = new List<Vector3>();
            List<Vector2> texCoords = new List<Vector2>();
            List<Vector4> colours = new List<Vector4>();

            StreamReader obj_file = new StreamReader(filepath);
            MeshData vertex_data = new MeshData();
            int[] faceParamsIndices = new int[3];

            int lineCount = 0;
            string line;
            string[] tokens;
            char[] splitchar = new char[] { ' ' };
            char[] splitparamschar = new char[] { '/' };

            while (!obj_file.EndOfStream) {
                line = obj_file.ReadLine();
                Console.WriteLine(lineCount++);
                if (line == "" || line.StartsWith("#"))
                    continue;

                if (line.StartsWith("v ")) {
                    tokens = line.Split(splitchar);

                    if (tokens.Length != 4) {
                        if (tokens.Length != 7)
                            throw new Exception("Invalid number of arguments for a vertex position.");
                    }


                    Vector3 newVertex = new Vector3(float.Parse(tokens[1]), float.Parse(tokens[2]), float.Parse(tokens[3]));

                    Vector4 newColour;
                    if (tokens.Length == 7)
                        newColour = new Vector4(float.Parse(tokens[4]), float.Parse(tokens[5]), float.Parse(tokens[6]), 1.0f);
                    else
                        newColour = new Vector4(-1, -1, -1, -1);

                    positions.Add(newVertex);
                    colours.Add(newColour);

                    continue;
                }

                if (line.StartsWith("vn ")) {
                    tokens = line.Split(splitchar);

                    if (tokens.Length != 4)
                        throw new Exception("Invalid number of arguments for a vertex normal.");

                    Vector3 newNormal = new Vector3(float.Parse(tokens[1]), float.Parse(tokens[2]), float.Parse(tokens[3]));
                    normals.Add(newNormal);

                    continue;
                }

                if (line.StartsWith("vt ")) {
                    tokens = line.Split(splitchar);

                    // Up to 4 arguments, only reads two
                    Vector2 newUV = new Vector2(float.Parse(tokens[1]), float.Parse(tokens[2]));
                    texCoords.Add(newUV);

                    continue;
                }

                if (line.StartsWith("f ")) {
                    tokens = line.Split(splitchar);

                    if (tokens.Length != 4)
                        throw new Exception("Invalid number of arguments for a face. Only triangles are supported.");

                    string[] values;
                    List<uint> faceIndexes = new List<uint>();

                    for (int i = 1; i < 4; i++) {
                        values = tokens[i].Split(splitparamschar);

                        if (values.Length != 3)
                            throw new Exception("Wrong number of parameters for a triangle face.");

                        for (int param = 0; param < 3; param++) {
                            if (values[param] != "")
                                faceParamsIndices[param] = int.Parse(values[param]) - 1;
                            else
                                faceParamsIndices[param] = -1;
                        }

                        Vector3 vertexPosition = positions[faceParamsIndices[0]];
                        Vector3 vertexNormal;
                        Vector2 vertexTexCoord;
                        Vector4 vertexColour = colours[faceParamsIndices[0]];

                        if (faceParamsIndices[2] >= 0)
                            vertexNormal = normals[faceParamsIndices[2]];
                        else
                            vertexNormal = Vector3.Zero;

                        if (faceParamsIndices[1] >= 0)
                            vertexTexCoord = texCoords[faceParamsIndices[1]];
                        else
                            vertexTexCoord = new Vector2(-1, -1);

                        VertexUnit unit = new VertexUnit(vertexPosition, vertexNormal, vertexTexCoord, vertexColour);

                        if (fast)
                            vertex_data.AddVertexRepeat(unit);
                        else
                            vertex_data.AddVertexUnit(unit);
                    }

                    continue;
                }

                // if line starts with anything else, just ignore it for now (g, v, s, o, mtllib, usemtl...)
            }

            obj_file.Close();

            Mesh mesh = new Mesh(vertex_data);
            mesh.SetUp();

            return mesh;
        }

        #endregion

        #endregion
    }
}
