using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace FrameworkGL
{
    class Shader : IDisposable
    {
        public enum ArrayIndex
        {
            VertexPosition = 0,
            VertexColor = 1,
            VertexNormal = 2,
            VertexTexCoord = 3
        }

        #region Attributes

        private int id;
        private Dictionary<string, int> uniforms;

        private int vertexShaderID;
        private int fragmentShaderID;

        // Variable names
        public string NameOf_ProjectionMatrix;
        public string NameOf_ModelMatrix;
        public string NameOf_ViewMatrix;

        public string NameOf_CameraMatrix;
        public string NameOf_ModelViewMatrix;
        public string NameOf_MvpMatrix;

        public string NameOf_VertexPosition;
        public string NameOf_VertexColor;
        public string NameOf_VertexNormal;
        public string NameOf_VertexTexCoord;

        public string NameOf_Texture;
        public string NameOf_Alpha;
        public string NameOf_BumpMap;

        // Variable values
        private Matrix4 projectionMatrix;
        private Matrix4 modelMatrix;
        private Matrix4 viewMatrix;

        private Matrix4 cameraMatrix;
        private Matrix4 modelviewMatrix;
        private Matrix4 mvpMatrix;

        private int texture;
        private float alpha;
        private int bumpMap;

        #endregion

        #region Presets

        public static Shader FixedLight {
            get {
                Shader shader = new Shader();
                shader.AddShaderFile(ShaderType.VertexShader, @"GLSL\vs_normal");
                shader.AddShaderFile(ShaderType.FragmentShader, @"GLSL\fs_fixedLight.glsl");
                shader.Link();

                return shader;
            }
        }

        public static Shader Color {
            get {
                Shader shader = new Shader();
                shader.AddShaderFile(ShaderType.VertexShader, @"GLSL\vs_mvp_color.glsl");
                shader.AddShaderFile(ShaderType.FragmentShader, @"GLSL\fs_color.glsl");
                shader.Link();

                return shader;
            }
        }

        #endregion

        #region Properties

        public static bool IsSupported {
            get { return (new Version(GL.GetString(StringName.Version).Substring(0, 3)) >= new Version(2, 0)); }
        }

        public static implicit operator int(Shader shader) {
            return shader.id;
        }

        public Matrix4 ProjectionMatrix {
            get { return projectionMatrix; }
            set {
                projectionMatrix = value;
                SetVariable(NameOf_ProjectionMatrix, projectionMatrix);
            }
        }

        public Matrix4 ViewMatrix {
            get { return viewMatrix; }
            set {
                viewMatrix = value;
                SetVariable(NameOf_ViewMatrix, viewMatrix);
            }
        }

        public Matrix4 ModelMatrix {
            get { return modelMatrix; }
            set {
                modelMatrix = value;
                SetVariable(NameOf_ModelMatrix, modelMatrix);
            }
        }

        public Matrix4 CameraMatrix {
            get { return cameraMatrix; }
            set {
                cameraMatrix = value;
                SetVariable(NameOf_CameraMatrix, cameraMatrix);
            }
        }

        public Matrix4 ModelviewMatrix {
            get { return modelviewMatrix; }
            set {
                modelviewMatrix = value;
                SetVariable(NameOf_ModelViewMatrix, modelviewMatrix);
            }
        }

        public Matrix4 TransformationMatrix {
            get { return mvpMatrix; }
            set {
                mvpMatrix = value;
                SetVariable(NameOf_MvpMatrix, mvpMatrix);
            }
        }

        public int Texture {
            get { return texture; }
            set {
                texture = value;
                SetVariable(NameOf_Texture, texture);
            }
        }

        public float Alpha {
            get { return alpha; }
            set {
                alpha = value;
                SetVariable(NameOf_Alpha, alpha);
            }
        }

        public int BumpMap {
            get { return bumpMap; }
            set {
                bumpMap = value;
                SetVariable(NameOf_BumpMap, bumpMap);
            }
        }

        #endregion

        #region Methods

        public Shader() {
            if (!IsSupported) throw new Exception("The system does not support shaders.\r\n");

            id = GL.CreateProgram();

            uniforms = new Dictionary<string, int>();
            vertexShaderID = -1;
            fragmentShaderID = -1;

            InitializeVariableNames();
            InitializeVariableValues();
        }

        private void InitializeVariableNames() {
            NameOf_ProjectionMatrix = "projection_matrix";
            NameOf_ModelMatrix = "model_matrix";
            NameOf_ViewMatrix = "view_matrix";

            NameOf_CameraMatrix = "camera_matrix";
            NameOf_ModelViewMatrix = "modelview_matrix";
            NameOf_MvpMatrix = "mvp_matrix";

            NameOf_VertexPosition = "vertex_position";
            NameOf_VertexColor = "vertex_color";
            NameOf_VertexNormal = "vertex_normal";
            NameOf_VertexTexCoord = "vertex_texCoord";

            NameOf_Texture = "texture_sampler";
            NameOf_Alpha = "alpha";
            NameOf_BumpMap = "bump_sampler";
        }

        private void InitializeVariableValues() {
            projectionMatrix = Matrix4.Identity;
            modelMatrix = Matrix4.Identity;
            viewMatrix = Matrix4.Identity;

            cameraMatrix = Matrix4.Identity;
            mvpMatrix = Matrix4.Identity;

            texture = -1;
            alpha = 1.0f;
            bumpMap = -1;
        }

        /// <summary>
        /// Creates a new shader
        /// </summary>
        /// <param name="type">Type of shader to be created (only vertex or fragment shaders supported)</param>
        /// <param name="source">Source code of the shader</param>
        public void AddShader(ShaderType type, string source) {
            int statusCode = -1;
            string info = "";

            int shaderID = GL.CreateShader(type);
            GL.ShaderSource(shaderID, source);
            GL.CompileShader(shaderID);

            GL.GetShaderInfoLog(shaderID, out info);
            GL.GetShader(shaderID, ShaderParameter.CompileStatus, out statusCode);

            if (statusCode != 1) {
                GL.DeleteShader(shaderID);
                throw new Exception("Error creating shader.\r\nStatus code: " + statusCode + " > " + info);
            }

            if (type == ShaderType.FragmentShader)
                fragmentShaderID = shaderID;
            else if (type == ShaderType.VertexShader)
                vertexShaderID = shaderID;
        }

        /// <summary>
        /// Adds a shader from a file containing GLSL code
        /// </summary>
        /// <param name="type">Type of the shader</param>
        /// <param name="path">Path of the source code file</param>
        public void AddShaderFile(ShaderType type, string path) {
            StreamReader file = new StreamReader(path);
            string source = file.ReadToEnd();

            file.Close();
            this.AddShader(type, source);
        }

        /// <summary>
        /// Links the shader so they are usable
        /// </summary>
        public void Link() {
            int statusCode = -1;
            string info = "";

            if (vertexShaderID != -1)
                GL.AttachShader(id, vertexShaderID);

            if (fragmentShaderID != -1)
                GL.AttachShader(id, fragmentShaderID);

            GL.BindAttribLocation(id, (int)ArrayIndex.VertexPosition, NameOf_VertexPosition);
            GL.BindAttribLocation(id, (int)ArrayIndex.VertexColor, NameOf_VertexColor);
            GL.BindAttribLocation(id, (int)ArrayIndex.VertexNormal, NameOf_VertexNormal);
            GL.BindAttribLocation(id, (int)ArrayIndex.VertexTexCoord, NameOf_VertexTexCoord);

            GL.LinkProgram(id);

            GL.GetProgramInfoLog(id, out info);
            GL.GetProgram(id, GetProgramParameterName.LinkStatus, out statusCode);

            if (statusCode != 1) {
                GL.DeleteProgram(id);
                throw new Exception("Error linking program.\r\nStatus code: " + statusCode + " > " + info);
            }

            if (vertexShaderID != -1) {
                GL.DetachShader(id, vertexShaderID);
                GL.DeleteShader(vertexShaderID);
                vertexShaderID = -1;
            }

            if (fragmentShaderID != -1) {
                GL.DetachShader(id, fragmentShaderID);
                GL.DeleteShader(fragmentShaderID);
                fragmentShaderID = -1;
            }
        }

        public void Activate() {
            GL.UseProgram(id);
        }

        public void Deactivate() {
            GL.UseProgram(0);
        }

        public void Dispose() {
            GL.DeleteProgram(id);
        }

        #region Change shader uniform values

        private int GetVariableLocation(string name) {
            if (uniforms.ContainsKey(name))
                return uniforms[name];

            int location = GL.GetUniformLocation(id, name);

            if (location != -1)
                uniforms.Add(name, location);
            else
                Console.WriteLine("Failed to retrieve location of uniform variable \"" + name + "\".", "Error");

            return location;
        }

        public void SetVariable(string name, float value) {
            GL.UseProgram(id);

            int location = GetVariableLocation(name);

            if (location != -1)
                GL.Uniform1(location, value);

            GL.UseProgram(0);
        }

        public void SetVariable(string name, float x, float y) {
            GL.UseProgram(id);

            int location = GetVariableLocation(name);

            if (location != -1)
                GL.Uniform2(location, x, y);

            GL.UseProgram(0);
        }

        public void SetVariable(string name, float x, float y, float z) {
            GL.UseProgram(id);

            int location = GetVariableLocation(name);

            if (location != -1)
                GL.Uniform3(location, x, y, z);

            GL.UseProgram(0);
        }

        public void SetVariable(string name, float x, float y, float z, float w) {
            GL.UseProgram(id);

            int location = GetVariableLocation(name);

            if (location != -1)
                GL.Uniform4(location, x, y, z, w);

            GL.UseProgram(0);
        }

        public void SetVariable(string name, Vector2 vec2) {
            SetVariable(name, vec2.X, vec2.Y);
        }

        public void SetVariable(string name, Vector3 vec3) {
            SetVariable(name, vec3.X, vec3.Y, vec3.Z);
        }

        public void SetVariable(string name, Vector4 vec4) {
            SetVariable(name, vec4.X, vec4.Y, vec4.Z, vec4.W);
        }

        public void SetVariable(string name, Color color) {
            SetVariable(name, color.R / 255, color.G / 255, color.B / 255, color.A / 255);
        }

        public void SetVariable(string name, Matrix4 mat4) {
            GL.UseProgram(id);

            int location = GetVariableLocation(name);

            if (location != -1)
                GL.UniformMatrix4(location, false, ref mat4);

            GL.UseProgram(0);
        }

        #endregion

        #endregion
    }
}
