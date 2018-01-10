using System;
using UnityEngine;
using UnityEditor;
using System.Collections;
using AKeNe.AI.HiddenMarkovModel;
using System.Collections.Generic;

/// \namespace AKeNe.Tools.MouseGestureRecognition
/// 
/// \brief     namespace that contains all classes of mouse gesture recongnition tool.
///
/// \author    Khoubaeib Klai | khoubaeib@gmail.com
/// \date      24/04/2014
namespace AKeNe.Tools.MouseGestureRecognition
{

    /// \class  AKN_MouseGesturesRecogniserProperties
    ///
    /// \brief  Contains all variables that must be serialized in the Editor
    ///
    /// \author Khoubaeib Klai | khoubaeib@gmail.com
    /// \date   24/04/2014
    [Serializable]
    public class AKN_MouseGesturesRecogniserProperties : ScriptableObject
    {

        public List<AKN_HiddenMarkovModel<int>> m_HMMs = null;

        public List<float> m_gesturesProbabilities;
        public List<int> m_currentGesture;
        public int m_recognizedAs;

        #region Editor
        public List<bool> m_ShowGestures;
        public List<int> m_HmmsStatesCount;
        public List<int> m_HmmsObservationsCount;
        public bool m_ShowProbabilitiesInScene;
        public bool m_ShowProbabilitiesInInspector;
        public bool m_DisplayRemoveDialog;
        #endregion

        #region Graphic
        public Texture2D m_minusTexture;
        public Texture2D m_plusTexture;
        public Texture2D m_editTexture;
        public Texture2D m_manualBuildTexture;
        public Texture2D m_autoBuildTexture;
        #endregion

        #region Serialiaing parameters
        public List<int> nb_states;
        public List<int> nb_observations;
        public List<String> m_gesturesName;
        public List<String> statesNames;
        public List<float> statesProbabilities;
        public List<float> transitions;
        public List<int> observations;
        public List<float> emissions;
        #endregion

        /// \fn     public void UpdateHMMFromSerializedValues()
        ///
        /// \brief  Deserialize All gestures values into  their origin format
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        public void UpdateHMMFromSerializedValues()
        {
            List<AKN_HiddenMarkovModel<int>> l_hmms = new List<AKN_HiddenMarkovModel<int>>();

            for (int i = 0; i < nb_states.Count; i++)
            {
                if (nb_states[i] != 0 && nb_observations[i] != 0)
                {
                    AKN_HiddenMarkovModel<int> hmm = new AKN_HiddenMarkovModel<int>(nb_states[i], nb_observations[i], m_gesturesName[i]);

                    int indexPreviousStates = 0; for (int j = 0; j < i; j++) indexPreviousStates += nb_states[j];
                    int indexPreviousObservations = 0; for (int j = 0; j < i; j++) indexPreviousObservations += nb_states[j];

                    hmm.MSetStates(
                        statesProbabilities.GetRange(indexPreviousStates, nb_states[i]).ToArray(),
                        statesNames.GetRange(indexPreviousStates, nb_states[i]).ToArray());

                    int indexPreviousTransitions = 0; for (int j = 0; j < i; j++) indexPreviousTransitions += (nb_states[j] * nb_states[j]);
                    float[] trs = transitions.GetRange(indexPreviousTransitions, (nb_states[i] * nb_states[i])).ToArray();
                    for (int j = 0; j < nb_states[i]; j++) for (int k = 0; k < nb_states[i]; k++) hmm.MSetTransition(j, k, trs[(j * nb_states[i]) + k]);

                    hmm.MSetObservations(observations.GetRange(indexPreviousObservations, nb_observations[i]).ToArray());

                    int indexPreviousEmissions = 0; for (int j = 0; j < i; j++) indexPreviousEmissions += (nb_states[j] * nb_observations[j]);
                    float[] ems = emissions.GetRange(indexPreviousEmissions, (nb_states[i] * nb_observations[i])).ToArray();
                    for (int j = 0; j < nb_states[i] * nb_observations[i]; j++) hmm.MSetEmissionByIndex(j, ems[j]);


                    l_hmms.Add(hmm);
                }
                else l_hmms.Add(null);
            }

            m_HMMs = l_hmms;

        }

        /// \fn     private void _MInitSerializingVars()
        ///
        /// \brief  init Serialising List
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        private void _MInitSerializingVars()
        {
            nb_states = new List<int>();
            nb_observations = new List<int>();
            m_gesturesName = new List<string>();
            statesNames = new List<String>();
            statesProbabilities = new List<float>();
            transitions = new List<float>();
            observations = new List<int>();
            emissions = new List<float>();
        }

        /// \fn     public void UpdateSerializingValues()
        ///
        /// \brief  Serialize All gestures values into a temporary saved values.
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        public void UpdateSerializingValues()
        {
            _MInitSerializingVars();

            for (int i = 0; i < m_HMMs.Count; i++)
            {
                if (m_HMMs[i] != null)
                {
                    List<String> names = new List<String>();
                    List<float> stProb = new List<float>();
                    List<float> transition = new List<float>();
                    List<int> _observations = new List<int>();
                    List<float> _emissions = new List<float>();

                    for (int j = 0; j < m_HMMs[i].m_StatesCount; j++) names.Add(m_HMMs[i][j].m_name);
                    for (int j = 0; j < m_HMMs[i].m_StatesCount; j++) stProb.Add(m_HMMs[i][j].m_startProbability);
                    for (int j = 0; j < m_HMMs[i].m_StatesCount * m_HMMs[i].m_StatesCount; j++) transition.Add(m_HMMs[i].MGetTransition(j));
                    for (int j = 0; j < m_HMMs[i].m_ObservationsCount; j++) _observations.Add(m_HMMs[i].MGetObservation(j));
                    for (int j = 0; j < m_HMMs[i].m_StatesCount * m_HMMs[i].m_ObservationsCount; j++) _emissions.Add(m_HMMs[i].MGetEmissionByIndex(j));

                    nb_states.Add(m_HMMs[i].m_StatesCount);
                    nb_observations.Add(m_HMMs[i].m_ObservationsCount);
                    m_gesturesName.Add(m_HMMs[i].m_label);
                    statesNames.AddRange(names);
                    statesProbabilities.AddRange(stProb);
                    transitions.AddRange(transition);
                    observations.AddRange(_observations);
                    emissions.AddRange(_emissions);
                }
                else
                {
                    nb_states.Add(0);
                    nb_observations.Add(0);
                }
            }
        }
    }
}