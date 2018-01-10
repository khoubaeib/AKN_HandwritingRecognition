/// \file   Graph\AKN_Graph_Base.cs
///
/// \brief  Implements the itk graph base class. 

using AKeNe.Graph;
using AKeNe.Tool;
using System.Linq;
using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Reflection;
using System.IO;

namespace AKeNe
{
    /// \namespace  Graph
    ///
    /// \brief   AKeNe Graph.

    namespace Graph
    {
        /// \fn public delegate bool DAccesibilty(InrevToolKit.graph.AKN_Node_Base _first,
        ///     InrevToolKit.graph.AKN_Node_Base _second)
        ///
        /// \brief  the accessibility rule for the graph generation. This class contains a DFS Graph finder and an A* Graph finder.
        ///
        /// \author Cedric Plessiet Inrev Paris Viii
        /// \date   21/11/2009
        ///
        /// \param  _first  The first node to check if connectable.
        /// \param  _second The second node to check if connectable. 
        ///
        /// \return true if it succeeds, false if it fails. 
        public delegate bool DAccesibilty(AKN_Node_Base _first, AKN_Node_Base _second);
        /// \class  AKN_Graph_Base
        ///
        /// \brief  Itk graph base. 
        ///
        /// \author Cedric Plessiet Inrev Paris Viii
        /// \date   21/11/2009
        public class AKN_Graph_Base : AKN_Base
        {
            #region Attributes

            ///< the list of all the node of our graph
            protected Dictionary<Guid, AKN_Node_Base> _m_NodeDict;
            ///< the list of the m_dicoEdge. 
            private Dictionary<Guid, AKN_GraphEdge> _m_DicoEdge;
            /// < the node which will be connected to an other graph's node
            private AKN_Node_Base _m_linkinNode;

            public int m_NodeCount
            {
                get
                {
                    return _m_NodeDict.Count;
                }
            }

            public Dictionary<Guid, AKN_Node_Base> m_NodeDict
            {
                get
                {
                    return _m_NodeDict;
                }
            }

            public AKN_Node_Base m_linkinNode
            {
                get
                {
                    return _m_linkinNode;
                }
            }

            public Dictionary<Guid, AKN_GraphEdge> m_DicoEdge
            {
                get
                {
                    return _m_DicoEdge;
                }
            }

            #endregion


            #region  Constructor

            /// \summary    Initializes a new instance of the <see cref="T:System.Object" /> class. 
            /// \fn public AKN_Graph_Base()
            ///
            /// \brief  Default constructor. 
            ///
            /// \author Cedric Plessiet Inrev Paris Viii
            /// \date   21/11/2009
            public AKN_Graph_Base()
                : this("")
            {

            }

            public AKN_Graph_Base(Guid _guid, string _label)
                : base(_guid, _label)
            {
                _m_NodeDict = new Dictionary<Guid, AKN_Node_Base>();
                _m_DicoEdge = new Dictionary<Guid, AKN_GraphEdge>();
                _m_linkinNode = null;
            }
			

            public AKN_Graph_Base(string _label)
                : this(AKeNe.Tool.AKN_IDGenerator.MGenerateNewID(), _label)
            {
            }

            #endregion

            public void MClear()
            {
                _m_NodeDict.Clear();
                _m_DicoEdge.Clear();

            }
            /// \fn public Dictionary<Guid, AKN_Node_Base> MGetEdgeNode()
            ///
            /// \brief  gets the connectedNode
            ///
            /// \author Michele
            /// \date   13/07/2011
            ///
            /// \return The edge node. 
            //TODO: perhaps we nee to rename it like MGetConnectedNode
            public Dictionary<Guid, AKN_Node_Base> MGetEdgeNode()
            {
                Dictionary<Guid, AKN_Node_Base> nodeConnectedInEdgeGraph = new Dictionary<Guid, AKN_Node_Base>();
                foreach (KeyValuePair<Guid, AKN_Node_Base> kvp in _m_NodeDict)
                {
                    if (_m_NodeDict[kvp.Key].m_IsConnected)
                    {
                        nodeConnectedInEdgeGraph.Add(_m_NodeDict[kvp.Key].m_id, _m_NodeDict[kvp.Key]);
                    }
                }
                return nodeConnectedInEdgeGraph;
            }

