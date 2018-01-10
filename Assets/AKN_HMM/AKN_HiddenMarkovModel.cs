using System;
using AKeNe.Graph;
using UnityEngine;

/// \namespace AKeNe.AI.HiddenMarkovModel
/// 
/// \brief namespace that contains all HMM class's implementation.
///
/// \author Khoubaeib Klai | khoubaeib@gmail.com
/// \date   24/04/2014
namespace AKeNe.AI.HiddenMarkovModel
{
    /// \enum   eUseAlgorithme
    ///
    /// \brief  Algorithme to use to evaluate a given observations sequence
    ///
    /// \author Khoubaeib Klai | khoubaeib@gmail.com
    /// \date   24/04/2014
    public enum eUseAlgorithme 
    {
        //! use forward algorithme to evaluate observations.
        kForward,
        //! use forward algorithme to evaluate observations.
        kBackward,
        //! use forward and backward algorithmes at the same time to evaluate observations.
        kForwardAndBackward 
    };

    /// \class  AKN_HiddenMarkovModel<T>
    ///
    /// \brief  Build a hidden Markov model with a specified <T> type of observations.
    ///
    /// \author Khoubaeib Klai | khoubaeib@gmail.com
    /// \date   24/04/2014
    public class AKN_HiddenMarkovModel<T> : AKN_MarkovModel
    {
        protected Guid[] _m_ObservationsGUID;
        protected Guid[] _m_EmissionsGUID;

        public int m_ObservationsCount { get { return _m_ObservationsGUID.Length; } }
 
        /// \fn     public AKN_HiddenMarkovModel(int _statesCount, int _observationsCount, Guid _id, string _label)
        ///
        /// \brief  HMM constructor, Creates an HMM with a defined numbers of states and observations.
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        ///
        /// \param  _statesCount       number of states in the HMM.
        /// \param  _observationsCount number of observations in the HMM.
        /// \param  _id                unique identifier of the hidden Markov model
        /// \param  _label             model's name
        public AKN_HiddenMarkovModel(int _statesCount, int _observationsCount, Guid _id, string _label) : base(_statesCount, _id, _label)
        {
            _MInitObservations(_observationsCount);
            _MInitEmissions(_statesCount, _observationsCount);
        }

        /// \fn     public AKN_HiddenMarkovModel(int _statesCount, int _observationsCount, string _label)
        ///
        /// \brief  HMM constructor, Creates an HMM with a defined numbers of states and observations.
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        ///
        /// \param  _statesCount       number of states in the HMM.
        /// \param  _observationsCount number of observations in the HMM.
        /// \param  _label             model's name
        public AKN_HiddenMarkovModel(int _statesCount, int _observationsCount, string _label) : this(_statesCount, _observationsCount, Guid.Empty, _label)
        {

        }

        /// \fn     public AKN_HiddenMarkovModel(int _statesCount, int _observationsCount)
        ///
        /// \brief  HMM constructor, Creates an HMM with a defined numbers of states and observations.
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        ///
        /// \param  _statesCount       number of states in the HMM.
        /// \param  _observationsCount number of observations in the HMM.
        public AKN_HiddenMarkovModel(int _statesCount, int _observationsCount) : this(_statesCount, _observationsCount, "")
        {

        }

        /// \fn     public AKN_HiddenMarkovModel(int _statesCount, int _observationsCount)
        ///
        /// \brief  HMM constructor, Creates an HMM with a defined numbers of states and observations.
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        ///
        /// \param  _statesCount       number of states in the HMM.
        /// \param  _observationsCount number of observations in the HMM.
        private void _MInitObservations(int _observationsCount) 
        {
            _m_ObservationsGUID = new Guid[_observationsCount];
            for (int i = 0; i < _observationsCount; i++)
            {
                AKN_Node<T> observation = new AKN_Node<T>();
                _m_ObservationsGUID[i] = observation.m_id;
                MAddNode(observation);
            }
        }

