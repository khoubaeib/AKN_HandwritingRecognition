/// \file   Graph\AKN_Node_Base.cs
///
/// \brief  Implements the AKN node base class. 

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AKeNe;
//using AKeNe.AI.Smart;
using System.Xml;

namespace AKeNe
{
    /// \namespace  Graph
    ///
    /// \brief   AKeNe Graph.

    namespace Graph
    {


        /// \class  AKN_Node_Base
        ///
        /// \brief  AKN node base, this class contains an A* graph finder. 
        ///
        /// \author Cedric Plessiet Inrev Paris Viii
        /// \date   21/11/2009
        public class AKN_Node_Base : AKN_Base
        {

            #region Attributes
            private Dictionary<Guid, AKN_Node_Base> _m_ConnectedNodeDict = new Dictionary<Guid, AKN_Node_Base>(); ///< the list of all the connected node
            private Dictionary<Guid, AKN_Node_Base> _m_InputConnectedNodeDict = new Dictionary<Guid, AKN_Node_Base>(); ///< the list of all the connected node
            private bool _m_isConnected;

            private bool _m_IsVisited = false;
            public bool m_IsVisited
            {
                get
                {
                    return _m_IsVisited;
                }
                set
                {
                    m_IsVisited = _m_IsVisited;
                }
            }

            private bool _m_hasInputConnection;
            public bool m_hasInputConnection
            {
                get
                {
                    return _m_hasInputConnection;
                }
            }

            public bool m_hasOutputConnection
            {
                get
                {
                    return (m_NbConnectedNode > 0);
                }
            }
            private Dictionary<Guid, AKN_GraphEdge> _m_linkedEdge = new Dictionary<Guid, AKN_GraphEdge>();


            public Dictionary<Guid, AKN_Node_Base> m_ConnectedNodeDict
            {
                get
                {
                    return _m_ConnectedNodeDict;
                }
            }

            public List<AKN_Node_Base> m_ConnectedNodes
            {
                get
                {
                    List<AKN_Node_Base> items = new List<AKN_Node_Base>();
                    items.AddRange(m_ConnectedNodeDict.Values);
                    return items;
                }
            }

            public Dictionary<Guid, AKN_Node_Base> m_InputConnectedNodeDict
            {
                get
                {
                    return _m_InputConnectedNodeDict;
                }
            }


            public Dictionary<Guid, AKN_GraphEdge> m_linkedEdge
            {
                get
                {
                    return _m_linkedEdge;
                }
            }
            /// \fn public int MGetNbConnectedNode()
            ///
            /// \brief  gets the number of connected node. 
            ///
            /// \author Cedric Plessiet Inrev Paris Viii
            /// \date   24/12/2009
            ///
            /// \return The nb connected node. 

            public bool m_IsConnected
            {
                get
                {
                    return _m_isConnected;
                }
            }

            public int m_NbConnectedNode
            {
                get
                {
                    return _m_ConnectedNodeDict.Count;
                }
            }


            public AKN_Node_Base this[int param]
            {
                get { return m_ConnectedNodes[param]; }
                set {
                    if (m_NbConnectedNode < param)
                        m_ConnectedNodes[param] = value;
                    else
                        MAddNode(value);
                }
            }

            public AKN_Node_Base m_parent
            {
                get
                {
                    return _m_parent;
                }
            }

            protected AKN_Node_Base _m_parent;
            #endregion

            #region Constructor
            /// \fn public AKN_Node_Base(int _id)
            ///
            /// \brief  Constructor. 
            ///
            /// \author Cedric Plessiet Inrev Paris Viii
            /// \date   21/11/2009
            ///
            /// \param  _id The identifier. 
            public AKN_Node_Base()
                : this("")
            {
            }

            public AKN_Node_Base(string _label)
                : this(AKeNe.Tool.AKN_IDGenerator.MGenerateNewID(), _label)
            {

            }

