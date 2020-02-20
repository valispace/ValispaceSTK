using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AGI.STKObjects;
using AGI.STKUtil;
using AGI.STKVgt;


namespace ValispacePlugin
{
    #region Orbit Data Class with Init State
    public class Satellite_OrbitData
    {
        //CommonState Method for TwoBody(J0), J2, J4
        public int m_varCount = 7;
        public string m_ElemsType;
        public object m_ultimateObject;
        public AgCrdnEventIntervalSmartInterval m_ObjectTimes;

        public double SemiMajorAxis;
        public double Eccentricity;
        public double Inclination;
        public double ArgOfPerigee;
        public double RAAN;
        public double TrueAnomaly;
        public double Step_Size;

        public Satellite_OrbitData()
        {

        }

        public Satellite_OrbitData(IAgStkObject Object0)
        {
            m_ElemsType = "Classical";

            if (Object0.ClassName == "Satellite")
            {
                IAgSatellite sat = Object0 as IAgSatellite;
                if (sat.PropagatorType == AgEVePropagatorType.ePropagatorJ2Perturbation)
                {
                    IAgVePropagatorJ2Perturbation prop = sat.Propagator as IAgVePropagatorJ2Perturbation;
                    m_ObjectTimes = prop.EphemerisInterval as AgCrdnEventIntervalSmartInterval;
                    Step_Size = prop.Step;
                    IAgOrbitStateClassical keplerState = prop.InitialState.Representation.ConvertTo(AgEOrbitStateType.eOrbitStateClassical) as IAgOrbitStateClassical;
                    m_ultimateObject = Object0;
                    get_InitStateJx(keplerState);
                }

                else if (sat.PropagatorType == AgEVePropagatorType.ePropagatorJ4Perturbation)
                {
                    IAgVePropagatorJ4Perturbation prop = sat.Propagator as IAgVePropagatorJ4Perturbation;
                    m_ObjectTimes = prop.EphemerisInterval as AgCrdnEventIntervalSmartInterval;
                    Step_Size = prop.Step;
                    IAgOrbitStateClassical keplerState = prop.InitialState.Representation.ConvertTo(AgEOrbitStateType.eOrbitStateClassical) as IAgOrbitStateClassical;
                    m_ultimateObject = keplerState;
                    get_InitStateJx(keplerState);
                }
                else if (sat.PropagatorType == AgEVePropagatorType.ePropagatorTwoBody)
                {
                    IAgVePropagatorTwoBody prop = sat.Propagator as IAgVePropagatorTwoBody;
                    m_ObjectTimes = prop.EphemerisInterval as AgCrdnEventIntervalSmartInterval;
                    Step_Size = prop.Step;
                    IAgOrbitStateClassical keplerState = prop.InitialState.Representation.ConvertTo(AgEOrbitStateType.eOrbitStateClassical) as IAgOrbitStateClassical;
                    m_ultimateObject = keplerState;
                    get_InitStateJx(keplerState);
                }
            }
        }
        public void get_InitStateJx(IAgOrbitStateClassical keplerState)
        {
            keplerState.SizeShapeType = AgEClassicalSizeShape.eSizeShapeSemimajorAxis;
            IAgClassicalSizeShapeSemimajorAxis Orbitsize = keplerState.SizeShape as IAgClassicalSizeShapeSemimajorAxis;

            keplerState.LocationType = AgEClassicalLocation.eLocationTrueAnomaly;
            IAgClassicalLocationTrueAnomaly OrbitLocation = keplerState.Location as IAgClassicalLocationTrueAnomaly;

            IAgClassicalOrientation OrbitOrientation = keplerState.Orientation as IAgClassicalOrientation;

            OrbitOrientation.AscNodeType = AgEOrientationAscNode.eAscNodeRAAN;
            IAgOrientationAscNodeRAAN OrbitAsc = keplerState.Orientation.AscNode as IAgOrientationAscNodeRAAN;


            SemiMajorAxis = Orbitsize.SemiMajorAxis;
            Eccentricity = Orbitsize.Eccentricity;
            Inclination = OrbitOrientation.Inclination;
            ArgOfPerigee = OrbitOrientation.ArgOfPerigee;
            RAAN = OrbitAsc.Value;
            TrueAnomaly = OrbitLocation.Value;

            //Console.WriteLine(Orbitsize.SemiMajorAxis);
            //IAgQuantity a = OrbitOrientation as IAgQuantity;
            //Console.WriteLine(OrbitOrientation.Inclination);
        }

