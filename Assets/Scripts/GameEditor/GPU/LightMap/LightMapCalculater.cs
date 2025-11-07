using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Frameworks.PBRLab
{
	public class LightMapCalculater : ScriptableWizard
	{
		public enum Format
		{
			png,
			exr,			
		}

		#region UI Param

		Format outputTextureFormat = Format.png;

		Texture DiffuseTex;
		Texture FinalTex;

		Vector2Int LightTextureSize = Vector2Int.one * 256;

		float LightPowerScale = 1.0f;

		float PreScale = 1.0f;

		Texture2D LightTex;
		Material LightMapMat;

		string lightMapFolderPath;

		string lightMapFilePath;

		bool IsAutoSettingTextureQualityToBest = false;

		#endregion

		[MenuItem("Tools/GPUResources/LightMapCalculater", false, 10)]
		static void CreateWizard()
		{
			ScriptableWizard.DisplayWizard<LightMapCalculater>("LightMapCalculater", "Create");
		}

		void RefresLightTexture()
		{
			if (LightTex != null)
			{
				if (LightTex.width != LightTextureSize.x || LightTex.height != LightTextureSize.y)
				{
					DestroyImmediate(LightTex);
					LightTex = new Texture2D(LightTextureSize.x, LightTextureSize.y, outputTextureFormat == Format.png ? TextureFormat.ARGB32 : TextureFormat.RGBAHalf, false);
				}
			}
			else
				LightTex = new Texture2D(LightTextureSize.x, LightTextureSize.y, outputTextureFormat == Format.png ? TextureFormat.ARGB32 : TextureFormat.RGBAHalf, false);

			if (LightMapMat == null)
			{
				var shader = Shader.Find("Hidden/SD_LightMapBaker");
				LightMapMat = new Material(shader);
			}

			var pass0Target = RenderTexture.GetTemporary(LightTextureSize.x, LightTextureSize.y, 0, RenderTextureFormat.ARGB32);

			LightMapMat.SetFloat("_LightPowerScale", LightPowerScale);
			LightMapMat.SetTexture("_DiffuseTex",	DiffuseTex);
			LightMapMat.SetTexture("_FinalTex",		FinalTex);

			LightMapMat.SetPass(0);

			Graphics.SetRenderTarget(pass0Target);

			Mesh mesh = new Mesh();

			mesh.vertices = new Vector3[] { Vector3.zero, Vector3.right, Vector3.up, new Vector3(1, 1, 0) };
			mesh.uv = new Vector2[] { Vector2.zero, Vector2.right, Vector2.up, Vector2.one };
			mesh.SetIndices(new int[] { 0, 2, 1, 1, 2, 3 }, MeshTopology.Triangles, 0);
			mesh.RecalculateBounds();

			Graphics.DrawMeshNow(mesh, Matrix4x4.identity);

			LightTex.ReadPixels(new Rect(0, 0, LightTextureSize.x, LightTextureSize.y), 0, 0);
			LightTex.Apply();

			RenderTexture.ReleaseTemporary(pass0Target);

			DestroyImmediate(mesh);
		}

		void OnSaveLightTex()
		{
			if (LightTex == null)
			{
				RefresLightTexture();
			}

			if (string.IsNullOrEmpty( lightMapFilePath))
			{
				Debug.LogErrorFormat("lightMapFilePath is null!");
				return;
			}

			byte[] bytes = outputTextureFormat == Format.png ? LightTex.EncodeToPNG() : LightTex.EncodeToEXR();		

			File.WriteAllBytes(lightMapFilePath, bytes);

			AssetDatabase.Refresh();

			string assetPath = null;

			if (lightMapFilePath != "" && lightMapFilePath.StartsWith(Application.dataPath))
			{
				assetPath = string.Format("Assets{0}", lightMapFilePath.Substring(Application.dataPath.Length, lightMapFilePath.Length - Application.dataPath.Length));
			}

			if (string.IsNullOrEmpty(assetPath) && IsAutoSettingTextureQualityToBest)
				return;

			// not set import settings for the texture
			// It needs to be clamped and it shouldn't be compressed.
			TextureImporter textureImporter = TextureImporter.GetAtPath(assetPath) as TextureImporter;
			//textureImporter.textureFormat = TextureImporterFormat.ARGB32;
			textureImporter.textureType = TextureImporterType.Default;
			//textureImporter.textureFormat = TextureImporterFormat.AutomaticTruecolor;
			//textureImporter.textureCompression = TextureImporterCompression.Uncompressed;
			
			textureImporter.mipmapEnabled = false;
			textureImporter.sRGBTexture = true;
			textureImporter.filterMode = FilterMode.Bilinear;
			textureImporter.wrapMode = TextureWrapMode.Clamp;


			TextureImporterPlatformSettings importerSettings_Andorid = new TextureImporterPlatformSettings();
			importerSettings_Andorid.overridden = true;
			importerSettings_Andorid.name = "Android";
			importerSettings_Andorid.compressionQuality = 100;
			importerSettings_Andorid.textureCompression = TextureImporterCompression.CompressedHQ;
			//importerSettings_Andorid.maxTextureSize = maxSize;
			

			TextureImporterPlatformSettings importerSettings_IOS = new TextureImporterPlatformSettings();
			importerSettings_IOS.overridden = true;
			importerSettings_IOS.name = "iPhone";
			importerSettings_IOS.textureCompression = TextureImporterCompression.CompressedHQ;
			importerSettings_IOS.compressionQuality = 100;
			//importerSettings_IOS.maxTextureSize = maxSize;

			textureImporter.SetPlatformTextureSettings(importerSettings_Andorid);
			textureImporter.SetPlatformTextureSettings(importerSettings_IOS);
			textureImporter.SaveAndReimport();

			//textureImporter.maxTextureSize = Mathf.Max(LUTTextureSize.x, LUTTextureSize.y);
			AssetDatabase.ImportAsset( lightMapFilePath, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);
		}

		protected override bool DrawWizardGUI()
		{
            float label = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = position.width / 2;
			EditorGUILayout.BeginVertical();
			EditorGUILayout.Space();

			IsAutoSettingTextureQualityToBest = EditorGUILayout.Toggle("是否设置将导出贴图设置为最佳质量：", IsAutoSettingTextureQualityToBest);

			var newFormat = (Format)EditorGUILayout.EnumPopup("导出贴图格式:", outputTextureFormat);
			if (outputTextureFormat != newFormat)
			{
				outputTextureFormat = newFormat;
				if (LightTex != null)
					DestroyImmediate(LightTex);
			}

			if (!string.IsNullOrEmpty(lightMapFilePath))
			{
				if (!lightMapFilePath.EndsWith(outputTextureFormat.ToString()))
				{
					lightMapFilePath = lightMapFilePath.Remove(lightMapFilePath.Length - 3) + outputTextureFormat.ToString();
				}
			}

			DiffuseTex	= (Texture)EditorGUILayout.ObjectField("Diffuse贴图:", DiffuseTex, typeof(Texture), false);
			FinalTex	= (Texture)EditorGUILayout.ObjectField("最终贴图:", FinalTex, typeof(Texture), false);

			LightTextureSize = EditorGUILayout.Vector2IntField("光照贴图分辨率:", LightTextureSize);

			LightPowerScale = EditorGUILayout.Slider("亮度调节:", LightPowerScale, 0.0f, 5.0f);

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Light Map路径：", GUILayout.Width(100));
			lightMapFilePath = EditorGUILayout.TextField(lightMapFilePath);
			if (GUILayout.Button("选择LightMap路径", GUILayout.Width(120)))
			{
				var path = EditorUtility.OpenFolderPanel("选择LightMap路径", "", lightMapFolderPath ?? "");
				if (!string .IsNullOrEmpty(path))
				{
					lightMapFolderPath = path;

					lightMapFilePath = string.Format("{0}/{1}", lightMapFolderPath, outputTextureFormat == Format.png ? "lightMap.png" : "lightMap.exr");
				}
			}
			EditorGUILayout.EndHorizontal();

			if (GUILayout.Button("贴图预览"))
			{
				RefresLightTexture();
			}

			if (LightTex != null)
			{
				PreScale = EditorGUILayout.Slider("预览贴图放缩比例", PreScale, 0.1f, 10f);
				var rect = EditorGUILayout.GetControlRect(true, LightTex.height * PreScale, EditorStyles.layerMaskField);
				rect.width = LightTex.width * PreScale;
				EditorGUI.DrawPreviewTexture(rect, LightTex);
			}

			EditorGUILayout.Space();
			EditorGUILayout.EndVertical();

            EditorGUIUtility.labelWidth = label;

            return base.DrawWizardGUI();
		}

		void OnWizardCreate()
		{
			OnSaveLightTex();
		}

		private void OnDestroy()
		{
			if (LightTex != null)
			{
				DestroyImmediate(LightTex);
			}

			if (LightMapMat != null)
				DestroyImmediate(LightMapMat);

		}
	}
}
