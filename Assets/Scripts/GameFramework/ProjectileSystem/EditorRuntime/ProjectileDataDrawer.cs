#if UNITY_EDITOR
/********************************************************************
生成日期:	25:7:2019   14:35
类    名: 	ProjectileDataDrawer
作    者:	HappLI
描    述:	飞行道具编辑器
*********************************************************************/
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using System.Linq;
using UnityEditor.IMGUI.Controls;
using Framework.Core;
using Framework.ED;
using Framework.Data;
using Framework.Base;
#if USE_CUTSCENE
using Framework.CutsceneAT.Runtime;
#endif

#if USE_FIXEDMATH
using ExternEngine;
#else
using FFloat = System.Single;
using FVector3 = UnityEngine.Vector3;
using FVector2 = UnityEngine.Vector2;
using FQuaternion = UnityEngine.Quaternion;
using FMatrix4x4 = UnityEngine.Matrix4x4;
#endif

namespace Framework.ED
{
    [PluginInspector(typeof(ProjectileData), "OnDrawProjectData")]
    public class ProjectileDataDrawer
    {
        public class DrawData
        {
            public class ProjectTrack
            {
                public List<Vector3> vTracks;
                public ProjectileNode pProjectile;

                private bool m_bInited = false;
                public Vector3 backupSpeed = Vector3.zero;
                public Vector3 backupPosition = Vector3.zero;
                public Vector3 backupAcceleration = Vector3.zero;

                public Color trackColor = Color.white;
                public ProjectTrack(ProjectileNode pProj)
                {
                    pProjectile = pProj;
                    vTracks = new List<Vector3>();

                    backupPosition = pProjectile.GetPosition();
                    backupSpeed = pProjectile.GetSpeed();
                    backupAcceleration = pProjectile.GetAcceleration();
                }
                public void Destroy()
                {
                    if (pProjectile != null) pProjectile.Destroy();
                }
                public void TestTrack(Vector3 vStart, Vector3 vEnd)
                {
                    if (pProjectile == null) return;

                    var csvData = pProjectile.GetProjectileData();
                    if (csvData == null)
                        return;
                    Color color = Handles.color;
                    Handles.color = trackColor;

                    if (csvData.type == EProjectileType.TrackPath)
                    {
                        pProjectile.BuildTrackPathKeyframe(vStart, vEnd, csvData.speedLerp.x);
                        pProjectile.DrawTrackPath(trackColor);
                    }
                    else
                    {
                        int trackCnt = 99;
                        vTracks.Clear();
                        vTracks.Add(vStart);
                        pProjectile.SetPosition(backupPosition);
                        pProjectile.SetSpeed(backupSpeed);
                        pProjectile.SetAcceleration(backupAcceleration);
                        pProjectile.SetRemainLifeTime(csvData.life_time);
                        pProjectile.SetDelayTime(csvData.launch_delay + 0.0666f);
                        pProjectile.SetSpeed(backupSpeed);
                        pProjectile.ResetTrackStates();
                        pProjectile.SetBounceTypeCount((int)csvData.speedLerp.x);
                        while (pProjectile.GetRemainLifeTime() >= 0 && trackCnt >= 0)
                        {
                            if (vTracks.Count <= 0 || (vTracks[vTracks.Count - 1] - pProjectile.GetPosition()).sqrMagnitude > 0.05f)
                                vTracks.Add(pProjectile.GetPosition());
                            pProjectile.Update(0.0333333f);
                            trackCnt--;
                        }


                        float len = 0;
                        for (int i = 0; i < vTracks.Count - 1; ++i)
                        {
                            Handles.DrawLine(vTracks[i], vTracks[i + 1]);

                            Vector3 toDir = vTracks[i + 1] - vTracks[i];
                            if (toDir.sqrMagnitude <= 0) toDir = Vector3.forward;
                            len += toDir.magnitude;
                            if (len >= 2)
                            {
                                Quaternion qt = Quaternion.LookRotation(toDir.normalized);
                                Handles.ArrowHandleCap(0, vTracks[i], qt, HandleUtility.GetHandleSize(vTracks[i]), EventType.Repaint);
                                len = 0;
                            }
                        }
                    }
                    Handles.color = color;
                }
            }
            public bool bExpandSpeedAcc = false;
            public List<ProjectTrack> testProjectile = new List<ProjectTrack>();
            public System.DateTime lastTime;

