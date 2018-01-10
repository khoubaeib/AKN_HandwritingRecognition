/// \file   Graph\AKN_Graph.cs
///
/// \brief  Implements the itk graph class. 

using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace AKeNe
{
    /// \namespace  Graph
    ///
    /// \brief   AKeNe Graph.

    namespace Graph
    {
        /// \class  AKN_Graph<T>
        ///
        /// \brief  Itk graph< t>. 
        ///
        /// \author Cedric Plessiet Inrev Paris Viii
        /// \date   21/11/2009
        public class AKN_Graph<T> : AKN_Graph_Base
        {
            /// \fn public AKN_Graph()
            ///
            /// \brief  Default constructor. 
            ///
            /// \author Cedric Plessiet Inrev Paris Viii
            /// \date   21/11/2009
            public AKN_Graph()
                : this("")
            {
            }

            /// \fn public AKN_Graph(string _label)
            ///
            /// \brief  Constructor. 
            ///
            /// \author Cedric Plessiet Cedric.plessiet Univ Paris 8.fr
            /// \date   12/02/2010
            ///
            /// \param  _label  The label. 
            public AKN_Graph(string _label)
                : base(_label)
            {

            }

            public AKN_Graph(Guid _guid, string _label)
                : base(_guid, _label)
            {
            }

            /// \fn public List<AKN_Node<T>> MFindAllNodeWithValue(T _t)
            ///
            /// \brief  Searches for all node with the given t. 
            ///
            /// \author Ze Taupe
            /// \date   13/07/2011
            ///
            /// \param  _t  The value to Find. 
            ///
            /// \return the list of all the node with this value. 
            public List<AKN_Node<T>> MFindAllNodeWithValue(T _t)
            {
                List<AKN_Node<T>> toReturn = new List<AKN_Node<T>>();
                foreach (KeyValuePair<Guid, AKN_Node_Base> kvp in _m_NodeDict)
                {
                  AKN_Node<T> toCheck = kvp.Value as AKN_Node<T>;
				  if (toCheck.m_value.Equals(_t))
                  {
                      toReturn.Add(toCheck);
                  }
                }
                return toReturn;
            }

            /// \fn public AKN_Node<T> MAddNode(T _value)
            ///
            /// \brief  create a node with the value T and connect it. 
            ///
            /// \author Ze Taupe
            /// \date   13/07/2011
            ///
            /// \param  _value  The value. 
            ///
            /// \return The node added. 
            public AKN_Node<T> MAddNode(T _value)
            {
                 AKN_Node<T> nodeToAdd = new AKN_Node<T>(_value);
                 _m_NodeDict.Add(nodeToAdd.m_id, nodeToAdd);

                 return nodeToAdd;
            }

            /// \fn public T MGetValueAt(int _index)
            ///
            /// \brief  get the value at index. 
            ///
            /// \author Ze Taupe
            /// \date   13/07/2011
            ///
            /// \param  _index  Zero-based index of the. 
            ///
            /// \return The value at index. 
            public T MGetValueAt(int _index)
            {
                AKN_Node<T> nodeBuffer = MGetNode(_index) as AKN_Node<T>;
                
                return nodeBuffer.m_value;
            }
        }
    }
}