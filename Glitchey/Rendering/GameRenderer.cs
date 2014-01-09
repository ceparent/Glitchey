using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;

using Glitchey.Entities;
using Glitchey.Levels;

namespace Glitchey.Rendering
{
     static class GameRenderer
     {
         public static void SetupViewportMenu()
         {
             GL.ClearColor(Color.Black);

             GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);

             GL.Enable(EnableCap.Texture2D);
             GL.Enable(EnableCap.Blend);
             GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
             GL.Disable(EnableCap.CullFace);
             GL.Disable(EnableCap.DepthTest);

             GL.Viewport(0, 0, GameOptions.window_width, GameOptions.window_height);

             float nearClip = 0f;
             float farClip = 1000.0f;

             GL.MatrixMode(MatrixMode.Projection);
             GL.LoadIdentity();

             GL.Ortho(0, GameOptions.viewport_width , GameOptions.viewport_height, 0, nearClip, farClip);
         }

         public static void SetupViewportGame()
         {

             GL.Enable(EnableCap.TextureCoordArray);
             GL.Enable(EnableCap.NormalArray);

             GL.Enable(EnableCap.CullFace);
             GL.Enable(EnableCap.DepthTest);
             GL.CullFace(CullFaceMode.Front);

         }



         public static void RenderWorld(World world)
         {
             if (world == null)
                 throw new InvalidOperationException("world is null");

             if (BspRenderer.BspFile != world.Level.BspFile)
                 BspRenderer.BspFile = world.Level.BspFile;

             BspRenderer.Draw();

         }
     }
}
