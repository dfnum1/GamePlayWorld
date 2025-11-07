#if UNITY_EDITOR
/********************************************************************
生成日期:	11:06:2023
类    名: 	EditorWindowBase
作    者:	HappLI
描    述:	基础编辑器窗口,所有将编辑都继承于他
*********************************************************************/
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine.UIElements;

namespace Framework.ED
{
    public abstract class EditorWindowBase : EditorWindow
    {
        private bool                m_bStarted = false;
        private bool                m_bRuntimingOpened = false;
        protected EditorTimer       m_pTimer = new EditorTimer();

        protected System.Object       m_pCurrentObj = null; // 当前选中的对象
        private List<AEditorLogic> m_vLogics = new List<AEditorLogic>();
        private List<ICustomVisualElement> m_vGraphViewLogics = new List<ICustomVisualElement>();
        private Dictionary<System.Type, AEditorLogic> m_vLogicKV = new Dictionary<System.Type, AEditorLogic>();
        private Dictionary<System.Type, ICustomVisualElement> m_vGraphViewLogicKV = new Dictionary<System.Type, ICustomVisualElement>();
        private Framework.Core.AFramework m_pEditorGame = null;
        //--------------------------------------------------------
        public List<AEditorLogic> GetLogics()
        {
            return m_vLogics;
        }
        //--------------------------------------------------------
        public System.Object GetCurrentObj()
        {
            return m_pCurrentObj;
        }
        //--------------------------------------------------------
        public void OnEnable()
        {
            EditorWindowMgr.RegisterWindow(this);
            ScanerRegisterLogics();

            SceneView.duringSceneGui += OnSceneView;

            OnInnerEnable();

            for (int i = 0; i < m_vLogics.Count; ++i)
                m_vLogics[i].Enable();

            for (int i = 0; i < m_vGraphViewLogics.Count; ++i)
            {
                m_vGraphViewLogics[i].Enable();
            }
            m_bStarted = false;
        }
        //--------------------------------------------------------
        private void OnDestroy()
        {
            for (int i = 0; i < m_vLogics.Count; ++i)
                m_vLogics[i].Destroy();

            for (int i = 0; i < m_vGraphViewLogics.Count; ++i)
            {
                m_vGraphViewLogics[i].Destroy();
            }
            OnInnerDestroy();
        }
        //--------------------------------------------------------
        protected virtual void OnInnerDestroy()
        {

        }
        //--------------------------------------------------------
        public virtual void OnSceneView(SceneView sceneView)
        {
            for (int i = 0; i < m_vLogics.Count; ++i)
                m_vLogics[i].OnSceneView(sceneView);
        }
        //--------------------------------------------------------
        protected virtual void OnStart()
        {
        }
        //--------------------------------------------------------
        public Framework.Core.AFramework GetEditorGame()
        {
            if(m_pEditorGame == null)
            {
                m_pEditorGame = ED.EditorUtil.BuildEditorInstnace();
            }
            return m_pEditorGame;
        }
        //--------------------------------------------------------
        public void CreateGUI()
        {
            OnCreateGUI();
            VisualElement root = rootVisualElement;
            RegisterGraphView(root);
        }
        //--------------------------------------------------------
        protected virtual void OnCreateGUI() { }
        //--------------------------------------------------------
        public void OnDisable()
        {
            EditorWindowMgr.UnRegisterWindow(this);
            SceneView.duringSceneGui -= OnSceneView;

            OnInnerDisable();
            for (int i =0; i < m_vLogics.Count; ++i)
                m_vLogics[i].Disable();

            for (int i = 0; i < m_vGraphViewLogics.Count; ++i)
                m_vGraphViewLogics[i].Disable();

            if (IsManaged() && !m_bRuntimingOpened)
            {
            }
            m_bRuntimingOpened = false;

            if (m_pEditorGame != null) m_pEditorGame.Destroy();
            m_pEditorGame = null;
			m_pCurrentObj = null;
        }
        //--------------------------------------------------------
        protected virtual void OnInnerEnable() { }
        protected virtual void OnInnerDisable() { }
        //--------------------------------------------------------
        void Update()
        {
            if(!m_bStarted)
            {
                m_bStarted = true;
                OnStart();
                return;
            }
            m_pTimer.Update();
            OnInnerUpdate();
            for (int i = 0; i < m_vLogics.Count; ++i)
                m_vLogics[i].Update(m_pTimer.deltaTime);

            for (int i = 0; i < m_vGraphViewLogics.Count; ++i)
                m_vGraphViewLogics[i].Update(m_pTimer.deltaTime);

            if (m_pEditorGame != null) m_pEditorGame.Update(m_pTimer.deltaTime);
            this.Repaint();
        }
        //--------------------------------------------------------
        protected virtual void OnInnerUpdate() { }
        //--------------------------------------------------------
        void OnGUI()
        {
            OnEvent(UnityEngine.Event.current);

            OnInnerGUI();
            for (int i = 0; i < m_vLogics.Count; ++i)
                m_vLogics[i].DrawGUI();

            for (int i = 0; i < m_vGraphViewLogics.Count; ++i)
                m_vGraphViewLogics[i].DrawGUI();
            OnInnerGUIEnd();
        }
        //--------------------------------------------------------
        protected virtual void OnEvent(UnityEngine.Event evt) 
        {
            OnInnerEvent(evt);
            for (int i = 0; i < m_vLogics.Count; ++i)
                m_vLogics[i].DoEvent(evt);
            for (int i = 0; i < m_vGraphViewLogics.Count; ++i)
                m_vGraphViewLogics[i].DoEvent(evt);
        }
        //--------------------------------------------------------
        protected virtual void OnInnerEvent(UnityEngine.Event evt) { }
        //--------------------------------------------------------
        protected virtual void OnInnerGUI() { }

