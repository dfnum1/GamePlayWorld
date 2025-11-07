using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using Framework.ED;

namespace TopGame.ED
{
    public class NormalAOBuilder : EditorWindow
	{
		class NoramlMaskGroup
		{
			public Texture2D normal;
			//public Texture2D roughness;
			public Texture2D ao;
			public string genSingleFileName;
		}

		bool IsMipmap = false;
        EditorFolderPath SaveFolderPath = new EditorFolderPath();

        EditorFilePath SaveArrayPath = new EditorFilePath();

        Vector2 scrollView = Vector2.zero;

		static bool IsInited = false;

		static List<NoramlMaskGroup> textureInArray = new List<NoramlMaskGroup>();

		ReorderableList m_TextureReorderableList;

		readonly static char[] sDirectorySplits = new char[] { '/', '\\' };

		[MenuItem("Tools/GPUResources/NormalAOPathcher", false, 10)]
		static void GeneratorTexture2DArray()
		{
			var objects = Selection.objects;

			textureInArray.Clear();

			for (int i = 0; i < objects.Length; ++i)
			{
				string path = AssetDatabase.GetAssetPath(objects[i]);
				var importer = AssetImporter.GetAtPath(path) as TextureImporter;
				if (importer == null)
					continue;

				if (importer.textureType != TextureImporterType.NormalMap)
					continue;

				NoramlMaskGroup group = new NoramlMaskGroup();
				group.normal = objects[i] as Texture2D;

				int extHeadIndex = path.LastIndexOf(".");

				string fileHeadPart = path.Substring(0, extHeadIndex);

				int lastUnderLineIndex = fileHeadPart.LastIndexOf('_');
				if (lastUnderLineIndex != -1)
				{
					int lastDirectorySplit = fileHeadPart.LastIndexOfAny(sDirectorySplits);

					if (lastDirectorySplit < lastUnderLineIndex)
					{
						fileHeadPart = fileHeadPart.Substring(0, lastUnderLineIndex);
					}
				}

				string aoPath = string.Format("{0}_ao{1}", fileHeadPart, path.Substring(extHeadIndex, path.Length - extHeadIndex));

				group.genSingleFileName = string.Format("{0}_NormalAO{1}", System.IO.Path.GetFileName(fileHeadPart), path.Substring(extHeadIndex, path.Length - extHeadIndex));

				if (System.IO.File.Exists(EditorFilePath.AssetPathToAbsotionPath(aoPath)))
				{
					group.ao = AssetDatabase.LoadAssetAtPath<Texture2D>(aoPath);
				}

				textureInArray.Add(group);
			}

			IsInited = false;

			Open("Texture2DArrayBuilder", new Vector2(800, 800));

			if (Instance != null)
			{
				Instance.Init();
			}
		}

        public static NormalAOBuilder Instance;

        public static void Open(string title, Vector2 size)
        {
            Instance = EditorWindow.GetWindow<NormalAOBuilder>();
            Instance.Show();

            if (Instance.position.x < 0 || Instance.position.y < 0
                || Instance.position.x > 1920 || Instance.position.y > 1080)
            {
                Instance.position = new Rect(0, 0, size.x, size.y);
            }

            //Instance.ReadPathFile();
            Instance.titleContent.text = title;
            Instance.minSize = size;
        }

        void Init()
		{
			if (IsInited)
				return;

			IsInited = true;

			BuildTextureReorderableList();

			if (textureInArray.Count < 1)
				return;

			AutoSelectMipmap();
		}

		void AutoSelectMipmap()
		{
			if (textureInArray.Count < 1)
				return;

			string textureAssetPath = AssetDatabase.GetAssetPath(textureInArray[0].normal);

			if (string.IsNullOrEmpty(textureAssetPath))
				return;

			var importer = UnityEditor.AssetImporter.GetAtPath(textureAssetPath) as UnityEditor.TextureImporter;
			if (importer == null)
				return;

			IsMipmap = importer.mipmapEnabled;
		}