        /// \fn     public AKN_HiddenMarkovModel(int _statesCount, int _observationsCount)
        ///
        /// \brief  HMM constructor, Creates an HMM with a defined numbers of states and observations.
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        ///
        /// \param  _statesCount       number of states in the HMM.
        /// \param  _observationsCount number of observations in the HMM.
        private void _MInitEmissions(int _statesCount, int _observationsCount)
        {
            _m_EmissionsGUID = new Guid[_statesCount * _observationsCount];
            for (int i = 0; i < _statesCount; i++)
            {
                for (int j = 0; j < _observationsCount; j++)
                {
                    AKN_GraphEdge _graphEdge = new AKN_GraphEdge(_m_NodeDict[_m_StatesGUID[i]], _m_NodeDict[_m_ObservationsGUID[j]], 0.0F);
                    _m_NodeDict[_m_StatesGUID[i]].MBiConnectNode(_m_NodeDict[_m_ObservationsGUID[j]], 0.0F, out _graphEdge);
                    m_DicoEdge[_graphEdge.m_id] = _graphEdge;
                    _m_EmissionsGUID[(i * _observationsCount) + j] = _graphEdge.m_id;
                }
            }
        }

        /// \fn     public void MSetObservation(int _index, T _observation)
        ///
        /// \brief  Sets an observation's value
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        ///
        /// \param  _index       the concerned observation.
        /// \param  _observation observation's new value.
        public void MSetObservation(int _index, T _observation)
        {
            AKN_Node<T> node = (AKN_Node<T>)(_m_NodeDict[_m_ObservationsGUID[_index]].Clone());
            node.m_value = _observation;
            _m_NodeDict[_m_ObservationsGUID[_index]] = node;
        }

        /// \fn     public T MGetObservation(int _index)
        ///
        /// \brief  Sets an observation's value
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        ///
        /// \param  _index       the concerned observation.
        ///
        /// \return observation's value.
        public T MGetObservation(int _index)
        {
            return ((AKN_Node<T>)_m_NodeDict[_m_ObservationsGUID[_index]]).m_value;
        }

        /// \fn     public void MSetObservations(T[] _observations)
        ///
        /// \brief  Sets a range of observation's value
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        ///
        /// \param  _observations table that contains new observation's value, table length must match  with the number of observations.
        public void MSetObservations(T[] _observations)
        {
            for (int i = 0; i < _observations.Length; i++) MSetObservation(i, _observations[i]);
        }

        /// \fn     public void MSetObservations(T[] _observations, AKN_State[] _states)
        ///
        /// \brief  Sets a range of observations and states.
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        ///
        /// \param  _observations table that contains new observations's value, table length must match with the number of observations in the HMM.
        /// \param  _states       table that contains new states's value, table length must match with the number of states in the HMM.
        public void MSetObservations(T[] _observations, AKN_State[] _states)
        {
            if (_observations.Length != m_ObservationsCount) return;
            for (int i = 0; i < m_ObservationsCount; i++) MSetObservation(i, _observations[i]);
            for (int i = 0; i < m_StatesCount; i++) this[i].m_name = _states[i].m_name;
        }

        /// \fn     public float MGetEmission(String _state, T _observation)
        ///
        /// \brief  Sets an emission's probability
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        ///
        /// \param  _state        name of concerned state.
        /// \param  _observation  index of concerned observation.
        ///
        /// \return Emission's value
        public float MGetEmission(String _state, T _observation)
        {
            for (int i = 0; i < m_StatesCount; i++)
                for (int j = 0; j < m_ObservationsCount; j++)
                    if (_state == this[i].m_name && _observation.Equals(MGetObservation(j))) return MGetEmissionByIndex(i, j);
            return -1F;
        }

        /// \fn     public void MSetEmission(String _state, T _observation, float _value)
        ///
        /// \brief  Sets an emission's probability
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        ///
        /// \param  _state        name of concerned state.
        /// \param  _observation  index of concerned observation.
        /// \param  _emission     new probablity.
        public void MSetEmission(String _state, T _observation, float _value)
        {
            for (int i = 0; i < m_StatesCount; i++)
                for (int j = 0; j < m_ObservationsCount; j++)
                    if (_state == this[i].m_name && _observation.Equals(MGetObservation(j))) { MSetEmissionByIndex(i, j, _value); return; }
            throw new NullReferenceException();
        }

