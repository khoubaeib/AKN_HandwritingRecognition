using System;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using AKeNe.AI.HiddenMarkovModel;

public enum eCallFor { kEvaluateGestures, kLearnGesture, kConstructHmm };

/// \namespace AKeNe.Tools.MouseGestureRecognition
/// 
/// \brief     namespace that contains all classes of mouse gesture recongnition tool.
///
/// \author    Khoubaeib Klai | khoubaeib@gmail.com
/// \date      24/04/2014
namespace AKeNe.Tools.MouseGestureRecognition
{

    /// \class   AKN_MouseGestureDrawWindow
    /// 
    /// \brief   Let the user draw a gesture, gestures can be evaluated, learned or used to determine HMM values.
    ///
    /// \author  Khoubaeib Klai | khoubaeib@gmail.com
    /// \date    24/04/2014
    ///
    /// \version 1.0
    /// 
    /// \WTF     many magic variables !!
    public class AKN_MouseGestureDrawWindow : EditorWindow
    {
        public static AKN_MouseGesturesRecogniserComponent m_AKN_MGR;
        public static int m_HMMIndex = -1;
        public static eCallFor m_CallFor;
        public static bool m_WindowOpened = false;
        public static String m_gestureName = "";

        public bool m_drawMode = false;

        private List<List<Vector2>> _m_points = new List<List<Vector2>>();
        private List<List<float>> _m_takeTime = new List<List<float>>();
        private float _m_tickTime = 0.0001F;
        private float _m_tickCounter = 0F;
        private int _m_gestureIndex = -1;
        private Material _m_lineMaterial;
        private float _m_gradiant = 0;
        float oldProb = 0, newProb = 0;

        /// \fn     public void OnEnable()
        ///
        /// \brief  Called when activate the Window, init window behaviour and draw parameters.
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        public void OnEnable()
        {
            titleContent = new GUIContent("Draw a gesture !");
            position = new Rect(position.x, position.y, 800, 600);
            wantsMouseMove = true;
            _m_points = new List<List<Vector2>>();
            _m_takeTime = new List<List<float>>();
            _m_gestureIndex = -1;
        }

