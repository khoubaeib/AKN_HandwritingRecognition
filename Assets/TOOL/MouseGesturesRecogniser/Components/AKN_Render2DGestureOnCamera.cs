using UnityEngine;
using System.Collections;

/// \namespace AKeNe.Tools.MouseGestureRecognition
/// 
/// \brief     namespace that contains all classes of mouse gesture recongnition tool.
///
/// \author    Khoubaeib Klai | khoubaeib@gmail.com
/// \date      24/04/2014
namespace AKeNe.Tools.MouseGestureRecognition
{
    /// \class   AKN_Render2DGestureOnCamera
    ///
    /// \brief   Render Mouse Gesture on scene in Play mode
    ///
    /// \author  Khoubaeib Klai | khoubaeib@gmail.com
    /// \date    24/04/2014
    /// 
    /// \version 1.0
    public class AKN_Render2DGestureOnCamera : MonoBehaviour
    {
        public Material m_lineMaterial;

        public AKN_MouseGesturesRecogniserComponent m_AKN_MGR_Component;//= null;

        private float _m_drawDepth;

        /// \fn     public void Start()
        ///
        /// \brief  Adjust draw depth to be in fornt of the camera
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        public void Start()
        {
            _m_drawDepth = transform.position.z > 0 ? transform.position.z + Camera.main.nearClipPlane + 0.001F : transform.position.z + Camera.main.nearClipPlane;
        }

        /// \fn     public void MCreateLineMaterial()
        ///
        /// \brief  Create Material to show gesture
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        public void MCreateLineMaterial()
        {
            if (!m_lineMaterial)
            {
                m_lineMaterial = new Material(Shader.Find("Custom/DrawShader"));
                m_lineMaterial.hideFlags = HideFlags.HideAndDontSave;
                m_lineMaterial.shader.hideFlags = HideFlags.HideAndDontSave;
            }
        }


        /// \fn     public void OnPostRender()
        ///
        /// \brief  Display gesture lines edge by edge with a range of colors between pink and purple.
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        public void OnPostRender()
        {
            if (m_AKN_MGR_Component == null) return;
            if (m_AKN_MGR_Component.m_points == null) return;

            MCreateLineMaterial();
            m_lineMaterial.SetPass(0);
            GL.Begin(GL.LINES);

            for (int i = 0; i < m_AKN_MGR_Component.m_points.Count; i++)
                for (int j = 1; j < m_AKN_MGR_Component.m_points[i].Count; j++)
                {
                    GL.Color(new Color(1 - ((float)j / (float)m_AKN_MGR_Component.m_points[i].Count), (float)j / (float)m_AKN_MGR_Component.m_points[i].Count, 1));

                    Vector3 from = new Vector3(m_AKN_MGR_Component.m_points[i][j - 1].x, m_AKN_MGR_Component.m_points[i][j - 1].y, _m_drawDepth);
                    Vector3 to = new Vector3(m_AKN_MGR_Component.m_points[i][j].x, m_AKN_MGR_Component.m_points[i][j].y, _m_drawDepth);

                    GL.Vertex(from); GL.Vertex(to);
                }
            GL.End();
        }

    }
}