        /// \fn     public float MGetEmission(int _state, T _observation)
        ///
        /// \brief  Gets an emission's probability
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        ///
        /// \param  _state        name of concerned state.
        /// \param  _observation  index of concerned observation.
        ///
        /// \return Emission's value
        public float MGetEmission(int _state, T _observation)
        {
            for (int i = 0; i < m_ObservationsCount; i++) if (_observation.Equals(MGetObservation(i))) return MGetEmissionByIndex(_state, i);
            return -1F;
        }

        /// \fn     public void MSetEmission(int _state, T _observation, float _value)
        ///
        /// \brief  Sets an emission's probability
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        ///
        /// \param  _state        name of concerned state.
        /// \param  _observation  index of concerned observation.
        /// \param  _value     new probablity.
        public void MSetEmission(int _state, T _observation, float _value)
        {
            for (int i = 0; i < m_ObservationsCount; i++) if (_observation.Equals(MGetObservation(i))) { MSetEmissionByIndex(_state, i, _value); return; }
            throw new NullReferenceException();
        }

        /// \fn     public float MGetEmissionByIndex(String _state, int _observation)
        ///
        /// \brief  Gets an emission's probability using elements index in Emission's table
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        ///
        /// \param  _state        name of concerned state.
        /// \param  _observation  index of concerned observation.
        ///
        /// \return Emission's value
        public float MGetEmissionByIndex(String _state, int _observation)
        {
            for (int i = 0; i < m_StatesCount; i++) if (_state == this[i].m_name) return MGetEmissionByIndex(i, _observation);
            return -1F;
        }

        /// \fn     MSetEmission
        ///
        /// \brief  Sets an emission's probability
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        ///
        /// \param  _state        name of concerned state.
        /// \param  _observation  index of concerned observation.
        /// \param  _emission     new probablity.
        public void MSetEmissionByIndex(String _state, int _observation, float _value)
        {
            for (int i = 0; i < m_StatesCount; i++) if (_state == this[i].m_name) { MSetEmissionByIndex(i, _observation, _value); return; }
            throw new NullReferenceException();
        }

        /// \fn     public float MGetEmissionByIndex(int _state, int _observation)
        ///
        /// \brief  Gets an emission's probability using elements index in Emission's table
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        ///
        /// \param  _state        index of concerned state.
        /// \param  _observation  index of concerned observation.
        ///
        /// \return Emission's value
        public float MGetEmissionByIndex(int _state, int _observation)
        {
            return m_DicoEdge[_m_EmissionsGUID[(_state * m_ObservationsCount) + _observation]].m_cost;
        }

        /// \fn     public void MSetEmissionByIndex(int _state, int _observation, float _value)
        ///
        /// \brief  Sets an emission's probability
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        ///
        /// \param  _state        index of concerned state.
        /// \param  _observation  index of concerned observation.
        /// \param  _value     new probablity.
        public void MSetEmissionByIndex(int _state, int _observation, float _value)
        {
            m_DicoEdge[_m_EmissionsGUID[(_state * m_ObservationsCount) + _observation]].m_cost = _value;
        }

        /// \fn     public float MGetEmissionByIndex(int _index)
        ///
        /// \brief  Gets an emission's probability using elements index in Emission's table
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        ///
        /// \param  _index index of concerned emission.
        ///
        /// \return Emission's value
        public float MGetEmissionByIndex(int _index)
        {
            return m_DicoEdge[_m_EmissionsGUID[_index]].m_cost;
        }

        /// \fn     public void MSetEmissionByIndex(int _state, int _observation, float _value)
        ///
        /// \brief  Sets an emission's probability using one dimension array 
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        ///
        /// \param  _index index of concerned emission.
        /// \param  _value new probablity.
        public void MSetEmissionByIndex(int _index, float _value)
        {
            m_DicoEdge[_m_EmissionsGUID[_index]].m_cost = _value;
        }

