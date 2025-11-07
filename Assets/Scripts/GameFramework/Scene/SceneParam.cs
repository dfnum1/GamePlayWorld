/********************************************************************
生成日期:	1:11:2020 13:07
类    名: 	SceneParam
作    者:	HappLI
描    述:	场景加载参数
*********************************************************************/
using UnityEngine;

namespace Framework.Core
{
    public enum ESceneSignType : byte
    {
        Add,
        Pop,
        PopAll,
    }
    public struct SceneParam
    {
        public int sceneID;
        public string sceneFile;
        public string sceneName;
        public string subScene;
        public bool isCompled;

        public ESceneSignType load;
        public ESceneSignType unload;

        public void Clear()
        {
            sceneID = -1;
            sceneFile = null;
            sceneName = null;
            subScene = null;
            isCompled = false;

            load = ESceneSignType.PopAll;
            unload = ESceneSignType.PopAll;
        }
        public bool IsValid()
        {
            return sceneID >= 0;
        }
        public static SceneParam DEF = new SceneParam() { sceneFile = null, sceneName = null, sceneID = -1, isCompled = false, load = ESceneSignType.PopAll, unload = ESceneSignType.PopAll };
    }
    public interface ISceneCallback
    {
        void OnSceneCallback(SceneParam param);
    }
}