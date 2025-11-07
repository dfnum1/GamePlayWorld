#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

namespace Framework.Plugin.AT
{
    public interface IPortNode : IPortEditor
    {
        GraphNode GetNode();
        BaseNode GetATNode();
        IPortNode Connection();
        bool CanConnectTo(IPortNode port);
        bool IsConnectedTo(IPortNode port);
        int GetConnectionIndex(IPortNode port);
        IPortNode GetConnection(int i);
        void Disconnect(IPortNode port);

        bool IsConnected();
        void VerifyConnections();
        void Disconnect(int i);
        void ClearConnections();
        List<Vector2> GetReroutePoints(int index);

        void SetConstFlag();
        void SetFlag(EFlag flag, bool bSet);
        bool IsFlag(EFlag flag);

        void ReName();

        void Equivalence();

        void ApplayToDef();

        bool IsLocal();
        bool IsGlobal();
        void SetLocal(bool bSet);
        void SetGlobal(bool bSet);
    }

    public class PortUtil
    {
        public static Dictionary<int, IPortNode> allPortPositions = new Dictionary<int, IPortNode>();
        //------------------------------------------------------
        public static void Clear()
        {
            allPortPositions.Clear();
        }
        //------------------------------------------------------
        public static IPortNode GetPort(int guid)
        {
            IPortNode outPort;
            if (allPortPositions.TryGetValue(guid, out outPort))
                return outPort;
            return null;
        }
    }

    public class ConditionLinkPort : LinkPort
    {
        public PortalNode partalNode;
        public List<GraphNode> linkNodes = new List<GraphNode>();

        public override void Clear()
        {
            base.Clear();
            partalNode = null;
            linkNodes.Clear();
        }

        public override bool CanConnectTo(IPortNode port)
        {
            if (port.GetType() != typeof(LinkPort)) return false;
            return port.getIO() != getIO();
        }

        public override void ClearConnections()
        {
            linkNodes.Clear();
        }
        public override void ReName() { }

        public override bool IsLocal() { return false; }
        public override bool IsGlobal() { return false; }
        public override void SetLocal(bool bSet) { }
        public override void SetGlobal(bool bSet) { }
    }

    public class DelegateLinkPort : LinkPort
    {
        public VariableDelegate delegateVar;
        public List<GraphNode> linkNodes = new List<GraphNode>();

        public override void Clear()
        {
            base.Clear();
            delegateVar = null;
            linkNodes.Clear();
        }

        public override bool CanConnectTo(IPortNode port)
        {
            if (port.GetType() != typeof(LinkPort)) return false;
            return port.getIO() != getIO();
        }

        public override void ClearConnections()
        {
            linkNodes.Clear();
        }
        public override void ReName() { }

        public override Color GetColor()
        {
            return Color.yellow;
        }

        public override bool IsLocal() { return false; }
        public override bool IsGlobal() { return false; }
        public override void SetLocal(bool bSet) { }
        public override void SetGlobal(bool bSet) { }

        public override int GetGUID()
        {
            if (delegateVar == null)
                return 0;
            if (delegateVar == null) return 0;
            return delegateVar.GUID * 10000000 + (((int)direction << 8) | index);
        }

    }

    public class LinkPort : IPortNode
    {
        public EPortIO direction;
        public GraphNode baseNode;

        public int index;

        public Rect rect;
        public Rect view;

        public string strDefaultName = "";

        public List<IPortNode> ConnectionPorts = new List<IPortNode>();

        public EPortIO getIO() { return direction; }

        public virtual void Clear()
        {
            baseNode = null;
            index = 0;
            ConnectionPorts.Clear();
        }

        public string GetDefaultName()
        {
            return strDefaultName;
        }
        public void SetDefaultName(string strValue)
        {
            strDefaultName = strValue;
        }

        public BaseNode GetATNode()
        {
            if (baseNode == null) return null;
            return baseNode.BindNode;
        }

        public virtual int GetGUID()
        {
            if (baseNode == null)
                return 0;
            if (baseNode.EnterTask != null) return baseNode.TaskID;
            if (baseNode.BindNode == null) return 0;
            return baseNode.BindNode.GUID * 10000000 + (((int)direction << 8) | index);
        }

        public int GetLinkGUID()
        {
            return GetGUID();
        }

        public Rect GetRect() { return rect; }
        public void SetRect(Rect rc) { rect = rc; }

