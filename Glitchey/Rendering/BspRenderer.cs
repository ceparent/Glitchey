﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;

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
                Vector2 lightmapcoord = new Vector2(v.TexCoord[0, 0],  v.TexCoord[0, 1]);
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
        private static void LoadIndexBuffer()
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

        private static int[] textureLengths;
        private static uint[] Indices;
        private static void DetermineVisibleFaces()
        {

            int leafIndex = findLeaf();
            int clusterIndex = BspFile.Leaves[leafIndex].Cluster;

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
            List<face> visibleFaces = new List<face>();
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

            foreach (leafface l in leafFaces)
            {
                face f = BspFile.Faces[l.Face];
                visibleFaces.Add(f);
            }



            List<uint>[] Texturearrays = new List<uint>[BspFile.Textures.Count()];
            for (int i = 0; i < BspFile.Textures.Count(); i++)
            {
                Texturearrays[i] = new List<uint>();
            }

            //arrays
            foreach (face f in visibleFaces)
            {
                if (f.Type == 1 || f.Type == 3)
                {
                    //Meshes and faces
                    for (int i = f.Meshvert; i < f.Meshvert + f.N_Meshverts; i++)
                    {
                        //index
                        uint index = Convert.ToUInt32(f.Vertex + BspFile.MeshVerts[i].Offset);
                        Texturearrays[f.Texture].Add(index);
                    }

                }


            }

            List<uint> realList = new List<uint>();
            textureLengths = new int[BspFile.Textures.Count()];
            int cpt = 0;
            for (int i = 0; i < Texturearrays.Length; i++)
            {
                realList.AddRange(Texturearrays[i]);
                cpt += Texturearrays[i].Count;
                textureLengths[i] = cpt;
            }


            Indices = realList.ToArray();

            GL.GenBuffers(1, out IBuffer);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, IBuffer);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(Indices.Length * sizeof(int)), Indices, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

        }

        private static int BeziersVertices;
        private static int BeziersIndex;
        private static uint[] BezierIndices;
        private static int[] BezierstextureLengths;
        private static void CreateBeziersWithTesselation()
        {
            List<int> bezierTextureIds = new List<int>();
            List<vertex[]> controlList = new List<vertex[]>();
            List<int> textureIds = new List<int>();
            foreach (face f in _bspFile.Faces)
            {
                if (f.Type == 2)
                {
                    int width = (f.Size[0] - 1) / 2;
                    int height = (f.Size[1] - 1) / 2;

                    bezierTextureIds.Add(f.Texture);

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
                            controlList.Add(controls);
                            textureIds.Add(f.Texture);
                        }
                    }


                }

            }


            List<vertex> bspVertices;

            bspVertices = new List<vertex>();



            List<int>[] Texturearrays = new List<int>[_bspFile.Textures.Count()];
            for (int i = 0; i < _bspFile.Textures.Count(); i++)
            {
                Texturearrays[i] = new List<int>();
            }

            TesselationOffset = 0;
            for (int i = 0; i < controlList.Count; i++)
            {
                Texturearrays[textureIds[i]].AddRange(TesselateOnePatch(bspVertices, controlList[i]));
            }


            List<int> realList = new List<int>();
            BezierstextureLengths = new int[_bspFile.Textures.Count()];
            int cpt3 = 0;
            for (int i = 0; i < Texturearrays.Length; i++)
            {
                realList.AddRange(Texturearrays[i]);
                cpt3 += Texturearrays[i].Count;
                BezierstextureLengths[i] = cpt3;
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
            if (bspVertices.Count > 0)
            {

                Vertex[] vertices = verticesList.ToArray();
                GL.GenBuffers(1, out BeziersVertices);
                GL.BindBuffer(BufferTarget.ArrayBuffer, BeziersVertices);
                GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertices.Length * Vertex.Stride), vertices, BufferUsageHint.StaticDraw);
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);


                BezierIndices = new uint[realList.Count];
                int cpt = 0;
                foreach (int i in realList)
                {
                    BezierIndices[cpt] = (uint)i;
                    cpt++;
                }
                GL.GenBuffers(1, out BeziersIndex);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, BeziersIndex);
                GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(BezierIndices.Length * sizeof(uint)), BezierIndices, BufferUsageHint.StaticDraw);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            }
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
        private static List<int> TesselateOnePatch(List<vertex> pVerticesList, vertex[] pControls)
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
                    vertices[y + x * Length].TexCoord = new float[,] { { p0.TexCoord[0, 0] + (float)a, p0.TexCoord[0, 0] + (float)c }, { p0.TexCoord[0, 1] + (float)a, p0.TexCoord[0, 1] + (float)c } };

                }
            }

            List<int> indices = new List<int>();
            int offset = Length * Length;

            for (int row = 0; row < Length - 1; row++)
            {
                for (int col = 0; col < Length - 1; col++)
                {
                    // 0, 0
                    indices.Add(col + (Length * row) + (TesselationOffset * offset));
                    // 1, 1
                    indices.Add((col + 1) + (Length * (row + 1)) + (TesselationOffset * offset));
                    // 1, 0
                    indices.Add((col + 1) + (Length * row) + (TesselationOffset * offset));


                    // 0, 0
                    indices.Add(col + (Length * row) + (TesselationOffset * offset));
                    // 0, 1
                    indices.Add(col + (Length * (row + 1)) + (TesselationOffset * offset));
                    // 1, 1
                    indices.Add((col + 1) + (Length * (row + 1)) + (TesselationOffset * offset));


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

            LoadIndexBuffer();

            GL.Color3(1.0f, 1.0f, 1.0f);
            GL.EnableClientState(ArrayCap.VertexArray);
            // Base

            GL.BindBuffer(BufferTarget.ArrayBuffer, Vbuffer);

            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Vertex.Stride, 0);

            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, Vertex.Stride, 3 * sizeof(float));

            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, Vertex.Stride, 6 * sizeof(float));


            GL.BindBuffer(BufferTarget.ElementArrayBuffer, IBuffer);
            int lastOFfset = 0;
            for (int i = 0; i < textureLengths.Count(); i++)
            {
                if ((textureLengths[i] - lastOFfset) > 0)
                {

                    GL.ActiveTexture(TextureUnit.Texture0);
                    string textureName = BspFile.Textures[i].Name + ".jpg";                    
                    GL.BindTexture(TextureTarget.Texture2D, Content.LoadTexture(textureName));

                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)(TextureWrapMode.Repeat));
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)(TextureWrapMode.Repeat));

                    
                    
                    GL.DrawRangeElements(PrimitiveType.Triangles, lastOFfset, textureLengths[i], textureLengths[i] - lastOFfset, DrawElementsType.UnsignedInt, new IntPtr(lastOFfset * sizeof(uint)));

                }
                lastOFfset = textureLengths[i];
            }

            
            // Bezier curves
            GL.BindBuffer(BufferTarget.ArrayBuffer, BeziersVertices);

            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Vertex.Stride, 0);

            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, Vertex.Stride, 3 * sizeof(float));

            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, Vertex.Stride, 6 * sizeof(float));


            GL.BindBuffer(BufferTarget.ElementArrayBuffer, BeziersIndex);
            lastOFfset = 0;
            for (int i = 0; i < BezierstextureLengths.Count(); i++)
            {
                GL.ActiveTexture(TextureUnit.Texture0);
                string textureName = BspFile.Textures[i].Name + ".jpg";
                GL.BindTexture(TextureTarget.Texture2D, Content.LoadTexture(textureName));
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)(TextureWrapMode.Repeat));
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)(TextureWrapMode.Repeat));

                GL.DrawRangeElements(PrimitiveType.Triangles, lastOFfset, BezierstextureLengths[i], BezierstextureLengths[i] - lastOFfset, DrawElementsType.UnsignedInt, new IntPtr(lastOFfset * 4));
                lastOFfset = BezierstextureLengths[i];
            }

             
            GL.BindTexture(TextureTarget.Texture2D, 0);


            // Clear buffers
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

            GL.DisableClientState(ArrayCap.VertexArray);

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