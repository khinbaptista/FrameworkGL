using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using OpenTK;

namespace FrameworkGL
{
    class HUD
    {
        private Camera camera;
        private Shader shader;

        private Model model;
        private Texture texture;

        public HUD(Rectangle client, int layer = 3) {
            camera = Camera.New2D(client);
            shader = Shader.Textured;

            Mesh wall = new Mesh();
            wall.AddVertex(new Vector3(0, 0, layer));
            wall.AddVertex(new Vector3(0, 256, layer));
            wall.AddVertex(new Vector3(256, 0, layer));
            wall.AddVertex(new Vector3(256, 256, layer));
            wall.AddTexCoord(new Vector2(0.0f, 1.0f));
            wall.AddTexCoord(new Vector2(0.0f, 0.0f));
            wall.AddTexCoord(new Vector2(1.0f, 1.0f));
            wall.AddTexCoord(new Vector2(1.0f, 0.0f));
            wall.AddIndices(new uint[] { 2, 1, 0, 3, 1, 2 });
            wall.SetUp();
            model = new Model(wall);
            
            texture = new Texture(@"img\gradientGB.png");
            shader.Texture = texture;
            shader.TransformationMatrix = model.ModelMatrix * camera.CameraMatrix;
        }

        public void Draw() {
            shader.TransformationMatrix = model.ModelMatrix * camera.CameraMatrix;
            shader.Activate();
            texture.Bind();

            model.Draw();
            
            texture.Unbind();
            shader.Deactivate();
        }
    }
}
