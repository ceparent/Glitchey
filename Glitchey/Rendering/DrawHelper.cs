using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;

using QuickFont;
namespace Glitchey.Rendering
{
    static class DrawHelper
    {
        public static void DrawRectangle(Vector2 origin, Vector2 size, Color color, bool filled, bool alpha = false)
        {
            Rectangle rec = new Rectangle((int)(origin.X - size.X / 2),(int)( origin.Y - size.Y / 2),(int) size.X,(int) size.Y);
            DrawRectangle(rec, color, filled, alpha);
        }


        public static void DrawRectangle(Rectangle rectangle, Color color, bool filled, bool alpha = false)
        {
            if (alpha)
                GL.Color4(color);
            else
                GL.Color3(color);

            GL.LineWidth(1);

            if (filled)
            {
                GL.Begin(PrimitiveType.Quads);
            }
            else
            {
                GL.Begin(PrimitiveType.LineLoop);
            }


            GL.Vertex2(rectangle.Left, rectangle.Top);
            GL.Vertex2(rectangle.Left, rectangle.Bottom);
            GL.Vertex2(rectangle.Right, rectangle.Bottom);
            GL.Vertex2(rectangle.Right, rectangle.Top);

            GL.End();
        }

        public static void DrawCircle(float radius, Vector2 position, Color color, bool filled)
        {
            int tesselation = 30;
            DrawCircle(radius, position, color, tesselation, filled);
        }

        public static void DrawCircle(float radius, Vector2 position, Color color, int tesselation, bool filled)
        {
            GL.Color3(color);
            GL.LineWidth(4);



            if (filled)
            {
                GL.Begin(PrimitiveType.TriangleFan);
                GL.Vertex2(position.X, position.Y);
            }
            else
            {
                GL.Begin(PrimitiveType.LineLoop);
            }

            for (float i = 0; i < MathHelper.Pi * 2; i += MathHelper.Pi / (float)tesselation)
            {
                GL.Vertex2(Math.Cos(i) * radius + position.X, Math.Sin(i) * radius + position.Y);
            }

            GL.End();

        }
        
        public static int LoadTexture(string path, Color? invisible, int quality = 1, bool repeat = false, bool flip_y = false)
        {
            Bitmap bitmap = new Bitmap(path);

            if (invisible != null)
            {
                bitmap.MakeTransparent(invisible.Value);

            }

            int texture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, texture);

            switch (quality)
            {
                case 0: // low
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Linear);
                    break;
                case 1: // high
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Nearest);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Nearest);
                    break;
            }

            if (repeat)
            {
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)All.Repeat);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)All.Repeat);
            }
            else
            {
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)All.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)All.ClampToEdge);
            }

            System.Drawing.Imaging.BitmapData bitmap_data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                        System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bitmap.Width, bitmap.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, bitmap_data.Scan0 );

            bitmap.UnlockBits(bitmap_data);
            bitmap.Dispose();

            GL.BindTexture(TextureTarget.Texture2D, 0);

            return texture;
        }

        public static void DrawTexture(int texture, Rectangle rectangle, double angle, Vector2 originOffset)
        {
            GL.MatrixMode(MatrixMode.Projection);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.Color4(Color.White);
            GL.BindTexture(TextureTarget.Texture2D, texture);

            // rotation
            Vector3 origin = new Vector3((rectangle.Left + rectangle.Right) / 2 + originOffset.X, (rectangle.Top + rectangle.Bottom) / 2 + originOffset.Y, 0);
            GL.Translate(origin);
            GL.Rotate(MathHelper.RadiansToDegrees(angle), Vector3d.UnitZ);
            GL.Translate(-origin);

            // draw
            GL.Begin(PrimitiveType.Quads);

            GL.TexCoord2(1.0f, 0.0f);
            GL.Vertex2(rectangle.Right, rectangle.Top);

            GL.TexCoord2(0f, 0f);
            GL.Vertex2(rectangle.Left, rectangle.Top);

            GL.TexCoord2(0f, 1f);
            GL.Vertex2(rectangle.Left, rectangle.Bottom);

            GL.TexCoord2(1f, 1f);
            GL.Vertex2(rectangle.Right, rectangle.Bottom);

            GL.End();

            GL.Translate(origin);
            GL.Rotate(-MathHelper.RadiansToDegrees(angle), Vector3d.UnitZ);
            GL.Translate(-origin);

            GL.BindTexture(TextureTarget.Texture2D, 0);

            

        }
        public static void DrawString(string message, QFont qfont, Color color, Vector2 position, bool center, bool backOffsetText = false)
        {
            qfont.Options.Colour = color;

            Vector2 offset = Vector2.Zero;
            SizeF stringSize = qfont.Measure(message);
            if (backOffsetText)
                offset += new Vector2(-stringSize.Width, -stringSize.Height);
            if (center)
                offset += new Vector2(-stringSize.Width / 2, -stringSize.Height / 2);


            QFont.Begin();
            qfont.Print(message, position + offset);
            QFont.End();
        }

        public static void DrawGrid(Vector3 CameraOffset, Vector2 gridSize, Color color, float thickness = 2f, float spaceWidth = 50)
        {

            GL.Color3(color);
            GL.LineWidth(thickness);


            GL.Begin(PrimitiveType.Lines);
            float startx = CameraOffset.X - gridSize.X / 2 - CameraOffset.X % spaceWidth - spaceWidth;
            float endx = CameraOffset.X + gridSize.X / 2;

            float starty = CameraOffset.Y - gridSize.Y / 2 - CameraOffset.Y % spaceWidth - spaceWidth;
            float endy = CameraOffset.Y + gridSize.Y / 2;

            for (float x = startx; x <= endx; x += spaceWidth)
            {
                GL.Vertex2(x, starty);
                GL.Vertex2(x, endy);
            }
            for (float y = starty; y <= endy; y += spaceWidth)
            {
                GL.Vertex2(startx, y);
                GL.Vertex2(endx, y);
            }

            GL.End();


        }

    }
}