		void BuildTextureReorderableList()
		{
			m_TextureReorderableList = new ReorderableList(textureInArray, typeof(NoramlMaskGroup), true, true, true, true);

			m_TextureReorderableList.drawHeaderCallback = (Rect rect) =>
			{
				EditorGUI.LabelField(rect, "Textures");
			};

			m_TextureReorderableList.elementHeight *= 6;

			m_TextureReorderableList.onAddCallback = AddTexture;
			m_TextureReorderableList.drawElementCallback = DrawTextureArrayElement;
		}

		void AddTexture(ReorderableList list)
		{
			textureInArray.Add(new NoramlMaskGroup());
		}

		void DrawTextureArrayElement(Rect rect, int index, bool selected, bool focused)
		{
			Vector2 offset = new Vector2(0, rect.height / 3.0f);
			Rect curRt = new Rect(rect.min.x, rect.min.y, rect.width, offset.y);

			textureInArray[index].normal = (Texture2D)EditorGUI.ObjectField(curRt, "normal", textureInArray[index].normal, typeof(Texture2D), false);

			curRt.position += offset;
			textureInArray[index].ao = (Texture2D)EditorGUI.ObjectField(curRt, "ao", textureInArray[index].ao, typeof(Texture2D), false);
		}

		void OnEnable()
		{
			Init();
		}

		void OnGUI()
		{
			IsMipmap = EditorGUILayout.Toggle("是否生成Mipmap", IsMipmap);

			SaveFolderPath.OnGUI("存储路径:");

			if (GUILayout.Button("批次生成(RG:法线_B:AO)贴图"))
			{
				CreateTextures();
			}

			scrollView = EditorGUILayout.BeginScrollView(scrollView);
			//int newSelectIndex = -1;
			if (m_TextureReorderableList != null)
			{
				m_TextureReorderableList.DoLayoutList();

				//newSelectIndex = m_TextureReorderableList.index;
			}



			EditorGUILayout.EndScrollView();
		}

		bool IsCheckCanPackageArray()
		{
			if (textureInArray == null || textureInArray.Count == 0)
				return false;

			if (textureInArray[0] == null)
				return false;

			int width = textureInArray[0].normal.width;
			int height = textureInArray[0].normal.height;

			bool isCanPackage = true;

			for (int i = 1; i < textureInArray.Count; ++i)
			{
				var group = textureInArray[i];

				if (group == null)
					return false;

				if (group.normal.width != width
					|| group.normal.height != height)
				{
					Debug.LogErrorFormat("texture index({0}): resolution is not equal the first texture", i);
					isCanPackage &= false;
				}
			}

			if (!SaveArrayPath.isAssetPath)
			{
				Debug.LogErrorFormat("ArraySavePath({0}) is not asset path.", SaveArrayPath);
				isCanPackage &= false;
			}

			return isCanPackage;
		}

		void CreateTextures()
		{
			Texture2D tempTex2D = null;

			for (int i = 0; i < textureInArray.Count; ++i)
			{
				var group = textureInArray[i];
				if (string.IsNullOrEmpty(group.genSingleFileName))
				{
					string path = AssetDatabase.GetAssetPath(group.normal);
					if (string.IsNullOrEmpty(path))
					{
						group.genSingleFileName = "normal" + i;
					}
					else
					{
						group.genSingleFileName = System.IO.Path.GetFileName(path);
					}
				}

				TextureConvertLogic.ConvertUnityNormalToNormalRG(group.normal, group.ao, IsMipmap, ref tempTex2D);

				string savePath = string.Format("{0}/{1}", SaveFolderPath.absotionPath, group.genSingleFileName);

				savePath = savePath.Substring(0, savePath.LastIndexOf('.')) + ".png";
				System.IO.File.WriteAllBytes(savePath, tempTex2D.EncodeToPNG());
			}

			AssetDatabase.Refresh();
		}
	}
}
