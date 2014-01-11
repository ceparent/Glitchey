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
using Glitchey.Systems;

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

             GL.Viewport(0, 0, GameOptions.GetVariable("w_width"), GameOptions.GetVariable("w_height"));

             float nearClip = 0f;
             float farClip = 1000.0f;

             GL.MatrixMode(MatrixMode.Projection);
             GL.LoadIdentity();

             GL.Ortho(0, GameOptions.GetVariable("vp_width") , GameOptions.GetVariable("vp_height"), 0, nearClip, farClip);
         }

         public static void SetupViewportGame()
         {

             GL.Enable(EnableCap.TextureCoordArray);
             GL.Enable(EnableCap.NormalArray);

             GL.Enable(EnableCap.CullFace);
             GL.Enable(EnableCap.DepthTest);
             GL.CullFace(CullFaceMode.Front);

         }



         public static void RenderWorld(GameWorld world)
         {
             if (world == null)
                 throw new InvalidOperationException("world is null");

             if (BspRenderer.BspFile != world.Level.BspFile)
                 BspRenderer.BspFile = world.Level.BspFile;

             BspRenderer.Draw();

         }

         public static void RenderDebugLines(Vector3[] vertices)
         {
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
     }
}
