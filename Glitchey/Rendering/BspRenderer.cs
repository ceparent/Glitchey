using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;
using System.Drawing;

using Glitchey.Levels;
using Glitchey.Systems;
using Glitchey.Rendering.Shaders;

namespace Glitchey.Rendering
{
    static class BspRenderer
    {
        private static BspShader _shader;
        static BspRenderer()
        {
            _shader = new BspShader();
        }

        private static BspFile _bspFile;
        public static BspFile BspFile
        {
            get { return _bspFile; }
            set 
            {
                _bspFile = value;
                LoadMap();
            }
        }

        private static void LoadMap()
        {
            _tesselation = GameOptions.patch_tesselation;
            CreateBeziersWithTesselation();
            LoadVBuffer();
            LoadIndexBuffer();
            GenerateLightmapTextures();
        }

        private static int[] _lightmaps;
        private static void GenerateLightmapTextures()
        {
            _lightmaps = new int[_bspFile.Lightmaps.Count()];
            int cpt = 0;
            foreach (lightmap lm in _bspFile.Lightmaps)
            {
                int lmId = GL.GenTexture();

                GL.BindTexture(TextureTarget.Texture2D, lmId);

                GL.TexImage2D<byte>(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, lm.Map.GetLength(0), lm.Map.GetLength(1), 0, PixelFormat.Rgb, PixelType.UnsignedByte, lm.Map);

                _lightmaps[cpt] = lmId;

                cpt++;
            }


        }

        private static int Vbuffer;
        private static void LoadVBuffer()
        {
            Vertex[] vertices = new Vertex[_bspFile.Vertices.Count()];

            int cpt = 0;
            foreach (vertex v in _bspFile.Vertices)
            {
                Vector3 position = V3FromFloatArray(v.Position);
                Vector2 texcoord = new Vector2(v.TexCoord[0, 0], v.TexCoord[0, 1]);
                Vector2 lightmapcoord = new Vector2(v.TexCoord[1, 0], v.TexCoord[1, 1]);
                Vector3 normal = V3FromFloatArray(v.Normal);

                vertices[cpt] =  new Vertex(position, normal, texcoord, lightmapcoord);
                cpt++;
            }

            GL.GenBuffers(1, out Vbuffer);
            GL.BindBuffer(BufferTarget.ArrayBuffer, Vbuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertices.Length * Vertex.Stride), vertices, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        private static int IBuffer;
        public static void LoadIndexBuffer()
        {
            DetermineVisibleFaces();
        }

        private static int findLeaf()
        {
            int index = 0;

            while (index >= 0)
            {
                node node = BspFile.Nodes[index];
                plane plane = BspFile.Planes[node.Plane];

                //dist
                double distance = Vector3.Dot(V3FromFloatArray(plane.Normal), Swizzle(CameraSystem._camera.Position.PositionVec)) - plane.Dist;

                if (distance >= 0)
                    index = node.Children[0];
                else
                    index = node.Children[node.Children.Count() - 1];

            }

            return -index - 1;

        }

        private static uint[] Indices;
        private static List<face> _goodVisibleFaces;
        private static int _lastCluster = int.MinValue;
        private static void DetermineVisibleFaces()
        {

            int leafIndex = findLeaf();
            int clusterIndex = BspFile.Leaves[leafIndex].Cluster;

            // Optimisation only update if we changed cluster
            if (_lastCluster == clusterIndex)
                return;
            else
                _lastCluster = clusterIndex;

            //Visible leaves
            List<leaf> visibleLeaves = new List<leaf>();
            foreach (leaf l in BspFile.Leaves)
            {
                //if(isLeafVisible(clusterIndex,l.Cluster))
                visibleLeaves.Add(l);
            }

            //visible faces
            HashSet<int> usedIndices = new HashSet<int>();
            List<leafface> leafFaces = new List<leafface>();
            foreach (leaf l in visibleLeaves)
            {
                for (int i = l.LeafFace; i < l.LeafFace + l.N_LeafFaces; i++)
                {
                    if (!usedIndices.Contains(i))
                    {
                        leafface f = BspFile.LeafFaces[i];
                        leafFaces.Add(f);
                        usedIndices.Add(i);

                    }

                }

            }

            //face
            List<face> visibleFaces = new List<face>();
            foreach (leafface l in leafFaces)
            {
                face f = BspFile.Faces[l.Face];
                visibleFaces.Add(f);
            }


            // Sort by texture
            visibleFaces = visibleFaces.OrderBy(o => o.Texture).ToList();

            List<uint> indiceList = new List<uint>();
            _goodVisibleFaces = new List<face>();
            foreach (face f in visibleFaces)
            {
                if (f.Type == 1 || f.Type == 3)
                {
                    _goodVisibleFaces.Add(f);
                    //Meshes and faces
                    for (int i = f.Meshvert; i < f.Meshvert + f.N_Meshverts; i++)
                    {
                        //index
                        uint index = Convert.ToUInt32(f.Vertex + BspFile.MeshVerts[i].Offset);
                        indiceList.Add(index);
                        
                    }

                }


            }



            Indices = indiceList.ToArray();
            GL.GenBuffers(1, out IBuffer);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, IBuffer);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(Indices.Length * sizeof(int)), Indices, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

        }

