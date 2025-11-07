#if UNITY_EDITOR && USE_ACTORSYSTEM
/********************************************************************
生成日期:	5:11:2020  20:36
类    名: 	GraphPlayableUtil
作    者:	HappLI
描    述:	
*********************************************************************/
using Framework.GraphVisualizer;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Framework.Core
{
    //------------------------------------------------------
    public class GraphPlayableUtil
    {
        static Dictionary<int, GraphAnimationPlayer> ms_GraphPlayables = new Dictionary<int, GraphAnimationPlayer>();
        internal static void CollectPlayable(GameObject pTarget, GraphAnimationPlayer player)
        {
            if (pTarget == null) return;
            ms_GraphPlayables[pTarget.GetInstanceID()] = player;
        }
        internal static void UnCollectPlayable(GameObject pTarget)
        {
            if (pTarget == null) return;
            ms_GraphPlayables.Remove(pTarget.GetInstanceID());
        }
        internal static void UnCollectPlayable(int instanceId)
        {
            ms_GraphPlayables.Remove(instanceId);
        }
        public static bool IsGraphPlayable(GameObject target)
        {
            if (target == null) return false;
            return ms_GraphPlayables.ContainsKey(target.GetInstanceID());
        }
#if USE_CUTSCENE
        /*public static bool IsPlaying(GameObject target, MotionClip clip)
        {
            if (target == null) return false;
            GraphAnimationPlayer player;
            if(ms_GraphPlayables.TryGetValue(target.GetInstanceID(), out player))
            {
                return player.IsPlaying(clip);
            }
            return false;
        }*/
#endif
        public static void StopAllPlayable(GameObject target )
        {
            if (target == null) return;
            GraphAnimationPlayer player;
            if (ms_GraphPlayables.TryGetValue(target.GetInstanceID(), out player))
            {
                player.StopAll();
            }
        }
#if UNITY_EDITOR
        public static void DebugPlayable(GameObject target, IUserPlayableExternDraw externDraw = null)
        {
            if (target == null) return;
            GraphAnimationPlayer player;
            if (ms_GraphPlayables.TryGetValue(target.GetInstanceID(), out player))
            {
                PlayableGraphVisualizerWindow.ShowWindow(player.graph, externDraw);
            }
        }
        public static void DebugPlayable(Actor target, IUserPlayableExternDraw externDraw = null)
        {
            if (target == null) return;
            var instance = target.GetObjectAble();
            if (instance == null) return;
            DebugPlayable(instance.gameObject, externDraw);
        }
#endif
    }
}

#endif