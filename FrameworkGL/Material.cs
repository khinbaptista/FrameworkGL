using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using OpenTK;

namespace FrameworkGL
{
    class Material
    {
        #region Attributes

        private string name;
        private Vector3 Ka;
        private Vector3 Kd;
        private Vector3 Ks;
        private float q;
        private float alpha;
        private Texture texture;

        #endregion

        #region Properties

        /// <summary>
        /// Name of the material (.mtl files use this)
        /// </summary>
        public string Name {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// Ambient reflection coefficient of the material
        /// </summary>
        public Vector3 Ambient {
            get { return Ka; }
            set { Ka = value; }
        }

        /// <summary>
        /// Diffuse reflection coefficient of the material
        /// </summary>
        public Vector3 Diffuse {
            get { return Kd; }
            set { Kd = value; }
        }

        /// <summary>
        /// Specular reflection coefficient of the material
        /// </summary>
        public Vector3 Specular {
            get { return Ks; }
            set { Ks = value; }
        }

        /// <summary>
        /// The shininness of the material (ranges from 1 to hundreds - the greater, the more polished it looks)
        /// </summary>
        public float Shininness {
            get { return q; }
            set { q = value; }
        }

        public float Alpha {
            get { return alpha; }
            set { alpha = value; }
        }

        public Texture Texture {
            get { return texture; }
            set { texture = value; }
        }

        #endregion

        #region Methods

        public Material() {
            name = "";
            Ka = new Vector3(0.0f, 0.0f, 0.0f);
            Kd = new Vector3(1.0f, 1.0f, 1.0f);
            Ks = new Vector3(0.4f, 0.4f, 0.4f);
            q = 1;
            alpha = 1;
        }

        public static Material FromFile(string filepath) {
            Material material = new Material();
            StreamReader file = new StreamReader(filepath);
            string line;
            char[] splitchar = new char[] { ' ' };
            bool done = false;

            while (!file.EndOfStream && !done) {
                line = file.ReadLine();

                if (line.StartsWith("#") || String.IsNullOrEmpty(line))
                    continue;

                if (line.StartsWith("newmtl ")) {
                    if (!String.IsNullOrEmpty(material.name)) {
                        done = true;
                        continue;
                    }

                    material.Name = line.Split(splitchar)[1];
                    continue;
                }

                if (line.StartsWith("Ka ")) {
                    string[] tokens = line.Split(splitchar);

                    if (tokens.Length != 4)
                        throw new Exception("Wrong number of parameters for Ka in the material file. File name: " + filepath);

                    Vector3 ambient = new Vector3();
                    ambient.X = float.Parse(tokens[1]);
                    ambient.Y = float.Parse(tokens[2]);
                    ambient.Z = float.Parse(tokens[3]);
                    material.Ambient = ambient;
                    continue;
                }

                if (line.StartsWith("Kd ")) {
                    string[] tokens = line.Split(splitchar);

                    if (tokens.Length != 4)
                        throw new Exception("Wrong number of parameters for Kd in the material file. File name: " + filepath);

                    Vector3 diffuse = new Vector3();
                    diffuse.X = float.Parse(tokens[1]);
                    diffuse.Y = float.Parse(tokens[2]);
                    diffuse.Z = float.Parse(tokens[3]);
                    material.Diffuse = diffuse;
                    continue;
                }

                if (line.StartsWith("Ks ")) {
                    string[] tokens = line.Split(splitchar);

                    if (tokens.Length != 4)
                        throw new Exception("Wrong number of parameters for Ks in the material file. File name: " + filepath);

                    Vector3 specular = new Vector3();
                    specular.X = float.Parse(tokens[1]);
                    specular.Y = float.Parse(tokens[2]);
                    specular.Z = float.Parse(tokens[3]);
                    material.Specular = specular;
                    continue;
                }

                if (line.StartsWith("Ns ")) {
                    material.Shininness = float.Parse(line.Split(splitchar)[1]);
                    continue;
                }

                if (line.StartsWith("d ")) {
                    string[] tokens = line.Split(splitchar);

                    material.Alpha = float.Parse(tokens[tokens.Length - 1]); // in case there is a different flag (-halo) it will ignore it, and work normally
                    continue;
                }

                if (line.StartsWith("map_Kd ")){
                    string[] tokens = line.Split(splitchar);
                    string texturePath = Path.GetDirectoryName(filepath) + tokens[tokens.Length - 1];

                    material.Texture = new Texture(texturePath);
                    continue;
                }
            }

            return material;
        }

        #endregion
    }
}