        /// \fn     public void MSetEmissions(float[,] _emissions)
        ///
        /// \brief  Sets all  emission's values
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        ///
        /// \param  _emissions emissions probability to set.
        public void MSetEmissions(float[,] _emissions) 
        {
            if ((_emissions.GetLength(0) != m_StatesCount) || (_emissions.GetLength(1) != m_ObservationsCount)) Debug.Log("Lengths doesn't match !");
            for (int i = 0; i < m_StatesCount; i++)
                for (int j = 0; j < m_ObservationsCount; j++)
                    MSetEmissionByIndex(i, j, _emissions[i, j]);
        }

        /// \fn     public float MEvaluate(T[] _observations)
        ///
        /// \brief  Evaluates a sequence of observations using forward algorithmes.
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        ///
        /// \param  _observations sequence of observations that needs to be evaluated
        ///
        /// \return float that indicate the probablity of recognising such observations's sequence. 
        public float MEvaluate(T[] _observations)
        {
            return MEvaluate(_observations, eUseAlgorithme.kForward, 0);
        }

        /// \fn     public float MEvaluate(T[] _observations, eUseAlgorithme _use)
        ///
        /// \brief  Evaluates a sequence of observations using one of three algorithmes(forward, backward or both)
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        ///
        /// \param  _observations sequence of observations that needs to be evaluated
        /// \param  _use          Algorithme to use , forward, backward or forward and backward at the same time
        ///
        /// \return float that indicate the probablity of recognising such observations's sequence. 
        public float MEvaluate(T[] _observations, eUseAlgorithme _use)
		{
			return MEvaluate (_observations, _use, 0);
		}

        /// \fn     public float MEvaluate(T[] _observations, eUseAlgorithme _use, int _forwardbackwardObservation)
        ///
        /// \brief  Evaluates a sequence of observations using one of three algorithmes(forward, backward or both)
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        ///
        /// \param  _observations               sequence of observations that needs to be evaluated
        /// \param  _use                        Algorithme to use , forward, backward or forward and backward at the same time
        /// \param  _forwardbackwardObservation index of state to use if you select to evaluate the sequence with forward and backward algorithm, default is 0.
        ///
        /// \return float that indicate the probablity of recognising such observations's sequence.
        public float MEvaluate(T[] _observations, eUseAlgorithme _use, int _forwardbackwardObservation)
        {
            if (_observations.Length == 0 || _observations == null) return 0.0F;

            switch (_use)
            {
                case eUseAlgorithme.kForward  : return _MForwardEvaluation(_observations);
                case eUseAlgorithme.kBackward : return _MBackwardEvaluation(_observations);
                default                       : return _MForwardBackwardEvaluation(_observations, _forwardbackwardObservation);
            }
        }

        /// \fn     public float _MForwardEvaluation(T[] _observations) 
        ///
        /// \brief  Evaluates a sequence of observations using forward algorithme.
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        ///
        /// \param  _observations sequence of observations that needs to be evaluated
        ///
        /// \return float that indicate the probablity of recognising such observations's sequence.
        public float _MForwardEvaluation(T[] _observations) 
        {
            float p = 0;    
            float[,] alpha = _MForward(_observations);
            for (int j = 0; j < alpha.GetLength(1); j++) p += alpha[alpha.GetLength(0) - 1, j];
            return p;
        }

        /// \fn     private float _MBackwardEvaluation(T[] _observations)
        ///
        /// \brief  Evaluates a sequence of observations using backward algorithme.
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        ///
        /// \param  _observations sequence of observations that needs to be evaluated
        ///
        /// \return float that indicate the probablity of recognising such observations's sequence.
        private float _MBackwardEvaluation(T[] _observations)
        {
 	        float p = 0;
            float[,] beta = _MBackward(_observations);
            for (int i = 0; i < beta.GetLength(1); i++) p += (beta[0, i] * this[i].m_startProbability * MGetEmissionByIndex(i, 0));
            return p;
        }

        /// \fn     _MForwardBackwardEvaluation
        ///
        /// \brief  Evaluates a sequence of observations using forward and backward algorithmes at the same time using state number 0.
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        ///
        /// \param  _observations               sequence of observations that needs to be evaluated.
        ///
        /// \return float that indicate the probablity of recognising such observations's sequence.
        private float _MForwardBackwardEvaluation(T[] _observations)
		{
			return _MForwardBackwardEvaluation( _observations, 0);
		}

