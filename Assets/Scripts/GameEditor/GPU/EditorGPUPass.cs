using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TopGame.ED
{
	public class EditorGPUPass
	{
		public Mesh DrawingMesh = null;

		public static Mesh CreateScreenAlignedQuadMesh()
		{
			Mesh mesh = new Mesh();

			mesh.vertices = new Vector3[] { Vector3.zero, Vector3.right, Vector3.up, new Vector3(1, 1, 0) };
			mesh.uv = new Vector2[] { Vector2.zero, Vector2.right, Vector2.up, Vector2.one };
			mesh.SetIndices(new int[] { 0, 2, 1, 1, 2, 3 }, MeshTopology.Triangles, 0);
			mesh.RecalculateBounds();

			return mesh;
		}
		public Material		bindMat		= null;
		public int			passIndex	= 0;

		public void CreateMaterialFromShaderPath(string shaderPath)
		{
			var shader = Shader.Find(shaderPath);
			if (shader != null)
			{
				bindMat = new Material(shader);
				Resources.UnloadUnusedAssets();
			}
		}

		public void Rendering(ref Texture2D finalTex, bool autoGenerateMips = false)
        {
            var tempRenderTexture = RenderTexture.GetTemporary(finalTex.width, finalTex.height, 0, RenderTextureFormat.ARGB32);
            tempRenderTexture.autoGenerateMips = autoGenerateMips;

            Rendering(tempRenderTexture);

			finalTex.ReadPixels(new Rect(0, 0, finalTex.width, finalTex.height), 0, 0);
			finalTex.Apply();

            Graphics.SetRenderTarget(null);

            tempRenderTexture.Release();
        }

        public void Rendering(RenderTexture renderTarget)
		{
			if (renderTarget == null)
			{
				Debug.LogError("EditorGPUPass Rendering Failed! render target is null");
				return;
			}

			if (bindMat == null)
			{
				Debug.LogError("EditorGPUPass Rendering Failed! material is null");
				return;
			}

			Graphics.SetRenderTarget(renderTarget);

			if (DrawingMesh == null)
			{
				Graphics.Blit( bindMat.mainTexture, bindMat, passIndex);
			}
			else
			{
				bindMat.SetPass(passIndex);
				Graphics.DrawMeshNow( DrawingMesh, Matrix4x4.identity);
			}
		}

		public delegate void OnFinishDrawFunction(RenderTexture renderTarget);

		public void RenderingOnce(RenderTexture renderTarget, OnFinishDrawFunction onFinishDrawFunction)
		{
			if (renderTarget == null)
			{
				Debug.LogError("EditorGPUPass Rendering Failed! render target is null");
				return;
			}

			if (bindMat == null)
			{
				Debug.LogError("EditorGPUPass Rendering Failed! material is null");
				return;
			}

			Graphics.SetRenderTarget(renderTarget);

			if (DrawingMesh == null)
			{
				Graphics.Blit(bindMat.mainTexture, bindMat, passIndex);
			}
			else
			{
				bindMat.SetPass(passIndex);
				Graphics.DrawMeshNow(DrawingMesh, Matrix4x4.identity);
			}

			onFinishDrawFunction?.Invoke(renderTarget);

			Graphics.SetRenderTarget(null);
		}
		//------------------------------------------------------
		public void Destroy()
        {
			if(bindMat)
            {
				GameObject.DestroyImmediate(bindMat);
				bindMat = null;
			}
            if (DrawingMesh)
            {
                GameObject.DestroyImmediate(DrawingMesh);
				DrawingMesh = null;
            }
        }
	}
}