        //--------------------------------------------------------
        protected virtual void OnInnerGUIEnd() { }
        //--------------------------------------------------------
        void ScanerRegisterLogics()
        {
            var types = this.GetType().Assembly.GetTypes();
            foreach (var type in types)
            {
                if (type.IsSubclassOf(typeof(AEditorLogic)) && type.IsDefined(typeof(EditorBinderAttribute), false))
                {
                    EditorBinderAttribute attr = type.GetCustomAttribute<EditorBinderAttribute>();
                    if(attr.bindType== this.GetType())
                        RegisterLogic(type);
                }
            }
        }
        //--------------------------------------------------------
        void RegisterLogic(AEditorLogic logic)
        {
            if (m_vLogicKV.ContainsKey(logic.GetType())) return;
            m_vLogics.Add(logic);
            m_vLogics.Sort((l1, l2) => {
                EditorBinderAttribute attr1 = l1.GetType().GetCustomAttribute<EditorBinderAttribute>();
                EditorBinderAttribute attr2 = l2.GetType().GetCustomAttribute<EditorBinderAttribute>();
                return attr2.order - attr1.order;
            });
            m_vLogicKV[logic.GetType()] = logic;
        }
        //--------------------------------------------------------
        public T RegisterLogic<T>() where T : AEditorLogic
        {
            AEditorLogic logic = (AEditorLogic)System.Activator.CreateInstance(typeof(T));
            logic.Init(this);
            RegisterLogic(logic);
            return logic as T;
        }
        //--------------------------------------------------------
        public AEditorLogic RegisterLogic(System.Type type)
        {
            AEditorLogic logic = (AEditorLogic)System.Activator.CreateInstance(type);
            logic.Init(this);
            RegisterLogic(logic);
            return logic;
        }
        //--------------------------------------------------------
        public void RegisterGraphView(VisualElement root)
        {
            if(root == null)
            {
                return;
            }
            ICustomVisualElement iView = root as ICustomVisualElement;
            if(iView!=null)
            {
                iView.Init(this);
                if (m_vGraphViewLogicKV.ContainsKey(iView.GetType())) return;
                m_vGraphViewLogics.Add(iView);
            }
            for (int i = 0; i < root.childCount; ++i)
                RegisterGraphView(root[i]);
        }
        //--------------------------------------------------------
        public T GetLogic<T>() where T : AEditorLogic
        {
            AEditorLogic logic;
            if (m_vLogicKV.TryGetValue(typeof(T), out logic))
                return logic as T;
            return null;
        }
        //--------------------------------------------------------
        public override void SaveChanges()
        {
            base.SaveChanges();
            for (int i = 0; i < m_vLogics.Count; ++i)
            {
                m_vLogics[i].OnSaveChanges();
            }
        }
        //--------------------------------------------------------
        public float GetTimeScale()
        {
            return m_pTimer.m_currentSnap;
        }
        //--------------------------------------------------------
        public void SetTimeScale(float scale)
        {
            if (scale <= 0f) scale = 0.01f;
            m_pTimer.m_currentSnap = scale;
        }		
        //--------------------------------------------------------
        public T GetGraphView<T>() where T : ICustomVisualElement
        {
            ICustomVisualElement logic;
            if (m_vGraphViewLogicKV.TryGetValue(typeof(T), out logic))
                return (T)logic;
            return default;
        }
        //--------------------------------------------------------
        public virtual int GetPriority() { return 0; }
        public virtual bool IsManaged() { return true; }
        public virtual bool IsRuntimeOpen() { return m_bRuntimingOpened; }
        //--------------------------------------------------------
        public virtual void OnChangeSelect(System.Object pObject)
        {
            if (m_pCurrentObj == pObject)
                return;
            m_pCurrentObj = pObject;
            for (int i = 0; i < m_vLogics.Count; ++i)
            {
                m_vLogics[i].OnChangeSelect(pObject);
            }
        }
    }
}
#endif