#region --- MIT License ---
/* Licensed under the MIT/X11 license.
 * Copyright (c) 2011 mjt[matola@sci.fi]
 * This notice may not be removed from any source distribution.
 * See license.txt for licensing details.
 */
#endregion
using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace CSatEng
{
    public class Billboard
    {
        Texture2D billBoard;

        public static Billboard Load(string fileName)
        {
            Billboard bb = new Billboard();
            bb.billBoard = Texture2D.Load(fileName);
            return bb;
        }

        public void Bind(int texUnit)
        {
            billBoard.Bind(texUnit);
        }

        public void BillboardBegin(float x, float y, float z, float zrot, float size)
        {
            int i, j;
            billBoard.Bind(0);
            GL.PushAttrib(AttribMask.ColorBufferBit | AttribMask.EnableBit | AttribMask.PolygonBit);
            GL.Disable(EnableCap.CullFace);
            GL.Disable(EnableCap.Lighting);
            GL.PushMatrix();
            GL.Translate(x, y, z);
            float[] modelMatrix = new float[16];
            GL.GetFloat(GetPName.ModelviewMatrix, modelMatrix);
            for (i = 0; i < 3; i++)
            {
                for (j = 0; j < 3; j++)
                {
                    if (i == j) modelMatrix[i * 4 + j] = 1;
                    else modelMatrix[i * 4 + j] = 0;
                }
            }
            GL.LoadMatrix(modelMatrix);
            size *= 0.01f;
            GL.Scale(size, size, size);
            GL.Rotate(zrot, 0, 0, 1);
        }

        /// <summary>
        /// lopeta billboardien renderointi. kutsuttava jos on kutsuttu BillboardBegin
        /// </summary>
        public void BillboardEnd()
        {
            GL.PopAttrib();
            GL.PopMatrix();
        }

        /// <summary>
        /// renderoi billboard. pit�� olla BillboardBegin ja BillboardEnd v�liss�.
        /// </summary>
        public void BillboardRender()
        {
            billBoard.Vbo.Render();
        }

        /// <summary>
        /// renderoi yhden billboardin.
        /// </summary>
        public void RenderBillboard(float x, float y, float z, float zrot, float size)
        {
            BillboardBegin(x, y, z, zrot, size);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.Enable(EnableCap.AlphaTest);
            GL.AlphaFunc(AlphaFunction.Greater, Texture2D.AlphaMin);
            billBoard.Vbo.Render();
            BillboardEnd();
        }
        public void RenderBillboard(Vector3 pos, float zrot, float size)
        {
            RenderBillboard(pos.X, pos.Y, pos.Z, zrot, size);
        }

        public void Dispose()
        {
            if (billBoard.Vbo != null)
            {
                billBoard.Vbo.Dispose();
                billBoard.Vbo = null;
                Log.WriteLine("Disposed: Billboard", true);
            }
        }
    }
}