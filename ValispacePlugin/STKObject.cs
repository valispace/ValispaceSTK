using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AGI.STKObjects;


namespace ValispacePlugin
{
    #region STK Object Class Constructors
    // Class for retreived STK Object Properties
    public class STKObject
    {
        public string m_ObjName;
        public string m_ObjType;
        public string m_propagator;
        public IAgStkObject m_ObjectHandle;
        public Satellite_OrbitData m_OrbitData;
        public Satellite_MassData m_MassData;
        public double m_Value;
        public string m_Unit;
        public int isParentObj; // 0 - basechild (value element)  1-parent comp 2-subcomp 3-subsubcomp 4- subsubsubcomp

        // Default Constructor
        public STKObject()
        {
            m_ObjName = null;
            m_ObjType = null;
            m_ObjectHandle = null;
            m_OrbitData = null;
            m_Unit = null;
            isParentObj = 99999;
        }
        // Attribute
        public STKObject(string name0, double value, string unit, params int[] ParentLevel)
        {
            m_ObjName = name0;
            m_ObjType = null;
            m_Value = value;
            m_ObjectHandle = null;
            m_OrbitData = null;
            m_Unit = unit;
            if (ParentLevel.Length != 0)
            {
                isParentObj = ParentLevel[0];
            }
            else
            {
                isParentObj = 0;
            }
        }

        //Object 
        public STKObject(string name0, string type0, IAgStkObject object0, params int[] ParentLevel)
        {
            m_ObjName = name0;
            m_ObjType = type0;
            m_ObjectHandle = object0;
            m_OrbitData = null;
            m_Unit = " ";
            if (ParentLevel.Length != 0)
            {
                isParentObj = ParentLevel[0];
            }
            else
            {
                isParentObj = 1;
            }
        }

        // subcomponents without object Passing
        public STKObject(string name0, string type0, params int[] ParentLevel)
        {
            m_ObjName = name0;
            m_ObjType = type0;
            m_ObjectHandle = null;
            m_OrbitData = null;
            m_Unit = " ";
            if (ParentLevel.Length != 0)
            {
                isParentObj = ParentLevel[0];
            }
            else
            {
                isParentObj = 2;
            }
        }

        //public STKObject(string name0, double value, params int[] ParentLevel)
        //{
        //    m_ObjName = name0;
        //    m_ObjType = null;
        //    m_Value = value;
        //    m_ObjectHandle = null;
        //    m_OrbitData = null;
        //    m_Unit = " ";
        //    if (ParentLevel.Length != 0)
        //    {
        //        isParentObj = ParentLevel[0];
        //    }
        //    else
        //    {
        //        isParentObj = 2;
        //    }
        //}

        //Satellite Object
        public STKObject(string name0, string type0, string propagatorType, IAgStkObject object0)
        {
            m_ObjName = name0;
            m_ObjType = type0;
            m_ObjectHandle = object0;
            m_propagator = propagatorType;
            m_OrbitData = new Satellite_OrbitData(object0);
            m_MassData = new Satellite_MassData(object0);
            m_Unit = " ";
            isParentObj = 1;
        }

    }
    #endregion
}
