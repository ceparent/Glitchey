using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using System.IO;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

using Glitchey.Entities;
using Glitchey.Levels;
using Glitchey.Components;
using Glitchey.Systems;
using System.Text.RegularExpressions;

namespace Glitchey.Rendering
{
    public struct GameResolution
    {
        public int Width;
        public int Height;
        public GameResolution(int width, int height)
        {
            Width = width;
            Height = height;
        }
    }

     static class GameRenderer
     {


         public static void SetupViewportMenu(Main main)
         {
             GL.ClearColor(Color.Black);

             GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);

             GL.Enable(EnableCap.Texture2D);
             GL.Enable(EnableCap.Blend);
             GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
             GL.Disable(EnableCap.CullFace);
             GL.Disable(EnableCap.DepthTest);
             GL.MatrixMode(MatrixMode.Projection);

             int width = GameOptions.GetVariable("w_width");
             int height = GameOptions.GetVariable("w_height");
             float aspect = (float)height / (float)width;
             float nearClip = 0f;
             float farClip = 1000.0f;
             GL.LoadIdentity();
             GL.Ortho(0, width, height, 0, nearClip, farClip);

         }

         public static void SetResolution(Main main)
         {
             int width = GameOptions.GetVariable("w_width");
             int height = GameOptions.GetVariable("w_height");
             

             if (GameOptions.GetVariable("w_state") == (int)WindowState.Fullscreen)
                 DisplayDevice.Default.ChangeResolution(width, height, DisplayDevice.Default.BitsPerPixel, DisplayDevice.Default.RefreshRate);

             
             main.ClientSize = new Size(width, height);
             GL.Viewport(main.ClientRectangle);
         }

         public static void SetupViewportGame()
         {

             GL.Enable(EnableCap.TextureCoordArray);
             GL.Enable(EnableCap.NormalArray);
             GL.Enable(EnableCap.CullFace);
             GL.Enable(EnableCap.DepthTest);
             GL.CullFace(CullFaceMode.Front);

         }

         public static List<GameResolution> GetResolutions()
         {
             List<GameResolution> resolutions = new List<GameResolution>();
             foreach (DisplayResolution res in DisplayDevice.Default.AvailableResolutions)
             {
                 int width = res.Width;
                 int height = res.Height;
                 double aspect = Math.Round(width / (float)height, 2);
                 double x = Math.Round(4 / 3f,2);
                 double y = Math.Round(16 / 9f, 2);
                 if (width < 800 || (aspect != x && aspect != y))
                     continue;

                 if (resolutions.Where(o => o.Height == height && o.Width == width).Count() == 0)
                     resolutions.Add(new GameResolution(width, height));


             }
             return resolutions.OrderByDescending(o => o.Width).ToList();
         }



         public static void RenderWorld(GameWorld world)
         {
             if (world == null)
                 throw new InvalidOperationException("world is null");

             BspRenderer.Draw();

         }

         public static void RenderDebugLines(Vector3[] vertices)
         {

             if (GL.GetError() != ErrorCode.NoError)
                 throw new Exception();

             
             GL.LoadIdentity();
             GL.Color4(Color.Red);
             Matrix4 trans = CameraSystem.viewMatrix * CameraSystem.projectionMatrix;
             GL.MatrixMode(MatrixMode.Projection);
             GL.LoadMatrix(ref trans);
             
             
             GL.EnableClientState(ArrayCap.VertexArray);
             GL.VertexPointer(3, VertexPointerType.Float, sizeof(float) * 3, vertices);
             GL.DrawArrays(PrimitiveType.Lines, 0, vertices.Length);

             GL.DisableClientState(ArrayCap.VertexArray);

         }



         public static void RenderMesh(IRender mesh)
         {
             if (GL.GetError() != ErrorCode.NoError)
                 throw new Exception();

             Shaders.BaseShader shader = mesh.Render.Shader;
             int VBuffer = mesh.Render.VBuffer;
             int IBuffer = mesh.Render.IBuffer;
             int Indexcount = mesh.Render.Indices.Count();
             int texture = mesh.Render.texture;

             Vector3 position = Vector3.Zero;

             IPosition pos = mesh as IPosition;
             if (pos != null)
                 position = pos.Position.PositionVec;

             Matrix4 trans = Matrix4.CreateTranslation(position);
             IPhysic phys = mesh as IPhysic;
             if (phys != null)
                 trans = phys.Physic.RigidBody.MotionState.WorldTransform;
            

             GL.UseProgram(shader.shaderProgramHandle);
             mesh.Render.Shader.SetProjectionMatrix(CameraSystem.projectionMatrix);
             mesh.Render.Shader.SetModelviewMatrix( trans * CameraSystem.viewMatrix);

             if (GL.GetError() != ErrorCode.NoError)
                 throw new Exception();
             
             GL.ActiveTexture(TextureUnit.Texture0);
             GL.BindTexture(TextureTarget.Texture2D, texture);
             mesh.Render.Shader.BindTexture(TextureUnit.Texture0, "text");

             GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
             GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
             GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)(TextureWrapMode.Repeat));
             GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)(TextureWrapMode.Repeat));


             DrawHelper.DrawMesh( VBuffer, IBuffer, Indexcount);

             GL.BindTexture(TextureTarget.Texture2D, 0);
         }

         public static void TakeScreenshot()
         {
             int width = GameOptions.GetVariable("w_width");
             int height = GameOptions.GetVariable("w_height");
             Bitmap bitmap = DrawHelper.GrabScreenshot(width, height);
             var files = Directory.EnumerateFiles("Screenshots").ToList();
             string regex = "Screenshots\\\\screenshot[0-9]{1,}\\.png$";
             var search = files.Where(o => Regex.IsMatch(o, regex)).OrderBy(o => int.Parse(o.Substring(o.LastIndexOf("t") + 1).Substring(0, o .LastIndexOf('.') - o.LastIndexOf("t") - 1))).ToList();

             int num = 0;
             if (search.Count > 0)
             {
                 string lastName = search.Last();
                 lastName = lastName.Substring(0, lastName.LastIndexOf('.'));

                 string numString = lastName.Substring(lastName.LastIndexOf("t") + 1);
                 num = int.Parse(numString) + 1;
             }
             
             string name = "Screenshots\\screenshot" + num + ".png";
             bitmap.Save(name);
         }
     }
}