        public Rect GetViewRect() { return view; }
        public void SetViewRect(Rect rc) { view = rc; }


        public bool IsInput() { return (direction & EPortIO.In) != 0; }
        public bool IsOutput() { return (direction & EPortIO.Out) != 0; }

        public bool IsConnected()
        {
            return false;
        }

        public IPortNode Connection()
        {
            //for (int i = 0; i < connections.Count; i++)
            //{
            //    if (connections[i] != null) return connections[i].Port;
            //}
            return null;
        }

        public GraphNode GetNode()
        {
            return baseNode;
        }

        public virtual Color GetColor()
        {
            return Color.white;
        }

        public void VerifyConnections()
        {
            //for (int i = connections.Count - 1; i >= 0; i--)
            //{
            //    if (connections[i].node != null &&
            //        !string.IsNullOrEmpty(connections[i].fieldName) &&
            //        connections[i].node.GetPort(connections[i].fieldName) != null)
            //        continue;
            //    connections.RemoveAt(i);
            //}
        }

        public IPortNode GetConnection(int i)
        {
            IPortNode port = null;
            ////If the connection is broken for some reason, remove it.
            //if (connections[i].node == null || string.IsNullOrEmpty(connections[i].fieldName))
            //{
            //    connections.RemoveAt(i);
            //    return null;
            //}
            //NodePort port = connections[i].node.GetPort(connections[i].fieldName);
            //if (port == null)
            //{
            //    connections.RemoveAt(i);
            //    return null;
            //}
            return port;
        }

        /// <summary> Get index of the connection connecting this and specified ports </summary>
        public int GetConnectionIndex(IPortNode port)
        {
            //for (int i = 0; i < ConnectionCount; i++)
            //{
            //    if (connections[i].Port == port) return i;
            //}
            return -1;
        }

        public bool IsConnectedTo(IPortNode port)
        {
            //for (int i = 0; i < connections.Count; i++)
            //{
            //    if (connections[i].Port == port) return true;
            //}
            return false;
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
        public void Disconnect(IPortNode port)
        {
            //// Remove this ports connection to the other
            //for (int i = connections.Count - 1; i >= 0; i--)
            //{
            //    if (connections[i].Port == port)
            //    {
            //        connections.RemoveAt(i);
            //    }
            //}
            //if (port != null)
            //{
            //    // Remove the other ports connection to this port
            //    for (int i = 0; i < port.connections.Count; i++)
            //    {
            //        if (port.connections[i].Port == this)
            //        {
            //            port.connections.RemoveAt(i);
            //        }
            //    }
            //}
            //// Trigger OnRemoveConnection
            //node.OnRemoveConnection(this);
            //if (port != null) port.node.OnRemoveConnection(port);
        }

        /// <summary> Disconnect this port from another port </summary>
        public void Disconnect(int i)
        {
            //// Remove the other ports connection to this port
            //NodePort otherPort = connections[i].Port;
            //if (otherPort != null)
            //{
            //    for (int k = 0; k < otherPort.connections.Count; k++)
            //    {
            //        if (otherPort.connections[k].Port == this)
            //        {
            //            otherPort.connections.RemoveAt(i);
            //        }
            //    }
            //}
            //// Remove this ports connection to the other
            //connections.RemoveAt(i);

            //// Trigger OnRemoveConnection
            //node.OnRemoveConnection(this);
            //if (otherPort != null) otherPort.node.OnRemoveConnection(otherPort);
        }

        public virtual void ClearConnections()
        {
            //while (connections.Count > 0)
            //{
            //    Disconnect(connections[0].Port);
            //}
            if(GetNode() != null)
            {
                foreach (var db in GetNode().PrevLinks)
                {
                    db.NextLinks.Remove(GetNode());
                }
            }
        }

        /// <summary> Get reroute points for a given connection. This is used for organization </summary>
        public List<Vector2> GetReroutePoints(int index)
        {
            //return connections[index].reroutePoints;
            return new List<Vector2>();
        }

        public void SetConstFlag() { }
        public void SetFlag(EFlag flag, bool bSet) { }
        public bool IsFlag(EFlag flag) { return false; }
        public void Equivalence() { }

        public virtual void ReName() { }

        public void ApplayToDef() { }

        public virtual bool IsLocal() { return false; }
        public virtual bool IsGlobal() { return false; }
        public virtual void SetLocal(bool bSet) { }
        public virtual void SetGlobal(bool bSet) { }
    }
    public class ArgvPort : IPortNode
    {
        public EPortIO direction;
        public GraphNode baseNode;
        public PortalNode portalNode;
        public Port port;
        public int index;

