using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;

namespace FrameworkGL
{
    class Font2D
    {
        #region Attributes

        private Vector2 charDimensions;
        private int charPerLine;
        private int lineCount;
        private Sprite canvas;
        private Texture source;

        #endregion

        #region Methods

        public Font2D(Texture sourceTexture, int charactersPerLine, int lineCount, int charWidth = -1, int charHeight = -1) {
            if (charWidth != -1 && charHeight != -1)
                charDimensions = new Vector2(charWidth, charHeight);
            else {
                int characterWidth, characterHeight;

                characterWidth = sourceTexture.Width / charactersPerLine;
                characterHeight = sourceTexture.Height / lineCount;

                charDimensions = new Vector2(characterWidth, characterHeight);
            }

            this.charPerLine = charactersPerLine;
            this.lineCount = lineCount;
            this.source = sourceTexture;
        }

        private Rectangle CharacterBound(char character) {
            Rectangle characterBound = new Rectangle(0, 0, (int)charDimensions.X, (int)charDimensions.Y);

            characterBound.X = (character % charPerLine) * (int)charDimensions.X;
            characterBound.Y = (character / charPerLine) * (int)charDimensions.Y;

            return characterBound;
        }

        public void Write(Shader shader, Matrix4 camera, Vector2 position, string text, int layer = 5) {
            shader.Texture = source;
            source.Bind();

            canvas = new Sprite(new Rectangle(0, 0, (int)charDimensions.X, (int)charDimensions.Y), source, layer);

            int counter = 0;
            foreach (char c in text) {
                canvas.Clip(CharacterBound(c));
                canvas.Position = position + new Vector2(counter * charDimensions.X, 0);
                shader.TransformationMatrix = canvas.ModelMatrix * camera;

                shader.Activate();
                canvas.Draw();
                shader.Deactivate();

                counter++;
            }

            source.Unbind();
        }

        public int MeasureString(string text) {
            int size = 0;

            foreach (char c in text)
                size += (int)charDimensions.X;

            return size;
        }

        #endregion
    }
}
