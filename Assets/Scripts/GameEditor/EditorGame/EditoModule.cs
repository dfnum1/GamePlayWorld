/********************************************************************
生成日期:	25:7:2019   14:35
类    名: 	EditorModule
作    者:	HappLI
描    述:	编辑器主模块
*********************************************************************/
using ExternEngine;
using Framework.Core;
using Framework.Cutscene.Runtime;
using Framework.Plugin.AT;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace TopGame.ED
{
    class EditorGame : IGame
    {
        ScriptableObject[] m_Datas = null;
        public EditorGame()
        {
        }
        public Coroutine BeginCoroutine(IEnumerator coroutine)
        {
            return null;
        }
        public void EndAllCoroutine()
        {
        }
        public void EndCoroutine(Coroutine cortuine)
        {
        }
        public void EndCoroutine(IEnumerator cortuine)
        {
        }

        public AudioManager GetAudioMgr()
        {
            return null;
        }

        public CameraSetting GetCameraSetting()
        {
            return null;
        }

        public ScriptableObject[] GetDatas()
        {
            return m_Datas;
        }

        public EFileSystemType GetFileStreamType()
        {
            return EFileSystemType.AssetData;
        }
        public int GetMaxThread()
        {
            return 4;
        }

        public Transform GetTransform()
        {
            return null;
        }

        public UISystem GetUISystem()
        {
            return null;
        }

        public RenderPipelineAsset GetURPAsset()
        {
            return null;
        }

        public bool IsEditor()
        {
            return true;
        }
    }
    //------------------------------------------------------
    [DisiableExport]
    public class EditorCharactor : Character
    {
        public EditorCharactor(AFramework pGame) : base(pGame)
        {
        }
    }
    //------------------------------------------------------
    [Framework.ED.EditorGameModule]
    public class EditorModule : AFramework
    {
        Dictionary<int, System.Type> m_vEventTypes = new Dictionary<int, System.Type>();
        //------------------------------------------------------
        EditorModule()
        {
            EditorGame game = new EditorGame();
            Init(game);
            Awake();
            Start();
        }
        //------------------------------------------------------
        ~EditorModule()
        {

        }
        //------------------------------------------------------
        protected override void OnAwake()
        {
            if (UnityEngine.Rendering.GraphicsSettings.defaultRenderPipeline == null)
                UnityEngine.Rendering.GraphicsSettings.defaultRenderPipeline = AssetDatabase.LoadAssetAtPath<UnityEngine.Rendering.RenderPipelineAsset>("Assets/DatasRef/Config/RenderURP/Default/UniversalRenderPipelineAsset.asset");

       //     if (Data.DataManager.getInstance() == null)
       //         AddModule<Data.DataManager>();

            TerrainManager.EnableTerrainLowerLimit(this, true);
            TerrainManager.SetTerrainLowerLimit(this, 0);
        }
        //------------------------------------------------------
        protected override void OnDestroy()
        {
        }
        //------------------------------------------------------
        protected override void OnInit()
        {
       //     AddModule<DataManager>();
        }
        //------------------------------------------------------
        protected override void OnStart()
        {
        }
        //------------------------------------------------------
        protected override void OnUpdate(float fTime)
        {
        }
        //------------------------------------------------------
        public override AWorldNode OnExcudeWorldNodeMalloc(EActorType type)
        {
            if (type == EActorType.Character) return new EditorCharactor(this);
            else if (type == EActorType.Projectile) return new ProjectileNode(this);
            return new Actor(this);
        }
        //-----------------------------------------------------
        public override void OnWorldNodeStatus(AWorldNode pNode, EWorldNodeStatus status, IUserData userVariable = null)
        {
            if (status == EWorldNodeStatus.Killed)
            {
                return;
            }
            if (status != EWorldNodeStatus.Create) return;

            IUserData userData = null;
            bool bAysnc = true;
            bool bRefreshOp = false;
            if (userVariable != null && userVariable is VariableMultiData)
            {
                VariableMultiData mutiData = (VariableMultiData)userVariable;
                Variable1 flag = (Variable1)mutiData.pData1;
                bAysnc = flag.byteVal0 != 0;
                bRefreshOp = flag.byteVal1 != 0;
                userData = mutiData.pData2;
            }
            else userData = userVariable;

            IUserData pActorData = pNode.GetContextData();
            string strActionGraph = "";
#if !USE_SERVER
            Transform byParentTransform = null;
#endif
            byte[] attriTypes = null;
            int[] attriValues = null;
            /*
            CsvData_Models.ModelsData pModelFiler = null;
            if (pNode is Actor)
            {
                Actor pActor = pNode as Actor;
#if !USE_SERVER
                byParentTransform = RootsHandler.FindActorRoot((int)pNode.GetActorType());
                if (byParentTransform == null)
                    byParentTransform = RootsHandler.ActorsRoot;
#endif
                CsvData_Monster.MonsterData pMonsterData = pActorData as CsvData_Monster.MonsterData;
                CsvData_Player.PlayerData pData = pActorData as CsvData_Player.PlayerData;
                if (pData != null)
                {
                    strActionGraph = pData.skillGraph;

                    //   model = pData.model;
                    //   if (model != null)
                    //       strModelFiler = model.strFile;
                    pModelFiler = pData.Models_model_data;
                    if (pData.Attribute_attrId_data != null)
                    {
                        attriTypes = pData.Attribute_attrId_data.attrTypes;
                        attriValues = pData.Attribute_attrId_data.attrValues;
                    }

                    var avatarDatas = pData.Avatar_avatars_data;
                    if (pNode is Character)
                    {
                        Character character = pNode as Character;
                        if (avatarDatas != null)
                        {
                            for (int i = 0; i < avatarDatas.Length; ++i)
                            {
                                CsvData_Avatar.AvatarData avatarData = avatarDatas[i];
                                if (avatarData != null)
                                {
                                    character.AddAvatar(avatarData.type, avatarData.avatar);
                                }
                            }
                        }
                    }
                }
                else if (pMonsterData != null)
                {
                    strActionGraph = pMonsterData.skillGraph;
                    pModelFiler = pMonsterData.Models_model_data;
                    if (pMonsterData.Attribute_attrId_data != null)
                    {
                        attriTypes = pMonsterData.Attribute_attrId_data.attrTypes;
                        attriValues = pMonsterData.Attribute_attrId_data.attrValues;
                    }
                }
                pNode.SetSpatial(true);
            }
            */
#if !USE_SERVER
            if (userData != null && userData is VariableObj)
            {
                AInstanceAble outInInstance = null;
                VariableObj pObj = (VariableObj)userData;
                if (pObj.pGO != null)
                {
                    outInInstance = pObj.pGO as AInstanceAble;
                }
                pNode.SetObjectAble(outInInstance);
            }
            else
            {
     //           if (pModelFiler == null)
                {
                    InstanceOperiaon prefabOp = null;
#if UNITY_EDITOR
                    prefabOp = new InstanceOperiaon();
                //    GameObject pDebugTest = new GameObject(pNode.GetInstanceID().ToString());
                    //  prefabOp.pPoolAble = pDebugTest.AddComponent<DebugEmptyInstanceAble>();
                    //   prefabOp.pPoolAble.SetParent(byParentTransform);
#endif
                    pNode.OnSpawnCallback(prefabOp);
                }
  //              else
                {
 /*
                    if (pModelFiler.model.EndsWith(".asset"))
                    {
                        bakerSkinManager.LoadSkinFrameData(pModelFiler.model, Framework.Plugin.ESkinType.GpuArray, pNode, pNode.OnCreateBakerSkin);
                    }
                    else
                    {
                        if (!pNode.IsObjected())
                        {
                            InstanceOperiaon prefabOp = FileSystemUtil.SpawnInstance(pModelFiler.model, bAysnc);
                            if (prefabOp != null)
                            {
                                pNode.SetInstanceOperiaon(prefabOp);
                                prefabOp.OnCallback = pNode.OnSpawnCallback;
                                prefabOp.OnSign = pNode.OnSpawnSign;
                                prefabOp.SetByParent(byParentTransform);
                                if (bRefreshOp)
                                {
                                    prefabOp.Refresh();
                                }
                            }
                            else
                            {
                                pNode.OnSpawnCallback(null);
                            }
                        }
                        else
                        {
                            AInstanceAble pAble = pNode.GetObjectAble();
                            if (pAble)
                            {
                                pAble.ClearMaterialBlock();
                                pAble.ResetLayer();
                            }
                        }
                    }
 */
                }
            }
#else
        pNode.OnSpawnCallback(null);
#endif
            if (pNode is Actor)
            {
                var pActor = (Actor)pNode;
                pActor.SetAttrs(attriTypes, attriValues);
                pActor.LoadActorGraph(strActionGraph);
            }
        }

        public override BaseEvent OnMallocEvent(int evntType)
        {
            if(m_vEventTypes.Count<=0)
            {
                foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
                {
                    System.Type[] types = assembly.GetTypes();
                    for (int i = 0; i < types.Length; ++i)
                    {
                        System.Type tp = types[i];
                        if (!tp.IsSubclassOf(typeof(BaseEvent)))
                            continue;

                        EventDeclarationAttribute attr = (EventDeclarationAttribute)tp.GetCustomAttribute(typeof(EventDeclarationAttribute));
                        if (attr == null)
                            continue;
                        m_vEventTypes[attr.eType] = tp;
                    }
                }
            }
            if(m_vEventTypes.TryGetValue(evntType, out var evnType))
            {
                return Activator.CreateInstance(evnType) as BaseEvent;
            }
            return null;
        }
    }
}