        /// \fn     public void OnGUI()
        ///
        /// \brief  Called when activate the Window, init window behaviour and draw parameters.
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        public void OnGUI()
        {
            EditorGUILayout.Space();
            GUILayout.BeginHorizontal();
            if (_m_points.Count > 0)
            {
                if (GUILayout.Button("Clear draw", new GUILayoutOption[] { GUILayout.MaxWidth(150) })) _MClearDraw();
                switch (m_CallFor)
                {
                    case eCallFor.kLearnGesture: { _MLearnGesture(); break; }
                    case eCallFor.kConstructHmm: { _MBuildHMM(); break; }
                    case eCallFor.kEvaluateGestures: { _MEvaluateGesture(); break; }
                }
            }
            GUILayout.EndHorizontal();
            _MHandleDrawEvents();
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

        /// \fn     public static void MLaunch()
        /// 
        /// \brief  Start gesture draw Window.
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        public static void MLaunch()
        {
            GetWindow(typeof(AKN_MouseGestureDrawWindow)).Show();
        }

        /// \fn     private void _MHandleDrawEvents()
        /// 
        /// \brief  Start gesture draw Window.
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        /// 
        /// \bug    if the mouse is out of the draw window, it can't detect any events !
        private void _MHandleDrawEvents()
        {
            if (Event.current.type == EventType.MouseDown) _MStartNewGesture();
            if (Event.current.type == EventType.MouseUp) _MEndGesture();
            if (Event.current.type == EventType.Repaint) _MRenderDraw();

            if (m_drawMode) _MTakePoint();
        }

        /// \fn     public static void MLaunch()
        /// 
        /// \brief  End gesture draw, also process draw to remove very small edges.
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        private void _MEndGesture()
        {
            m_drawMode = false;
            _MRemoveFalseGesture();
            _MRemoveSmallEdges();
            Repaint();
        }

        /// \fn     private void _MRenderDraw()
        /// 
        /// \brief  Show draw on the Editor Window.
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        private void _MRenderDraw()
        {
            _MCreateLineMaterial();
            _m_lineMaterial.SetPass(0);
            GL.Begin(GL.LINES);
            for (int i = 0; i < _m_points.Count; i++)
                for (int j = 1; j < _m_points[i].Count; j++)
                {
                    _m_gradiant = ((float)j / (float)_m_points[i].Count);
                    GL.Color(new Color(1 - _m_gradiant, _m_gradiant, 1));
                    GL.Vertex(_m_points[i][j - 1]); GL.Vertex(_m_points[i][j]);
                }
            GL.End();
        }

        /// \fn     public void MCreateLineMaterial()
        ///
        /// \brief  Create Material to show gesture
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        private void _MCreateLineMaterial()
        {
            if (!_m_lineMaterial)
            {
                _m_lineMaterial = new Material(Shader.Find("Custom/DrawShader"));
                _m_lineMaterial.hideFlags = HideFlags.HideAndDontSave;
                _m_lineMaterial.shader.hideFlags = HideFlags.HideAndDontSave;
            }
        }

        /// \fn     private void _MEvaluateGesture()
        /// 
        /// \brief  Evaluate current gesture to recognise it correspond to any HMM.
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        private void _MEvaluateGesture()
        {
            if (GUILayout.Button("Evaluate gesture !", new GUILayoutOption[] { GUILayout.MaxWidth(150) }))
            {
                m_AKN_MGR.MProcessDraw(_m_points);
                m_AKN_MGR.MRecogniseGesture();
            }

            GUILayout.FlexibleSpace();
            GUILayout.TextField(m_AKN_MGR.m_properties.m_recognizedAs == -1 ? "Unknown !" : m_AKN_MGR.m_properties.m_HMMs[m_AKN_MGR.m_properties.m_recognizedAs].m_label, new GUILayoutOption[] { GUILayout.MaxWidth(150) });
        }

        /// \fn     private void _MLearnGesture()
        /// 
        /// \brief  After selecting a HMM. draw and Learn a gesture to refine HMM values.
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        private void _MLearnGesture()
        {
            if (GUILayout.Button("Learn this gesture !", new GUILayoutOption[] { GUILayout.MaxWidth(150) }))
            {
                m_AKN_MGR.MProcessDraw(_m_points);

                oldProb = m_AKN_MGR.m_properties.m_HMMs[m_HMMIndex].MEvaluate(m_AKN_MGR.m_properties.m_currentGesture.ToArray());
                if (oldProb > 0)
                {
                    m_AKN_MGR.m_properties.m_HMMs[m_HMMIndex].MUpdate(m_AKN_MGR.m_properties.m_currentGesture.ToArray(), 0, 5);
                    newProb = m_AKN_MGR.m_properties.m_HMMs[m_HMMIndex].MEvaluate(m_AKN_MGR.m_properties.m_currentGesture.ToArray());
                    m_AKN_MGR.m_properties.UpdateSerializingValues();
                    Debug.Log("Learning Success !");
                }
            }

            GUILayout.FlexibleSpace();
            if (oldProb <= 0)
            {
                GUILayout.TextField("Coudn't recognize this gesture !", new GUILayoutOption[] { GUILayout.MaxWidth(300) });
            }
            else
            {
                GUILayout.TextField("Old probability : " + oldProb, new GUILayoutOption[] { GUILayout.MaxWidth(300) });
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.TextField("new probability : " + newProb, new GUILayoutOption[] { GUILayout.MaxWidth(300) });
            }

        }

        /// \fn     private void _MRemoveSmallEdges()
        /// 
        /// \brief  Remove small edges to accelerate algorithme execution.
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        private void _MRemoveSmallEdges()
        {
            for (int i = 0; i < _m_points.Count; i++)
            {
                for (int j = 1; j < _m_points[i].Count; j++)
                {
                    if (Vector2.Distance(_m_points[i][j], _m_points[i][j - 1]) < 10)
                    {
                        _m_points[i].RemoveAt(j);
                        //if (_m_TakeTime[i].Count > j+1) _m_TakeTime[i][j + 1] += _m_TakeTime[i][j];
                        //_m_TakeTime[i].RemoveAt(j);
                        j--;
                    }
                }
            }
        }

        /// \fn     private void _MRemoveFalseGesture()
        /// 
        /// \brief  A gesture is not considered if it is composed by less than two points.
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        private void _MRemoveFalseGesture()
        {
            for (int i = 0; i < _m_points.Count; i++)
            {
                if (_m_points[i].Count < 2)
                {
                    _m_points.RemoveAt(i);
                    _m_takeTime.RemoveAt(i);
                }
            }
        }

        /// \fn     private void _MBuildHMM()
        /// 
        /// \brief  Build an HMM from current gesture.
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        private void _MBuildHMM()
        {
            if (GUILayout.Button("Construct HMM !", new GUILayoutOption[] { GUILayout.MaxWidth(150) }))
            {
                m_AKN_MGR.MProcessDraw(_m_points);
                m_AKN_MGR.m_properties.m_HMMs[m_HMMIndex] = _MGenerateHMMParameters();
                m_AKN_MGR.m_properties.UpdateSerializingValues();
#if DEBUG
                Debug.Log("HMM building success !");
#endif
                _MClearDraw();
            }
        }

        /// \fn     private void _MStartNewGesture()
        /// 
        /// \brief  Initiate the process of drawing a new gesture.
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        private void _MStartNewGesture()
        {
            _m_takeTime.Add(new List<float>());
            _m_tickCounter = _m_tickTime + 1;
            _m_gestureIndex++;
            _m_points.Add(new List<Vector2>());
            m_drawMode = true;
            m_AKN_MGR.m_properties.m_recognizedAs = -1;
        }

        /// \fn     private AKN_HiddenMarkovModel<int> _MBuildHMMFromDirections(List<int> _directions)
        /// 
        /// \brief  Determine HMM praobabilities from given directions.
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        /// 
        /// \param  _directions directions calculated from draw points
        /// 
        /// \return Constructed HMM
        private AKN_HiddenMarkovModel<int> _MBuildHMMFromDirections(List<int> _directions)
        {
            _directions = _MRemoveDirectionRepetition(_directions);

            AKN_HiddenMarkovModel<int> gestureHmm = new AKN_HiddenMarkovModel<int>(_directions.Count, 17, m_gestureName);

            gestureHmm = _MSetObservations(gestureHmm);
            gestureHmm = _MSetStates(gestureHmm);
            gestureHmm = _MSetTransitions(gestureHmm);
            gestureHmm = _MSetEmissions(gestureHmm, _directions);

            return gestureHmm;
        }

        /// \fn     private AKN_HiddenMarkovModel<int> _MSetObservations(AKN_HiddenMarkovModel<int> _gestureHmm)
        /// 
        /// \brief  Update constructed Hmm with new observations values.
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        ///
        /// \param  _gestureHmm HMM to update.
        /// 
        /// \return new HMM with updated values.
        private AKN_HiddenMarkovModel<int> _MSetObservations(AKN_HiddenMarkovModel<int> _gestureHmm)
        {
            for (int k = 0; k < _gestureHmm.m_ObservationsCount; k++) _gestureHmm.MSetObservation(k, k + 1);
            return _gestureHmm;
        }

        /// \fn     private AKN_HiddenMarkovModel<int> _MSetEmissions(AKN_HiddenMarkovModel<int> _gestureHmm, List<int> _directions)
        /// 
        /// \brief  Update constructed Hmm with new emissions values.
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        /// 
        /// \param  _gestureHmm HMM to update.
        /// 
        /// \return new HMM with updated values.
        private AKN_HiddenMarkovModel<int> _MSetEmissions(AKN_HiddenMarkovModel<int> _gestureHmm, List<int> _directions)
        {
            for (int j = 0; j < _gestureHmm.m_StatesCount; j++)
            {
                if (_directions[j] == 17)
                    _gestureHmm.MSetEmission(j, 17, 1f);
                else if (_directions[j] > 0)
                {
                    _gestureHmm.MSetEmission(j, _MAdjustAngle(_directions[j] + 2), 0.15F);
                    _gestureHmm.MSetEmission(j, _MAdjustAngle(_directions[j] + 1), 0.2F);
                    _gestureHmm.MSetEmission(j, _MAdjustAngle(_directions[j]), 0.3F);
                    _gestureHmm.MSetEmission(j, _MAdjustAngle(_directions[j] - 1), 0.20F);
                    _gestureHmm.MSetEmission(j, _MAdjustAngle(_directions[j] - 2), 0.15F);
                }
            }
            return _gestureHmm;
        }

        /// \fn     private AKN_HiddenMarkovModel<int> _MSetTransitions(AKN_HiddenMarkovModel<int> _gestureHmm)
        /// 
        /// \brief  Update constructed Hmm with new transitions values.
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        /// 
        /// \param  _gestureHmm HMM to update.
        /// 
        /// \return new HMM with updated values.
        private AKN_HiddenMarkovModel<int> _MSetTransitions(AKN_HiddenMarkovModel<int> _gestureHmm)
        {
            for (int j = 0; j < _gestureHmm.m_StatesCount; j++) _gestureHmm.MSetTransition((j * _gestureHmm.m_StatesCount) + j, 0.4F);
            for (int k = 0; k < _gestureHmm.m_StatesCount - 1; k++) _gestureHmm.MSetTransition((k * _gestureHmm.m_StatesCount) + k + 1, 0.35F);
            for (int k = 0; k < _gestureHmm.m_StatesCount - 2; k++) _gestureHmm.MSetTransition((k * _gestureHmm.m_StatesCount) + k + 2, 0.25F);
            _gestureHmm.MSetTransition(((_gestureHmm.m_StatesCount - 1) * _gestureHmm.m_StatesCount) + _gestureHmm.m_StatesCount - 1, 1F);
            return _gestureHmm;
        }

        /// \fn     private AKN_HiddenMarkovModel<int> _MSetStates(AKN_HiddenMarkovModel<int> _gestureHmm)
        /// 
        /// \brief  Update constructed Hmm with new states values.
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        /// 
        /// \param  _gestureHmm HMM to update.
        /// 
        /// \return new HMM with updated values.
        private AKN_HiddenMarkovModel<int> _MSetStates(AKN_HiddenMarkovModel<int> _gestureHmm)
        {
            for (int j = 0; j < _gestureHmm.m_StatesCount; j++)
            {
                _gestureHmm[j].m_name = ("Etat " + j);
                _gestureHmm[j].m_startProbability = 0F;
                for (int k = 0; k < _gestureHmm.m_ObservationsCount; k++) _gestureHmm.MSetEmissionByIndex(j, k, 0);
            }

            _gestureHmm[0].m_startProbability = 0.5F;
            _gestureHmm[1].m_startProbability = 0.5F;

            return _gestureHmm;
        }

        /// \fn     private List<int> _MRemoveDirectionRepetition(List<int> _directions)
        /// 
        /// \brief  Remove repeated direction to refine HMM concluded values.
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        /// 
        /// \param  All directions derived from draw points.
        ///  
        /// \return Cleaned directions 
        private List<int> _MRemoveDirectionRepetition(List<int> _directions)
        {
            for (int i = 1; i < _directions.Count; )
            {
                if ((_directions[i] != -1) && (_directions[i - 1] == _directions[i]))
                {
                    _directions.RemoveAt(i - 1);
                }
                else i++;
            }
            return _directions;
        }

        /// \fn     public List<List<float>> _MCalculateSpeed()
        /// 
        /// \brief  Calculates accelerations by deriving values from speeds.
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        /// 
        /// \return calculated speeds
        public List<List<float>> _MCalculateSpeed()
        {
            List<List<float>> speeds = new List<List<float>>();
            float derivedValue;

            for (int i = 0; i < _m_points.Count; i++)
            {
                speeds.Add(new List<float>());
                speeds[i].Add(0.0F);
                for (int j = 1; j < _m_points[i].Count; j++)
                {
                    derivedValue = Vector2.Distance(_m_points[i][j], _m_points[i][j - 1]);
                    speeds[i].Add(derivedValue / _m_takeTime[i][j]);
                }
            }
            return speeds;
        }

        /// \fn     private List<List<float>> _MCalculateAccelerations(List<List<float>> _speeds)
        /// 
        /// \brief  Calculates accelerations by deriving values from speeds.
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        /// 
        /// \param _speeds speed of drawing between each points.
        /// 
        /// \return calculated accelerations
        private List<List<float>> _MCalculateAccelerations(List<List<float>> _speeds)
        {
            List<List<float>> accelerations = new List<List<float>>();
            float derivedValue;

            for (int i = 0; i < _speeds.Count; i++)
            {
                accelerations.Add(new List<float>());
                accelerations[i].Add(0.0F);
                for (int j = 1; j < _speeds[i].Count; j++)
                {
                    derivedValue = _speeds[i][j] - _speeds[i][j - 1];
                    accelerations[i].Add(derivedValue / _m_takeTime[i][j]);
                }
            }
            return accelerations;
        }

        /// \fn     private List<List<float>> _MCalculateShakes()
        /// 
        /// \brief  Calculates shakes by deriving accelerations.
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        private List<List<float>> _MCalculateShakes()
        {
            List<List<float>> speeds = _MCalculateSpeed();
            List<List<float>> accelerations = _MCalculateAccelerations(speeds);

            List<List<float>> shakes = new List<List<float>>(); float derivedValue;

            for (int i = 0; i < accelerations.Count; i++)
            {
                shakes.Add(new List<float>());
                shakes[i].Add(0.0F);
                for (int j = 1; j < accelerations[i].Count; j++)
                {
                    derivedValue = accelerations[i][j] - accelerations[i][j - 1];
                    shakes[i].Add(derivedValue / _m_takeTime[i][j]);
                }
            }

            return shakes;
        }

        /// \fn     private List<int> _MGetDirectionsAverage(List<List<float>> _shakes)
        /// 
        /// \brief  Determine wich directions must be saved based on calculated shakes.
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        private List<int> _MGetDirectionsAverage(List<List<float>> _shakes)
        {
            List<int> directions = new List<int>();
            List<int> tmpdirs = new List<int>();
            int index = 0;

            for (int i = 0; i < _shakes.Count; i++)
            {
                int signe = System.Math.Sign(_shakes[i][0]);
                for (int j = 1; j < _shakes[i].Count; j++)
                {
                    tmpdirs.Add(m_AKN_MGR.m_properties.m_currentGesture[index]);
                    index++;
                    if (signe != System.Math.Sign(_shakes[i][j]))
                    {
                        signe = System.Math.Sign(_shakes[i][j]);
                        directions.Add(_MAverageNumber(tmpdirs));
                        tmpdirs = new List<int>();
                    }
                }
                directions.Add(m_AKN_MGR.m_properties.m_currentGesture[index]);
                index++;
            }
            return directions;
        }

        /// \fn     private AKN_HiddenMarkovModel<int> _MGenerateHMMParameters()
        /// 
        /// \brief  Generate HMM and determine it's parametres to describe a drawed gesture.
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        private AKN_HiddenMarkovModel<int> _MGenerateHMMParameters()
        {
            if (_m_points.Count < 0 && _m_points[0].Count > 1) return null;
            return _MBuildHMMFromDirections(_MGetDirectionsAverage(_MCalculateShakes()));
        }

        /// \fn     private int _MAdjustNegativeAngle(int _i)
        /// 
        /// \brief  Adjust over borders angle. Angle must be between -1 and -16.
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        ///
        /// \param  _i angle to adjust
        /// 
        /// \return corrected angle
        private int _MAdjustNegativeAngle(int _i)
        {
            //if (i < -16) return i - 16;
            //if (i < 1) return i + 16;
            return -_MAdjustAngle(System.Math.Abs(_i));
        }

        /// \fn     private int _MAdjustAngle(int _i)
        /// 
        /// \brief  Adjust over borders angle. Angle must be between 1 and 16.
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        ///
        /// \param  _i angle to adjust
        /// 
        /// \return corrected angle
        private int _MAdjustAngle(int _i)
        {
            if (_i > 16) return _i - 16;
            if (_i < 1) return _i + 16;
            return _i;
        }

        /// \fn     private void _MTakePoint()
        /// 
        /// \brief  Take mouse pointer position at each instant amount of time.
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        private void _MTakePoint()
        {
            if (_m_tickCounter > _m_tickTime)
            {
                _MTakePointAtTick();
            }
            _m_tickCounter += Time.deltaTime;
        }

        /// \fn     private void _MTakePointAtTick()
        /// 
        /// \brief  Save mouse pointer position in the list of drawed points when called.
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        private void _MTakePointAtTick()
        {
            if (_m_takeTime[_m_gestureIndex].Count > 0)
                _m_takeTime[_m_gestureIndex].Add(_m_tickCounter);
            else
                _m_takeTime[_m_gestureIndex].Add(0);
            _m_points[_m_gestureIndex].Add(Event.current.mousePosition);
            _m_tickCounter = 0;
            Repaint();
        }

        /// \fn     private void _MClearDraw()
        /// 
        /// \brief  Clear screen from all drawed gesture.
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        private void _MClearDraw()
        {
            _m_takeTime.Clear();
            _m_gestureIndex = -1;
            _m_points.Clear();
            m_AKN_MGR.m_properties.m_currentGesture.Clear();
        }

        /// \fn     private int _MAverageNumber(List<int> _values)
        /// 
        /// \brief  Determine the average number at a range of values.
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        /// 
        /// \param _values range of numbers.
        /// 
        /// \return average number.
        private int _MAverageNumber(List<int> _values)
        {
            int sum = 0;
            bool leftCircle = false, rightCircle = false;
            for (int i = 0; i < _values.Count; i++)
            {
                if (_values[i] < 4) leftCircle = true;
                if (_values[i] > 12) rightCircle = true;
                if (leftCircle && rightCircle) break;
            }

            if (leftCircle && rightCircle)
            {
                for (int i = 0; i < _values.Count; i++) { if (_values[i] < 4) _values[i] += 16; sum += _values[i]; }
            }
            else
            {
                for (int i = 0; i < _values.Count; i++) sum += _values[i];
            }

            return _MAdjustAngle((int)(sum / _values.Count));
        }

    }
}