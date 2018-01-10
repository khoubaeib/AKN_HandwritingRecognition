using System;
using System.Collections.Generic;
using AKeNe.Graph;
using AKeNe;

namespace AKeNe
{
    /// \namespace  Graph
    ///
    /// \brief   AKeNe Graph.

 	namespace Graph
	{
        /// \fn public delegate void DEgdgeFunction()
        ///
        /// \brief  Egdge function. 
        /// Use this if you want to allocate a behavior of your object when the Edge is use.
        ///
        /// \author Samy
        /// \date   04/06/2010     
        //public delegate void DEgdgeFunction();
        public class AKN_GraphEdge : AKN_Base
        {

            
            private float _m_cost;

             private AKN_Node_Base _m_NodeTo;
             public AKN_Node_Base m_nodeTo
             {
                 get
                 {
                     return _m_NodeTo;
                 }
             }
            /// \class  AKN_GraphEdge
            ///
            /// \brief  Itk graph edge. 
            ///,
            /// \author Samy
            /// \date   23/04/2010
            /// \Constructor
            public AKN_GraphEdge(AKN_Node_Base _NodeToConnect1, AKN_Node_Base _NodeToConnect2)
                : this(_NodeToConnect1,_NodeToConnect2,0f)
            {
                //AKN_Debug.MSetLogMode(AKN_Debug.eLogMode.kVerboseLogMode);
                //AKN_Debug.MSetOutputMode(AKN_Debug.eOutputMode.kConsoleOutputMode);
            }
             
            /// \fn public AKN_GraphEdge(AKN_Node_Base _node1, AKN_Node_Base _node2, float _cost)
            ///
            /// \brief  Constructor. 
            ///
            /// \author Alexandre Sambo
            /// \date   01/06/2010
            ///
            /// \param  _node1  The first node. 
            /// \param  _node2  The second node. 
            /// \param  _cost   The cost. 
            public AKN_GraphEdge(AKN_Node_Base _node1, AKN_Node_Base _node2, float _cost)
                : this(_node1,_node2,_cost,"")
            {
                //AKN_Debug.MSetLogMode(AKN_Debug.eLogMode.kVerboseLogMode);
                //AKN_Debug.MSetOutputMode(AKN_Debug.eOutputMode.kConsoleOutputMode);
                if(_node1.MNodeIsConnectedTo(_node2))
                {
                    m_cost = _cost;
                }
            }
             
            /// \fn public AKN_GraphEdge(AKN_Node_Base _node1, AKN_Node_Base _node2, float _cost,
            ///     String _label)
            ///
            /// \brief  Constructor. 
            ///
            /// \author Alexandre Sambo
            /// \date   01/06/2010
            ///
            /// \param  _node1  The first node. 
            /// \param  _node2  The second node. 
            /// \param  _cost   The cost. 
            /// \param  _label  The label. 
            public AKN_GraphEdge(AKN_Node_Base _node1, AKN_Node_Base _node2, float _cost, String _label)
                : base(_label)
            
            {
                //AKN_Debug.MSetLogMode(AKN_Debug.eLogMode.kVerboseLogMode);
                //AKN_Debug.MSetOutputMode(AKN_Debug.eOutputMode.kConsoleOutputMode);
                if (_node1.MNodeIsConnectedTo(_node2))
                {
                    m_cost = _cost;
                }
            }

            /// \property   public float m_cost
            ///
            /// \brief  Gets the cost. 
            ///
            /// \return The m cost. 
            public float m_cost
            {
                get
                {
                    return _m_cost;
                }
                set
                {
                    _m_cost = value;
                }
            }
             
            
            public void MEdgeConnect(AKN_Node_Base _nodeToConnect1, AKN_Node_Base _nodeToConnect2)
            {
                _nodeToConnect1.MConnectNode(_nodeToConnect2);
                _m_NodeTo = _nodeToConnect2; ;
            }
           

        }
	}
}