            public AKN_Node_Base(Guid _guid, string _label)
                : base(_guid, _label)
            {
                _m_isConnected = false;
                _m_hasInputConnection = false;
            }
			public AKN_Node_Base(params AKN_Node_Base[] _nodes):this("",_nodes)
			{
			}
			public AKN_Node_Base(string _label,params AKN_Node_Base[] _nodes):this(_label)
			{
				foreach (AKN_Node_Base node in _nodes)
				{
                    MAddChildNode(node);
				}
			}
            #endregion

            public bool MNodeIsConnectedTo(AKN_Node_Base _NodeToConnect)
            {
                return _m_ConnectedNodeDict.ContainsKey(_NodeToConnect.m_id);
            }


            /// \fn MConnectNode(AKN_Node_Base _nodeToConnect)
            /// 
            /// \brief  connect a node to another
            ///            
            /// \author Cedric Plessiet Inrev Paris Viii
            /// \date   21/11/2009
            /// 
            ///  \param _NodeToConnect  the node to connect

            public AKN_Status MConnectNode(AKN_Node_Base _nodeToConnect)
            {
                if (!MNodeIsConnectedTo(_nodeToConnect))
                {
                    _m_ConnectedNodeDict.Add(_nodeToConnect.m_id, _nodeToConnect);
                    _nodeToConnect._m_InputConnectedNodeDict[m_id] = this;
                    _nodeToConnect._m_hasInputConnection = true;
                    _m_isConnected = true;
                    return AKN_Status.m_Success;
                }
                else
                {
                    return AKN_Status.m_Failure;
                }
            }

            /// \fn MConnectNode(AKN_Node_Base _nodeToConnect)
            /// 
            /// \brief  connect a node to another
            ///            
            /// \author Cedric Plessiet Inrev Paris Viii
            /// \date   21/11/2009
            /// 
            ///  \param _NodeToConnect  the node to connect

            public AKN_Status MAddChildNode(AKN_Node_Base _nodeToConnect)
            {
                MConnectNode(_nodeToConnect);
                _nodeToConnect._m_parent = this;
                return AKN_Status.m_Success;
                
            }

            /// \fn MBiConnectNode(AKN_Node_Base _nodeToConnect)
            /// 
            /// \brief  connect a node to another and vice versa
            ///            
            /// \author Cedric Plessiet Inrev Paris Viii
            /// \date   21/11/2009
            /// 
            ///  \param _NodeToConnect  the node to connect

            public AKN_Status MBiConnectNode(AKN_Node_Base _nodeToConnect1)
            {
                if (!MConnectNode(_nodeToConnect1))
                    return AKN_Status.m_Failure;
                if (!_nodeToConnect1.MConnectNode(this))
                    return AKN_Status.m_Failure;
                return AKN_Status.m_Success;
            }


            /// \fn public AKN_Status MBiConnectNode(AKN_Node_Base _nodeToConnect1)
            ///
            /// \brief  connect a node to another and vice versa with an heuristic value 
            ///
            /// \author Alexandre Sambo
            /// \date   27/05/2010
            ///
            /// \param  _nodeToConnect1 The node to connect. 
            ///
            /// \return . 

            public AKN_Status MBiConnectNode(AKN_Node_Base _nodeToConnect1, float _cost)
            {
                AKN_Status newStatus = AKN_Status.m_Success;

                if (!MConnectNode(_nodeToConnect1))
                    newStatus = AKN_Status.m_Failure;
                if (!_nodeToConnect1.MConnectNode(this))
                    newStatus = AKN_Status.m_Failure;
                if (_cost < 0f) newStatus = AKN_Status.m_Failure;
                AKN_GraphEdge edge = new AKN_GraphEdge(this, _nodeToConnect1, _cost, "edge_" + this.m_label + "_" + _nodeToConnect1.m_label);
                m_linkedEdge[edge.m_id] = edge;
                _nodeToConnect1.m_linkedEdge[edge.m_id] = edge;

                return newStatus;
            }

