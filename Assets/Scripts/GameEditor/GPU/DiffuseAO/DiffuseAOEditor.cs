using System;
using System.Collections.Generic;
using System.IO;
using TopGame.ED;
using UnityEditor;
using UnityEngine;

namespace Frameworks.PBRLab
{
	public class DiffuseAOEditor : ScriptableWizard
	{
		#region UI Param
		Texture DiffuseTex;
		Texture AOTex;
		Texture2D FinalTex;
		float AOPowerScale = 1;

		EditorGPUPass m_pGpuPass = new EditorGPUPass();
		float PreScale=1;

		bool IsAutoSettingTextureQualityToBest = false;

		#endregion

		[MenuItem("Tools/GPUResources/DiffuseAO", false, 10)]
		static void CreateWizard()
		{
			ScriptableWizard.DisplayWizard<DiffuseAOEditor>("DiffuseAO", "Save");
		}

		void CalcFinalTexture()
		{
			if(DiffuseTex == null || AOTex == null)
            {
				if(FinalTex!=null) DestroyImmediate(FinalTex);
				FinalTex = null;
				return;
            }
			if (FinalTex != null)
			{
				if (FinalTex.width != DiffuseTex.width || FinalTex.height != DiffuseTex.height)
				{
					DestroyImmediate(FinalTex);
					FinalTex = new Texture2D(DiffuseTex.width, DiffuseTex.height, TextureFormat.ARGB32, false);
				}
			}
			else
                FinalTex = new Texture2D(DiffuseTex.width, DiffuseTex.height, TextureFormat.ARGB32, false);

			m_pGpuPass.CreateMaterialFromShaderPath("Hidden/Editor_DiffuseAO");
			m_pGpuPass.DrawingMesh = EditorGPUPass.CreateScreenAlignedQuadMesh();

			m_pGpuPass.bindMat.SetFloat("_AOStrength", AOPowerScale);
			m_pGpuPass.bindMat.SetTexture("_MainTex",	DiffuseTex);
			m_pGpuPass.bindMat.SetTexture("_AOMap",		AOTex);

			m_pGpuPass.Rendering(ref FinalTex);

        }

		void OnSaveLightTex()
		{
			if (FinalTex == null || DiffuseTex == null) return;
			string path = AssetDatabase.GetAssetPath(DiffuseTex);
			string strRoot = System.IO.Path.GetDirectoryName(path).Replace("\\", "/");
			path = strRoot + "/" + DiffuseTex.name + "_D_AO.png";
			byte[] bytes = FinalTex.EncodeToPNG();		

			File.WriteAllBytes(path, bytes);

			AssetDatabase.Refresh();

			string assetPath = null;

			if (path.StartsWith(Application.dataPath))
			{
				assetPath = string.Format("Assets{0}", path.Substring(Application.dataPath.Length, path.Length - Application.dataPath.Length));
			}

			if (string.IsNullOrEmpty(assetPath) && IsAutoSettingTextureQualityToBest)
				return;

			TextureImporter diffTexImport = TextureImporter.GetAtPath(AssetDatabase.GetAssetPath(DiffuseTex)) as TextureImporter;


			TextureImporter textureImporter = TextureImporter.GetAtPath(assetPath) as TextureImporter;
			textureImporter.textureType = TextureImporterType.Default;

			
			textureImporter.mipmapEnabled = diffTexImport.mipmapEnabled;
			textureImporter.sRGBTexture = diffTexImport.sRGBTexture;
			textureImporter.filterMode = diffTexImport.filterMode;
			textureImporter.wrapMode = diffTexImport.wrapMode;

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

			AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);
		}

		protected override bool DrawWizardGUI()
		{
            float label = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = position.width / 2;
			EditorGUILayout.BeginVertical();
			EditorGUILayout.Space();

			IsAutoSettingTextureQualityToBest = EditorGUILayout.Toggle("是否设置将导出贴图设置为最佳质量：", IsAutoSettingTextureQualityToBest);


			DiffuseTex	= (Texture)EditorGUILayout.ObjectField("Diffuse贴图:", DiffuseTex, typeof(Texture), false);
			AOTex	= (Texture)EditorGUILayout.ObjectField("AO:", AOTex, typeof(Texture), false);
			AOPowerScale = EditorGUILayout.Slider("AO強度:", AOPowerScale, 0.0f, 5.0f);

			if (GUILayout.Button("贴图预览"))
			{
				CalcFinalTexture();
			}

			if (FinalTex != null)
			{
				PreScale = EditorGUILayout.Slider("预览贴图放缩比例", PreScale, 0.1f, 10f);
				var rect = EditorGUILayout.GetControlRect(true, FinalTex.height * PreScale, EditorStyles.layerMaskField);
				rect.width = FinalTex.width * PreScale;
				EditorGUI.DrawPreviewTexture(rect, FinalTex);
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
			if (FinalTex != null)
			{
				DestroyImmediate(FinalTex);
			}
			if (m_pGpuPass != null) m_pGpuPass.Destroy();
		}
	}
}
