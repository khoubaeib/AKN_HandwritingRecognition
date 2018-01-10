using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Runtime.Serialization;

/// \namespace AKeNe.AI.HiddenMarkovModel
///
/// \brief namespace that contains all HMM class's implementation.
///
/// \author Khoubaeib Klai | khoubaeib@gmail.com
/// \date   24/04/2014
/// 
/// \version 1.0
namespace AKeNe.AI.HiddenMarkovModel
{
    /// \class  AKN_State
    ///
    /// \brief  Describes a State in a Hidden Markov model or in a Markov model.
    ///
    /// \author Khoubaeib Klai | khoubaeib@gmail.com
    /// \date   24/04/2014
    /// 
    /// \version 1.0
    public class AKN_State 
    {
        public string m_name             { get; set; }
        public float  m_startProbability { get; set; }

        /// \fn     public AKN_State()
        ///
        /// \brief  Default contructor.
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        /// 
        /// \version 1.0
        public AKN_State()
        {

        }

        /// \fn     public AKN_State(float _startProbability, string _name) 
        ///
        /// \brief  Constructor that intiates start probability and name.
        ///
        /// \author Khoubaeib Klai | khoubaeib@gmail.com
        /// \date   24/04/2014
        /// 
        /// \version 1.0
        ///
        /// \param _startProbability praobability that
        /// \param _name             state's name
        public AKN_State(float _startProbability, string _name) 
        {
            this.m_startProbability = _startProbability;
            this.m_name = _name;
        }

    }
}