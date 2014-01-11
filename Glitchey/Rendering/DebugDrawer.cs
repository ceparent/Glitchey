using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BulletSharp;
using OpenTK;
using OpenTK.Graphics.OpenGL;

using Glitchey.Systems;

namespace Glitchey.Rendering
{
    class DebugDrawer: IDebugDraw
    {
        private DebugDrawModes _mode;
        public DebugDrawModes DebugMode
        {
            get
            {
                return _mode;
            }
            set
            {
                _mode = value;
            }
        }

        public void Draw3dText(ref Vector3 location, string textString)
        {

        }

        public void DrawAabb(ref Vector3 from, ref Vector3 to, OpenTK.Graphics.Color4 color)
        {
        }

        public void DrawArc(ref Vector3 center, ref Vector3 normal, ref Vector3 axis, float radiusA, float radiusB, float minAngle, float maxAngle, OpenTK.Graphics.Color4 color, bool drawSect, float stepDegrees)
        {
        }

        public void DrawArc(ref Vector3 center, ref Vector3 normal, ref Vector3 axis, float radiusA, float radiusB, float minAngle, float maxAngle, OpenTK.Graphics.Color4 color, bool drawSect)
        {
        }

        public void DrawBox(ref Vector3 bbMin, ref Vector3 bbMax, ref Matrix4 trans, OpenTK.Graphics.Color4 color)
        {
        }

        public void DrawBox(ref Vector3 bbMin, ref Vector3 bbMax, OpenTK.Graphics.Color4 color)
        {

        }

        public void DrawCapsule(float radius, float halfHeight, int upAxis, ref Matrix4 transform, OpenTK.Graphics.Color4 color)
        {

        }

        public void DrawCone(float radius, float height, int upAxis, ref Matrix4 transform, OpenTK.Graphics.Color4 color)
        {

        }

        public void DrawContactPoint(ref Vector3 pointOnB, ref Vector3 normalOnB, float distance, int lifeTime, OpenTK.Graphics.Color4 color)
        {

        }

        public void DrawCylinder(float radius, float halfHeight, int upAxis, ref Matrix4 transform, OpenTK.Graphics.Color4 color)
        {

        }

        public List<Vector3> LineVertices;
        public void DrawLine(ref Vector3 from, ref Vector3 to, OpenTK.Graphics.Color4 color)
        {
            LineVertices.Add(from);
            LineVertices.Add(to);
        }

        public void DrawLine(ref Vector3 from, ref Vector3 to, OpenTK.Graphics.Color4 fromColor, OpenTK.Graphics.Color4 toColor)
        {
        }

        public void DrawPlane(ref Vector3 planeNormal, float planeConst, ref Matrix4 transform, OpenTK.Graphics.Color4 color)
        {
        }

        public void DrawSphere(ref Vector3 p, float radius, OpenTK.Graphics.Color4 color)
        {
        }

        public void DrawSphere(float radius, ref Matrix4 transform, OpenTK.Graphics.Color4 color)
        {
        }

        public void DrawSpherePatch(ref Vector3 center, ref Vector3 up, ref Vector3 axis, float radius, float minTh, float maxTh, float minPs, float maxPs, OpenTK.Graphics.Color4 color, float stepDegrees, bool drawSphere)
        {
        }

        public void DrawSpherePatch(ref Vector3 center, ref Vector3 up, ref Vector3 axis, float radius, float minTh, float maxTh, float minPs, float maxPs, OpenTK.Graphics.Color4 color, float stepDegrees)
        {
        }

        public void DrawSpherePatch(ref Vector3 center, ref Vector3 up, ref Vector3 axis, float radius, float minTh, float maxTh, float minPs, float maxPs, OpenTK.Graphics.Color4 color)
        {
        }

        public void DrawTransform(ref Matrix4 transform, float orthoLen)
        {
        }

        public void DrawTriangle(ref Vector3 v0, ref Vector3 v1, ref Vector3 v2, OpenTK.Graphics.Color4 color, float __unnamed004)
        {
        }

        public void DrawTriangle(ref Vector3 v0, ref Vector3 v1, ref Vector3 v2, ref Vector3 __unnamed003, ref Vector3 __unnamed004, ref Vector3 __unnamed005, OpenTK.Graphics.Color4 color, float alpha)
        {
        }

        public void ReportErrorWarning(string warningString)
        {

        }
    }
}
