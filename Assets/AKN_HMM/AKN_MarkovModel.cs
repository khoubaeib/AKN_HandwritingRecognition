using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Runtime.Serialization;
using AKeNe.Graph;

/// \namespace AKeNe.AI.HiddenMarkovModel
///
/// \brief  namespace that contains all HMM class's implementation.
///
/// \author Khoubaeib Klai | khoubaeib@gmail.com
/// \date   24/04/2014
///
/// \version 1.0
namespace AKeNe.AI.HiddenMarkovModel
{
    /// \class  AKN_HiddenMarkovModel<T>
    ///
    /// \brief  Build a Markov model.
    ///
    /// \author Khoubaeib Klai | khoubaeib@gmail.com
    /// \date   24/04/2014
    ///
    /// \version 1.0
    public class AKN_MarkovModel : AKN_Graph<AKN_State>
    {
        protected Guid[] _m_StatesGUID;
        protected Guid[] _m_TransitionsGUID;

        public int m_StatesCount { get { return _m_StatesGUID.Length; } }

        public AKN_State this[int _index]
        {
            get { return ((AKN_Node<AKN_State>)m_NodeDict[_m_StatesGUID[_index]]).m_value; }
            set
            {
                AKN_Node<AKN_State> nodeToSet = (AKN_Node<AKN_State>)_m_NodeDict[_m_StatesGUID[_index]].Clone();
                nodeToSet.m_value = value;
                _m_NodeDict[_m_StatesGUID[_index]] = nodeToSet;
            }
        }

        public AKN_State this[string _name]
        {
            get
            {
                for (int i = 0; i < m_StatesCount; i++) 
                    if (((AKN_Node<AKN_State>)m_NodeDict[_m_StatesGUID[i]]).m_value.m_name.Equals(_name)) return this[i];
                return null;
            }
            set
            {
                for (int i = 0; i < m_StatesCount; i++)
                    if (((AKN_Node<AKN_State>)m_NodeDict[_m_StatesGUID[i]]).m_value.m_name.Equals(_name)) { this[i] = value; return; }
            }
        }

        public float this[int _from, int _to]
        {
            get { return m_DicoEdge[_m_TransitionsGUID[(_from * m_StatesCount) + _to]].m_cost; }
            set { m_DicoEdge[_m_TransitionsGUID[(_from * m_StatesCount) + _to]].m_cost = value; }
        }

        public float this[string _from, string _to]
        {
            get
            {
                for (int i = 0; i < m_StatesCount; i++)
                    for (int j = 0; j < m_StatesCount; j++)
                        if (this[i].m_name.Equals(_from) && this[j].m_name.Equals(_to)) return this[i, j];
                return -1F;
            }
            set
            {
                for (int i = 0; i < m_StatesCount; i++)
                    for (int j = 0; j < m_StatesCount; j++)
                        if (this[i].m_name.Equals(_from) && this[j].m_name.Equals(_to)) { this[i, j] = value; return; }
            }
        }

        /// \fn     public AKN_MarkovModel(int _statesCount, Guid _guid, string _label)
        ///
        /// \brief  Markov Model constructor, Creates an Markov Model with a defined number of states, a gloabal identifier and a label.
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        ///
        /// \version 1.0
        ///
        /// \param _statesCount number of states in the MM.
        /// \param _guid        unique identifier of the Markov model
        /// \param _label       model's name
        public AKN_MarkovModel(int _statesCount, Guid _guid, string _label) : base(_guid, _label)
        {
            _MInitStates(_statesCount);
            _MInitTransitions(_statesCount);
        }

        /// \fn     public AKN_MarkovModel(int _statesCount, string _label)
        ///
        /// \brief  Markov Model constructor, Creates an Markov Model with a defined number of states and a label.
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        ///
        /// \version 1.0
        ///
        /// \param _statesCount number of states in the MM.
        /// \param _label       model's name
        public AKN_MarkovModel(int _statesCount, string _label) : this(_statesCount, Guid.Empty, _label)
        {

        }

        /// \fn     public AKN_MarkovModel(int _statesCount)
        ///
        /// \brief  Markov Model constructor, Creates an Markov Model with a defined number of states.
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        ///
        /// \version 1.0
        ///
        /// \param  _statesCount number of states in the MM.
        public AKN_MarkovModel(int _statesCount)  : this(_statesCount , "")
        {

        }

        /// \fn     public void _MInitStates(int _statesCount)
        ///
        /// \brief  Inits Markov model states.
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        ///
        /// \version 1.0
        ///
        /// \param  _statesCount number of states in the MM.
        private void _MInitStates(int _statesCount)
        {
            _m_StatesGUID = new Guid[_statesCount];
            for (int i = 0; i < _statesCount; i++)
            {
                AKN_Node<AKN_State> state = new AKN_Node<AKN_State>();
                state.m_value = new AKN_State(0, "");
                _m_StatesGUID[i] = state.m_id;
                MAddNode(state);
            }
        }