        /// \fn     _MForwardBackwardEvaluation
        ///
        /// \brief  Evaluates a sequence of observations using forward and backward algorithmes at the same time.
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        ///
        /// \param  _observations               sequence of observations that needs to be evaluated.
        /// \param  _forwardbackwardObservation index of state to use if you select to evaluate the sequence with forward and backward algorithm.
        ///
        /// \return float that indicate the probablity of recognising such observations's sequence.
        private float _MForwardBackwardEvaluation(T[] _observations,int _forwardbackwardObservation )
        {
            float p = 0;
            float[,] alpha = _MForward(_observations);
            float[,] beta = _MBackward(_observations);
            for (int j = 0; j < alpha.GetLength(1); j++) p += (alpha[_forwardbackwardObservation, j] * beta[_forwardbackwardObservation, j]);
            return p; 
        }

        /// \fn     public void MUpdate(T[] _sequence, float _tolerance, int _iterations)
        ///
        /// \brief  Updates HMM's values by learning from a sequence of observations. learning is limited by a constant number of iteration or by a epsilon value, both are permitted.
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        ///
        /// \param  _sequence   sequence of observations that the HMM learn from.
        /// \param  _tolerance  an epsilon value that stop learning process if new evaluation probability minus the old one are minor than it.
        /// \param  _iterations number of iterations that the HMM must do to learn from the sequence.
        public void MUpdate(T[] _sequence, float _tolerance, int _iterations)
        {
            if (_tolerance <= 0 && _iterations <= 0) return;
            
			float oldProbability = float.MinValue;
            float probability = 0;
            
			bool  stop = false;
            do
            {
                float[,] alpha = _MForward(_sequence);
                float[,] beta = _MBackward(_sequence);

                float[,] gamma = _MCalculateGamma(_sequence, alpha, beta);
                float[, ,] xi = _MCalculateXi(_sequence, alpha, beta);

                oldProbability = probability;
				probability = MEvaluate(_sequence, eUseAlgorithme.kForward);
				
				if(_MShouldStop(oldProbability, probability, _iterations, _tolerance)) 
				{
					stop = true;
				}
				else
				{				
					_iterations--;

					_MEstimateStartProbablities(gamma);
					_MEstimateTransitions(gamma, xi);
					_MEstimateEmissions(_sequence, gamma);
                }
                
            } while (!stop);
        }

        /// \fn     private bool _MShouldStop(float _oldProbability, float _probability, int _iterations, float _tolerance)
        ///
        /// \brief  Determines if the HMM should stop learning process by verifing iteration's count and comparing probabilities's difference and tolerence.
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        ///
        /// \param  _oldProbability   sequence's probablity before the current learning iteration.
        /// \param  _probability      sequence's probablity after the current learning iteration.
        /// \param  _tolerance        an epsilon value that stop learning process if new evaluation probability minus the old one are minor than it.
        /// \param  _iterations       number of iterations that the HMM must do to learn from the sequence.
        /// 
        /// \return true if the HMM should stop, false otherwise.
        private bool _MShouldStop(float _oldProbability, float _probability, int _iterations, float _tolerance)
        {
            if (_tolerance > 0)
            {
                if (System.Math.Abs(_oldProbability - _probability) <= _tolerance) return true;
                if (_iterations == 1) return true;
            }
            else if (_iterations == 1) return true;    
            if (Double.IsNaN(_probability) || Double.IsInfinity(_probability)) return true;
            return false;
        }
        
