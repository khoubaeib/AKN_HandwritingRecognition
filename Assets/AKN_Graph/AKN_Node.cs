/// \file   Graph\AKN_Node.cs
///
/// \brief  Implements the itk node class. 
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace AKeNe
{
    /// \namespace  Graph
    ///
    /// \brief   AKeNe Graph.

    namespace Graph
    {
		
        /// \class  AKN_Node<T>
        ///
        /// \brief  Itk node that is connected< t>. 
        ///
        /// \author Cedric Plessiet Inrev Paris Viii
        /// \date   21/11/2009
        public class AKN_Node<T> : AKN_Node_Base
        {
            //private FSM _m_FSM;
            protected T _m_value;           ///< the value of the node 
            public T m_value
            {
                get
                {
                    return _m_value;
                }
                set
                {
                    _m_value = value;
                }
            }

            /// \fn public AKN_Node()
            ///
            /// \brief  Default constructor. 
            ///
            /// \author Michele
            /// \date   19/07/2011

            public AKN_Node()
                : this("")
            {
            }

            /// \fn public AKN_Node(string _label)
            ///
            /// \brief  Constructor. 
            ///
            /// \author Michele
            /// \date   19/07/2011
            ///
            /// \param  _label  The label. 

            public AKN_Node(string _label)
                : base(_label)
            {
            }

            /// \fn public AKN_Node(int _id,T _t)
            ///
            /// \brief  Constructor. 
            ///
            /// \author Cedric Plessiet Inrev Paris Viii
            /// \date   21/11/2009
            ///
            /// \param  _id The identifier. 
            /// \param  _t  The. 
            public AKN_Node(T _t)
                : this("",_t)
            {
            }
			
			public AKN_Node(string _label,T _t)
                : base(_label)
            {
                m_value = _t;
            }
			
            public AKN_Node(Guid _guid, string _label,T _t)
                : base(_guid,_label)
            {
                 m_value = _t;
            }

            public bool MEqualValue(T _t)
            {
                return m_value.Equals(_t);
            }
			
        }
    }
}