        public bool inputToOutput;

        public System.Type alignType = null;
        public System.Type displayType = null;

        public Rect rect;
        public Rect view;

        public Variable variable
        {
            get { return port.variable; }
            set { port.variable = value; }
        }

        public string strDefaultName = "";

        public List<IPortNode> ConnectionPorts = new List<IPortNode>();
        public virtual void Clear()
        {
            portalNode = null;
            port = null;
            alignType = null;
            displayType = null;
            baseNode = null;
            index = 0;
            strDefaultName = null;
            ConnectionPorts.Clear();
        }
        public EPortIO getIO() { return direction; }

        public string GetDefaultName()
        {
            if (variable != null && !string.IsNullOrEmpty(variable.strName)) return variable.strName;
            return strDefaultName;
        }
        public void SetDefaultName(string strValue)
        {
            strDefaultName = strValue;
        }

        public int GetGUID()
        {
            if (baseNode == null || variable == null) return 0;
            return baseNode.GetGUID()*10000000 + variable.GUID*1000 + (((int)direction <<8)|index );
        }

        public int GetLinkGUID()
        {
            return GetGUID();
        }

        public Rect GetRect() { return rect; }
        public void SetRect(Rect rc) { rect = rc; }

        public Rect GetViewRect() { return view; }
        public void SetViewRect(Rect rc) { view = rc ; }


        public bool IsInput() { return (direction & EPortIO.In)!=0;  }
        public bool IsOutput() {  return (direction & EPortIO.Out)!=0;  }

        public bool IsConnected()
        {
            return false;
        }

        public IPortNode Connection()
        {
                //for (int i = 0; i < connections.Count; i++)
                //{
                //    if (connections[i] != null) return connections[i].Port;
                //}
                return null;
        }

        public GraphNode GetNode()
        {
            return baseNode;
        }

        public BaseNode GetATNode()
        {
            return variable;
        }

        public Color GetColor()
        {
            if(variable != null)
                return AgentTreePreferences.GetTypeColor(variable.GetType());
            return Color.white;
        }

        public void VerifyConnections()
        {
            //for (int i = connections.Count - 1; i >= 0; i--)
            //{
            //    if (connections[i].node != null &&
            //        !string.IsNullOrEmpty(connections[i].fieldName) &&
            //        connections[i].node.GetPort(connections[i].fieldName) != null)
            //        continue;
            //    connections.RemoveAt(i);
            //}
        }

        public IPortNode GetConnection(int i)
        {
            IPortNode port = null;
            ////If the connection is broken for some reason, remove it.
            //if (connections[i].node == null || string.IsNullOrEmpty(connections[i].fieldName))
            //{
            //    connections.RemoveAt(i);
            //    return null;
            //}
            //NodePort port = connections[i].node.GetPort(connections[i].fieldName);
            //if (port == null)
            //{
            //    connections.RemoveAt(i);
            //    return null;
            //}
            return port;
        }

        /// <summary> Get index of the connection connecting this and specified ports </summary>
        public int GetConnectionIndex(IPortNode port)
        {
            //for (int i = 0; i < ConnectionCount; i++)
            //{
            //    if (connections[i].Port == port) return i;
            //}
            return -1;
        }

        public bool IsConnectedTo(IPortNode port)
        {
            //for (int i = 0; i < connections.Count; i++)
            //{
            //    if (connections[i].Port == port) return true;
            //}
            return false;
        }

        public bool CanConnectTo(IPortNode port)
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
            return port.getIO() != getIO();
        }
        public void Disconnect(IPortNode port)
        {
            //// Remove this ports connection to the other
            //for (int i = connections.Count - 1; i >= 0; i--)
            //{
            //    if (connections[i].Port == port)
            //    {
            //        connections.RemoveAt(i);
            //    }
            //}
            //if (port != null)
            //{
            //    // Remove the other ports connection to this port
            //    for (int i = 0; i < port.connections.Count; i++)
            //    {
            //        if (port.connections[i].Port == this)
            //        {
            //            port.connections.RemoveAt(i);
            //        }
            //    }
            //}
            //// Trigger OnRemoveConnection
            //node.OnRemoveConnection(this);
            //if (port != null) port.node.OnRemoveConnection(port);
        }