            public void Destroy()
            {

            }
        }


        static private Dictionary<ProjectileData, DrawData> ms_vDraws = new Dictionary<ProjectileData, DrawData>();
        static private List<ProjectileData> ms_vDestroys = new List<ProjectileData>();
        //-----------------------------------------------------
        static DrawData CreateDraw(ProjectileData projectile)
        {
            foreach(var db in ms_vDraws)
            {
                if( System.DateTime.Now.Subtract(db.Value.lastTime).Seconds>= 300)
                {
                    db.Value.Destroy();
                    ms_vDestroys.Add(db.Key);
                }
            }
            foreach(var db in ms_vDestroys)
            {
                ms_vDraws.Remove(db);
            }
            ms_vDestroys.Clear();
            if (!ms_vDraws.TryGetValue(projectile, out var drawData))
            {
                drawData = new DrawData();
                ms_vDraws[projectile] = drawData;
            }
            drawData.lastTime = System.DateTime.Now;
            return drawData;
        }
        //-----------------------------------------------------
        public static System.Object OnDrawProjectData(System.Object data, System.Object parentData, System.Reflection.FieldInfo parentField)
        {
            ProjectileData projecitle;
            if (data != null) projecitle = (ProjectileData)data;
            else projecitle = new ProjectileData();
            Rect window = GUILayoutUtility.GetLastRect();
            Vector2 size = window.size;
            if (size.x <=400) size.x = 400;
            if (size.y <= 50) size.y = 500;
            OnInsepctor(projecitle, size);
            return projecitle;
        }
        //-----------------------------------------------------
        static string[] SPEED_AXIS_NAME = {"X轴速度", "Y轴速度", "Z轴速度" };
        static string[] ROTATE_AXIS_NAME = { "X轴旋转", "Y轴旋转", "Z轴旋转" };
        public static void OnInsepctor(ProjectileData projectile, Vector2 size)
        {
            DrawData drawData = CreateDraw(projectile);
            projectile.desc = EditorGUILayout.TextField("名称描述",projectile.desc);
     //       projectile.type = (Core.EProjectileType)EditorGUILayout.EnumPopup("类型", projectile.type);
            InspectorDrawUtil.DrawPropertyByFieldName(projectile, "type");
            InspectorDrawUtil.DrawPropertyByFieldName(projectile, "bornType");

            InspectorDrawUtil.DrawPropertyByFieldName(projectile, "classify");

            float labelWidth = EditorGUIUtility.labelWidth;
            if(projectile.type != EProjectileType.Trap)
            {
                if(projectile.type == EProjectileType.TrackPath)
                {
                    List<Vector3> vInTags = (projectile.speedLows != null) ? new List<Vector3>(projectile.speedLows) : new List<Vector3>();
                    List<Vector3> vOutTags = (projectile.speedUppers != null) ? new List<Vector3>(projectile.speedUppers) : new List<Vector3>();
                    List<Vector3> vPoint = (projectile.speedMaxs != null) ? new List<Vector3>(projectile.speedMaxs) : new List<Vector3>();
                    List<Vector3> vAccSpeed = (projectile.accelerations != null) ? new List<Vector3>(projectile.accelerations) : new List<Vector3>();
                    drawData.bExpandSpeedAcc = EditorGUILayout.Foldout(drawData.bExpandSpeedAcc, "路径点");
                    if(drawData.bExpandSpeedAcc)
                    {
                        float width = size.x - 40;
                        GUILayoutOption[] layOp = new GUILayoutOption[] { GUILayout.Width(width / 4) };
                        GUILayout.BeginHorizontal(new GUILayoutOption[] { GUILayout.Width(width) });
                        GUILayout.Label("index", layOp);
                        GUILayout.Label("点", layOp);
                        GUILayout.Label("速度", layOp);
                        GUILayout.Label("", layOp);
                        GUILayout.EndHorizontal();

                        for (int i = 0; i < vPoint.Count; ++i)
                        {
                            Vector3 inTag = vInTags[i];
                            Vector3 outTag = vOutTags[i];
                            Vector3 point = vPoint[i];
                            Vector3 accSpeed = vAccSpeed[i];

                            GUILayout.BeginHorizontal(new GUILayoutOption[] { GUILayout.Width(width) });
                            EditorGUILayout.LabelField((i + 1).ToString(), layOp);
                            point = EditorGUILayout.Vector3Field("",point, layOp);
                            accSpeed.x = Mathf.Max(0.1f, EditorGUILayout.FloatField("", accSpeed.x, layOp));
                            if (GUILayout.Button("删除", layOp))
                            {
                                vInTags.RemoveAt(i);
                                vOutTags.RemoveAt(i);
                                vPoint.RemoveAt(i);
                                vAccSpeed.RemoveAt(i);
                                break;
                            }
                            GUILayout.EndHorizontal();

                            vInTags[i] = inTag;
                            vOutTags[i] = outTag;
                            vPoint[i] = point;
                            vAccSpeed[i] = accSpeed;
                        }
                        if (GUILayout.Button("新建组"))
                        {
                            vInTags.Add(Vector3.zero);
                            vOutTags.Add(Vector3.zero);
                            if (vPoint.Count >= 2) vPoint.Add(vPoint[vPoint.Count - 1] + (vPoint[vPoint.Count - 1] - vPoint[vPoint.Count - 2]).normalized * 5);
                            else if (vPoint.Count > 1) vPoint.Add(vPoint[vPoint.Count - 1] + Vector3.forward * 5);
                            else vPoint.Add(Vector3.zero);
                            vAccSpeed.Add(Vector3.one);
                        }

                        if (projectile.accelerations != null && projectile.accelerations.Length > 0)
                        {
                            EProjectileParabolicType preType = (EProjectileParabolicType)projectile.accelerations[0].y;
                            projectile.accelerations[0].y = InspectorDrawUtil.PopEnum(new GUIContent("构建方式"), (int)projectile.accelerations[0].y, typeof(EProjectileParabolicType));
                            if (preType != (EProjectileParabolicType)projectile.accelerations[0].y)
                            {
                                vInTags.Clear();
                                vOutTags.Clear();
                                vPoint.Clear();
                                vAccSpeed.Clear();

                                vInTags.Add(Vector3.zero);
                                vOutTags.Add(Vector3.zero);
                                vPoint.Add(Vector3.zero);
                                vAccSpeed.Add(new Vector3(1, projectile.accelerations[0].y, 0));
                                if ((EProjectileParabolicType)projectile.accelerations[0].y == EProjectileParabolicType.StartEnd)
                                {
                                    vInTags.Add(Vector3.zero);
                                    vOutTags.Add(Vector3.zero);
                                    vPoint.Add(Vector3.zero);
                                    vAccSpeed.Add(new Vector3(1, 0, 0));
                                }
                            }
                        }
                        projectile.speedLows = vInTags.ToArray();
                        projectile.speedUppers = vOutTags.ToArray();
                        projectile.speedMaxs = vPoint.ToArray();
                        projectile.accelerations = vAccSpeed.ToArray();
                    }
                    projectile.speedLerp = EditorGUILayout.Vector2Field("速度区间", projectile.speedLerp);
                }
                else
                {
                    List<Vector3> vLowerSpeeds = (projectile.speedLows != null) ? new List<Vector3>(projectile.speedLows) : new List<Vector3>();
                    List<Vector3> vUpperSpeeds = (projectile.speedUppers != null) ? new List<Vector3>(projectile.speedUppers) : new List<Vector3>();
                    List<Vector3> vMaxSpeeds = (projectile.speedMaxs != null) ? new List<Vector3>(projectile.speedMaxs) : new List<Vector3>();
                    List<Vector3> vAccelerations = (projectile.accelerations != null) ? new List<Vector3>(projectile.accelerations) : new List<Vector3>();
                    drawData.bExpandSpeedAcc = EditorGUILayout.Foldout(drawData.bExpandSpeedAcc, "速度组");
                    if (drawData.bExpandSpeedAcc)
                    {
                        float width = size.x - 40;
                        GUILayoutOption[] layOp = new GUILayoutOption[] { GUILayout.Width(width / 4) };
                        for (int i = 0; i < vLowerSpeeds.Count; ++i)
                        {
                            Vector3 lower = vLowerSpeeds[i];
                            Vector3 upper = vUpperSpeeds[i];
                            Vector3 maxer = vMaxSpeeds[i];
                            Vector3 acc = vAccelerations[i];
                            GUILayout.BeginHorizontal(new GUILayoutOption[] { GUILayout.Width(width) });
                            GUILayout.Label("轴向", layOp);
                            GUILayout.Label("随机区间", layOp);
                            GUILayout.Label("最大速度(0为无限制)", layOp);
                            if (GUILayout.Button("删除", layOp))
                            {
                                vLowerSpeeds.RemoveAt(i);
                                vUpperSpeeds.RemoveAt(i);
                                vMaxSpeeds.RemoveAt(i);
                                vAccelerations.RemoveAt(i);
                                break;
                            }
                            GUILayout.EndHorizontal();

                            GUILayout.BeginVertical();
                            for (int j = 0; j < 3; ++j)
                            {
                                GUILayout.BeginHorizontal();
                                EditorGUILayout.LabelField(SPEED_AXIS_NAME[j], layOp);

                                GUILayout.BeginHorizontal(layOp);
                                float lowSpeed = EditorGUILayout.FloatField(lower[j]);
                                EditorGUIUtility.labelWidth = 20;
                                EditorGUILayout.LabelField("--", new GUILayoutOption[] { GUILayout.Width(20) });
                                EditorGUIUtility.labelWidth = labelWidth;
                                float upperSpeed = EditorGUILayout.FloatField(upper[j]);
                                GUILayout.EndHorizontal();

                                lower[j] = Mathf.Min(lowSpeed, upperSpeed);
                                upper[j] = Mathf.Max(lowSpeed, upperSpeed);

                                maxer[j] = EditorGUILayout.FloatField(maxer[j], layOp);
                                GUILayout.EndHorizontal();
                            }

                            GUILayout.EndVertical();

                            acc = EditorGUILayout.Vector3Field("加速度", acc, new GUILayoutOption[] { GUILayout.Width(width) });
                            vLowerSpeeds[i] = lower;
                            vUpperSpeeds[i] = upper;
                            vMaxSpeeds[i] = maxer;
                            vAccelerations[i] = acc;
                        }
                        if (GUILayout.Button("新建组"))
                        {
                            vLowerSpeeds.Add(Vector3.zero);
                            vUpperSpeeds.Add(Vector3.zero);
                            vMaxSpeeds.Add(Vector3.zero);
                            vAccelerations.Add(Vector3.zero);
                        }
                        projectile.speedLows = vLowerSpeeds.ToArray();
                        projectile.speedUppers = vUpperSpeeds.ToArray();
                        projectile.speedMaxs = vMaxSpeeds.ToArray();
                        projectile.accelerations = vAccelerations.ToArray();
                    }
                }
            }
            if (ProjectileData.IsTrack(projectile.type))
            {
                projectile.speedLerp.x = EditorGUILayout.FloatField("弹道朝目标过度快慢", projectile.speedLerp.x);
                projectile.speedLerp.y = EditorGUILayout.FloatField("飞离多远朝向目标", projectile.speedLerp.y);
            }
            else if(projectile.type == EProjectileType.Bounce)
            {
                projectile.speedLerp.x = EditorGUILayout.IntField("弹跳次数(<=0不限制)", (int)projectile.speedLerp.x);
                projectile.speedLerp.y = EditorGUILayout.FloatField("弹力衰减值", projectile.speedLerp.y);
            }
            {
                float width = size.x - 40;
                GUILayoutOption[] layOp = new GUILayoutOption[] { GUILayout.Width(width / 3) };
                GUILayout.BeginHorizontal(new GUILayoutOption[] { GUILayout.Width(width) });
                GUILayout.Label("旋转随机区间", layOp);
                GUILayout.Label("最小区间", layOp);
                GUILayout.Label("最大区间", layOp);
                GUILayout.EndHorizontal();
                GUILayout.BeginVertical();
                for (int j = 0; j < 3; ++j)
                {
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(ROTATE_AXIS_NAME[j], layOp);

                    GUILayout.BeginHorizontal(layOp);
                    float lowValue = EditorGUILayout.FloatField(projectile.minRotate[j]);
                    EditorGUIUtility.labelWidth = 20;
                    EditorGUILayout.LabelField("--", new GUILayoutOption[] { GUILayout.Width(20) });
                    EditorGUIUtility.labelWidth = labelWidth;
                    float upperValue = EditorGUILayout.FloatField(projectile.maxRotate[j]);
                    GUILayout.EndHorizontal();

                    projectile.minRotate[j] = Mathf.Min(lowValue, upperValue);
                    projectile.maxRotate[j] = Mathf.Max(lowValue, upperValue);
                    GUILayout.EndHorizontal();
                }

                GUILayout.EndVertical();
            }
           
            EditorGUIUtility.labelWidth = labelWidth;
            projectile.life_time = EditorGUILayout.FloatField("生命时长", projectile.life_time);
            InspectorDrawUtil.DrawPropertyByFieldName(projectile, "launch_flag");

            if (ProjectileData.IsTrack(projectile.type))
            {
                InspectorDrawUtil.DrawPropertyByFieldName(projectile, "track_target_slot");
                projectile.track_target_offset = EditorGUILayout.Vector3Field("追踪目标绑点偏移", projectile.track_target_offset);
            }

            projectile.collisionType = (EProjectileCollisionType)EditorGUILayout.EnumPopup("碰撞体类型", projectile.collisionType);
            if(projectile.collisionType == EProjectileCollisionType.BOX)
            {
                projectile.aabb_min = EditorGUILayout.Vector3Field("包围盒-Min", projectile.aabb_min);
                projectile.aabb_max = EditorGUILayout.Vector3Field("包围盒-Max", projectile.aabb_max);
            }
            else if (projectile.collisionType == EProjectileCollisionType.CAPSULE)
            {
                projectile.aabb_min.x = EditorGUILayout.FloatField("半径大小", projectile.aabb_min.x);
            }
            projectile.penetrable = EditorGUILayout.Toggle("是否可穿透", projectile.penetrable);
            projectile.counteract = EditorGUILayout.Toggle("是否可抵消", projectile.counteract);
            projectile.explode_range = EditorGUILayout.FloatField("爆炸范围", projectile.explode_range);
            if(projectile.explode_range>0)
            {
                projectile.explode_effect = EditorUtil.DrawUIObjectByPath<GameObject>("爆炸击中特效", projectile.explode_effect);
                projectile.explode_effect_offset = EditorGUILayout.Vector3Field("爆炸击中特效偏移", projectile.explode_effect_offset);
                InspectorDrawUtil.DrawPropertyByFieldName(projectile, "explode_damage_id");
            }

            projectile.launch_delay = EditorGUILayout.FloatField("延迟发射", projectile.launch_delay);
            projectile.externLogicSpeed = EditorGUILayout.Toggle("附加酷跑/站位速速", projectile.externLogicSpeed);
            string preEffectPrefab = projectile.effect;
            projectile.effectSpeed = EditorGUILayout.FloatField("特效播放速度", projectile.effectSpeed);
            projectile.effect = EditorUtil.DrawUIObjectByPath<GameObject>("特效资源", projectile.effect);
            projectile.target_effect_hit = EditorUtil.DrawUIObjectByPath<GameObject>("击中特效", projectile.target_effect_hit);
            InspectorDrawUtil.DrawPropertyByFieldName(projectile, "effect_hit_slot", null, "击中特效绑点");
            projectile.target_effect_hit_offset = EditorGUILayout.Vector3Field("击中特效位置偏移", projectile.target_effect_hit_offset);
            projectile.target_effect_hit_scale = EditorGUILayout.FloatField("击中特效缩放", projectile.target_effect_hit_scale);

            string preWaringEffectPrefab = projectile.waring_effect;
            projectile.waring_duration = EditorGUILayout.FloatField("预警时长", projectile.waring_duration);
            projectile.waring_effect = EditorUtil.DrawUIObjectByPath<GameObject>("预警特效", projectile.waring_effect);

            if (projectile.sound_launch_id <= 0)
                InspectorDrawUtil.DrawPropertyByFieldName(projectile, "sound_launch");
            if (string.IsNullOrEmpty(projectile.sound_launch))
                InspectorDrawUtil.DrawPropertyByFieldName(projectile, "sound_launch_id");

            if (projectile.sound_hit_id <= 0)
                InspectorDrawUtil.DrawPropertyByFieldName(projectile, "sound_hit",null, "击中音效(路径)");
            if (string.IsNullOrEmpty(projectile.sound_hit))
                InspectorDrawUtil.DrawPropertyByFieldName(projectile, "sound_hit_id",null, "击中音效(id)");


            InspectorDrawUtil.DrawPropertyByFieldName(projectile, "damage");

            InspectorDrawUtil.DrawPropertyByFieldName(projectile, "scale");
            //if (preEffectPrefab!=null && preEffectPrefab.CompareTo(projectile.effect) != 0)
            //{
            //    if (drawData.m_pPreveObject) GameObject.DestroyImmediate(drawData.m_pPreveObject);
            //    GameObject pObj = AssetDatabase.LoadAssetAtPath<GameObject>(projectile.effect);
            //    if (pObj != null)
            //    {
            //        drawData.m_pPreveObject = GameObject.Instantiate<GameObject>(pObj);
            //        BaseUtil.ResetGameObject(drawData.m_pPreveObject, EResetType.All);
            //    }
            //}
            //             if (preWaringEffectPrefab != null && preWaringEffectPrefab.CompareTo(projectile.waring_effect) != 0)
            //             {
            //                 if (m_pWaringObj) GameObject.DestroyImmediate(m_pWaringObj);
            //                 GameObject pObj = AssetDatabase.LoadAssetAtPath<GameObject>(m_pCurItem.waring_effect);
            //                 if (pObj != null)
            //                 {
            //                     m_pWaringObj = GameObject.Instantiate<GameObject>(pObj);
            //                     Base.Util.ResetGameObject(m_pWaringObj, Base.EResetType.All);
            //                 }
            //             }
            projectile.unSceneTest = EditorGUILayout.Toggle("忽略场景地表检测", projectile.unSceneTest);
            InspectorDrawUtil.DrawPropertyByFieldName(projectile, "stuck_time_hit");

            projectile.target_action_hit = DrawActionAndTag(projectile.target_action_hit, "受击", true);
            projectile.hit_back_speed = EditorGUILayout.Vector3Field("击退力度", projectile.hit_back_speed);
            InspectorDrawUtil.DrawPropertyByFieldName(projectile, "hit_back_fraction");
            InspectorDrawUtil.DrawPropertyByFieldName(projectile, "hit_back_gravity");

            InspectorDrawUtil.DrawPropertyByFieldName(projectile, "target_direction_postion");
            InspectorDrawUtil.DrawPropertyByFieldName(projectile, "target_duration_hit");

            InspectorDrawUtil.DrawPropertyByFieldName(projectile, "AttackEventID");
            InspectorDrawUtil.DrawPropertyByFieldName(projectile, "HitEventID");
            InspectorDrawUtil.DrawPropertyByFieldName(projectile, "OverEventID");

            InspectorDrawUtil.DrawPropertyByFieldName(projectile, "bImmedate");
            InspectorDrawUtil.DrawPropertyByFieldName(projectile, "fEventStepGap");
            InspectorDrawUtil.DrawPropertyByFieldName(projectile, "StepEventID");

            projectile.hit_count = (byte)EditorGUILayout.IntField("攻击次数", (int)projectile.hit_count);
            projectile.max_oneframe_hit = (byte)EditorGUILayout.IntField("同一帧最大可攻击次数", (int)projectile.max_oneframe_hit);
            projectile.hit_step = EditorGUILayout.FloatField("击中后攻击间隔", projectile.hit_step);
            projectile.bound_count = EditorGUILayout.IntField("弹射次数", projectile.bound_count);
            if (projectile.bound_count > 0)
            {
                projectile.bound_range = EditorGUILayout.FloatField("弹射范围", projectile.bound_range);
                InspectorDrawUtil.DrawPropertyByFieldName(projectile, "bound_flag");

                if (projectile.bound_sound_launch_id <= 0)
                    InspectorDrawUtil.DrawPropertyByFieldName(projectile, "bound_sound_launch", null, "弹射发射音效(路径)");
                if (string.IsNullOrEmpty(projectile.bound_sound_launch))
                    InspectorDrawUtil.DrawPropertyByFieldName(projectile, "bound_sound_launch_id", null, "弹射发射音效(id)");

                if (projectile.bound_hit_sound_id <= 0)
                    InspectorDrawUtil.DrawPropertyByFieldName(projectile, "bound_hit_sound", null, "弹射击中音效(路径)");
                if (string.IsNullOrEmpty(projectile.bound_hit_sound))
                    InspectorDrawUtil.DrawPropertyByFieldName(projectile, "bound_hit_sound_id", null, "弹射击中音效(id)");

                projectile.bound_effectSpeed = EditorGUILayout.FloatField("弹射特效播放速度", projectile.bound_effectSpeed);
                projectile.bound_effect = EditorUtil.DrawUIObjectByPath<GameObject>("弹射特效资源", projectile.bound_effect);
                projectile.bound_hit_effect = EditorUtil.DrawUIObjectByPath<GameObject>("弹射击中特效", projectile.bound_hit_effect);

                if (projectile.bound_range > 0)
                {
                    InspectorDrawUtil.DrawPropertyByFieldName(projectile, "bound_damage_id");
                    projectile.bound_speed = EditorGUILayout.Vector3Field("弹射速度", projectile.bound_speed);
                    InspectorDrawUtil.DrawPropertyByFieldName(projectile, "bound_buffs");
                    InspectorDrawUtil.DrawPropertyByFieldName(projectile, "bound_lock_type");
                    //if (projectile.bound_lock_type != ELockHitType.None)
                    //{
                    //    InspectorDrawUtil.DrawPropertyByFieldName(projectile, "bound_lock_num");
                    //    InspectorDrawUtil.DrawPropertyByFieldName(projectile, "bound_lock_conditions");
                    //    if (projectile.bound_lock_conditions!=null && projectile.bound_lock_conditions.Length > 0)
                    //    {
                    //        InspectorDrawUtil.DrawPropertyByFieldName(projectile, "bound_lock_param1");
                    //        InspectorDrawUtil.DrawPropertyByFieldName(projectile, "bound_lock_param2");
                    //        InspectorDrawUtil.DrawPropertyByFieldName(projectile, "bound_lock_param3");
                    //        InspectorDrawUtil.DrawPropertyByFieldName(projectile, "bound_lock_rode");

                    //        projectile.bound_lock_param1 = CheckConditionParam(projectile.bound_lock_conditions.Length, projectile.bound_lock_param1);
                    //        projectile.bound_lock_param2 = CheckConditionParam(projectile.bound_lock_conditions.Length, projectile.bound_lock_param2);
                    //        projectile.bound_lock_param3 = CheckConditionParam(projectile.bound_lock_conditions.Length, projectile.bound_lock_param3);
                    //        projectile.bound_lock_rode = CheckConditionParam(projectile.bound_lock_conditions.Length, projectile.bound_lock_rode);
                    //    }
                    //    InspectorDrawUtil.DrawPropertyByFieldName(projectile, "bound_lockHeight");
                    //    if (projectile.bound_lockHeight)
                    //    {
                    //        InspectorDrawUtil.DrawPropertyByFieldName(projectile, "bound_minLockHeight");
                    //        InspectorDrawUtil.DrawPropertyByFieldName(projectile, "bound_maxLockHeight");
                    //    }
                    //}
                }
            }
        }
        //-----------------------------------------------------
        public static void OnSceneView(ProjectileData projectile)
        {
            if (!ms_vDraws.TryGetValue(projectile, out var drawData))
            {
                drawData = new DrawData();
                ms_vDraws[projectile] = drawData;
            }
        }
        //-----------------------------------------------------
        static uint DrawActionAndTag(uint stateAndTag, string name = null, bool isHorizontal = false, GUILayoutOption[] op = null)
        {
            return ActorSystem.ED.EditorUtil.DrawActionAndTag(stateAndTag, name, isHorizontal, op);
        }
    }
}
#endif