        private static int BeziersVertices;
        private static int BeziersIndex;
        private static uint[] BezierIndices;
        private static List<BezierInfos> _beziers;
        private static void CreateBeziersWithTesselation()
        {
            List<int> textureIds = new List<int>();
            _beziers = new List<BezierInfos>();

            foreach (face f in _bspFile.Faces)
            {
                if (f.Type == 2)
                {
                    int width = (f.Size[0] - 1) / 2;
                    int height = (f.Size[1] - 1) / 2;

                    vertex[,] patch = new vertex[f.Size[0], f.Size[1]];

                    int cpt = f.Vertex;
                    for (int y = 0; y < f.Size[1]; y++)
                    {
                        for (int x = 0; x < f.Size[0]; x++)
                        {
                            patch[x, y] = _bspFile.Vertices[cpt];
                            cpt++;
                        }
                    }

                    BezierInfos info = new BezierInfos(f);

                    for (int x = 0; x < width; x++)
                    {
                        for (int y = 0; y < height; y++)
                        {
                            int i = 2 * x;
                            int j = 2 * y;
                            vertex[] controls = new vertex[3 * 3];
                            for (int u = 0; u < 3; u++)
                            {
                                for (int v = 0; v < 3; v++)
                                {
                                    vertex vert = patch[u + i, v + j];
                                    controls[u * 3 + v] = vert;

                                }
                            }
                            info.Controls.Add(controls);
                            textureIds.Add(f.Texture);
                        }
                    }

                    _beziers.Add(info);
                }

            }


            List<vertex> bspVertices = new List<vertex>();
            // Reset TessalationOffset from last time for TesselateOnePatch()
            TesselationOffset = 0;
            List<uint> realList = new List<uint>();
            int indexBezier = 0;
            foreach (BezierInfos Bezier in _beziers)
            {
                Bezier.Start = indexBezier;
                for (int i = 0; i < Bezier.Controls.Count; i++)
                {
                    List<uint> indices = TesselateOnePatch(ref bspVertices, Bezier.Controls[i]);
                    realList.AddRange(indices);
                    indexBezier += indices.Count;
                }
                Bezier.End = indexBezier;
            }



            List<Vertex> verticesList = new List<Vertex>();
            int cpt2 = 0;
            foreach (vertex vert in bspVertices)
            {
                Vector3 normal = V3FromFloatArray(vert.Normal);
                normal.Normalize();

                verticesList.Add(new Vertex(V3FromFloatArray(vert.Position), normal, new Vector2(vert.TexCoord[0, 0], vert.TexCoord[0, 1]), new Vector2(vert.TexCoord[1, 0], vert.TexCoord[1, 1])));
                cpt2++;
            }


            Vertex[] vertices = verticesList.ToArray();
            GL.GenBuffers(1, out BeziersVertices);
            GL.BindBuffer(BufferTarget.ArrayBuffer, BeziersVertices);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertices.Length * Vertex.Stride), vertices, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            BezierIndices = new uint[realList.Count];
            int cpt4 = 0;
            foreach (uint i in realList)
            {
                BezierIndices[cpt4] = i;
                cpt4++;
            }

