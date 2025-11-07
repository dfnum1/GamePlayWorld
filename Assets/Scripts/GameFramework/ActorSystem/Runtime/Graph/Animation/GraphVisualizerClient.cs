#if UNITY_EDITOR
/********************************************************************
生成日期:	5:11:2020  20:36
类    名: 	GraphVisualizerClient
作    者:	HappLI
描    述:	
*********************************************************************/
using System.Collections.Generic;
using UnityEngine.Playables;

namespace Framework.Core
{
    public interface IUserPlayableExternDraw
    {
        void OnDrawInfo(ref System.Text.StringBuilder sb);
    }
    public class GraphVisualizerClient
    {
        private static GraphVisualizerClient s_Instance;
        private List<PlayableGraph> m_Graphs = new List<PlayableGraph>();

        public static GraphVisualizerClient instance
        {
            get
            {
                if (s_Instance == null)
                    s_Instance = new GraphVisualizerClient();
                return s_Instance;
            }
        }

        ~GraphVisualizerClient()
        {
            m_Graphs.Clear();
        }

        public static void Show(PlayableGraph graph)
        {
            if (!instance.m_Graphs.Contains(graph))
            {
                instance.m_Graphs.Add(graph);
            }
        }

        public static void Hide(PlayableGraph graph)
        {
            if (instance.m_Graphs.Contains(graph))
            {
                instance.m_Graphs.Remove(graph);
            }
        }

        public static void ClearGraphs()
        {
            instance.m_Graphs.Clear();
        }

        public static IEnumerable<PlayableGraph> GetGraphs()
        {
            return instance.m_Graphs;
        }
    }

}
#endif