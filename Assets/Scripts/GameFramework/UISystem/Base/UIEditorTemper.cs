#if UNITY_EDITOR
/********************************************************************
生成日期:	25:7:2019   14:35
类    名: 	UIEditorTemper
作    者:	HappLI
描    述:	UI 编辑器临时数据，预制体中不存在该挂件
*********************************************************************/
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.UI
{
#if UNITY_EDITOR
    [ExecuteInEditMode, DisallowMultipleComponent]
#endif
    public class UIEditorTemper : MonoBehaviour
    {
#if UNITY_EDITOR
        public bool isFullUI = false;
        public string SaveToPrefab = "";
		public string ReferToPic = "";
        public string strAuthor = "";
        public string Desc = "";
        public bool trackAble = true;
        public bool permanent;
        public bool alwayShow;
        public int uiOrder = 0;
        [Framework.ED.DisplayDrawType("Framework.UI.EUIType")]
        public int eUIType = 0;

        [Header("选择打开场景时自动选择得UI")]
        public Transform SelectTarget;
        void Start()
        {
            PingObject();
            SceneViewToUICenter();
            Debug.Log("Start");
        }
        //------------------------------------------------------
        void PingObject()
        {
            Transform target = SelectTarget ? SelectTarget : transform;
            Selection.activeObject = target;
            EditorGUIUtility.PingObject(target);
        }
        //------------------------------------------------------
        void SceneViewToUICenter()
        {
            Transform target = SelectTarget ? SelectTarget : transform;
            SceneView.lastActiveSceneView.AlignViewToObject(target);
            //SceneView.lastActiveSceneView.FrameSelected();

            SceneView.lastActiveSceneView.in2DMode = true;
        }
        //------------------------------------------------------
        public void ResetData()
        {
            GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
            RectTransform trans = (transform as RectTransform);
            trans.sizeDelta = new Vector2(720, 1280);
            trans.anchorMin = Vector2.zero;
            trans.anchorMax = Vector2.one;
            trans.offsetMax = Vector3.zero;
            trans.offsetMin = Vector3.zero;

            gameObject.layer = LayerMask.NameToLayer("UI");
            UnityEditor.Selection.activeObject = gameObject;
        }
        //------------------------------------------------------
        public void LoadReferToPic(GameObject pReferTo)
        {
            if (pReferTo == null)
            {
                GameObject root = GameObject.Find("UIRoot");
                if (root == null) return;
                Transform referTo = root.transform.Find("Canvas/referTo");
                if (referTo)
                {
                    pReferTo = referTo.gameObject;
                }
                else
                {
                    UnityEditor.EditorUtility.DisplayDialog("加载参考图失败", "找不到 referTo,请确保 referTo 存在 UIRoot/Canvas 底下", "确定");
                }
            }

            RectTransform trans = pReferTo.transform as RectTransform;
            Image pImage = pReferTo.GetComponent<Image>();
            if (pImage == null)
                pImage = pReferTo.AddComponent<Image>();
            if (!string.IsNullOrEmpty(ReferToPic))
                pImage.sprite = LoadSpriteInLocal(ReferToPic);
            else
                pImage.sprite = null;
            pReferTo.SetActive(pImage.sprite != null);
        }
        //------------------------------------------------------
        Sprite LoadSpriteInLocal(string file_path)
        {
            if (!File.Exists(file_path))
            {
                Debug.Log("LoadSpriteInLocal() cannot find sprite file : " + file_path);
                return null;
            }
            Texture2D texture = LoadTextureInLocal(file_path);
            //创建Sprite
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            return sprite;
        }
        //------------------------------------------------------
        Texture2D LoadTextureInLocal(string file_path)
        {
            FileStream fileStream = new FileStream(file_path, FileMode.Open, FileAccess.Read);
            fileStream.Seek(0, SeekOrigin.Begin);
            //创建文件长度缓冲区
            byte[] bytes = new byte[fileStream.Length];
            //读取文件
            fileStream.Read(bytes, 0, (int)fileStream.Length);
            //释放文件读取流
            fileStream.Close();
            fileStream.Dispose();
            fileStream = null;

            //创建Texture
            int width = 300;
            int height = 372;
            Texture2D texture = new Texture2D(width, height);
            texture.LoadImage(bytes);
            return texture;
        }
#endif
    }
}
#endif