            /// \fn     public AKN_Status MBiConnectNode(AKN_Node_Base _nodeToConnect1, float _cost, out AKN_GraphEdge _graphEdge)
            ///
            /// \brief  
            ///
            /// \author Alexandre Sambo
            /// \date   27/05/2010
            ///
            /// \param  _nodeToConnect1 The node to connect. 
            /// \param  _cost cost of connection (can be time, distance, ...)
            /// \param  _graphEdge return created graphEdge
            ///
            /// \return return  if the connection success or note
            public AKN_Status MBiConnectNode(AKN_Node_Base _nodeToConnect1, float _cost, out AKN_GraphEdge _graphEdge)
            {
                AKN_Status newStatus = AKN_Status.m_Success;

                if (!MConnectNode(_nodeToConnect1))
                    newStatus = AKN_Status.m_Failure;
                if (!_nodeToConnect1.MConnectNode(this))
                    newStatus = AKN_Status.m_Failure;
                if (_cost < 0f) newStatus = AKN_Status.m_Failure;
                _graphEdge = new AKN_GraphEdge(this, _nodeToConnect1, _cost, "edge_" + this.m_label + "_" + _nodeToConnect1.m_label);
                m_linkedEdge[_graphEdge.m_id] = _graphEdge;
                _nodeToConnect1.m_linkedEdge[_graphEdge.m_id] = _graphEdge;

                return newStatus;
            }

            public AKN_Status MDisconnectNode(AKN_Node_Base _nodeToConnect1)
            {
                if (MNodeIsConnectedTo(_nodeToConnect1))
                {
                    _nodeToConnect1.m_ConnectedNodeDict.Remove(m_id);
                    m_ConnectedNodeDict.Remove(_nodeToConnect1.m_id);
                    return AKN_Status.m_Success;
                }
                return AKN_Status.m_Failure;
            }

            /// \fn public AKN_Status MAddNode(AKN_Node_Base _node)
            ///
            /// \brief  Adds a node. 
            ///
            /// \author Ze Taupe
            /// \date   25/08/2012
            ///
            /// \param  _node   The node. 
            ///
            /// \return The calculated node. 
            public AKN_Status MAddNode(AKN_Node_Base _node)
            {
                if (!_m_ConnectedNodeDict.ContainsKey(_node.m_id))
                {
                    _m_ConnectedNodeDict.Add(_node.m_id, _node);
                    return AKN_Status.m_Success;
                }
                else
                {
                    return AKN_Status.m_Failure;
                }
            }

            /// \fn public AKN_Node_Base MGetNodeBase(Guid _key)
            ///
            /// \brief  gets a node base. 
            ///
            /// \author Michele
            /// \date   08/07/2011
            ///
            /// \param  _key    The key. 
            ///
            /// \return The node base. 

            public AKN_Node_Base MGetNodeBase(Guid _key)
            {
                return _m_ConnectedNodeDict[_key];
            }

            /// \fn public AKN_Node_Base MGetNodeBase(int _index)
            ///
            /// \brief  Get a node base. 
            ///
            /// \author Ze Taupe
            /// \date   25/08/2012
            ///
            /// \param  _index  Zero-based index of the. 
            ///
            /// \return The node base. 
            public AKN_Node_Base MGetNodeBase(int _index)
            {
                List<AKN_Node_Base> _nodeList = new List<AKN_Node_Base>(_m_ConnectedNodeDict.Values);
                if (_index < m_NbConnectedNode)
                {
                    return _nodeList[_index];
                }

                return null;
            }

            /// \fn public void MAddEdgeInLinckedEdge(AKN_GraphEdge _Edge)
            ///
            /// \brief  Connect link. 
            ///
            /// \author Ze Taupe
            /// \date   25/08/2012
            ///
            /// \param  _Edge   The edge. 
            public void MAddEdgeInLinckedEdge(AKN_GraphEdge _Edge)
            {
                this._m_linkedEdge.Add(_Edge.m_id, _Edge);
            }

        }
    }
}