        /// <summary> Disconnect this port from another port </summary>
        public void Disconnect(int i)
        {
            //// Remove the other ports connection to this port
            //NodePort otherPort = connections[i].Port;
            //if (otherPort != null)
            //{
            //    for (int k = 0; k < otherPort.connections.Count; k++)
            //    {
            //        if (otherPort.connections[k].Port == this)
            //        {
            //            otherPort.connections.RemoveAt(i);
            //        }
            //    }
            //}
            //// Remove this ports connection to the other
            //connections.RemoveAt(i);

            //// Trigger OnRemoveConnection
            //node.OnRemoveConnection(this);
            //if (otherPort != null) otherPort.node.OnRemoveConnection(otherPort);
        }

        public virtual void ClearConnections()
        {
            //while (connections.Count > 0)
            //{
            //    Disconnect(connections[0].Port);
            //}
            if (variable!=null && GetNode() != null && GetNode().BindNode != null && index>=0)
            {
                if(direction == EPortIO.In)
                {
                    if(port!=null)
                    {
                        if (port.dummyMap != null) port.dummyMap.Clear();
                        if (port.dummys != null) port.dummys.Clear();
                        if(port.taskDummyCatch!=null) port.taskDummyCatch.Clear();
                    }

                    GetNode().Editor.AdjustMaxGuid();
                    Variable newVar = AgentTreeManager.getInstance().GetVariableFactory().NewVariableByType(variable.GetType());
                    GetNode().BindNode.ReplaceInArgv(GetNode().Editor.GetCurrentAgentTree(), variable, newVar, index);
                }
            }
        }

        /// <summary> Get reroute points for a given connection. This is used for organization </summary>
        public List<Vector2> GetReroutePoints(int index)
        {
            //return connections[index].reroutePoints;
            return new List<Vector2>();
        }
        public void SetConstFlag()
        {
            if(variable != null)
            {
                variable.SetFlag(EFlag.Const, !variable.IsFlag(EFlag.Const));
            }
        }
        public void SetFlag(EFlag flag, bool bSet) 
        {
            if (variable != null)
            {
                variable.SetFlag(flag, bSet);
            }
        }
        public bool IsFlag(EFlag flag) 
        {
            if (variable != null)
            {
                return variable.IsFlag(flag);
            }
            return false; 
        }

        public void Equivalence()
        {
            if (variable != null)
            {
                GetNode().Editor.PopEquivalence(this);
            }
        }

        public void ApplayToDef()
        {
            if (variable != null)
                variable.Save();
        }

        public void ReName()
        {
            if (variable != null)
            {
                GetNode().Editor.VaribleReName(variable);
            }
        }

        public bool IsLocal() { return variable!=null? variable.IsFlag(EFlag.Local):false; }
        public bool IsGlobal() { return variable != null ? variable.IsFlag(EFlag.Global) : false; }
        public void SetLocal(bool bSet) 
        {
            if (variable == null) return;
            if(bSet)
            {
                variable.SetFlag(EFlag.Local, true);
                variable.SetFlag(EFlag.Global, false);
            }
            else
            {
                variable.SetFlag(EFlag.Local, false);
            }
        }
        public void SetGlobal(bool bSet) 
        {
            if (variable == null) return;
            if (bSet)
            {
                variable.SetFlag(EFlag.Global, true);
                variable.SetFlag(EFlag.Local, false);
            }
            else
            {
                variable.SetFlag(EFlag.Global, false);
            }
        }
    }

    public struct ReroutePort
    {
        public IPortNode port;
        public int connectionIndex;
        public int pointIndex;

        public ReroutePort(IPortNode port, int connectionIndex, int pointIndex)
        {
            this.port = port;
            this.connectionIndex = connectionIndex;
            this.pointIndex = pointIndex;
        }

        public void InsertPoint(Vector2 pos)
        {
            //port.GetReroutePoints(connectionIndex).Insert(pointIndex, pos);
        }
        public void SetPoint(Vector2 pos)
        {
            //port.GetReroutePoints(connectionIndex)[pointIndex] = pos;
        }
        public void RemovePoint()
        {
            //   port.GetReroutePoints(connectionIndex).RemoveAt(pointIndex);
        }
        public Vector2 GetPoint()
        {
            //   return port.GetReroutePoints(connectionIndex)[pointIndex];
            return Vector2.zero;
        }
    }
}
#endif
