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
    /// \class   AKN_MouseGesturesRecogniserComponent
    ///
    /// \brief   Unity MonoBehaviour that gesture recongnition logique and let possible gesture draw in Play mode.
    ///
    /// \author  Khoubaeib Klai | khoubaeib@gmail.com
    /// \date    24/04/2014
    ///  
    /// \version 1.0
    [Serializable]
    public class AKN_MouseGesturesRecogniserComponent : MonoBehaviour
    {
        public AKN_MouseGesturesRecogniserProperties m_properties;
        public AKN_Render2DGestureOnCamera m_cameraRender;
        public bool _m_showDrawOnRender = true;
        public List<List<Vector2>> m_points;

        private double _m_accuracyDouble = 1.0e-06F;
        private string iconsSource;
        private float _m_tickTime = 0.05F;
        private float _m_tickCounter = 0F;
        private int _m_gestureIndex;
        private bool _m_drawMode;

        /// \fn     public void Reset()
        ///
        /// \brief  Called when adding Mouse gesture recongnition tool. init all needed values if there are empty.
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        public void Reset()
        {
            if (m_properties == null) _MInitAllValues();
        }

        /// \fn     public void Start()
        ///
        /// \brief  Called at the beginning of the play Mode. initiate draw variables.
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        public void Start()
        {
            m_properties.UpdateHMMFromSerializedValues();
            m_points = new List<List<Vector2>>();
            m_properties.m_recognizedAs = -1;
            _m_gestureIndex = -1;
            if (Camera.main == null) return;
            m_cameraRender = (AKN_Render2DGestureOnCamera)Camera.main.gameObject.AddComponent(typeof(AKN_Render2DGestureOnCamera));
            m_cameraRender.m_AKN_MGR_Component = this;
        }

        /// \fn     public void OnGUI()
        ///
        /// \brief  Called every frame to display GUI elments.Show gesture probabilities and gesture draw options.
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        public void OnGUI()
        {
            if (m_properties.m_ShowProbabilitiesInScene)
            {
                for (int i = 0; i < m_properties.m_gesturesProbabilities.Count; i++)
                {
                    GUI.Label(new Rect(Screen.width - 280, (i * 20) + 30, 250, 25), ("Probabilité HMM " + (i + 1)) + " : " + m_properties.m_gesturesProbabilities[i]);
                }
            }

            if (m_points.Count > 0 && m_points[0].Count > 0)
            {
                if (GUI.Button(new Rect(50, 50, 100, 25), "Evaluate")) MEvaluateCurrentGesture();
                if (GUI.Button(new Rect(50, 85, 100, 25), "Clear")) MClearDraw();

                if (m_properties.m_recognizedAs != -1)
                    GUI.Label(new Rect(50, 115, 200, 25), "HMM " + (m_properties.m_recognizedAs + 1) + " reconnue !");
                else
                    GUI.Label(new Rect(50, 115, 100, 25), "Non reconnue !");
            }
        }

        /// \fn     public void Update()
        ///
        /// \brief  Called every frame. Handle mouse input events.
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        public void Update()
        {
            if (Input.GetMouseButtonDown(0)) MStartNewGesture();
            if (Input.GetMouseButtonUp(0)) _m_drawMode = false;
            if (_m_drawMode) _MTakePoint();
        }

        /// \fn     private void _MInitAllValues()
        ///
        /// \brief  Init all values used in the plugin, some of theme are related to Algorithme logic and the other are just to control interfaces.
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        private void _MInitAllValues()
        {
            m_properties = ScriptableObject.CreateInstance<AKN_MouseGesturesRecogniserProperties>();

            m_properties.m_HMMs = new List<AKN_HiddenMarkovModel<int>>();
            m_properties.m_gesturesProbabilities = new List<float>();
            m_points = new List<List<Vector2>>();
            m_properties.m_currentGesture = new List<int>();

            iconsSource = @"/Resources/Icon/";

            m_properties.nb_states = new List<int>();
            m_properties.nb_observations = new List<int>();
            m_properties.statesNames = new List<String>();
            m_properties.statesProbabilities = new List<float>();
            m_properties.transitions = new List<float>();
            m_properties.observations = new List<int>();
            m_properties.emissions = new List<float>();

            m_properties.UpdateSerializingValues();

            m_properties.m_recognizedAs = -1;
            _m_gestureIndex = -1;
            _m_drawMode = false;

            m_properties.m_ShowProbabilitiesInScene = false;
            m_properties.m_ShowProbabilitiesInInspector = false;
            m_properties.m_DisplayRemoveDialog = false;

            m_properties.m_HmmsStatesCount = new List<int>();
            m_properties.m_HmmsObservationsCount = new List<int>();
            m_properties.m_ShowGestures = new List<bool>();

            for (int i = 0; i < m_properties.m_HMMs.Count; i++)
            {
                m_properties.m_ShowGestures.Add(false);
                m_properties.m_HmmsStatesCount.Add(3);
                m_properties.m_HmmsObservationsCount.Add(2);
            }

            m_properties.hideFlags = HideFlags.HideInInspector;

            _MInitIcons();
        }

        /// \fn     private void _MInitIcons()
        ///
        /// \brief  Init All dispayed Icons.
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        private void _MInitIcons()
        {
            m_properties.m_plusTexture = new Texture2D(1, 1);
            m_properties.m_minusTexture = new Texture2D(1, 1);
            m_properties.m_editTexture = new Texture2D(1, 1);
            m_properties.m_manualBuildTexture = new Texture2D(1, 1);
            m_properties.m_autoBuildTexture = new Texture2D(1, 1);

            m_properties.m_plusTexture.LoadImage(System.IO.File.ReadAllBytes(Application.dataPath + iconsSource + "add_icon.png"));
            m_properties.m_minusTexture.LoadImage(System.IO.File.ReadAllBytes(Application.dataPath + iconsSource + "del_icon.png"));
            m_properties.m_editTexture.LoadImage(System.IO.File.ReadAllBytes(Application.dataPath + iconsSource + "edit_icon.png"));
            m_properties.m_editTexture.LoadImage(System.IO.File.ReadAllBytes(Application.dataPath + iconsSource + "edit_icon.png"));
            m_properties.m_manualBuildTexture.LoadImage(System.IO.File.ReadAllBytes(Application.dataPath + iconsSource + "description_icon.png"));
            m_properties.m_autoBuildTexture.LoadImage(System.IO.File.ReadAllBytes(Application.dataPath + iconsSource + "sd_icon.png"));
        }

        /// \fn     private void _MTakePoint()
        ///
        /// \brief  Take mouse pointer position in each  specific amount of time, this will lead to constructing draw by saving a range of pointer coordinates.
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        private void _MTakePoint()
        {
            _m_tickCounter += Time.deltaTime;
            if (_m_tickCounter > _m_tickTime)
            {
                m_points[_m_gestureIndex].Add(Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane)));
                _m_tickCounter = 0;
            }
        }
        
        /// \fn     public void MRecogniseGesture()
        ///
        /// \brief  Evaluate a gesture and determine the most likely stored HMM that describe the gesture.
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        public void MRecogniseGesture()
        {
            if (m_properties.m_gesturesProbabilities.Count == 0) return;

            int[] gesture = m_properties.m_currentGesture.ToArray();

            for (int i = 0; i < m_properties.m_HMMs.Count; i++)
                m_properties.m_gesturesProbabilities[i] = m_properties.m_HMMs[i] != null ? m_properties.m_HMMs[i].MEvaluate(gesture, eUseAlgorithme.kForward) : -1;

            float maxProb = -1;
            m_properties.m_recognizedAs = -1;

            for (int i = 0; i < m_properties.m_HMMs.Count; i++)
            {
                if (m_properties.m_gesturesProbabilities[i] > maxProb && m_properties.m_gesturesProbabilities[i] != 0)
                {
                    maxProb = m_properties.m_gesturesProbabilities[i]; m_properties.m_recognizedAs = i;
                }
            }
        }

        /*public void MGenerateDefaultHMMs()
        {
            AKN_HiddenMarkovModel<int> hmm;
            
            #region digit 0
                hmm = new AKN_HiddenMarkovModel<int>(4, 17,"Character 0");
                hmm.MSetStates(new float[] { 0.125F, 0.125F, 0.125F, 0.125F, 0.125F, 0.125F, 0.125F, 0.125F, 0.0F },
                    new string[] { "MicroGeste_1", "MicroGeste_2", "MicroGeste_3", "MicroGeste_4", "MicroGeste_5", "MicroGeste_6", "MicroGeste_7", "MicroGeste_8", "MicroGeste_9" });
                hmm.MSetTransitions(new float[,] { { 0.6F, 0.35F, 0.05F, 0.0F }, { 0.03F, 0.47F, 0.5F, 0.0F }, { 0.0F, 0.0F, 0.6F, 0.4F }, { 0.4F, 0.0F, 0.0F, 0.6F } });
                hmm.MSetObservations(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17});
                hmm.MSetEmissions(new float[,] { 
                                                 { 0.4F, 0.1F, 0.05F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.15F, 0.3F, 0.0F},
                                                 { 0.0F, 0.0F, 0.0F, 0.1F, 0.4F, 0.4F, 0.1F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F},
                                                 { 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.2F, 0.3F, 0.3F, 0.2F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F},
                                                 { 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.05F, 0.2F, 0.25F, 0.25F, 0.2F, 0.05F, 0.0F, 0.0F},
                                                 { 0.4F, 0.1F, 0.05F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.15F, 0.3F, 0.0F},
                                                 { 0.0F, 0.0F, 0.0F, 0.1F, 0.4F, 0.4F, 0.1F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F},
                                                 { 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.2F, 0.3F, 0.3F, 0.2F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F},
                                                 { 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.05F, 0.2F, 0.25F, 0.25F, 0.2F, 0.05F, 0.0F, 0.0F},
                                                 { 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 1.0F}
                                               });
                hmm.MCheckValues();
                m_properties.m_HMMs.Add(hmm);
                m_properties.m_gesturesProbabilities.Add(0);
            #endregion
        
            #region digit 1 // validé
                hmm = new AKN_HiddenMarkovModel<int>(3, 17, "Character 1");
                hmm.MSetStates(new float[] { 0.7F, 0.3F, 0.0F }, new string[] { "MicroGeste_1", "MicroGeste_2", "STOP" });
                hmm.MSetTransitions(new float[,] { { 0.5F, 0.5F, 0.0F }, { 0.0F, 0.8F, 0.2F }, { 0.0F, 0.0F, 1.0F } });
                hmm.MSetObservations(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17});
                hmm.MSetEmissions(new float[,] { { 0.4F, 0.4F, 0.2F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F},
                                                 { 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.1F, 0.4F, 0.4F, 0.1F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F}, 
                                                 { 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 1.0F}
                                                });
                hmm.MCheckValues();    
                m_properties.m_HMMs.Add(hmm);
                m_properties.m_gesturesProbabilities.Add(0);
            #endregion
            /*
            #region digit 2
                hmm = new AKN_HiddenMarkovModel<int>(8, 16, "Character 2");
                hmm.MSetStates(
                new float[] { 0.4F, 0.4F, 0.2F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F },
                new string[] { "MicroGeste_1", "MicroGeste_2", "MicroGeste_3", "MicroGeste_4", "MicroGeste_5", "MicroGeste_6", "MicroGeste_7", "MicroGeste_8" });
                hmm.MSetTransitions(new float[,] { { 0.29F, 0.6F, 0.1F, 0.01F, 0.0F, 0.0F, 0.0F, 0.0F },
                                                   { 0.0F, 0.3F, 0.5F, 0.15F, 0.0F, 0.0F, 0.05F, 0.0F },
                                                   { 0.0F, 0.0F, 0.3F, 0.5F, 0.2F, 0.0F, 0.0F, 0.0F },
                                                   { 0.0F, 0.0F, 0.0F, 0.3F, 0.5F, 0.2F, 0.0F, 0.0F },
                                                   { 0.0F, 0.0F, 0.0F, 0.0F, 0.3F, 0.5F, 0.2F, 0.0F },
                                                   { 0.0F, 0.05F, 0.0F, 0.0F, 0.0F, 0.3F, 0.5F, 0.15F },
                                                   { 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.7F, 0.3F },
                                                   { 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 1.0F }});
                hmm.MSetObservations(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 });
                hmm.MSetEmissions(new float[,] { { 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.03F, 0.05F, 0.07F, 0.15F, 0.33F, 0.25F, 0.12F, 0.0F},
                                                 { 0.4F, 0.1F, 0.05F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.15F, 0.3F},
                                                 { 0.0F, 0.0F, 0.1F, 0.4F, 0.4F, 0.1F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F},
                                                 { 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.2F, 0.3F, 0.3F, 0.2F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F},
                                                 { 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.05F, 0.3F, 0.3F, 0.3F, 0.05F, 0.0F, 0.0F, 0.0F},
                                                 { 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.2F, 0.25F, 0.25F, 0.2F, 0.1F, 0.0F},
                                                 { 0.0F, 0.0F, 0.0F, 0.0F, 0.1F, 0.4F, 0.4F, 0.1F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F},
                                                 { 0.1F, 0.3F, 0.3F, 0.3F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F}});
                hmm.MCheckValues();    
                m_properties.m_HMMs.Add(hmm);
                m_properties.m_gesturesProbabilities.Add(0);
            #endregion
       
            #region digit 3
                hmm = new AKN_HiddenMarkovModel<int>(8, 16, "Character 3");
                hmm.MSetStates(
                    new float[] { 0.5F, 0.3F, 0.2F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F },
                    new string[] { "MicroGeste_1", "MicroGeste_2", "MicroGeste_3", "MicroGeste_4", "MicroGeste_5", "MicroGeste_6", "MicroGeste_7", "MicroGeste_8" });
                hmm.MSetTransitions(new float[,] { { 0.3F, 0.55F, 0.1F, 0.0F, 0.05F, 0.0F, 0.0F ,0.0F },
                                                       { 0.0F, 0.4F, 0.55F, 0.05F, 0.0F, 0.0F, 0.0F ,0.0F  },
                                                       { 0.0F, 0.0F, 0.5F, 0.45F, 0.05F, 0.0F, 0.0F  ,0.0F },
                                                       { 0.1F, 0.0F, 0.0F, 0.6F, 0.3F, 0.0F, 0.0F  ,0.0F },
                                                       { 0.0F, 0.0F, 0.0F, 0.0F, 0.6F, 0.35F, 0.05F  ,0.0F },
                                                       { 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.5F, 0.5F  ,0.0F },
                                                       { 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.7F  ,0.3F },
                                                       { 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F  ,1.0F }});//
                hmm.MSetObservations(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 });
                hmm.MSetEmissions(new float[,] { { 0.4F, 0.1F, 0.05F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.15F, 0.3F},
                                                     { 0.0F, 0.0F, 0.1F, 0.4F, 0.4F, 0.1F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F},
                                                     { 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.2F, 0.3F, 0.3F, 0.2F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F},
                                                     { 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.2F, 0.25F, 0.25F, 0.2F, 0.1F, 0.0F},
                                                     { 0.0F, 0.0F, 0.0F, 0.1F, 0.4F, 0.3F, 0.2F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F},
                                                     { 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.2F, 0.3F, 0.3F, 0.2F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F},
                                                     { 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.2F, 0.25F, 0.25F, 0.2F, 0.1F, 0.0F},
                                                     { 0.3F, 0.2F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.2F, 0.3F}});
                hmm.MCheckValues();
                m_properties.m_HMMs.Add(hmm);
                m_properties.m_gesturesProbabilities.Add(0);
            #endregion

            #region digit 4
                hmm = new AKN_HiddenMarkovModel<int>(3, 16, "Character 4");
                hmm.MSetStates(new float[] { 1.0F, 0.0F, 0.0F }, new string[] { "MicroGeste_1", "MicroGeste_2", "MicroGeste_3" });
                hmm.MSetTransitions(new float[,] { { 0.6F, 0.4F, 0.0F }, { 0.0F, 0.6F, 0.4F }, { 0.0F, 0.0F, 1.0F } });
                hmm.MSetObservations(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 });
                hmm.MSetEmissions(new float[,] { { 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.1F, 0.4F, 0.4F, 0.1F, 0.0F, 0.0F},
                                                     { 0.1F, 0.4F, 0.4F, 0.1F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F},
                                                     { 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.1F, 0.4F, 0.4F, 0.1F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F}});

                hmm.MCheckValues();    
                m_properties.m_HMMs.Add(hmm);
                m_properties.m_gesturesProbabilities.Add(0);
            #endregion

            #region digit 5
                hmm = new AKN_HiddenMarkovModel<int>(5, 16, "Character 5");
                hmm.MSetStates(new float[] { 0.7F, 0.3F, 0.0F, 0.0F, 0.0F }, new string[] { "MicroGeste_1", "MicroGeste_2", "MicroGeste_3", "MicroGeste_4", "MicroGeste_5" });
                hmm.MSetTransitions(new float[,] { { 0.4F, 0.6F, 0.0F, 0.0F, 0.0F }, 
                                                       { 0.0F, 0.4F, 0.6F, 0.0F, 0.0F },
                                                       { 0.0F, 0.0F, 0.4F, 0.6F, 0.0F },
                                                       { 0.0F, 0.0F, 0.0F, 0.4F, 0.6F },
                                                       { 0.0F, 0.0F, 0.0F, 0.0F, 1.0F }});
                hmm.MSetObservations(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 });
                hmm.MSetEmissions(new float[,] { { 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.1F, 0.4F, 0.4F, 0.1F, 0.0F, 0.0F},
                                                     { 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.1F, 0.4F, 0.4F, 0.1F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F},
                                                     { 0.0F, 0.1F, 0.1F, 0.3F, 0.25F, 0.1F, 0.1F, 0.05F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F},
                                                     { 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.05F, 0.2F, 0.25F, 0.25F, 0.2F, 0.05F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F},
                                                     { 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.05F, 0.2F, 0.25F, 0.25F, 0.2F, 0.05F, 0.0F}});
                hmm.MCheckValues();    
                m_properties.m_HMMs.Add(hmm);
                m_properties.m_gesturesProbabilities.Add(0);
            #endregion 

            #region digit 6
                hmm = new AKN_HiddenMarkovModel<int>(6, 16, "Character 6");
                hmm.MSetStates(
                    new float[] { 0.5F, 0.4F, 0.1F, 0.0F, 0.0F, 0.0F },
                    new string[] { "MicroGeste_1", "MicroGeste_2", "MicroGeste_3", "MicroGeste_4", "MicroGeste_5", "MicroGeste_6" });
                hmm.MSetTransitions(new float[,] { { 0.4F, 0.6F, 0.0F, 0.0F, 0.0F, 0.0F},
                                                       { 0.0F, 0.6F, 0.3F, 0.1F, 0.0F, 0.0F},
                                                       { 0.0F, 0.0F, 0.6F, 0.3F, 0.1F, 0.0F },
                                                       { 0.0F, 0.0F, 0.0F, 0.6F, 0.3F, 0.1F},
                                                       { 0.0F, 0.0F, 0.0F, 0.0F, 0.6F, 0.4F},
                                                       { 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 1.0F}});
                hmm.MSetObservations(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 });
                hmm.MSetEmissions(new float[,] { { 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.2F, 0.3F, 0.3F, 0.2F},
                                                     { 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.1F, 0.3F, 0.3F, 0.2F, 0.1F, 0.0F, 0.0F, 0.0F},
                                                     { 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.2F, 0.3F, 0.3F, 0.2F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F},
                                                     { 0.0F, 0.0F, 0.2F, 0.3F, 0.3F, 0.2F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F},
                                                     { 0.3F, 0.15F, 0.05F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.05F, 0.15F, 0.3F},
                                                     { 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.2F, 0.3F, 0.3F, 0.2F, 0.0F, 0.0F}});
                hmm.MCheckValues();    
                m_properties.m_HMMs.Add(hmm);
                m_properties.m_gesturesProbabilities.Add(0);
            #endregion

            #region digit 7
                hmm = new AKN_HiddenMarkovModel<int>(2, 16, "Character 7");
                hmm.MSetStates(new float[] { 1.0F, 0.0F }, new string[] { "MicroGeste_1", "MicroGeste_2" });
                hmm.MSetTransitions(new float[,] { { 0.6F, 0.4F }, { 0.0F, 1.0F } });
                hmm.MSetObservations(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 });
                hmm.MSetEmissions(new float[,] { { 0.0F, 0.1F, 0.4F, 0.4F, 0.1F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F},
                                                     { 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.2F, 0.4F, 0.35F, 0.05F, 0.0F, 0.0F, 0.0F, 0.0F} });
                hmm.MCheckValues();    
                m_properties.m_HMMs.Add(hmm);
                m_properties.m_gesturesProbabilities.Add(0);
            #endregion
        
            #region digit 8
                hmm = new AKN_HiddenMarkovModel<int>(8, 16, "Character 8");
                hmm.MSetStates(
                    new float[] { 0.125F, 0.125F, 0.125F, 0.125F, 0.125F, 0.125F, 0.125F, 0.125F },
                    new string[] { "MicroGeste_1", "MicroGeste_2", "MicroGeste_3", "MicroGeste_4", "MicroGeste_5", "MicroGeste_6", "MicroGeste_7", "MicroGeste_8" });
                hmm.MSetTransitions(new float[,] { { 0.4F, 0.6F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F ,0.0F  },
                                                       { 0.0F, 0.4F, 0.6F, 0.0F, 0.0F, 0.0F, 0.0F ,0.0F  },
                                                       { 0.0F, 0.0F, 0.4F, 0.6F, 0.0F, 0.0F, 0.0F ,0.0F  },
                                                       { 0.0F, 0.0F, 0.0F, 0.4F, 0.6F, 0.0F, 0.0F ,0.0F  },
                                                       { 0.0F, 0.0F, 0.0F, 0.0F, 0.4F, 0.6F, 0.0F ,0.0F  },
                                                       { 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.4F, 0.6F ,0.0F  },
                                                       { 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.4F ,0.6F  },
                                                       { 0.4F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F ,0.6F  }});
                hmm.MSetObservations(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 });
                hmm.MSetEmissions(new float[,] { { 0.2F, 0.3F, 0.3F, 0.2F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F},
                                                     { 0.3F, 0.15F, 0.05F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.05F, 0.15F, 0.3F},
                                                     { 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.05F, 0.15F, 0.3F, 0.3F, 0.15F, 0.05F, 0.0F},
                                                     { 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.05F, 0.15F, 0.3F, 0.3F, 0.15F, 0.05F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F},
                                                     { 0.0F, 0.0F, 0.0F, 0.0F, 0.2F, 0.3F, 0.3F, 0.2F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F},
                                                     { 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.05F, 0.15F, 0.3F, 0.3F, 0.15F, 0.05F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F},
                                                     { 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.05F, 0.15F, 0.3F, 0.3F, 0.15F, 0.05F, 0.0F},
                                                     { 0.3F, 0.15F, 0.05F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.05F, 0.15F, 0.3F}});
                hmm.MCheckValues();    
                m_properties.m_HMMs.Add(hmm);
                m_properties.m_gesturesProbabilities.Add(0);
            #endregion
            
            #region digit 9
                hmm = new AKN_HiddenMarkovModel<int>(6, 16, "Character 9");
                hmm.MSetStates(
                    new float[] { 0.8F, 0.2F, 0.0F, 0.0F, 0.0F, 0.0F },
                    new string[] { "MicroGeste_1", "MicroGeste_2", "MicroGeste_3", "MicroGeste_4", "MicroGeste_5", "MicroGeste_6" });
                hmm.MSetTransitions(new float[,] { { 0.4F, 0.6F, 0.0F, 0.0F, 0.0F, 0.0F},
                                                       { 0.0F, 0.6F, 0.3F, 0.1F, 0.0F, 0.0F},
                                                       { 0.0F, 0.0F, 0.6F, 0.3F, 0.1F, 0.0F },
                                                       { 0.0F, 0.0F, 0.0F, 0.6F, 0.3F, 0.1F},
                                                       { 0.0F, 0.0F, 0.0F, 0.0F, 0.6F, 0.4F},
                                                       { 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 1.0F}});
                hmm.MSetObservations(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 });
                hmm.MSetEmissions(new float[,] { { 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.1F, 0.1F, 0.1F, 0.3F, 0.3F, 0.1F, 0.0F, 0.0F},
                                                     { 0.3F, 0.1F, 0.1F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.1F, 0.1F, 0.3F},
                                                     { 0.0F, 0.1F, 0.1F, 0.3F, 0.3F, 0.1F, 0.1F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F},
                                                     { 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.1F, 0.1F, 0.3F, 0.3F, 0.1F, 0.1F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F},
                                                     { 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.2F, 0.3F, 0.3F, 0.2F, 0.0F, 0.0F, 0.0F, 0.0F},
                                                     { 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.1F, 0.1F, 0.1F, 0.3F, 0.3F, 0.1F, 0.0F, 0.0F}});

                hmm.MCheckValues();
                m_properties.m_HMMs.Add(hmm);
                m_properties.m_gesturesProbabilities.Add(0);
            #endregion
            

        }*/
        
        /// \fn     public void MProcessDraw(List<List<Vector2>> _points)
        ///
        /// \brief  Transform a range of points to a range of euler angles by calculating directions and angle degree of each two points.
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        ///
        /// \param  _points  points draw to process
        public void MProcessDraw(List<List<Vector2>> _points)
        {
            float angle; int shift = -1; int angleDirection; Vector2 v;
            for (int i = 0; i < _points.Count; i++)
            {
                for (int j = 1; j < _points[i].Count; j++)
                {
                    v = new Vector2(_points[i][j - 1].x, _points[i][j - 1].y + shift);
                    angleDirection = MGetAngleDirection(_points[i][j - 1], v, _points[i][j]);
                    if (angleDirection == -1) angle = ((-1 * MGetAngle(_points[i][j - 1], v, _points[i][j])) + 360F); else angle = MGetAngle(_points[i][j - 1], v, _points[i][j]);

                    angle = ((int)(angle / 22.5F));
                    angle = angle > 16 ? angle - 16 : angle;

                    m_properties.m_currentGesture.Add((int)angle);
                }
                m_properties.m_currentGesture.Add(17);
            }
        }
        
        /// \fn     public void MStartNewGesture()
        ///
        /// \brief  Initiate draw of new gesture.
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        public void MStartNewGesture()
        {
            _m_gestureIndex++;
            m_points.Add(new List<Vector2>());
            _m_drawMode = true;
            _m_tickCounter = _m_tickTime + 1;
        }
        
        /// \fn     public void MClearDraw()
        ///
        /// \brief  Clear gesture drawed by user (delete all points).
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        public void MClearDraw()
        {
            m_points.Clear();
            m_properties.m_currentGesture.Clear();
            _m_gestureIndex = -1;
            m_properties.m_recognizedAs = -1;
        }
        
        /// \fn     public void MEvaluateCurrentGesture()
        ///
        /// \brief  Evaluate drawed gesture by processing gesture draw and cheking likelyhood probabilities
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        public void MEvaluateCurrentGesture()
        {
            m_points[_m_gestureIndex].Clear();
            _m_gestureIndex--;
            MProcessDraw(m_points);
            MRecogniseGesture();
        }
        
        /// \fn     public int MGetAngleDirection(Vector2 _viewPoint, Vector2 _point1, Vector2 _point2)
        ///
        /// \brief  Calculate direction(left, right) of an angle.
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        /// 
        /// \param  _viewPoint view point.
        /// \param  _point1    first point.
        /// \param  _point2    second point.
        /// 
        /// \return euler angle , -1 if the angle is pointing to the left, 1 otherwise.
        public int MGetAngleDirection(Vector2 _viewPoint, Vector2 _point1, Vector2 _point2)
        {
            float dx1, dx2, dy1, dy2;

            dx1 = _point1.x - _viewPoint.x; dx2 = _point2.x - _viewPoint.x;
            dy1 = _point1.y - _viewPoint.y; dy2 = _point2.y - _viewPoint.y;

            if (System.Math.Abs(dx1) < _m_accuracyDouble) dx1 = 0.0F;
            if (System.Math.Abs(dx2) < _m_accuracyDouble) dx2 = 0.0F;
            if (System.Math.Abs(dy1) < _m_accuracyDouble) dy1 = 0.0F;
            if (System.Math.Abs(dy2) < _m_accuracyDouble) dy2 = 0.0F;

            if ((dx1 * dy2) > (dy1 * dx2)) return (+1);
            if ((dx1 * dy2) < (dy1 * dx2)) return (-1);
            if (((dx1 * dx2) < 0.0) || ((dy1 * dy2) < 0.0)) return (-1);
            if ((dx1 * dx1 + dy1 * dy1) < (dx2 * dx2 + dy2 * dy2)) return (+1);

            return (0);
        }

        /// \fn     private void _MTakePoint()
        ///
        /// \brief  Calculate angle degree using three points.
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        ///         
        /// \param  _viewPoint view point.
        /// \param  _point1    first point.
        /// \param  _point2    second point.
        /// 
        /// \return euler angle
        public float MGetAngle(Vector2 _viewPoint, Vector2 _point1, Vector2 _point2)
        {
            float a1, b1, a2, b2, a, b;

            a1 = _point1.x - _viewPoint.x;
            a2 = _point1.y - _viewPoint.y;
            b1 = _point2.x - _viewPoint.x;
            b2 = _point2.y - _viewPoint.y;

            a = (float)System.Math.Sqrt((a1 * a1) + (a2 * a2));
            b = (float)System.Math.Sqrt((b1 * b1) + (b2 * b2));

            if (a == 0.0F || b == 0.0F) return 0.0F;

            return ((float)System.Math.Acos((a1 * b1 + a2 * b2) / (a * b)) * 180.0F / (float)System.Math.PI);
        }

        /// \fn     public void OnDestroy()
        ///
        /// \brief  Called when ending play mode to remove Camera renderer.
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        public void OnDestroy()
        {
            try
            {
                if (Camera.main == null) return;
                Component comp = Camera.main.gameObject.GetComponent((typeof(AKN_Render2DGestureOnCamera)));
                Destroy(comp);
            } catch 
            {
            
            }
        }

    }
}