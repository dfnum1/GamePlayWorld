/********************************************************************
生成日期:	24:7:2019   11:12
类    名: 	EditorInitalizeOnLoad
作    者:	HappLI
描    述:	unity 编辑器加载启动回调
*********************************************************************/
#if UNITY_EDITOR
using Framework.ED;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace TopGame.ED
{
    [InitializeOnLoad]
    public class EditorInitalizeOnLoad
    {
        static EditorInitalizeOnLoad()
        {
            SetMacros();
            SetOrientation();

            EditorApplication.playModeStateChanged += OnPlayModeStateChange;
            UnityEditor.SceneManagement.EditorSceneManager.sceneSaving += SceneSavingCallback;
            UnityEditor.SceneManagement.EditorSceneManager.sceneClosing += SceneClosingCallback;
            InspectorDrawUtil.CheckInspector();
#if USE_URP
            RenderPipelineAsset urpAsset= AssetDatabase.LoadAssetAtPath<RenderPipelineAsset>("Assets/DatasRef/Config/RenderURP/Default/UniversalRenderPipelineAsset.asset");
            if(QualitySettings.renderPipeline == null)
                QualitySettings.renderPipeline = urpAsset;

            if (UnityEngine.Rendering.GraphicsSettings.defaultRenderPipeline == null)
                UnityEngine.Rendering.GraphicsSettings.defaultRenderPipeline = urpAsset;
#endif

            //!code editor
       //     MenuTools.AutoCode();
        }
        //------------------------------------------------------
        static void OnPlayModeStateChange(PlayModeStateChange state)
        {
            if(state == PlayModeStateChange.ExitingPlayMode)
            {
                EditorUtil.StopAllAudioClips();
#if USE_URP
                UnityEngine.Rendering.GraphicsSettings.defaultRenderPipeline = AssetDatabase.LoadAssetAtPath<RenderPipelineAsset>("Assets/DatasRef/Config/RenderURP/Default/UniversalRenderPipelineAsset.asset");
#endif
            }
            else if(state == PlayModeStateChange.ExitingPlayMode)
            {
                if (Framework.Core.AFramework.mainFramework!=null && Framework.Core.AFramework.isStartup)
                    Framework.Core.AFramework.mainFramework.Destroy();
            }
        }
        //------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void RuntimeInit()
        {

        }
        //------------------------------------------------------
        static void SceneSavingCallback(UnityEngine.SceneManagement.Scene scene, string path)
        {

        }
        //------------------------------------------------------
        static void SceneClosingCallback(UnityEngine.SceneManagement.Scene scene, bool removingScene)
        {
        }
        //------------------------------------------------------
        static void SetMacros()
        {
            AutoMarcros.SetMacros(null);
        }
        //------------------------------------------------------
        static void SetOrientation()
        {
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.AutoRotation;
            PlayerSettings.allowedAutorotateToPortrait = false;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
            PlayerSettings.allowedAutorotateToLandscapeRight = true;
            PlayerSettings.allowedAutorotateToLandscapeLeft = true;
        }
    }
}
#endif
