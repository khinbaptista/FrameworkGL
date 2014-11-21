using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;

namespace FrameworkGL
{
    class VertexUnit
    {
        public Vector3 position { get; private set; }
        public Vector3 normal { get; private set; }
        public Vector2 texCoord { get; private set; }

        public VertexUnit(Vector3 position, Vector3 normal, Vector2 texCoord) {
            this.position = position;
            this.normal = normal;
            this.texCoord = texCoord;
        }

        public static bool operator ==(VertexUnit a, VertexUnit b) {
            return
                a.position == b.position &&
                a.normal == b.normal &&
                a.texCoord == b.texCoord;
        }

        public static bool operator !=(VertexUnit a, VertexUnit b) {
            return !(a == b);
        }

        public static bool PositionEquals(VertexUnit a, VertexUnit b) {
            return a.position == b.position;
        }

        public static bool NormalEquals(VertexUnit a, VertexUnit b) {
            return a.normal == b.normal;
        }

        public static bool TexCoordEquals(VertexUnit a, VertexUnit b) {
            return a.texCoord == b.texCoord;
        }

        public override bool Equals(object obj) {
            return base.Equals(obj);
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }
    }
}
