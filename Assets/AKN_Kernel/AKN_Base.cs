/// \file   InrevToolKit\AKN_Base.cs
///
/// \brief  Implements the AKN base class. 
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Collections;

/// \namespace  AKeNe
///
/// \brief   AKeNe.
namespace AKeNe
{
    /// \class AKN_Base
    ///
    /// \brief  AKN base all AKN_object inherit from it. 
    ///
    /// \author Cedric Plessiet Inrev Paris Viii
    /// \date   24/12/2009
    public class AKN_Base : ICloneable
    {
        private readonly Guid _m_id;                 ///< the unique identifier of a node
        private static Dictionary<Guid, AKN_Base> _m_AKeNe_Manager = new Dictionary<Guid, AKN_Base>();


        private static Dictionary<string, Dictionary<Guid, AKN_Base>> _m_AKeNe_ManagerByLabel = new Dictionary<string, Dictionary<Guid, AKN_Base>>();

        /// \property   public static Dictionary<Guid, AKN_Base> m_AKeNe_Manager
        ///
        /// \brief  Gets the AKeNe_Manager.
        ///
        /// \author Cedric Plessiet Inrev Paris Viii
        /// \date   28/06/2013
        /// 
        /// \return The m AKeNe_Manager.

        // m_AKeNe_manager is used in the unitary test.
        public static Dictionary<Guid, AKN_Base> m_AKeNe_Manager
        {
            get
            {
                return _m_AKeNe_Manager;
            }
        }

        /// \property   public Guid m_id
        ///
        /// \brief  Gets the identifier. 
        ///
        /// \author Cedric Plessiet Inrev Paris Viii
        /// \date   24/12/2009
        /// 
        /// \return The m identifier. 
        
        // m_id is used in the unitary test.
        public Guid m_id
        {
            get
            {
                return _m_id;
            }
        }

        /// \fn public AKN_Base MFind(Guid _id)
        ///
        /// \brief  Return the m_id.
        ///
        /// \author Cedric Plessiet Inrev Paris Viii
        /// \date   24/12/2009
        ///
        /// \param  _id  The identifier.
        
        // Used to find the object with it's id
        public static AKN_Base MFind(Guid _id)
        {
            return _m_AKeNe_Manager[_id];
        }

        public string m_label;	///< the label. 

        /// \fn   public AKN_Base(Guid _id, string _label)
        ///
        /// \brief  Constructor.
        /// 
        /// \author Cedric Plessiet Inrev Paris Viii
        /// \date   24/12/2009
        ///
        /// \param _id  The identifier. 
        /// \param _label  The label. 
        
        // AKN_Base Creation with _id and _label.
		public AKN_Base(Guid _id, string _label)
        {
            _m_id = _id;
            m_label = _label;
            _m_AKeNe_Manager[_id] = this;

            if (_m_AKeNe_ManagerByLabel.ContainsKey(_label))
            {
                if (!_m_AKeNe_ManagerByLabel[_label].ContainsKey(_id))
                {
                    _m_AKeNe_ManagerByLabel[_label].Add(_id, this);
                }
            }
            else
            {
                Dictionary<Guid, AKN_Base> dictBuffer = new Dictionary<Guid, AKN_Base>();
                dictBuffer[_id] = this;
                _m_AKeNe_ManagerByLabel[_label] = dictBuffer;
            }
        }

        /// \fn public static List<AKN_Base> MFindByLabel(string _label)
        ///
        /// \brief  Return the m_labelList, it is a list containing the objects that have the same label.
        ///
        /// \author Noellie Velez noellievelez@gmail.com
        /// \date   01/07/2013
        ///
        /// \param  _label  The label.

        public static List<AKN_Base> MFindByLabel(string _label)
        {
            List<AKN_Base> _m_labelList = new List<AKN_Base>();
			
            if (_m_AKeNe_ManagerByLabel.ContainsKey(_label))
                foreach (KeyValuePair<Guid, AKN_Base> kvp2 in _m_AKeNe_ManagerByLabel[_label])
                {
                    _m_labelList.Add(kvp2.Value);
                }
           
            return _m_labelList;
        }
        