            /// \fn public void MAddNode(AKN_Node_Base _node)
            ///
            /// \brief  Adds a node. 
            ///
            /// \attention modified as virtual by paulhiac.cyrille
            /// \author Cedric Plessiet Inrev Paris Viii
            /// \author cyr.paulhiac@gmail.com
            /// \date   24/12/2009
            ///
            /// \param  _node   The node. 
            virtual public AKN_Status MAddNode(AKN_Node_Base _node) //GON_MOD:  public void MAddNode(AKN_Node_Base _node)
            {
                if (_m_NodeDict.ContainsKey(_node.m_id))
                    return AKN_Status.m_Failure;
                _m_NodeDict.Add(_node.m_id, _node);
                return AKN_Status.m_Success;
            }

            /// \fn public AKN_Node_Base MGetNode(Guid _key)
            ///
            /// \brief  get the node. 
            ///
            /// \author cedric plessiet
            /// \date   24/05/2010
            ///
            /// \param  _key    The key. 
            ///
            /// \return The node. 

            public AKN_Node_Base MGetNode(Guid _key)
            {
                if (_m_NodeDict.ContainsKey(_key))
                {
                    return _m_NodeDict[_key];
                }
                return null;
            }

            /// \fn public AKN_Node_Base MGetNode(int _index)
            ///
            /// \brief  get the node at index. 
            ///
            /// \author cedric plessiet
            /// \date   24/05/2010
            ///
            /// \param  _index  Zero-based index of the. 
            ///
            /// \return The node at index. 

            public AKN_Node_Base MGetNode(int _index)
            {
                if (_index > m_NodeCount)
                {
                    return null;
                }
                List<AKN_Node_Base> nodeList = new List<AKN_Node_Base>(_m_NodeDict.Values);
                return nodeList[_index];
            }

            /// \fn public bool MIsInsideGraph(AKN_Node_Base _toCheck)
            ///
            /// \brief  Is inside graph. 
            ///
            /// \author Cedric Plessiet Inrev Paris Viii
            /// \date   24/12/2009
            ///
            /// \param  _toCheck    to check. 
            ///
            /// \return true if inside graph, false if not. 
            public bool MIsInsideGraph(AKN_Node_Base _toCheck)
            {
                return _m_NodeDict.ContainsKey(_toCheck.m_id);
            }

            #region SEEK
            /// \fn public AKN_Node_Base MSeek(string _label)
            ///
            /// \brief  Seek a labeled node. 
            ///
            /// \author Michele
            /// \date   13/07/2011
            ///
            /// \param  _label  The label. 
            ///
            /// \return a kvp.value. 

            public AKN_Node_Base MSeek(string _label)
            {
                foreach (KeyValuePair<Guid, AKN_Node_Base> kvp in _m_NodeDict)
                {
                    if (kvp.Value.m_label == _label)
                        return kvp.Value;
                }

                return null;
            }

            /// \fn public List<AKN_Node_Base> MSeekAll(string _label)
            ///
            /// \brief  Seek all. 
            ///
            /// \author Michele
            /// \date   13/07/2011
            ///
            /// \param  _label  The label. 
            ///
            /// \return the list. 

            public List<AKN_Node_Base> MSeekAll(string _label)
            {
                List<AKN_Node_Base> list = new List<AKN_Node_Base>();
                foreach (KeyValuePair<Guid, AKN_Node_Base> kvp in _m_NodeDict)
                {
                    if (kvp.Value.m_label == _label)
                        list.Add(kvp.Value);
                }
                return list;
            }

            #endregion



            /// \fn public void MAddNodeToCurrent(AKN_Node_Base _node)
            ///
            /// \brief  Adds a node to the current observed node . 
            ///
            /// \author cedric plessiet
            /// \date   24/05/2010
            ///
            /// \param  _node   The node. 
            /// 
            /// FIXME: A MCreateNodeObserver needs to be created to have this function working!(selon Cédric)


            public AKN_Status MAddNodeToCurrent(AKN_Node_Base _node)
            {
                if (_m_NodeDict.ContainsKey(_node.m_id))
                    return AKN_Status.m_Failure;
                _m_NodeDict[_node.m_id] = _node;

                return AKN_Status.m_Success;
            }


            /// \fn public AKN_Status MAddAsLinkinNode(AKN_Node_Base _node)
            ///
            /// \brief  Adds as linkin node. 
            ///
            /// \author Alexandre Sambo
            /// \date   04/06/2010
            ///
            /// \param  _node   The node. 
            ///
            /// \return The calculated as linkin node. 
            //FIXME: WTF??????
            public AKN_Status MAddAsLinkinNode(AKN_Node_Base _node)
            {
                _m_linkinNode = _node;
                return MAddNode(_node);
            }

        }
    }
}