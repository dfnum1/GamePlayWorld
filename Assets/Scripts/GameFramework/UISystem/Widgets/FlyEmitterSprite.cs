/********************************************************************
生成日期:	12:28:2020 13:16
类    名: 	FlyEmitterSprite
作    者:	Happli
描    述:	精灵发射器
*********************************************************************/

using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;
using Framework.Core;

#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Framework.UI
{
    [UIWidgetExport]
    [ExecuteAlways]
    public class FlyEmitterSprite : Image
    {
        public Vector3Int CircleCount = Vector3Int.zero;
        public Vector3 Spawn = new Vector3(0,0,1);
        public Vector3 Speed = new Vector3(200,0,10);
        public Vector3 Acceleration = new Vector3(-200, 0, 10);
        public AnimationCurve AccelerationTrack;
        public Vector3 SpreadRadius = new Vector3(0, 0, 60);
        public Vector3 Rotate = new Vector3(0, 10, 60);
        public Vector3 Scale = Vector3.right;
        public Vector3 Life = new Vector3(1, 0, 0);
        public float FlySpeed = 10;
        public bool SpawnReverseFlyTo = true;

        [System.NonSerialized]
        public System.Action<FlyEmitterSprite> OnStopCallback = null;
        [System.NonSerialized]
        public System.Action<FlyEmitterSprite> OnStartCallback = null;
        struct Particle
        {
            public Vector3 _vPositon;
            public Vector3 _vSpeed;
            public float _fInitSpeedMag;
            public float _fAcceleration;
            public Vector3 _vRotation;
            public float _fScale;
            public float _fMaxLife;
            public float _fCurLife;
            public float _fDelay;
            public bool _bFlying;
            public Vector3 _vFlyPoition;
        }
        List<Particle> m_Particles;
        private bool m_bConvertUIPos = false;
        private Transform m_FlyTo = null;
        private Vector3 m_FlyToDir = Vector3.zero;
        private float m_fFlyDuration = 0;
#if UNITY_EDITOR
        [System.NonSerialized]
        public bool bEditor = false;
#endif
        private bool m_bIsPlaying = false;
        //------------------------------------------------------
        protected override void Awake()
        {
            m_bIsPlaying = false;
            base.Awake();
        }
        //------------------------------------------------------
        public void Play(int assignCnt =-1)
        {
            if (m_Particles == null) m_Particles = new List<Particle>();
            else m_Particles.Clear();

           // m_FinalPos = finalToPos;
            RectTransform rectTrans = this.rectTransform;
            Vector3 invFlyDir = -m_FlyToDir;

            m_bIsPlaying = true;
            int parCnt = assignCnt;
            if(parCnt<=0) parCnt = CircleCount.x + UnityEngine.Random.Range(CircleCount.y, CircleCount.z);
            if (parCnt <= 0) return;
            for(int i = 0; i < parCnt; ++i)
            {
                Particle par = new Particle();
                float spreadR = SpreadRadius.x + UnityEngine.Random.Range(SpreadRadius.y, SpreadRadius.z);
                Quaternion rot = Quaternion.Euler(0, 0, Rotate.x + UnityEngine.Random.Range(Rotate.y, Rotate.z));
                par._vRotation = rot.eulerAngles;
                par._fAcceleration = Acceleration.x + UnityEngine.Random.Range(Acceleration.y, Acceleration.z);
                par._fInitSpeedMag = (Speed.x + UnityEngine.Random.Range(Speed.y, Speed.z));
                if (m_fFlyDuration>0 && SpawnReverseFlyTo && invFlyDir.sqrMagnitude>0)
                {
                    par._vSpeed = rot* invFlyDir * par._fInitSpeedMag;
                }
                else
                {
                    par._vSpeed = rot * Vector3.right * par._fInitSpeedMag;
                }
                par._vPositon = par._vSpeed.normalized* spreadR;
                par._fScale = Scale.x + UnityEngine.Random.Range(Scale.y, Scale.z);
                par._fCurLife = 0;
                par._fDelay = Spawn.x + UnityEngine.Random.Range(Spawn.y, Spawn.z);
                par._fMaxLife = Life.x + UnityEngine.Random.Range(Life.y, Life.z);
                par._bFlying = false;
                m_Particles.Add(par);
            }
            if (OnStartCallback != null) OnStartCallback(this);
        }
        //------------------------------------------------------

        //------------------------------------------------------
        public void FlyTo(Transform trans, float fFlyTime, int spriteCnt = -1)
        {
            if (trans == null) return;
            m_bConvertUIPos = !(trans is RectTransform);
            m_FlyTo = trans;

            if(m_bConvertUIPos)
            {
                Vector3 toPos = Vector3.zero;
                if (WorldPosToUIPos(trans.position, false, ref toPos))
                {
                    m_FlyToDir = (toPos - rectTransform.position).normalized;
                }
                else
                {
                    return;
                }
            }
            else
            {
                m_FlyToDir = (m_FlyTo.position - rectTransform.position).normalized;
            }
            m_fFlyDuration = fFlyTime;
            Play(spriteCnt);
        }
        //------------------------------------------------------
        public bool IsPlaying()
        {
            return m_bIsPlaying;
        }
        //------------------------------------------------------
        public float GetMaxTime()
        {
            if (m_Particles == null) return 0;
            float time = 0;
            for(int i = 0; i < m_Particles.Count; ++i)
            {
                time = Mathf.Max(time, m_Particles[i]._fDelay + m_Particles[i]._fMaxLife);
            }
            return time;
        }
        //------------------------------------------------------
        public static bool WorldPosToUIPos(Vector3 worldPos, bool bLocal, ref Vector3 point, Camera cam = null)
        {
            if (AFramework.mainFramework == null) return false;
            var mgr = AFramework.mainFramework.uiManager;
            if (mgr == null) return false;
            return mgr.ConvertWorldPosToUIPos(worldPos, bLocal, ref point, cam);
        }
        //------------------------------------------------------
        public void ForceUpdate(float deltaTime)
        {
            if (!m_bIsPlaying)
            {
                return;
            }
            if(m_Particles == null || m_Particles.Count<=0)
            {
                this.SetVerticesDirty();
                m_bIsPlaying = false;
                return;
            }
            Particle par;
            for (int i = 0; i < m_Particles.Count;)
            {
                par = m_Particles[i];
                if (par._fMaxLife <= 0 || par._fCurLife >= par._fMaxLife)
                {
                    m_Particles.RemoveAt(i);
                    continue;
                }
                if (par._fDelay >= 0)
                {
                    par._fDelay -= deltaTime;
                }
                else
                {
                    if(par._bFlying)
                    {
                        if(m_FlyTo == null)
                        {
                            m_Particles.RemoveAt(i);
                            continue;
                        }
                        par._fCurLife += deltaTime*Mathf.Max(0.01f, FlySpeed);
                        float fFact = Mathf.Clamp01(par._fCurLife / par._fMaxLife);
                        Vector3 flyTo = Vector3.zero;
                        if (m_bConvertUIPos)
                        {
                            Vector3 toPos = Vector3.zero;
                            if (WorldPosToUIPos(m_FlyTo.position, false, ref toPos))
                            {
                                flyTo = toPos - rectTransform.position;
                            }
                            else
                            {
                                m_FlyTo = null;
                            }
                        }
                        else 
                        {
                            flyTo = m_FlyTo.position - rectTransform.position;
                        }

                        par._vPositon = par._vFlyPoition * (1 - fFact) + flyTo * fFact;
                    }
                    else
                    {
                        par._fCurLife += deltaTime;
                        float AccSpeed = 1;
                        if (AccelerationTrack != null && AccelerationTrack.length>0)
                            AccSpeed = AccelerationTrack.Evaluate(par._fCurLife / par._fMaxLife);
//                         APPLY_FRACTION(ref par._vSpeed.x, par._fAcceleration * AccSpeed, deltaTime);
//                         APPLY_FRACTION(ref par._vSpeed.y, par._fAcceleration * AccSpeed, deltaTime);
//                         APPLY_FRACTION(ref par._vSpeed.z, par._fAcceleration * AccSpeed, deltaTime);

                        if(m_FlyTo)
                            par._vSpeed = Vector3.Lerp(par._vSpeed, m_FlyToDir* par._fInitSpeedMag, deltaTime* par._fAcceleration * AccSpeed);
                        else
                        {
                            par._vSpeed.x += par._fAcceleration * AccSpeed * deltaTime;
                            par._vSpeed.y += par._fAcceleration * AccSpeed * deltaTime;
                            par._vSpeed.z += par._fAcceleration * AccSpeed * deltaTime;
                        }
                        par._vPositon += par._vSpeed * deltaTime;
                        if (par._fCurLife >= par._fMaxLife)
                        {
                            if (m_fFlyDuration > 0)
                            {
                                par._fMaxLife = m_fFlyDuration;
                                par._fCurLife = 0;
                                par._bFlying = true;
                                par._vFlyPoition = par._vPositon;
                            }
                        }
                    }
                }
                m_Particles[i] = par;
                ++i;
            }
            this.SetVerticesDirty();

            if(m_Particles.Count<=0)
            {
                Stop();
            }
        }
        //------------------------------------------------------
        void APPLY_FRACTION(ref float speed, float fraction, float time)
        {
            if (Mathf.Abs(fraction) > 0.01f && Mathf.Abs(speed) > 0.01f)
            {
                float temp_speed = speed;
                if (temp_speed > 0f)
                    temp_speed -= fraction * time;
                else
                    temp_speed += fraction * time;
                if (temp_speed * speed < 0f)
                    speed = 0f;
                else
                    speed = temp_speed;
            }
        }
        //------------------------------------------------------
        private void LateUpdate()
        {
            ForceUpdate(Time.deltaTime);
        }
        //------------------------------------------------------
        public void Pause()
        {
            m_bIsPlaying = false;
        }
        //------------------------------------------------------
        public void Resume()
        {
            if (!m_bIsPlaying)
            {
                m_bIsPlaying = true;
            }
        }
        //------------------------------------------------------
        public void Stop()
        {
            if (!m_bIsPlaying) return;
            if (OnStopCallback != null) OnStopCallback(this);
            OnStopCallback = null;
            OnStartCallback = null;
             m_bIsPlaying = false;
            m_fFlyDuration = 0;
            m_FlyTo = null;
            
            if (m_Particles!=null) m_Particles.Clear();
#if UNITY_EDITOR
            if (bEditor) return;
#endif
            sprite = null;
        }
        //------------------------------------------------------
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            if (!m_bIsPlaying) return;
            if (m_Particles == null || m_Particles.Count <= 0 || sprite == null)
                return;
            Rect rect = GetPixelAdjustedRect();
            Vector4 sprite_uv = UnityEngine.Sprites.DataUtility.GetOuterUV(sprite);
            Rect uvRect = new Rect(sprite_uv.x, sprite_uv.y, sprite_uv.z - sprite_uv.x, sprite_uv.w - sprite_uv.y);
            for (int i = 0; i < m_Particles.Count; ++i)
            {
                // 1------2
                // |      |
                // |      |
                // 0------3
                Particle par = m_Particles[i];
                if (par._fDelay > 0) continue;

                Color color = new Color(1, 1, 1, 1);

                Vector4 v = Vector4.zero;

                Rect uv = uvRect;// * frame.uvRect;

                int w = (int)sprite.rect.width;
                int h = (int)sprite.rect.height;

                float paddedW = ((w & 1) == 0) ? w : w + 1;
                float paddedH = ((h & 1) == 0) ? h : h + 1;

                v.x = 0f;
                v.y = 0f;
                v.z = w / paddedW;
                v.w = h / paddedH;

                v.x -= rectTransform.pivot.x;
                v.y -= rectTransform.pivot.y;
                v.z -= rectTransform.pivot.x;
                v.w -= rectTransform.pivot.y;

                v.x *= rectTransform.rect.width * par._fScale;
                v.y *= rectTransform.rect.height * par._fScale;
                v.z *= rectTransform.rect.width * par._fScale;
                v.w *= rectTransform.rect.height * par._fScale;

                Quaternion rot = Quaternion.Euler(par._vRotation);
                int verCnt = vh.currentVertCount;
                Vector3 pos = rot * new Vector3(v.x, v.y) + par._vPositon;
                vh.AddVert(pos, color, new Vector2(uv.xMin, uv.yMin));

                pos = rot * new Vector3(v.x, v.w) + par._vPositon;
                vh.AddVert(pos, color, new Vector2(uv.xMin, uv.yMax));

                pos = rot * new Vector3(v.z, v.w) + par._vPositon;
                vh.AddVert(pos, color, new Vector2(uv.xMax, uv.yMax));

                pos = rot * new Vector3(v.z, v.y) + par._vPositon;
                vh.AddVert(pos, color, new Vector2(uv.xMax, uv.yMin));
                vh.AddTriangle(verCnt, verCnt + 1, verCnt + 2);
                vh.AddTriangle(verCnt, verCnt + 2, verCnt + 3);
            }
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(FlyEmitterSprite), true)]
    [CanEditMultipleObjects]
    public class FlyEmitterSpriteEditor : UnityEditor.UI.ImageEditor
    {
        public RectTransform m_pFlyTest;
        Framework.ED.EditorTimer m_pTimer = new Framework.ED.EditorTimer();
        //------------------------------------------------------
        protected override void OnEnable()
        {
            FlyEmitterSprite image = target as FlyEmitterSprite;
            image.bEditor = true;
            EditorApplication.update += Update;
            base.OnEnable();
        }
        //------------------------------------------------------
        protected override void OnDisable()
        {
            FlyEmitterSprite image = target as FlyEmitterSprite;
            image.bEditor = false;
            EditorApplication.update -= Update;
            base.OnDisable();
        }
        //------------------------------------------------------
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            FlyEmitterSprite image = target as FlyEmitterSprite;
            EditorGUI.BeginChangeCheck();
            image.sprite = EditorGUILayout.ObjectField("Sprite", image.sprite, typeof(Sprite), false) as Sprite;
            image.FlySpeed = EditorGUILayout.FloatField("飞入速度", image.FlySpeed);
            m_pFlyTest = EditorGUILayout.ObjectField("FlyTest", m_pFlyTest, typeof(RectTransform), true) as RectTransform;
            image.SpawnReverseFlyTo = EditorGUILayout.Toggle("与飞入点方向反向", image.SpawnReverseFlyTo);

            image.CircleCount = DrawVec3("发射个数", image.CircleCount);
            image.Spawn = DrawVec3("出生延迟", image.Spawn);
            image.Speed = DrawVec3("发射速度", image.Speed);
            image.Acceleration = DrawVec3("加速度", image.Acceleration);
            image.AccelerationTrack = EditorGUILayout.CurveField("加速度曲线", image.AccelerationTrack);
            image.SpreadRadius = DrawVec3("离散半径", image.SpreadRadius);
            image.Rotate = DrawVec3("角度", image.Rotate);
            image.Scale = DrawVec3("缩放", image.Scale);
            image.Life = DrawVec3("生命时长", image.Life);


            if (EditorGUI.EndChangeCheck())
            {
                image.SetMaterialDirty();
                SceneView.lastActiveSceneView.Repaint();
            }
            serializedObject.ApplyModifiedProperties();
            if (GUILayout.Button("刷新"))
            {
                EditorUtility.SetDirty(target);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            }
        }
        //------------------------------------------------------
        Vector3 DrawVec3(string label, Vector3 vec)
        {
            EditorGUILayout.LabelField(label);
            float labelWidth =  EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 60;
            EditorGUI.indentLevel++;
            vec.x = EditorGUILayout.FloatField("基础值", vec.x);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("随机区间");
            vec.y = EditorGUILayout.FloatField(vec.y);
            EditorGUIUtility.labelWidth = 10;
            EditorGUILayout.LabelField("-");
            EditorGUIUtility.labelWidth = 60;
            vec.z = EditorGUILayout.FloatField(vec.z);
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel--;
            EditorGUIUtility.labelWidth = labelWidth;
            return vec;
        }
        //------------------------------------------------------
        Vector3Int DrawVec3(string label, Vector3Int vec)
        {
            EditorGUILayout.LabelField(label);
            float labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 60;
            EditorGUI.indentLevel++;
            vec.x = EditorGUILayout.IntField("基础值", vec.x);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("随机区间");
            vec.y = EditorGUILayout.IntField(vec.y);
            EditorGUIUtility.labelWidth = 10;
            EditorGUILayout.LabelField("-");
            EditorGUIUtility.labelWidth = 60;
            vec.z = EditorGUILayout.IntField(vec.z);
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel--;
            EditorGUIUtility.labelWidth = labelWidth;
            return vec;
        }
        //------------------------------------------------------
        private void OnSceneGUI()
        {
            FlyEmitterSprite image = target as FlyEmitterSprite;
            GUILayout.BeginArea(new Rect(0, 0, 500, 70));
            if (GUILayout.Button("播放", new GUILayoutOption[] { GUILayout.MaxWidth(60) }))
            {
                image.Play();
            }
            if (m_pFlyTest!=null && GUILayout.Button("飞入", new GUILayoutOption[] { GUILayout.MaxWidth(60) }))
            {
                image.FlyTo(m_pFlyTest, 1);
            }
            if (GUILayout.Button("停止", new GUILayoutOption[] { GUILayout.MaxWidth(60) }))
            {
                image.Stop();
            }
            if(Event.current.type == EventType.MouseUp)
            {
                if(Event.current.button == 0)
                {
                    image.Play();
                }
            }
            GUILayout.EndArea();
        }
        //------------------------------------------------------
        void Update()
        {
            if (Application.isPlaying) return;
            m_pTimer.Update();
            Framework.ED.EditorUtil.RepaintPlayModeView();
            if (SceneView.currentDrawingSceneView) SceneView.currentDrawingSceneView.Repaint();
            SceneView.lastActiveSceneView.Repaint();
        }
    }
#endif
}