        /// \fn public AKN_Base(string _label)
        ///
        /// \brief  Constructor. 
        ///
        /// \author Cedric Plessiet Inrev Paris Viii
        /// \date   24/12/2009
        ///
        /// \param  _label  The label.

        // AKN_Base Creation with _label
        public AKN_Base(string _label):this( AKeNe.Tool.AKN_IDGenerator.MGenerateNewID(),_label)
        {
        }

        /// \fn public AKN_Base():this("")
        ///
        /// \brief  Constructor. 
        ///
        /// \author Cedric Plessiet Inrev Paris Viii
        /// \date   24/12/2009

        // AKN_Base Creation (the _label will be "")
        public AKN_Base():this("")
        {
        }

        public object Clone()
        {
            //First we create an instance of this specific type.

            AKN_Base newObject = Activator.CreateInstance(this.GetType()) as AKN_Base;

            //We get the array of fields for the new type instance.

            FieldInfo[] fields = newObject.GetType().GetFields();

            int i = 0;
            
            foreach (FieldInfo fi in this.GetType().GetFields())
            {
                //We query if the fiels support the ICloneable interface.
                Type ICloneType = fi.FieldType.
                            GetInterface("ICloneable", true);

                if (ICloneType != null)
                {
                    //Getting the ICloneable interface from the object.

                    ICloneable IClone = (ICloneable)fi.GetValue(this);

                    //We use the clone method to set the new value to the field.

                    fields[i].SetValue(newObject, IClone.Clone());
                }
                else
                {
                    // If the field doesn't support the ICloneable 

                    // interface then just set it.
                    if (fields[i].Name == "_m_id")
                    {
                        fields[i].SetValue(newObject,AKeNe.Tool.AKN_IDGenerator.MGenerateNewID());
                    }
                    else
                    {
                        fields[i].SetValue(newObject, fi.GetValue(this));
                    }
                }

                //Now we check if the object support the 

                //IEnumerable interface, so if it does

                //we need to enumerate all its items and check if 

                //they support the ICloneable interface.

                Type IEnumerableType = fi.FieldType.GetInterface
                                ("IEnumerable", true);
                if (IEnumerableType != null)
                {
                    //Get the IEnumerable interface from the field.

                    IEnumerable IEnum = (IEnumerable)fi.GetValue(this);

                    //This version support the IList and the 

                    //IDictionary interfaces to iterate on collections.

                    Type IListType = fields[i].FieldType.GetInterface
                                        ("IList", true);
                    Type IDicType = fields[i].FieldType.GetInterface
                                        ("IDictionary", true);

                    int j = 0;
                    if (IListType != null)
                    {
                        //Getting the IList interface.

                        IList list = (IList)fields[i].GetValue(newObject);

                        foreach (object obj in IEnum)
                        {
                            //Checking to see if the current item 

                            //support the ICloneable interface.

                            ICloneType = obj.GetType().
                                GetInterface("ICloneable", true);

                            if (ICloneType != null)
                            {
                                //If it does support the ICloneable interface, 

                                //we use it to set the clone of

                                //the object in the list.

                                ICloneable clone = (ICloneable)obj;

                                list[j] = clone.Clone();
                            }

                            //NOTE: If the item in the list is not 

                            //support the ICloneable interface then in the 

                            //cloned list this item will be the same 

                            //item as in the original list

                            //(as long as this type is a reference type).


                            j++;
                        }
                    }
                    else if (IDicType != null)
                    {
                        //Getting the dictionary interface.

                        IDictionary dic = (IDictionary)fields[i].
                                            GetValue(newObject);
                        j = 0;

                        foreach (DictionaryEntry de in IEnum)
                        {
                            //Checking to see if the item 

                            //support the ICloneable interface.

                            ICloneType = de.Value.GetType().
                                GetInterface("ICloneable", true);

                            if (ICloneType != null)
                            {
                                ICloneable clone = (ICloneable)de.Value;

                                dic[de.Key] = clone.Clone();
                            }
                            j++;
                        }
                    }
                }
                i++;
            }
            return newObject;
        }

    }
}