        /// \fn     public int[] MDecode(T[] _observations)
        ///
        /// \brief  Determines the highest probablity's path that generate a given sequence.
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        ///
        /// \param  _observations sequence of observations to decode.
        /// 
        /// \return a table of int that indicate the states's index of the path.
        public int[] MDecode(T[] _observations)
        {
            if (_observations == null || _observations.Length == 0) return new int[0];

            float[,] delta = new float[_observations.Length, m_StatesCount];
            int[,] psi = new int[_observations.Length, m_StatesCount];

            _MInitDecodingMatrix(_observations, ref delta, ref psi);
            _MCalculateDecodingMatrix(_observations, ref delta, ref psi);

            int maxState = _MGetMaxState(_observations, delta);

            int[] path = new int[_observations.Length];
            path[_observations.Length - 1] = maxState;
            for (int t = _observations.Length - 2; t >= 0; t--) path[t] = psi[t + 1, path[t + 1]];
            return path;

        }

        /// \fn     private void _MInitDecodingMatrix(T[] _observations, ref float[,] _delta, ref int[,] _psi)
        ///
        /// \brief  Initializes DELTA an PSI matrix that the HMM use to decode a sequence of observations. both matrix are initilized at the same time for optimisation's reasons.
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        ///
        /// \param  _observations sequence of observations to decoded.
        /// \param  _delta        DELTA Matrix that needs to be initailized .
        /// \param  _psi          PSI Matrix that needs to be initailized .
        private void _MInitDecodingMatrix(T[] _observations, ref float[,] _delta, ref int[,] _psi)
        {
            for (int i = 0; i < m_StatesCount; i++)
            {
                _delta[0, i] = this[i].m_startProbability * MGetEmission(i, _observations[0]);
                _psi[0, i] = 0;
            }
        }

        /// \fn     private void _MCalculateDecodingMatrix(T[] _observations, ref float[,] _delta, ref int[,] _psi)
        ///
        /// \brief  Calculates DELTA an PSI matrix that the HMM use to decode a sequence of observations. both matrix are calculated at the same time for optimisation's reasons.
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        ///
        /// \param  _observations sequence of observations to decode.
        /// \param  _delta        DELTA Matrix that needs to be calculated.
        /// \param  _psi          PSI Matrix that needs to be calculated.
        private void _MCalculateDecodingMatrix(T[] _observations, ref float[,] _delta, ref int[,] _psi) 
        {
            for (int t = 1; t < _observations.Length; t++)
            {
                for (int j = 0; j < m_StatesCount; j++)
                {
                    //float max = delta[t - 1, 0] * m_Transitions[0][j]; 
                    float max = _delta[t - 1, 0] * this[0,j]; 
                    int maxIndice = 0;
                    for (int k = 0; k < m_StatesCount; k++)
                    {
                        if (_delta[t - 1, k] * this[k, j] > max)
                        {
                            max = _delta[t - 1, k] * this[k,j];
                            maxIndice = k;
                        }
                    }
                    _delta[t, j] = max * MGetEmission(j, _observations[t]);
                    _psi[t, j] = maxIndice;
                }
            }
        }
        
        /// \fn     private int _MGetMaxState(T[] _observations, float[,] _delta)
        ///
        /// \brief  Gets the Last state with the maximum probability.
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        ///
        /// \param  _observations sequence of observations to decode.
        /// \param  _delta        DELTA Matrix that needs to be cal.
        ///
        /// \return index of the last highest state.
        private int _MGetMaxState(T[] _observations, float[,] _delta)
        {
            int maxState = 0;
            float maxWeight = _delta[_observations.Length - 1, 0];

            for (int i = 1; i < m_StatesCount; i++)
                if (_delta[_observations.Length - 1, i] > maxWeight)
                {
                    maxState = i;
                    maxWeight = _delta[_observations.Length - 1, i];
                }
            return maxState;
        }

        /// \fn     private float[,] _MForward(T[] _observations)
        ///
        /// \brief  Calculates ALPHA matrix from a given sequence of observations.
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        ///
        /// \param  _observations sequence of observations.
        ///
        /// \return float matrix that contains ALPHA values.
        private float[,] _MForward(T[] _observations)
        {
            float[,] alpha = new float[_observations.Length, m_StatesCount];
            for (int i = 0; i < m_StatesCount; i++) alpha[0, i] = this[i].m_startProbability * MGetEmission(i, _observations[0]);
            for (int t = 1; t < _observations.Length; t++)
                for (int i = 0; i < m_StatesCount; i++)
                {
                    float sum = 0.0F;
                    for (int j = 0; j < m_StatesCount; j++) sum += alpha[t - 1, j] * this[j, i];
                    alpha[t, i] = sum * MGetEmission(i, _observations[t]);
                }

            return alpha;
        }
        
