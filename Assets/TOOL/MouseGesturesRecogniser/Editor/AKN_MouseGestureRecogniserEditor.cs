using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using AKeNe.AI.HiddenMarkovModel;
using System;

/// \namespace AKeNe.Tools.MouseGestureRecognition
/// 
/// \brief     namespace that contains all classes of mouse gesture recongnition tool.
///
/// \author    Khoubaeib Klai | khoubaeib@gmail.com
/// \date      24/04/2014
namespace AKeNe.Tools.MouseGestureRecognition
{

    /// \class  AKN_MouseGestureRecogniserEditor
    ///
    /// \brief  Create and edit gestures using HMM.
    ///
    /// \author Khoubaeib Klai | khoubaeib@gmail.com
    /// \date   24/04/2014
    /// 
    /// \version 1.0
    [CustomEditor(typeof(AKN_MouseGesturesRecogniserComponent))]
    public class AKN_MouseGestureRecogniserEditor : Editor
    {
        /// \fn     public static void MouseGestureRecogniser()
        ///
        /// \brief  Insert an item in AI menu to import mouse gesture editor
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        [MenuItem("AKeNe/1.AI/Mouse Gesture Recogniser _%m", false, 1)]
        public static void MouseGestureRecogniser()
        {
            UnityEngine.Object[] previous = GameObject.FindObjectsOfType(typeof(AKN_MouseGesturesRecogniserComponent));
            if (previous.Length > 0)
            {
                Debug.LogError("You should have only one instance of Gesture Mouse Recogniser !");
            }
            else
            {
                GameObject newObject = new GameObject("MouseGesturesRecogniser", typeof(AKN_MouseGesturesRecogniserComponent));
                newObject.GetComponent<Transform>().hideFlags = HideFlags.HideInInspector;
            }
        }

        public AKN_MouseGesturesRecogniserComponent m_AKN_MGR = null;

        private string _m_newGestureName = "new gesture";
        private int _m_buttomHeight = 25;

        /// \fn     public void OnEnable()
        ///
        /// \brief  Initialize Mouse gesture component and HMM
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        public void OnEnable()
        {
            if (m_AKN_MGR == null) m_AKN_MGR = (AKN_MouseGesturesRecogniserComponent)target;
            if (m_AKN_MGR.m_properties.m_HMMs == null) m_AKN_MGR.m_properties.UpdateHMMFromSerializedValues();
        }

        /// \fn     public override void OnInspectorGUI()
        ///
        /// \brief  Show gestures edit interface on Inspector window
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        public override void OnInspectorGUI()
        {
            _MShowEditorHeader();
            _MShowGestures();
            _MAddGesture();
            _MShowEvaluateGesture();
        }

        /// \fn     private void _MShowEditorHeader()
        ///
        /// \brief  Show interface header's
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        private void _MShowEditorHeader()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Gestures");
            GUILayout.FlexibleSpace();
            GUILayout.FlexibleSpace();
            GUILayout.Label("(" + m_AKN_MGR.m_properties.m_HMMs.Count + ")");
            GUILayout.EndHorizontal();
        }

        /// \fn     private void _MShowGestures()
        ///
        /// \brief  Show Gestures in the inspector window
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        private void _MShowGestures()
        {
            for (int i = 0; i < m_AKN_MGR.m_properties.m_HMMs.Count; i++)
            {
                GUILayout.BeginVertical(GUI.skin.box);
                _MShowGestureFoldOut(i);
                if (m_AKN_MGR.m_properties.m_ShowGestures[i]) _MShowGesture(ref i);
                GUILayout.EndVertical();
            }
        }

