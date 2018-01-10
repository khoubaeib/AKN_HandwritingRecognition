using System;
using System.Collections.Generic;

namespace AKeNe
{
    namespace Tool
    {
        /// \class  AKN_IDGenerator
        ///
        /// \brief  AKN identifier generator. 
        ///
        /// \author Cedric Plessiet
        /// \date   12/04/2010

        public class AKN_IDGenerator
        {
            /// \fn static public Guid MGenerateNewID()
            ///
            /// \brief  Generates a new identifier. 
            ///
            /// \author Cedric Plessiet
            /// \date   12/04/2010
            ///
            /// \return The new identifier. 
            static public  Guid MGenerateNewID()
            {
                return Guid.NewGuid();
            }

            /// \fn static public uint MGenerateIDFrom2Integer(uint _firstId, uint _secondId, uint _step)
            ///
            /// \brief  Generates an identifier from 2 integer. 
            ///
            /// \author Cedric Plessiet
            /// \date   12/04/2010
            ///
            /// \param  _firstId    Identifier of the first. 
            /// \param  _secondId   Identifier of the second. 
            /// \param  _step       Amount to increment by. 
            ///
            /// \return The identifier from 2 integer. 
            static public int MGenerateIDFrom2Integer(int _firstId, int _secondId, int _step)
            {
                return _firstId + (_step) * _secondId; ; 
            }

            /// \fn static public uint MGenerateIDFrom3DIntegerList(uint _x, uint _y, uint _z, uint _height,
            ///     uint _width)
            ///
            /// \brief  Generates an identifier from 3 d integer list. 
            ///
            /// \author Cedric Plessiet
            /// \date   12/04/2010
            ///
            /// \param  _x      The x coordinate. 
            /// \param  _y      The y coordinate. 
            /// \param  _z      The z coordinate. 
            /// \param  _height The height. 
            /// \param  _width  The width. 
            ///
            /// \return The identifier from 3 d integer list. 
            static public int MGenerateIDFrom3DIntegerList(int _x, int _y, int _z, int _height, int _width)
            {
                return _x + (_height) * _y + (_width * _height) * _z;
            }


        }
    }
}
