using System;
using System.Collections.Generic;

/// \namespace  AKeNe
///
/// \brief  AKeNe.

namespace AKeNe
{
    public class AKN_Status
    {
		
		public static string m_StaticLogBuffer;

        public string m_log;

        /// \enum   eStatusCode
        ///
        /// \brief  Values that represent eStatusCode success or failure. 

        public enum eStatusCode
        {
            kSuccess,
            kFailure
        };

        /// \property   public static AKN_Status m_Success
        ///
        /// \brief  Gets the success or failure status. 
        ///
        /// \return The m success. 

        public static AKN_Status m_Success
        {
            get
            {
                return new AKN_Status(eStatusCode.kSuccess);
            }
        }
        public static AKN_Status m_Failure
        {
            get
            {
                return new AKN_Status(eStatusCode.kFailure);
            }
        }

        
        private eStatusCode _m_Status;

        /// \property   public eStatusCode m_status
        ///
        /// \brief  Gets the status. 
        ///
        /// \return The m status. 

        public eStatusCode m_status
        {
            get
            {
                return _m_Status;
            }
        }

        /// \fn public AKN_Status()
        ///
        /// \brief  Default constructor. 
        ///
        /// \author Michele
        /// \date   10/06/2011

        public AKN_Status() :  this(true)
        {
           
        }

        /// \fn public AKN_Status(eStatusCode _errorCode)
        ///
        /// \brief  Constructor. 
        ///
        /// \author Michele
        /// \date   10/06/2011
        ///
        /// \param  _errorCode  The error code. 

        public AKN_Status(eStatusCode _errorCode)
        {
            _m_Status = _errorCode;
        }

        /// \fn public AKN_Status(bool _success)
        ///
        /// \brief  Constructor, logs true if the operation was a success, false if it failed.. 
        ///
        /// \author Michele
        /// \date   10/06/2011
        ///
        /// \param  _success    
        /// 
        public AKN_Status(bool _success)
        {
            m_log = "";
            if (_success)
            {
                _m_Status = eStatusCode.kSuccess;
            }
            else
            {
                _m_Status = eStatusCode.kFailure;
            }
        }

        /// \fn public AKN_Status(bool _success, string _buffer)
        ///
        /// \brief  Constructor,  logs true if the operation was a success, false if it failed.. 
        ///
        /// \author Michele
        /// \date   10/06/2011
        ///
        /// \param  _success   
        /// \param  _buffer     The buffer. 

        public AKN_Status(bool _success, string _buffer)
        {
            m_log = "";
            if (_success)
            {
                _m_Status = eStatusCode.kSuccess;
				
            }
            else
            {
                _m_Status = eStatusCode.kFailure;
            }
        }		

        /// \fn public static implicit operatorAKN_Status(bool _isSuccess)
        ///
        /// \brief  AKN status casting operator. 
        ///
        /// \author Michele
        /// \date   10/06/2011
        ///
        /// \param  _isSuccess  
        ///
        /// \return true if the is operation was a success, false if it failed.  

        public static implicit operator AKN_Status(bool _isSuccess)
        {
            return new AKN_Status(_isSuccess);
        }

        /// \fn public static implicit operatorbool(AKN_Status _error)
        ///
        /// \brief   casting operator. 
        ///
        /// \author Michele
        /// \date   14/06/2011
        ///
        /// \param  _error  The error. 
        ///
        /// \return error as true. 

        public static implicit operator bool(AKN_Status _error)
        {
            return (_error.m_status == eStatusCode.kSuccess);
        }
		        /// \fn public static AKN_Status MTest(bool _toTest)
        ///
        /// \brief  is there something to test? 
        ///
        /// \author Michele
        /// \date   10/06/2011
        ///
        /// \param  _toTest true to to test. 
        ///
        /// \return . 

    }
	
	
}