        /// \fn     private void _MShowGestureFoldOut(int _index)
        ///
        /// \brief  Activate/Desactivate gesture options display
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        /// 
        /// \param  _index   index of the gesture in gestures table
        private void _MShowGestureFoldOut(int _index)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            m_AKN_MGR.m_properties.m_ShowGestures[_index] =
                EditorGUILayout.Foldout(m_AKN_MGR.m_properties.m_ShowGestures[_index], m_AKN_MGR.m_properties.m_HMMs[_index] == null ?
                "" : m_AKN_MGR.m_properties.m_HMMs[_index].m_label);
            GUILayout.EndHorizontal();
        }

        /// \fn     private void _MShowGesture(ref int _index) 
        ///
        /// \brief  Show gesture option(Create, Edit, View, Remove)
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        /// 
        /// \param  _index   index of the gesture in gestures table
        private void _MShowGesture(ref int _index)
        {
            if (m_AKN_MGR.m_properties.m_HMMs[_index] == null) _MShowHMMCreationOptions(_index); else _MShowHMMOptions(_index);
            _MRemoveGesture(ref _index);
        }

        /// \fn     private void _MShowHMMCreationOptions(int _index)
        ///
        /// \brief  Display gesture creation Options(Manual, Automatic construction)
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        /// 
        /// \param  _index  index of the gesture in gestures table
        private void _MShowHMMCreationOptions(int _index)
        {
            _m_newGestureName = GUILayout.TextArea(_m_newGestureName);

            if (GUILayout.Button("Automatic construction")) _MAutomaticBuild(_index);

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField("OR", new GUILayoutOption[] { GUILayout.MaxWidth(20) });
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Manual Construction"))
            {
                if (m_AKN_MGR.m_properties.m_HmmsStatesCount[_index] < 3 || m_AKN_MGR.m_properties.m_HmmsObservationsCount[_index] < 2)
                {
                    EditorUtility.DisplayDialog("Can not create gesture !", "Can not Made a gesture with only 3 states or 2 observations !", "OK");
                }
                else
                {
                    _MManualBuild(_index);
                    m_AKN_MGR.m_properties.UpdateSerializingValues();
                }
            }

            m_AKN_MGR.m_properties.m_HmmsStatesCount[_index] = EditorGUILayout.IntField("States Counts", m_AKN_MGR.m_properties.m_HmmsStatesCount[_index]);
            m_AKN_MGR.m_properties.m_HmmsObservationsCount[_index] = EditorGUILayout.IntField("Observations Counts", m_AKN_MGR.m_properties.m_HmmsObservationsCount[_index]);
        }

        /// \fn     public void OnEnable()
        ///
        /// \brief  Build a gesture by defining manually probabilities of its HMM.
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014    
        /// 
        /// \param  _index  index of the gesture in gestures table
        private void _MManualBuild(int _index)
        {
            m_AKN_MGR.m_properties.m_HMMs[_index] = new AKN_HiddenMarkovModel<int>(m_AKN_MGR.m_properties.m_HmmsStatesCount[_index], m_AKN_MGR.m_properties.m_HmmsObservationsCount[_index], _m_newGestureName);
            for (int k = 0; k < m_AKN_MGR.m_properties.m_HMMs[_index].m_ObservationsCount; k++) m_AKN_MGR.m_properties.m_HMMs[_index].MSetObservation(k, k + 1);
            for (int j = 0; j < m_AKN_MGR.m_properties.m_HMMs[_index].m_StatesCount; j++)
            {
                for (int k = 0; k < m_AKN_MGR.m_properties.m_HMMs[_index].m_StatesCount; k++)
                    m_AKN_MGR.m_properties.m_HMMs[_index].MSetTransition((j * m_AKN_MGR.m_properties.m_HMMs[_index].m_StatesCount) + k, 0F);
                m_AKN_MGR.m_properties.m_HMMs[_index][j].m_name = ("Etat " + j);
                m_AKN_MGR.m_properties.m_HMMs[_index][j].m_startProbability = 0F;
                for (int k = 0; k < m_AKN_MGR.m_properties.m_HMMs[_index].m_ObservationsCount; k++) m_AKN_MGR.m_properties.m_HMMs[_index].MSetEmission(j, k, 0F);
            }
            _m_newGestureName = "new gesture";
        }

        /// \fn      private void _MShowHMMOptions(int _index)
        ///
        /// \brief  Show Post creation options for a gesture(Show/Edit values, Learn gesture)
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        /// 
        /// \param  _index  index of the gesture in gestures table
        private void _MShowHMMOptions(int _index)
        {
            if (GUILayout.Button(new GUIContent("    Show gesture parameters", m_AKN_MGR.m_properties.m_manualBuildTexture), new GUILayoutOption[] { GUILayout.MaxHeight(_m_buttomHeight) })) _MShowHMMValues(_index);
            if (GUILayout.Button(new GUIContent("    Learn gesture", m_AKN_MGR.m_properties.m_autoBuildTexture), new GUILayoutOption[] { GUILayout.MaxHeight(_m_buttomHeight) })) _MLearnDraw(_index);
        }

        /// \fn     private void _MRemoveGesture(ref int _index)
        ///
        /// \brief  Remove a gesture from  gestures Table
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        ///   
        /// \param  _index  index of the gesture in gestures table
        private void _MRemoveGesture(ref int _index)
        {
            if (GUILayout.Button(new GUIContent("    Remove gesture", m_AKN_MGR.m_properties.m_minusTexture), new GUILayoutOption[] { GUILayout.MaxHeight(_m_buttomHeight) }))
                if (EditorUtility.DisplayDialog("Remove gesture !", "Are you sure ?", "Yes", "No"))
                {
                    _MRemoveHMMAt(_index);
                    _index--;
                    m_AKN_MGR.m_properties.UpdateSerializingValues();
                }
        }

        /// \fn     private void _MRemoveHMMAt(int _index)
        ///
        /// \brief  Show Interface elements
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        /// 
        /// \param  _index  index of the gesture in gestures table
        private void _MRemoveHMMAt(int _index)
        {
            m_AKN_MGR.m_properties.m_HMMs[_index] = null;
            m_AKN_MGR.m_properties.m_HMMs.RemoveAt(_index);
            m_AKN_MGR.m_properties.m_gesturesProbabilities.RemoveAt(_index);
            m_AKN_MGR.m_properties.m_ShowGestures.RemoveAt(_index);
            m_AKN_MGR.m_properties.m_HmmsStatesCount.RemoveAt(_index);
            m_AKN_MGR.m_properties.m_HmmsObservationsCount.RemoveAt(_index);
        }

        /// \fn     private void _MShowEvaluateGesture()
        ///
        /// \brief  Dispaly Evaluate Gesture option
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        private void _MShowEvaluateGesture()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            if (GUILayout.Button(
                new GUIContent("    Evaluate a gesture", m_AKN_MGR.m_properties.m_autoBuildTexture),
                new GUILayoutOption[] { GUILayout.MaxHeight(_m_buttomHeight) }))
                _MEvaluateGestures();
            EditorGUILayout.Space();
            _MShowGesturesProbability();
            EditorGUILayout.EndVertical();
        }

        /// \fn     private void _MShowGesturesProbability()
        ///
        /// \brief  Show evaluation probabilities for each HMM gesture 
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        private void _MShowGesturesProbability()
        {
            if (m_AKN_MGR.m_properties.m_HMMs.Count > 0)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(10);
                m_AKN_MGR.m_properties.m_ShowProbabilitiesInInspector = EditorGUILayout.Foldout(m_AKN_MGR.m_properties.m_ShowProbabilitiesInInspector, "Gestures's probability");
                EditorGUILayout.EndHorizontal();
                if (m_AKN_MGR.m_properties.m_ShowProbabilitiesInInspector)
                    for (int i = 0; i < m_AKN_MGR.m_properties.m_HMMs.Count; i++)
                        if (m_AKN_MGR.m_properties.m_HMMs[i] != null)
                            EditorGUILayout.LabelField(m_AKN_MGR.m_properties.m_HMMs[i].m_label + " = " + m_AKN_MGR.m_properties.m_gesturesProbabilities[i]);

                EditorGUILayout.Separator();

                m_AKN_MGR.m_properties.m_ShowProbabilitiesInScene = EditorGUILayout.ToggleLeft("Show gestures's probability in scene", m_AKN_MGR.m_properties.m_ShowProbabilitiesInScene);
            }
        }

        /// \fn     private void _MAddGesture()
        ///
        /// \brief  Add an empty gesture to gesture table.
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        private void _MAddGesture()
        {
            if (GUILayout.Button(new GUIContent("    Add gesture", m_AKN_MGR.m_properties.m_plusTexture), new GUILayoutOption[] { GUILayout.MaxHeight(_m_buttomHeight) }))
            {
                _MAddHMM();
                m_AKN_MGR.m_properties.UpdateSerializingValues();
            }
            EditorGUILayout.Space();
        }

        /// \fn     private void _MAddHMM()
        ///
        /// \brief  Add apropriate HMM to a new Gesture, update also all display options
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        private void _MAddHMM()
        {
            m_AKN_MGR.m_properties.m_HMMs.Add(null);
            m_AKN_MGR.m_properties.m_gesturesProbabilities.Add(0);
            m_AKN_MGR.m_properties.m_ShowGestures.Add(false);
            m_AKN_MGR.m_properties.m_HmmsStatesCount.Add(0);
            m_AKN_MGR.m_properties.m_HmmsObservationsCount.Add(0);
        }

        /// \fn     private void _MShowHMMValues(int _index)
        ///
        /// \brief  Show HMM probabilities in separated window
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        /// 
        /// \param  _index  index of the gesture in gestures table
        private void _MShowHMMValues(int _index)
        {
            AKN_HMMViewParametersWindow.m_AKN_MGR = m_AKN_MGR;
            AKN_HMMViewParametersWindow.m_HMM = m_AKN_MGR.m_properties.m_HMMs[_index];
            AKN_HMMViewParametersWindow.m_HIndex = _index;
            AKN_HMMViewParametersWindow.MLaunch();
            AKN_HMMViewParametersWindow.m_WindowOpened = true;
        }

        /// \fn     private void _MEvaluateGestures()
        ///
        /// \brief  Open a Window to draw and evaluate gestures
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        private void _MEvaluateGestures()
        {
            AKN_MouseGestureDrawWindow.MLaunch();
            AKN_MouseGestureDrawWindow.m_AKN_MGR = m_AKN_MGR;
            AKN_MouseGestureDrawWindow.m_CallFor = eCallFor.kEvaluateGestures;
            AKN_MouseGestureDrawWindow.m_WindowOpened = true;
        }

        /// \fn     private void _MAutomaticBuild(int _index)
        ///
        /// \brief  Open a Window to draw gesture and construct automatically its HMM
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        /// 
        /// \param  _index  index of the gesture in gestures table
        private void _MAutomaticBuild(int _index)
        {
            AKN_MouseGestureDrawWindow.MLaunch();
            AKN_MouseGestureDrawWindow.m_AKN_MGR = m_AKN_MGR;
            AKN_MouseGestureDrawWindow.m_CallFor = eCallFor.kConstructHmm;
            AKN_MouseGestureDrawWindow.m_HMMIndex = _index;
            AKN_MouseGestureDrawWindow.m_WindowOpened = true;
            AKN_MouseGestureDrawWindow.m_gestureName = _m_newGestureName;
        }

        /// \fn     private void _MLearnDraw(int _index)
        ///
        /// \brief  Open a Window to learn gesture from user draws
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        /// 
        /// \param  _index  index of the gesture in gestures table
        private void _MLearnDraw(int _index)
        {
            AKN_MouseGestureDrawWindow.MLaunch();
            AKN_MouseGestureDrawWindow.m_AKN_MGR = m_AKN_MGR;
            AKN_MouseGestureDrawWindow.m_CallFor = eCallFor.kLearnGesture;
            AKN_MouseGestureDrawWindow.m_HMMIndex = _index;
            AKN_MouseGestureDrawWindow.m_WindowOpened = true;
        }

    }
}