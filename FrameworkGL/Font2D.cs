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

        private Vector2 dimensions;
        private int charactersPerLine;
        private int lineCount;
        private Sprite canvas;
        private Texture texture;

        #endregion


        #region Properties

        public Shader Shader {
            get { return canvas.Shader; }
        }

        #endregion


        #region Methods

        public Font2D(Texture texture, int charactersPerLine, int lineCount, int characterWidth = -1, int characterHeight = -1) {
            if (characterWidth != -1 && characterHeight != -1)
                dimensions = new Vector2(characterWidth, characterHeight);
            else {
                int charWidth, charHeight;

                charWidth = texture.Width / charactersPerLine;
                charHeight = texture.Height / lineCount;

                dimensions = new Vector2(charWidth, charHeight);
            }

            this.charactersPerLine = charactersPerLine;
            this.lineCount = lineCount;

            this.texture = texture;
        }

        /// <summary>
        /// Draws a string on the screen
        /// </summary>
        /// <param name="text">Text to be rendered</param>
        /// <param name="position">The position where to draw the given text, in screen coordinates</param>
        public void Write(Camera camera2d, Vector2 position, string text, int layer = 2) {
            canvas = new Sprite(new Rectangle(0, 0, (int)dimensions.X, (int)dimensions.Y), layer);
            canvas.Texture = texture;

            Shader.SetVariable(Shader.NameOf_ProjectionMatrix, camera2d.ProjectionMatrix);
            Shader.SetVariable(Shader.NameOf_ViewMatrix, camera2d.ViewMatrix);

            int counter = 0;
            foreach (char c in text) {
                canvas.ChangeClipper(CharacterBound(c));
                canvas.Position = position + new Vector2(counter * dimensions.X, 0);

                canvas.Draw();
                counter++;
            }
        }

        /// <summary>
        /// Gets the position of the character inside the source texture
        /// </summary>
        /// <param name="character">Character to be found</param>
        /// <returns>The rectangle containing the asked character</returns>
        public Rectangle CharacterBound(char character) {
            Rectangle characterBound = new Rectangle(0, 0, (int)dimensions.X, (int)dimensions.Y);

            characterBound.X = (character % charactersPerLine) * (int)dimensions.X;
            characterBound.Y = (character / charactersPerLine) * (int)dimensions.Y;

            return characterBound;
        }

        /// <summary>
        /// Gets the size, in pixels, that the string will take to be rendered using this font.
        /// </summary>
        /// <param name="text">String text to measure</param>
        /// <returns>Size in pixels</returns>
        public int MeasureString(string text) {
            int size = 0;

            foreach (char c in text)
                size += (int)dimensions.X;

            return size;
        }

        #endregion
    }
}