        /// \fn     private float[,] _MBackward(T[] _observations)
        ///
        /// \brief  Calculates BETA matrix from a given sequence of observations.
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        ///
        /// \param  _observations sequence of observations.
        ///
        /// \return float matrix that contains BETA values.
        private float[,] _MBackward(T[] _observations)
        {
            float[,] beta = new float[_observations.Length, m_StatesCount];
            for (int i = 0; i < m_StatesCount; i++) beta[_observations.Length - 1, i] = 1.0F;
            for (int t = _observations.Length - 2; t >= 0; t--)
                for (int i = 0; i < m_StatesCount; i++)
                {
                    beta[t, i] = 0;
                    for (int j = 0; j < m_StatesCount; j++)
                        beta[t, i] += this[i,j] * MGetEmission(j, _observations[t + 1]) * beta[t + 1, j];//
                }
            return beta;
        }

        /// \fn     private float[,] _MCalculateGamma(float[,,] _xi)
        ///
        /// \brief  Calculates GAMMA matrix from XI matrix.
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        ///
        /// \param  _xi 3D matrix that we use do determine GAMMA matrix.
        ///
        /// \return float matrix that contains GAMMA values.
        private float[,] _MCalculateGamma(float[,,] _xi)
        {
            float[,] gamma = new float[_xi.GetLength(0), m_StatesCount];
            for (int t = 0; t < _xi.GetLength(0); t++)
            {
                for (int i = 0; i < _xi.GetLength(1); i++) 
                {
                    gamma[t, i] = 0;
                    for (int j = 0; j < _xi.GetLength(2); j++) 
                    {
                        gamma[t, i] += _xi[t,i,j];
                    }
                }
            }
            return gamma;
        }

        /// \fn     private float[,] _MCalculateGamma(T[] _sequence, float[,] _alpha, float[,] _beta)
        ///
        /// \brief  Calculates GAMMA matrix from ALPHA and BETA matrix.
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        ///
        /// \param  _sequence sequence of observations.
        /// \param  _alpha    ALPHA matrix.
        /// \param  _beta     BETA matrix.
        ///
        /// \return float matrix that contains GAMMA values.
        private float[,] _MCalculateGamma(T[] _sequence, float[,] _alpha, float[,] _beta)
        {
            float[,] gamma = new float[_sequence.Length, m_StatesCount]; float scale;
            for (int t = 0; t < gamma.GetLength(0); t++)
            {
                scale = 0;
                for (int i = 0; i < gamma.GetLength(1); i++) scale += gamma[t, i] = _alpha[t, i] * _beta[t, i];
                if (scale != 0) for (int i = 0; i < gamma.GetLength(1); i++) gamma[t, i] /= scale;
            }
            return gamma;
        }

        /// \fn     private float[, ,] _MCalculateXi(T[] _sequence, float[,] _alpha, float[,] _beta)
        ///
        /// \brief  Calculates XI matrix from ALPHA and BETA matrix.
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        ///
        /// \param  _sequence sequence of observations.
        /// \param  _alpha    ALPHA matrix.
        /// \param  _beta     BETA matrix.
        ///
        /// \return float matrix that contains XI values. 
        private float[, ,] _MCalculateXi(T[] _sequence, float[,] _alpha, float[,] _beta) 
        {
            float[, ,] xi = new float[_sequence.Length, m_StatesCount, m_StatesCount]; float scale;
            
            for (int t = 0; t < xi.GetLength(0) - 1; t++)
            {
                scale = 0;
                for (int i = 0; i < xi.GetLength(1); i++)
                    for (int j = 0; j < xi.GetLength(2); j++)
                        scale += xi[t, i, j] = _alpha[t, i] * this[i, j] * _beta[t + 1, j] * MGetEmission(j, _sequence[t + 1]);//
                for (int i = 0; i < xi.GetLength(1); i++) for (int j = 0; j < xi.GetLength(2); j++) xi[t, i, j] /= scale;
            }
            return xi;
        }
        
