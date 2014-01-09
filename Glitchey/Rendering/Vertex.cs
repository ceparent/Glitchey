using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Runtime.InteropServices;

namespace Glitchey.Rendering
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct Vertex
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 TextCoord;
        public Vector2 LightmapCoord;
        public Vertex(Vector3 position, Vector3 normal, Vector2 textCoord, Vector2 lightmap)
        {
            Position = position;
            Normal = normal;
            TextCoord = textCoord;
            LightmapCoord = lightmap;
        }
        public const int Stride = 40;
    }
}