        public void set_InitStateJx(object InitStateObj, string name, double value)
        {
            if (name == "SemiMajorAxis") { SemiMajorAxis = value; }
            else if (name == "Eccentricity") { Eccentricity = value; }
            else if (name == "Inclination") {Inclination = value; }
            else if (name == "ArgOfPerigee") { ArgOfPerigee = value;}
            else if (name == "RAAN") { RAAN = value;}
            else if (name == "TrueAnomaly") { TrueAnomaly = value;}

            IAgSatellite sat = InitStateObj as IAgSatellite;
            if (sat.PropagatorType == AgEVePropagatorType.ePropagatorJ2Perturbation)
            {
                IAgVePropagatorJ2Perturbation prop = sat.Propagator as IAgVePropagatorJ2Perturbation;
                prop.InitialState.Representation.AssignClassical(AgECoordinateSystem.eCoordinateSystemICRF, SemiMajorAxis, Eccentricity, Inclination, ArgOfPerigee, RAAN, TrueAnomaly);
                prop.Propagate();
            }
            if (sat.PropagatorType == AgEVePropagatorType.ePropagatorJ4Perturbation)
            {
                IAgVePropagatorJ4Perturbation prop = sat.Propagator as IAgVePropagatorJ4Perturbation;
                prop.InitialState.Representation.AssignClassical(AgECoordinateSystem.eCoordinateSystemICRF, SemiMajorAxis, Eccentricity, Inclination, ArgOfPerigee, RAAN, TrueAnomaly);
                prop.Propagate();
            }
            if (sat.PropagatorType == AgEVePropagatorType.ePropagatorTwoBody)
            {
                IAgVePropagatorTwoBody prop = sat.Propagator as IAgVePropagatorTwoBody;
                prop.InitialState.Representation.AssignClassical(AgECoordinateSystem.eCoordinateSystemICRF, SemiMajorAxis, Eccentricity, Inclination, ArgOfPerigee, RAAN, TrueAnomaly);
                prop.Propagate();
            }
        }

    }
    #endregion

    #region Mass Data Class with Mass Properties
    public class Satellite_MassData
    {
        public double Mass;
        public double Ixx;
        public double Ixy;
        public double Ixz;
        public double Iyy;
        public double Iyz;
        public double Izz;
        public Unit m_unitM; //kg
        public Unit m_unitI; //kg m^2

        public object m_ultimateObject;

        public Satellite_MassData()
        {

        }

        public Satellite_MassData(IAgStkObject object0) // Devise get,set methods
        {
            IAgSatellite satellite0 = object0 as IAgSatellite;
            IAgVeMassProperties MassProp = satellite0.MassProperties as IAgVeMassProperties;
            m_ultimateObject = MassProp;
            get_MassProps(MassProp);
        }
        private void get_MassProps(IAgVeMassProperties MassPropObj)
        {
            Mass = MassPropObj.Mass;
            IAgVeInertia tensor_I = MassPropObj.Inertia as IAgVeInertia;
            Ixx = tensor_I.Ixx;
            Ixy = tensor_I.Ixy;
            Ixz = tensor_I.Ixz;
            Iyy = tensor_I.Iyy;
            Iyz = tensor_I.Iyz;
            Izz = tensor_I.Izz;
        }
        public void set_MassProp(object PropsObj, string name, double value)
        {
            IAgVeMassProperties MassPropObj = PropsObj as IAgVeMassProperties;
            if (name == "Mass") { MassPropObj.Mass = value; }
            else if (name == "Ixx") { (MassPropObj.Inertia as IAgVeInertia).Ixx = value; }
            else if (name == "Ixy") { (MassPropObj.Inertia as IAgVeInertia).Ixy = value; }
            else if (name == "Ixz") { (MassPropObj.Inertia as IAgVeInertia).Ixz = value; }
            else if (name == "Iyy") { (MassPropObj.Inertia as IAgVeInertia).Iyy = value; }
            else if (name == "Iyz") { (MassPropObj.Inertia as IAgVeInertia).Iyz = value; }
            else if (name == "Izz") { (MassPropObj.Inertia as IAgVeInertia).Izz = value; }
        }
        #endregion
    }


    public class Unit
    {
        public string u_Distance;
        public string u_Angle;
        public string u_Velocity;
        public string u_Time;
        public string u_Null;
        public string u_Inertia;
        public string u_Mass;
        public string u_AngleRate;
        public string u_Pressure;
        public string u_Area;
        public string u_Volume;
        public string u_Temperature;

        public Unit()
        {
            u_Null = "";

            u_Time = "s";
            u_Distance = "km";
            u_Velocity = "km/s";
            u_Angle = "deg";
            u_AngleRate = "rad/s";

            u_Mass = "kg";
            u_Inertia = "kg m^2";

            u_Area = "m^2";
            u_Volume = "m^3";
            u_Pressure = "Pa";
            u_Temperature = "K";

        }
    }
}
