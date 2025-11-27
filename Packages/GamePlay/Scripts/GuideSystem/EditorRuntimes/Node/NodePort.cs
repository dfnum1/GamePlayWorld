#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using Framework.Guide;
namespace Framework.Guide.Editor
{
    public enum EPortIO
    {
        In = 0,
        Out = 1,
        LinkIn = 2,
        LinkOut=3,
    }
    public interface IPortNode : IPortEditor
    {
        int GetGUID();
        GraphNode GetNode();

        IPort GetPort();

        Color GetColor();
        bool IsOutput();
        bool IsInput();
        EPortIO getIO();

        Rect GetRect();
        void SetRect(Rect rc);
        Rect GetViewRect();
        void SetViewRect(Rect rc);
        bool CanConnectTo(IPortNode port);

        void ClearConnections();
        string GetTips();
        string SetTips(string name);

        Attribute GetAttribute();
        void SetAttribute(Attribute attri);
    }

    public class PortUtil
    {
        public struct PortDisplay
        {
            public System.Type displayType;
            public int enumBit;
        }

        public static Dictionary<int, IPortNode> allPortPositions = new Dictionary<int, IPortNode>();
        public static Dictionary<int, PortDisplay> allPortDisplays = new Dictionary<int, PortDisplay>();

        //------------------------------------------------------
        public static void Clear()
        {
            allPortPositions.Clear();
            allPortDisplays.Clear();
        }
        //------------------------------------------------------
        public static IPortNode GetPort(int guid)
        {
            IPortNode outPort;
            if (allPortPositions.TryGetValue(guid, out outPort))
                return outPort;
            return null;
        }
        //------------------------------------------------------
        public static void SetDisplayType(int guid, System.Type type, int enumBit)
        {
            if (type == null) return;
            allPortDisplays[guid] = new PortDisplay() { displayType = type, enumBit = enumBit };
        }
        //------------------------------------------------------
        public static System.Type GetDisplayType(int guid)
        {
            if (allPortDisplays.TryGetValue(guid, out var displayType))
                return displayType.displayType;
            return null;
        }
        //------------------------------------------------------
        public static int GetDisplayTypeBit(int guid)
        {
            if (allPortDisplays.TryGetValue(guid, out var displayType))
                return displayType.enumBit;
            return 0;
        }
    }
    public class SlotPort : IPortNode
    {
        public EPortIO direction;
        public GraphNode baseNode;
        public IPort port;
        public int index;

        public Rect rect;
        public Rect view;

        public IPort GetPort()
        {
            return port;
        }

        public virtual void Clear()
        {
            baseNode = null;
            index = 0;
        }
        public EPortIO getIO() { return direction; }

        public int GetGUID()
        {
            if (baseNode == null) return 0;
            return baseNode.GetGUID()*10000000 + index + (((int)direction <<8)|index );
        }

        public Rect GetRect() { return rect; }
        public void SetRect(Rect rc) { rect = rc; }

        public Rect GetViewRect() { return view; }
        public void SetViewRect(Rect rc) { view = rc ; }


        public bool IsInput() { return direction == EPortIO.In;  }
        public bool IsOutput() {  return direction == EPortIO.Out;  }

        public GraphNode GetNode()
        {
            return baseNode;
        }

        public Color GetColor()
        {
            return GuidePreferences.GetTypeColor("port");
        }

        public bool CanConnectTo(IPortNode port)
        {
            return port.getIO() != getIO();
        }
        public void Disconnect(IPortNode port)
        {

        }

        /// <summary> Disconnect this port from another port </summary>
        public void Disconnect(int i)
        {

        }

        public virtual void ClearConnections()
        {
            if(port is VarPort)
            {
                if((port as VarPort).dummyMaps!=null) (port as VarPort).dummyMaps.Clear();
            }
            else if(port is ArgvPort)
            {
                GetNode().bindNode.GetArgvPorts()[index] = new ArgvPort() { guid = GuideEditorLogic.BuildPortGUID() };
            }
        }

        string m_strTips = "";
        public string GetTips()
        {
            return m_strTips;
        }
        public string SetTips(string name) { return m_strTips = name; }

        Attribute m_Attribute = null;
        public Attribute GetAttribute() { return m_Attribute; }
        public void SetAttribute(Attribute attri)
        {
            m_Attribute = attri;
        }

    }

    public class LinkPort : IPortNode
    {
        public EPortIO direction;
        public GraphNode baseNode;

        public Rect rect;
        public Rect view;

        public EPortIO getIO() { return direction; }

        public IPort GetPort() { return null; }

        public virtual void Clear()
        {
            baseNode = null;
        }
        public virtual void ClearConnections()
        {
            baseNode.Links.Clear();
        }

        public virtual int GetGUID()
        {
            if (baseNode == null)
                return 0;
            if (baseNode.bindNode == null) return 0;
            return baseNode.GetGUID() * 10000000 + (((int)direction << 8));
        }

        public int GetLinkGUID()
        {
            return GetGUID();
        }

        public Rect GetRect() { return rect; }
        public void SetRect(Rect rc) { rect = rc; }

        public Rect GetViewRect() { return view; }
        public void SetViewRect(Rect rc) { view = rc; }


        public bool IsInput() { return direction == EPortIO.LinkIn; }
        public bool IsOutput() { return direction == EPortIO.LinkOut; }


        public GraphNode GetNode()
        {
            return baseNode;
        }

        public virtual Color GetColor()
        {
            return Color.white;
        }


        public virtual bool CanConnectTo(IPortNode port)
        {
            // Figure out which is input and which is output
            //NodePort input = null, output = null;
            //if (IsInput) input = this;
            //else output = this;
            //if (port.IsInput) input = port;
            //else output = port;
            //// If there isn't one of each, they can't connect
            //if (input == null || output == null) return false;
            //// Check type constraints
            //if (input.typeConstraint == XNode.Node.TypeConstraint.Inherited && !input.ValueType.IsAssignableFrom(output.ValueType)) return false;
            //if (input.typeConstraint == XNode.Node.TypeConstraint.Strict && input.ValueType != output.ValueType) return false;
            //// Success
            if (port.GetType() != GetType()) return false;
            return port.getIO() != getIO();
        }
        public void SetConstFlag() { }

        string m_strTips = "";
        public string GetTips()
        {
            return m_strTips;
        }
        public string SetTips(string name) { return m_strTips = name; }

        Attribute m_Attribute = null;
        public Attribute GetAttribute() { return m_Attribute; }
        public void SetAttribute(Attribute attri)
        {
            m_Attribute = attri;
        }
    }
    public class ExternPort : LinkPort
    {
        public System.Type reqNodeType;
        public Vector2 portRect;
        public int externID = 1;

        public List<GraphNode> vLinks = new List<GraphNode>();

        public override int GetGUID()
        {
            if (baseNode == null)
                return 0;
            if (baseNode.bindNode == null) return 0;
            return baseNode.GetGUID() * 10000000 + (((int)direction << 8 | externID));
        }
        
        public override Color GetColor()
        {
            return Color.green;
        }

        public override bool CanConnectTo(IPortNode port)
        {
            if (port.GetType() != typeof(LinkPort)) return false;
            return port.getIO() != getIO();
        }

        public override void ClearConnections()
        {
            vLinks.Clear();
        }
    }
}
#endif