        /// \fn     public void _MInitTransitions(int _statesCount)
        ///
        /// \brief  Inits Markov model transitions.
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        ///
        /// \version 1.0
        ///
        /// \param  _statesCount number of states in the MM.
        private void _MInitTransitions(int _statesCount)
        {
            _m_TransitionsGUID = new Guid[_statesCount * _statesCount];
            for (int i = 0; i < _statesCount; i++)
            {
                for (int j = 0; j < _statesCount; j++)
                {
                    AKN_GraphEdge _graphEdge = new AKN_GraphEdge(_m_NodeDict[_m_StatesGUID[i]], _m_NodeDict[_m_StatesGUID[j]], 0.0F);
                    _m_NodeDict[_m_StatesGUID[i]].MBiConnectNode(_m_NodeDict[_m_StatesGUID[j]], 0.0F, out _graphEdge);
                    m_DicoEdge[_graphEdge.m_id] = _graphEdge;
                    _m_TransitionsGUID[(i * _statesCount) + j] = _graphEdge.m_id;
                }
            }
        }

        /// \fn     public void MSetStates(float[] _startProbabilities)
        ///
        /// \brief  Sets the Markov model's states by indicating their starts probabilities (names are set automatically).
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        ///
        /// \version 1.0
        ///
        /// \param  statesCount number of states in the MM.
        public void MSetStates(float[] _startProbabilities)
		{
            String[] names = new String[m_StatesCount];
            for (int i = 0; i < m_StatesCount; i++) names[i] = "State_" + i;
            MSetStates(_startProbabilities, names);
		}

        /// \fn     public void MSetStates(float[] _startProbabilities, String[] _names)
        ///
        /// \brief  Sets the Markov model's states by indicating their names and their starts probabilities.
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        ///
        /// \version 1.0
        ///
        /// \param  startProbabilities float table that contains starting probability of each state
        /// \param  names              String table that contains state's name.
        public void MSetStates(float[] _startProbabilities, String[] _names)
        {
            if (_startProbabilities.Length != m_StatesCount || _names.Length != m_StatesCount) return;
            for (int j = 0; j < m_StatesCount; j++)
                this[j] = new AKN_State(_startProbabilities[j], this[j].m_name == "" ? _names[j] : this[j].m_name);
        }

        /// \fn     public void MSetStates(AKN_State[] _states)
        ///
        /// \brief  Sets the Markov model's states.
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        ///
        /// \version 1.0
        ///
        /// \param  statesCount number of states in the MM.
        public void MSetStates(AKN_State[] _states)
        {
            float[] sp = new float[_states.Length];
            String[] names = new String[_states.Length];

            for (int i = 0; i < _states.Length; i++) { sp[i] = _states[i].m_startProbability; names[i] = _states[i].m_name; }
            MSetStates(sp, names);
        }

        /// \fn     public void MSetTransition(int _from, int _to, float _probability)
        ///
        /// \brief  Sets a transition's probability.
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        ///
        /// \version 1.0
        ///
        /// \param  from        index of state from .
        /// \param  to          index of state to.
        /// \param  probability transition's new probability.
        public void MSetTransition(int _from, int _to, float _probability)
        {
            this[_from, _to] = _probability;
        }

        /// \fn     public float MGetTransition(int from, int to)
        ///
        /// \brief  Gets transition's probability.
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        ///
        /// \version 1.0
        ///
        /// \param  from        index of state from .
        /// \param  to          index of state to.
        ///
        /// \return transition's probability.
        public float MGetTransition(int from, int to)
        {
            return this[from, to];
        }

        /// \fn     public void MSetTransition(int _index, float _probability)
        ///
        /// \brief  Sets a transition's probability by indicating index in one dimensions array .
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        ///
        /// \version 1.0
        ///
        /// \param  _index      index of the transition .
        /// \param  probability transition's new probability.
        public void MSetTransition(int _index, float _probability)
        {
            m_DicoEdge[_m_TransitionsGUID[_index]].m_cost = _probability;
        }

        /// \fn     public float MGetTransition(int _index)
        ///
        /// \brief  Gets transition's probability.
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        ///
        /// \version 1.0
        ///
        /// \param  _index index of the transition .
        ///
        /// \return transition's probability.
        public float MGetTransition(int _index)
        {
            return m_DicoEdge[_m_TransitionsGUID[_index]].m_cost;
        }

        /// \fn     public void MSetTransition(String _from, String _to, float _probability)
        ///
        /// \brief  Sets an observation's value
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        ///
        /// \version 1.0
        ///
        /// \param  _from         name of state from .
        /// \param  _to           name of state to.
        /// \param  _probability transition's new probability.
        public void MSetTransition(String _from, String _to, float _probability)
        {
            this[_from, _to] = _probability;
        }

        /// \fn     public void MSetTransitions(float[,] _transitions) 
        ///
        /// \brief  Sets all the transition in the MM.
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        ///
        /// \version 1.0
        ///
        /// \param  transitions 2D matrix that contains all transitions's probablities
        public void MSetTransitions(float[,] _transitions) 
        {
            if ((_transitions.GetLength(0) != m_StatesCount) || (_transitions.GetLength(1) != m_StatesCount)) return;
            for (int i = 0; i < m_StatesCount; i++) for (int j = 0; j < m_StatesCount; j++) this[i, j] = _transitions[i, j];
        }

    }
}