using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace FrameworkGL
{
    class LightSource
    {
        #region Attributes

        private Vector3 position;
        private Vector3 colour;
        private Vector3 ambient;

        #endregion

        #region Properties

        public Vector3 Position {
            get { return position; }
            set { position = value; }
        }

        public Vector3 Colour {
            get { return colour; }
            set { colour = value; }
        }

        public Vector3 Ambient {
            get { return ambient; }
            set { ambient = value; }
        }

        #endregion

        #region Methods

        public LightSource(Vector3 position, Vector3 colour, Vector3 ambient) {
            this.position = position;
            this.colour = colour;
            this.ambient = ambient;
        }

        #endregion
    }
}