        /// \fn     private void _MEstimateStartProbablities(float[,] _gamma)
        ///
        /// \brief  Calculates the new probability of each state's start.
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        ///
        /// \param  _gamma GAMMA matrix.
        private void _MEstimateStartProbablities(float[,] _gamma)
        {
            for (int i = 0; i < m_StatesCount; i++) this[i].m_startProbability = _gamma[0, i];
        }

        /// \fn     private void _MEstimateEmissions(T[] _sequence, float[,] _gamma)
        ///
        /// \brief  Calculates the new probabilities of emissions.
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        ///
        /// \param  _observations sequence of observations.
        /// \param  _gamma        GAMMA matrix.
        private void _MEstimateEmissions(T[] _sequence, float[,] _gamma)
        {
            float a,b;
            for (int i = 0; i < m_StatesCount; i++)
            {
                for (int k = 0; k < m_ObservationsCount; k++)
                {
                    a = 0; b = 0;
                    for (int t = 0; t < _gamma.GetLength(0); t++) if (_sequence[t].Equals(MGetObservation(k))) a += _gamma[t, i];//
                    for (int t = 0; t < _gamma.GetLength(0); t++) b += _gamma[t, i];
                    MSetEmissionByIndex(i, k, (a == 0) ? (float)(1e-10) : a / b);
                }
            }

        }

        /// \fn     private void _MEstimateTransitions(float[,] _gamma, float[, ,] _xi)
        ///
        /// \brief  Calculates the new probabilities of transitions between states.
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        ///
        /// \param  _gamma GAMMA matrix.
        /// \param  _xi    XI matrix.
        private void _MEstimateTransitions(float[,] _gamma, float[, ,] _xi)
        {
            float a,b ;
            for (int i = 0; i < m_StatesCount; i++)
            {
                for (int j = 0; j < m_StatesCount; j++)
                {
                    a = 0; b = 0;
                    for (int t = 0; t < _xi.GetLength(0) - 1; t++) a += _xi[t, i, j];
                    for (int t = 0; t < _gamma.GetLength(0) - 1; t++) b += _gamma[t, i];
                    this[i,j] = b != 0 ? a / b : 0.0F;
                }
            }
        }

        /// \fn     public bool MCheckValues()
        ///
        /// \brief  Verifys that all probabilities are equl to 1 !
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        ///
        /// \return false if there is an error, true otherwise.
        public bool MCheckValues()
        {
            float sumPorb = 0;

            for (int i = 0; i < m_StatesCount; i++) { sumPorb += this[i].m_startProbability; if (sumPorb > 1) break; }
            if (sumPorb != 1) 
            { 
                #if DEBUG
                Debug.Log("HMM with " + m_StatesCount + ":" + m_ObservationsCount +" Error in start Probabilities =  "+sumPorb+" !");
                #endif
                return false;
            }

            for (int i = 0; i < m_StatesCount; i++) 
            {
                sumPorb = 0;
                for (int j = 0; j < m_StatesCount; j++) { sumPorb += this[i, j]; if (sumPorb > 1) break; }
                if(sumPorb != 1)
                {
                    #if DEBUG
                    Debug.Log("HMM with " + m_StatesCount + ":" + m_ObservationsCount + " Error in transitions probabilities =  " + sumPorb + " !");
                    #endif
                    return false;
                }
            }

            for (int i = 0; i < m_StatesCount; i++)
            {
                sumPorb = 0;
                for (int j = 0; j < m_ObservationsCount; j++)
                {
                    sumPorb += MGetEmissionByIndex(i, j);
                    if (sumPorb > 1) break;
                }
                if (sumPorb != 1)
                {
                    #if DEBUG
                    Debug.Log("HMM with " + m_StatesCount + ":" + m_ObservationsCount + " Error in emissions probabilities =  " + sumPorb + " at "+i+" !");
                    #endif
                    return false;
                }
            }

            return true;
        }
    }
}
