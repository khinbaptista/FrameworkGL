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
        
        Sprite sprite;
        Font2D font;

        public HUD(Rectangle client, int layer = 5) {
            camera = Camera.New2D(client);
            shader = Shader.Textured;
            
            sprite = new Sprite(new Rectangle(0, 0, 128, 128), new Texture(@"img\gradientGB.png"), layer);
            
            font = new Font2D(new Texture(@"img\Courier.png"), 16, 8);
        }

        public void Write(string message) {
            Vector2 position = new Vector2();
            position.X = GameMain.Viewport.Width - 300;
            position.Y = GameMain.Viewport.Height - 50;
            font.Write(shader, camera.CameraMatrix, position, message);
        }

        public void Draw() {
            font.Write(shader, camera.CameraMatrix, new Vector2(50, GameMain.Viewport.Height - 50),
                "CGP2012M - ROD14465894 - Khin Baptista");

            shader.TransformationMatrix = sprite.ModelMatrix * camera.CameraMatrix;
            shader.Texture = sprite.Image;
            shader.Activate();
            sprite.Draw();
            shader.Deactivate();
        }
    }
}
