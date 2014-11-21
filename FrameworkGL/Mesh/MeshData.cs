using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrameworkGL
{
    class MeshData
    {
        public List<VertexUnit> vertices { get; private set; }
        public List<uint> indices { get; private set; }

        public MeshData() {
            vertices = new List<VertexUnit>();
            indices = new List<uint>();
        }

        public void AddVertexUnit(VertexUnit unit) {
            bool add = true;
            int i = 0;

            while (add == true && i < vertices.Count) {
                if (vertices[i] == unit) {
                    add = false;
                    indices.Add((uint)i);
                }

                i++;
            }

            if (add) {
                vertices.Add(unit);
                indices.Add((uint)vertices.Count - 1);
            }
        }
    }
}
