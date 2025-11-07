/********************************************************************
生成日期:	6:15:2020 10:42
类    名: 	EmptyImage
作    者:	HappLi
描    述:	空图片不产生drawcall
*********************************************************************/

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Framework.UI
{
    [RequireComponent(typeof(CanvasRenderer))]
    public class EmptyImage : MaskableGraphic, ICanvasRaycastFilter
    {
        public UnityEngine.PolygonCollider2D polygonCollider2D;
        protected EmptyImage()
        {
            useLegacyMeshGeneration = false;
        }
        //------------------------------------------------------
        public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
        {
            if(polygonCollider2D!=null)
            {
                Vector2 local;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, sp, eventCamera, out local);
                return Contains(local, polygonCollider2D.points);
            }
            return true;
        }
        //------------------------------------------------------
        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            toFill.Clear();
        }
        //------------------------------------------------------
        private bool Contains(Vector2 p, Vector2[] outterVertices)
        {
            var crossNumber = 0;
            RayCrossing(p, outterVertices, ref crossNumber);
            return (crossNumber & 1) == 1;
        }
        //------------------------------------------------------
        private void RayCrossing(Vector2 p, Vector2[] vertices, ref int crossNumber)
        {
            for (int i = 0, count = vertices.Length; i < count; i++)
            {
                var v1 = vertices[i];
                var v2 = vertices[(i + 1) % count];

                if (((v1.y <= p.y) && (v2.y > p.y))
                    || ((v1.y > p.y) && (v2.y <= p.y)))
                {
                    if (p.x < v1.x + (p.y - v1.y) / (v2.y - v1.y) * (v2.x - v1.x))
                    {
                        crossNumber += 1;
                    }
                }
            }
        }
    }
}
