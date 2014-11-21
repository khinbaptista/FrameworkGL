using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrameworkGL
{
    class MeshData
    {
        public List<VertexUnit> Vertices { get; private set; }
        public List<uint> Indices { get; private set; }

        public MeshData() {
            Vertices = new List<VertexUnit>();
            Indices = new List<uint>();
        }

        public void AddVertexUnit(VertexUnit unit) {
            bool add = true;
            int i = 0;

            while (add == true && i < Vertices.Count) {
                if (Vertices[i] == unit) {
                    add = false;
                    Indices.Add((uint)i);
                }

                i++;
            }

            if (add) {
                Vertices.Add(unit);
                Indices.Add((uint)Vertices.Count - 1);
            }
        }
    }
}
