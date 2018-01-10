using System;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using AKeNe.AI.HiddenMarkovModel;

/// \namespace AKeNe.Tools.MouseGestureRecognition
/// 
/// \brief     namespace that contains all classes of mouse gesture recongnition tool.
///
/// \author    Khoubaeib Klai | khoubaeib@gmail.com
/// \date      24/04/2014
namespace AKeNe.Tools.MouseGestureRecognition
{

    /// \class  AKN_HMMViewParametersWindow
    /// 
    /// \brief  Class that handle HMM View Window. it show all values related to the HMM representing the gesture. 
    ///
    /// \author Khoubaeib Klai | khoubaeib@gmail.com
    /// \date   24/04/2014
    ///
    /// \version 1.0
    [Serializable]
    public class AKN_HMMViewParametersWindow : EditorWindow
    {
        public static int m_HIndex;
        public static AKN_HiddenMarkovModel<int> m_HMM;
        public static AKN_MouseGesturesRecogniserComponent m_AKN_MGR;

        public static bool m_WindowOpened = false;

        [SerializeField]
        private bool _m_showStates = false;
        [SerializeField]
        private bool _m_showTransitions = false;
        [SerializeField]
        private bool _m_showEmissions = false;
        //[SerializeField]
        //private bool _m_showGestureDraw = false;

        private Vector2 _m_scoller = Vector2.zero;

        /// \fn     public static void MLaunch()
        /// 
        /// \brief  Start HMM view Window
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        public static void MLaunch()
        {
            GetWindow(typeof(AKN_HMMViewParametersWindow)).Show();
        }

        /// \fn     void OnEnable()
        /// 
        /// \brief  Called when activate the Window, init Window title also.
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        void OnEnable()
        {
            titleContent = new GUIContent("Hidden Markov model parameters");
            if (m_HMM == null) return;
        }

        /// \fn     public void OnGUI()
        /// 
        /// \brief  Called to display Interface elements. 
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        public void OnGUI()
        {

            EditorGUILayout.IntField("States Counts", m_HMM.m_StatesCount);
            EditorGUILayout.IntField("Observations Counts", m_HMM.m_ObservationsCount);

            _m_scoller = EditorGUILayout.BeginScrollView(_m_scoller, false, false);
            EditorGUILayout.Space();
            _MShowStates();
            EditorGUILayout.Space();
            _MShowTransitions();
            EditorGUILayout.Space();
            _MShowEmissions();
            EditorGUILayout.Space();
            _MSaveChanges();
            EditorGUILayout.EndScrollView();
        }

        /// \fn     private void _MSaveChanges()
        /// 
        /// \brief  Save new values after clicking "Save" button.
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        private void _MSaveChanges()
        {
            if (GUILayout.Button("Save changes"))
            {
                try
                {
                    m_AKN_MGR.m_properties.m_HMMs[m_HIndex] = m_HMM;
                    m_AKN_MGR.m_properties.UpdateSerializingValues();
                    Debug.Log("Changes saved !");
                }
                catch (Exception e) { Debug.Log("An Error has occured when saving changes : " + e.Message); }
            }
        }

        /// \fn     private void _MShowStates()
        /// 
        /// \brief  Show start probabilities of the HMM. 
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        private void _MShowStates()
        {
            _m_showStates = EditorGUILayout.Foldout(_m_showStates, "States");
            if (_m_showStates)
            {
                for (int i = 0; i < m_HMM.m_StatesCount; i++)
                {
                    m_HMM[i].m_startProbability = EditorGUILayout.FloatField("Etat " + (i + 1), m_HMM[i].m_startProbability);
                }
            }
        }

        /// \fn     private void _MShowTransitions()
        /// 
        /// \brief  Show transition probabilites between states.
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        private void _MShowTransitions()
        {
            _m_showTransitions = EditorGUILayout.Foldout(_m_showTransitions, "Transition's Matrix");

            if (_m_showTransitions)
            {
                GUILayout.BeginVertical();
                for (int i = 0; i < m_HMM.m_StatesCount; i++) _MShowTransitionsValuesAtState(i);
                GUILayout.EndVertical();
            }
        }

        /// \fn     private void _MShowEmissions()
        /// 
        /// \brief  Show observations probabilities within each states. 
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        private void _MShowEmissions()
        {
            _m_showEmissions = EditorGUILayout.Foldout(_m_showEmissions, "Emissions's Matrix");

            if (_m_showEmissions)
            {
                GUILayout.BeginVertical();
                for (int i = 0; i < m_HMM.m_StatesCount; i++) _MShowEmissionsValuesAtState(i);
                GUILayout.EndVertical();
            }
        }

        /// \fn     private void _MShowTransitionsValuesAtState(int _index)
        /// 
        /// \brief  Show transition probabilites from a specific state.
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        /// 
        /// \param  _index index of the state
        private void _MShowTransitionsValuesAtState(int _index)
        {
            GUILayout.BeginHorizontal();
            for (int j = 0; j < m_HMM.m_StatesCount; j++)
                m_HMM.MSetTransition((_index * m_HMM.m_StatesCount) + j, EditorGUILayout.FloatField(m_HMM.MGetTransition((_index * m_HMM.m_StatesCount) + j)));
            GUILayout.EndHorizontal();
        }

        /// \fn     public void _MShowEmissionsValuesAtState(int _index)
        /// 
        /// \brief  Show observations probabilities within a specific state.
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        /// 
        /// \param  _index index of the state
        public void _MShowEmissionsValuesAtState(int _index)
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Etat " + (_index + 1), new GUILayoutOption[] { GUILayout.MaxWidth(50) });
            for (int j = 0; j < m_HMM.m_ObservationsCount; j++) //m_HMM.m_Emissions[i, j + 1] = EditorGUILayout.FloatField(m_HMM.m_Emissions[i, j + 1]);
                //m_HMM.MSetEmissionByIndex((i * m_HMM.m_ObservationsCount) + j, EditorGUILayout.FloatField(m_HMM.MGetEmissionByIndex((i * m_HMM.m_ObservationsCount) + j)));
                m_HMM.MSetEmissionByIndex(_index, j, EditorGUILayout.FloatField(m_HMM.MGetEmissionByIndex(_index, j)));
            GUILayout.EndHorizontal();
        }

        /// \fn     public void OnDestroy()
        /// 
        /// \brief  Called when the window is closing. change th status of the window to closed(opened before).
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        public void OnDestroy()
        {
            m_WindowOpened = false;
        }
    }
}