            GL.GenBuffers(1, out BeziersIndex);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, BeziersIndex);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(BezierIndices.Length * sizeof(uint)), BezierIndices, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            
        }

        private static int _tesselation;
        public static int Tesselation
        {
            get { return _tesselation; }
            set
            {
                _tesselation = value;
                if (_tesselation < 1)
                    _tesselation = 1;
                if (_tesselation > 10)
                    _tesselation = 10;

                CreateBeziersWithTesselation();
            }
        }
        private static int TesselationOffset = 0;
        private static List<uint> TesselateOnePatch(ref List<vertex> pVerticesList, vertex[] pControls)
        {
            int Length = Tesselation + 1;

            vertex[,] newControls = new vertex[3, Length];

            for (int i = 0; i < 3; i++)
            {

                vertex p0 = pControls[i * 3];
                vertex p1 = pControls[(i * 3) + 1];
                vertex p2 = pControls[(i * 3) + 2];

                for (int l = 0; l < Length; l++)
                {
                    double a = l / (double)Tesselation;
                    double b = 1 - a;

                    newControls[i, l] = p0 * b * b + p1 * 2 * b * a + p2 * a * a;
                }
            }

            vertex[] vertices = new vertex[Length * Length];

            for (int x = 0; x < Length; x++)
            {
                vertex p0 = newControls[0, x];
                vertex p1 = newControls[1, x];
                vertex p2 = newControls[2, x];

                double c = x / (double)Tesselation; // texcoord

                for (int y = 0; y < Length; y++)
                {

                    double a = y / (double)Tesselation;
                    double b = 1 - a;

                    //2nd pass
                    vertices[y + x * Length] = p0 * b * b + p1 * 2 * b * a + p2 * a * a;
                    /*
                    vertices[y + x * Length].TexCoord = new float[,] 
                    { 
                        { p0.TexCoord[0, 0] + (float)a, p0.TexCoord[0, 0] + (float)c },
                        { p0.TexCoord[1, 1] + (float)a, p0.TexCoord[1, 1] + (float)c }
                    };*/

                }
            }

            List<uint> indices = new List<uint>();
            int offset = Length * Length;

            for (int row = 0; row < Length - 1; row++)
            {
                for (int col = 0; col < Length - 1; col++)
                {
                    // 0, 0
                    indices.Add((uint)(col + (Length * row) + (TesselationOffset * offset)));
                    // 1, 1
                    indices.Add((uint)((col + 1) + (Length * (row + 1)) + (TesselationOffset * offset)));
                    // 1, 0
                    indices.Add((uint)((col + 1) + (Length * row) + (TesselationOffset * offset)));


                    // 0, 0
                    indices.Add((uint)(col + (Length * row) + (TesselationOffset * offset)));
                    // 0, 1
                    indices.Add((uint)(col + (Length * (row + 1)) + (TesselationOffset * offset)));
                    // 1, 1
                    indices.Add((uint)((col + 1) + (Length * (row + 1)) + (TesselationOffset * offset)));


                }
            }



            TesselationOffset++;
            pVerticesList.AddRange(vertices);
            return indices;

        }

        public static void Draw()
        {
            GL.PushMatrix();
            
            GL.UseProgram(_shader.shaderProgramHandle);
            _shader.SetProjectionMatrix(CameraSystem.projectionMatrix);
            _shader.SetModelviewMatrix(CameraSystem.viewMatrix * Matrix4.Identity);

            GL.Color3(1.0f, 1.0f, 1.0f);
            GL.EnableClientState(ArrayCap.VertexArray);
            // Base

            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
            GL.EnableVertexAttribArray(2);
            GL.EnableVertexAttribArray(3);


            GL.BindBuffer(BufferTarget.ArrayBuffer, Vbuffer);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Vertex.Stride, 0);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, Vertex.Stride, 3 * sizeof(float));
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, Vertex.Stride, 6 * sizeof(float));
            GL.VertexAttribPointer(3, 2, VertexAttribPointerType.Float, false, Vertex.Stride, 8 * sizeof(float));
            
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, IBuffer);
            
            int cptVertex = 0;
            int lastTexture = -1;
            int lastLm = -1;
            foreach (face f in _goodVisibleFaces)
            {
                string textureName = BspFile.Textures[f.Texture].Name + ".jpg";
                int texture = Content.LoadTexture(textureName);
                if (texture != lastTexture)
                {
                    GL.ActiveTexture(TextureUnit.Texture0);
                    GL.BindTexture(TextureTarget.Texture2D, texture);
                    _shader.BindTexture(TextureUnit.Texture0, "text");

                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)(TextureWrapMode.Repeat));
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)(TextureWrapMode.Repeat));
                    lastTexture = texture;
                }

                if (f.Lm_index > 0 && f.Lm_index != lastLm)
                {
                    GL.ActiveTexture(TextureUnit.Texture1);
                    GL.BindTexture(TextureTarget.Texture2D, _lightmaps[f.Lm_index]);
                    _shader.BindTexture(TextureUnit.Texture1, "lm");

                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)(TextureWrapMode.Repeat));
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)(TextureWrapMode.Repeat));
                }

                GL.DrawRangeElements(PrimitiveType.Triangles, cptVertex, cptVertex + f.N_Meshverts, f.N_Meshverts, DrawElementsType.UnsignedInt, new IntPtr(cptVertex * sizeof(uint)));

                cptVertex += f.N_Meshverts;
            }

            
            // Bezier curves
            GL.BindBuffer(BufferTarget.ArrayBuffer, BeziersVertices);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Vertex.Stride, 0);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, Vertex.Stride, 3 * sizeof(float));
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, Vertex.Stride, 6 * sizeof(float));
            GL.VertexAttribPointer(3, 2, VertexAttribPointerType.Float, false, Vertex.Stride, 8 * sizeof(float));

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, BeziersIndex);
            cptVertex = 0;
            foreach (BezierInfos bezier in _beziers)
            {
                face f = bezier.Face;

                string textureName = BspFile.Textures[f.Texture].Name + ".jpg";
                int texture = Content.LoadTexture(textureName);
                if (texture != lastTexture)
                {
                    GL.ActiveTexture(TextureUnit.Texture0);
                    GL.BindTexture(TextureTarget.Texture2D, texture);
                    _shader.BindTexture(TextureUnit.Texture0, "text");

                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)(TextureWrapMode.Repeat));
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)(TextureWrapMode.Repeat));
                    lastTexture = texture;
                }

                if (f.Lm_index > 0 && f.Lm_index != lastLm)
                {
                    GL.ActiveTexture(TextureUnit.Texture1);
                    GL.BindTexture(TextureTarget.Texture2D, _lightmaps[f.Lm_index]);
                    _shader.BindTexture(TextureUnit.Texture1, "lm");

                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)(TextureWrapMode.Repeat));
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)(TextureWrapMode.Repeat));
                }

                GL.DrawRangeElements(PrimitiveType.Triangles, bezier.Start, bezier.End, bezier.End - bezier.Start, DrawElementsType.UnsignedInt, new IntPtr(cptVertex * sizeof(uint)));

                cptVertex += bezier.End - bezier.Start;
            }
            
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            GL.DisableVertexAttribArray(0);
            GL.DisableVertexAttribArray(1);
            GL.DisableVertexAttribArray(2);
            GL.DisableVertexAttribArray(3);


            GL.DisableClientState(ArrayCap.VertexArray);

            // Clear buffers
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            
            
            GL.PopMatrix();
        }

        private static Vector3 V3FromFloatArray(float[] array)
        {
            if (array.Count() != 3)
                throw new InvalidOperationException();

            return new Vector3(array[0], array[2], -array[1]);
        }

        private static Vector3 Swizzle(Vector3 pVector)
        {
            float x = pVector.X;
            float y = -pVector.Z;
            float z = pVector.Y;
            return new Vector3(x, y, z);
        }
    }
}
