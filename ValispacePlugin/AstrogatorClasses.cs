using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AGI.STKObjects;
using AGI.STKUtil;
using AGI.Ui.Application;
using AGI.STKObjects.Astrogator;


namespace ValispacePlugin
{
    #region Astrogator Data Class
    public class Astg_SegData
    {
        
        //Initial State
        public Dictionary<string, double> OrbElems = new Dictionary<string, double>();
        public Dictionary<string, double> SCInitParams = new Dictionary<string, double>();
        public Dictionary<string, double> FuelTankParams = new Dictionary<string, double>();
        //Maneuvers
        public Dictionary<string, double> DeltaVParams = new Dictionary<string, double>();

        //Attribute Lists for objects & attributes
        public List<string> l_Names = new List<string>();
        public List<object> l_SegObj = new List<object>();
        public List<double> l_Values = new List<double>();
        public List<int> l_depth = new List<int>();
        public List<string> l_unit = new List<string>();
        public List<bool> l_isQuantity = new List<bool>();
        public List<string> l_types = new List<string>();
        public List<implem_Classes> l_implemClass = new List<implem_Classes>();
        public List<List<string>> l_localParents = new List<List<string>>();

        //Unit Object for all Units used
        private Unit units = new Unit();

        public Astg_SegData() { }

        //Constructor with MCS Segment argument: depth - segment depth in main nest
        public Astg_SegData(IAgVAMCSSegment thisSegment, int depth)
        {
            
            l_SegObj.Add(thisSegment);
            l_Names.Add((thisSegment as IAgComponentInfo).Name);
            l_Values.Add(double.PositiveInfinity);
            l_types.Add(thisSegment.Type.ToString());
            l_depth.Add(depth);
            l_unit.Add("");
            l_isQuantity.Add(false);
            l_implemClass.Add(implem_Classes.NULL);

            List<string> localParent = new List<string>() {};
            l_localParents.Add(localParent);
            localParent.Add((thisSegment as IAgComponentInfo).Name);

            if (thisSegment.Type == AgEVASegmentType.eVASegmentTypeInitialState)
            {
                get_InitialState(thisSegment as IAgVAMCSInitialState, depth,localParent);
                get_SpacecraftParams(thisSegment as IAgVAMCSInitialState, depth,localParent);
                get_FuelTankParams(thisSegment as IAgVAMCSInitialState, depth,localParent);
            }
            else if (thisSegment.Type == AgEVASegmentType.eVASegmentTypeManeuver)
            {
                get_ManeuverParams(thisSegment, depth, localParent);

            }
        }

        #region Initial State Object Methods
        public void get_InitialState(IAgVAMCSInitialState InitStateObj, int depth, List<string> localParent)
        {
            depth++;
            #region List Header
            l_SegObj.Add(null);
            l_Names.Add("Orbit State/Elements");
            l_unit.Add("");
            l_Values.Add(double.PositiveInfinity);
            l_types.Add(" ");
            l_depth.Add(depth);
            l_isQuantity.Add(false);
            l_implemClass.Add(implem_Classes.NULL);
            l_localParents.Add(localParent);
            #endregion

            AgEVAElementType m_CoordType = InitStateObj.ElementType;
            if (m_CoordType == AgEVAElementType.eVAElementTypeKeplerian)
            {
                IAgVAElementKeplerian keplerState = InitStateObj.Element as IAgVAElementKeplerian;
                OrbElems.Add("Apoapsis Altitude", keplerState.ApoapsisAltitudeSize); l_unit.Add(units.u_Distance);
                OrbElems.Add("Apoapsis Radius", keplerState.ApoapsisRadiusSize); l_unit.Add(units.u_Distance);
                OrbElems.Add("Eccentricity", keplerState.Eccentricity); l_unit.Add(units.u_Null);
                OrbElems.Add("Inclination", keplerState.Inclination); l_unit.Add(units.u_Angle);
                OrbElems.Add("RAAN", keplerState.RAAN); l_unit.Add(units.u_Angle);
                OrbElems.Add("True Anomaly", keplerState.TrueAnomaly); l_unit.Add(units.u_Angle);
                OrbElems.Add("Semi-Major Axis", keplerState.SemiMajorAxis); l_unit.Add(units.u_Distance);
                OrbElems.Add("Period", keplerState.Period); l_unit.Add(units.u_Time);
                OrbElems.Add("Mean Motion", keplerState.MeanMotion); l_unit.Add(units.u_AngleRate);
                OrbElems.Add("Arg Of Latitude", keplerState.ArgOfLatitude); l_unit.Add(units.u_Angle);
                OrbElems.Add("Arg Of Periapsis", keplerState.ArgOfPeriapsis); l_unit.Add(units.u_Angle);
                OrbElems.Add("Mean Anomaly", keplerState.MeanMotion); l_unit.Add(units.u_Angle);
                OrbElems.Add("LAN", keplerState.LAN); l_unit.Add(units.u_Angle);
                OrbElems.Add("Periapsis Altitude", keplerState.PeriapsisAltitudeSize); l_unit.Add(units.u_Distance);
                OrbElems.Add("Periapsis Radius", keplerState.PeriapsisRadiusSize); l_unit.Add(units.u_Distance);

                update_Lists(OrbElems, depth,InitStateObj, implem_Classes.Astg_InitState,localParent);
            }
            else if (m_CoordType == AgEVAElementType.eVAElementTypeCartesian)
            {
                IAgVAElementCartesian cartesianState = InitStateObj.Element as IAgVAElementCartesian;
                OrbElems.Add("X", cartesianState.X); l_unit.Add(units.u_Distance);
                OrbElems.Add("Y", cartesianState.Y); l_unit.Add(units.u_Distance);
                OrbElems.Add("Z", cartesianState.Z); l_unit.Add(units.u_Distance);
                OrbElems.Add("Vx", cartesianState.Vx); l_unit.Add(units.u_Velocity);
                OrbElems.Add("Vy", cartesianState.Vy); l_unit.Add(units.u_Velocity);
                OrbElems.Add("Vz", cartesianState.Vz); l_unit.Add(units.u_Velocity);
                update_Lists(OrbElems, depth, InitStateObj, implem_Classes.Astg_InitState, localParent);
            }
        }

        public void set_InitialState(object InitStateObj, string name, double value)
        {

            IAgVAMCSInitialState state = InitStateObj as IAgVAMCSInitialState;

            AgEVAElementType CoordType = state.ElementType;
            if (CoordType == AgEVAElementType.eVAElementTypeKeplerian)
            {
                IAgVAElementKeplerian keplerState = state.Element as IAgVAElementKeplerian;
                if (name == "Apoapsis Altitude"){ keplerState.ApoapsisAltitudeSize = value; }
                else if (name == "Apoapsis Radius")  { keplerState.ApoapsisRadiusSize = value; }
                else if (name == "Eccentricity")     { keplerState.Eccentricity = value; }
                else if (name == "Inclination")      { keplerState.Inclination = value; }
                else if (name == "RAAN")             { keplerState.RAAN = value; }
                else if (name == "True Anomaly")     { keplerState.TrueAnomaly = value; }
                else if (name == "Semi-Major Axis")  { keplerState.SemiMajorAxis = value; }
                else if (name == "Period")           { keplerState.Period = value; }
                else if (name == "Mean Motion")      { keplerState.MeanMotion = value; }
                else if (name == "Arg Of Latitude")  { keplerState.ArgOfLatitude = value; }
                else if (name == "Arg Of Periapsis") { keplerState.ArgOfPeriapsis = value; }
                else if (name == "Mean Anomaly")     { keplerState.MeanMotion = value; }
                else if (name == "LAN")              { keplerState.LAN = value; }
                else if (name == "Periapsis Altitude") { keplerState.PeriapsisAltitudeSize = value; }
                else if (name =="Periapsis Radius")  { keplerState.PeriapsisRadiusSize = value; }
            }
            else if (CoordType == AgEVAElementType.eVAElementTypeCartesian)
            {
                IAgVAElementCartesian cartesianState = state.Element as IAgVAElementCartesian;
                if (name == "X") { cartesianState.X = value; }
                else if (name =="Y"){ cartesianState.Y = value; }
                else if (name =="Z"){ cartesianState.Z = value; }
                else if (name =="Vx"){ cartesianState.Vx = value; }
                else if (name =="Vy"){ cartesianState.Vy = value; }
                else if (name == "Vz"){cartesianState.Vz = value; }
            }
        }

        public void get_SpacecraftParams(IAgVAMCSInitialState InitStateObj, int depth, List<string> localParent)
        {
            depth++;
            #region List Header
            l_SegObj.Add(null);
            l_Names.Add("Spacecraft Initial Parameters");
            l_unit.Add("");
            l_Values.Add(double.PositiveInfinity);
            l_types.Add(" ");
            l_depth.Add(depth);
            l_isQuantity.Add(false);
            l_implemClass.Add(implem_Classes.NULL);
            l_localParents.Add(localParent);
            #endregion

            
            IAgVASpacecraftParameters InitParams = InitStateObj.SpacecraftParameters as IAgVASpacecraftParameters;
            SCInitParams.Add("Dry Mass", InitParams.DryMass);                           l_unit.Add(units.u_Mass);
            SCInitParams.Add("Drag Area", InitParams.DragArea);                         l_unit.Add(units.u_Area);
            SCInitParams.Add("Cd (Drag)", InitParams.Cd);                               l_unit.Add(units.u_Null);
            SCInitParams.Add("Area (CB Radiation)", InitParams.RadiationPressureArea);  l_unit.Add(units.u_Area);
            SCInitParams.Add("Ck (Reflectivity CB Radiation)", InitParams.Ck);          l_unit.Add(units.u_Null);
            SCInitParams.Add("Area (SRP)", InitParams.SolarRadiationPressureArea);      l_unit.Add(units.u_Area);
            SCInitParams.Add("Cr (Reflectivity SRP)", InitParams.Cr);                   l_unit.Add(units.u_Null);
            SCInitParams.Add("K1 (Non-spherical SRP)", InitParams.K1);                  l_unit.Add(units.u_Null);
            SCInitParams.Add("K2 (Non-spherical SRP)", InitParams.K2);                  l_unit.Add(units.u_Null);

            update_Lists(SCInitParams, depth,InitStateObj, implem_Classes.Astg_SC_InitParams, localParent);
        }

        public void set_SpacecraftParams(object InitStateObj, string name, double value)
        {
            IAgVASpacecraftParameters InitParams = (InitStateObj as IAgVAMCSInitialState).SpacecraftParameters as IAgVASpacecraftParameters;
            if (name == "Dry Mass") { InitParams.DryMass = value; }
            if (name =="Drag Area") { InitParams.DragArea = value; }
            if (name =="Cd (Drag)") { InitParams.Cd= value; }
            if (name =="Area (CB Radiation)") {InitParams.RadiationPressureArea = value; }
            if (name =="Ck (Reflectivity CB Radiation)") {InitParams.Ck = value; }
            if (name =="Area (SRP)") {InitParams.SolarRadiationPressureArea = value; }
            if (name =="Cr (Reflectivity SRP)") { InitParams.Cr = value; }
            if (name =="K1 (Non-spherical SRP)") { InitParams.K1 = value; }
            if (name == "K2 (Non-spherical SRP)") { InitParams.K2 = value; }

        }

        public void get_FuelTankParams(IAgVAMCSInitialState InitStateObj, int depth, List<string> localParent)
        {
            depth++;
            #region List Header
            l_SegObj.Add(null);
            l_Names.Add("Fuel Tank Parameters");
            l_unit.Add(" ");
            l_Values.Add(double.PositiveInfinity);
            l_types.Add(" ");
            l_depth.Add(depth);
            l_isQuantity.Add(false);
            l_implemClass.Add(implem_Classes.NULL);
            l_localParents.Add(localParent);
            #endregion
            IAgVAFuelTank FuelTank = InitStateObj.FuelTank as IAgVAFuelTank;
            FuelTankParams.Add("Tank Pressure", FuelTank.TankPressure);                 l_unit.Add(units.u_Pressure);
            FuelTankParams.Add("Tank Volume", FuelTank.TankVolume);                     l_unit.Add(units.u_Volume);
            FuelTankParams.Add("Tank Temperature", FuelTank.TankTemperature);           l_unit.Add(units.u_Temperature);
            FuelTankParams.Add("Fuel Density", FuelTank.FuelDensity);                   l_unit.Add(units.u_Area);
            FuelTankParams.Add("Fuel Mass", FuelTank.FuelMass);                         l_unit.Add(units.u_Mass);
            FuelTankParams.Add("Max Fuel Mass", FuelTank.MaximumFuelMass);              l_unit.Add(units.u_Area);

            update_Lists(FuelTankParams, depth,InitStateObj,implem_Classes.Astg_FuelTankParams,localParent);
        }

        public void set_FuelTankParams(object InitStateObj, string name, double value)
        {
            
            IAgVAFuelTank FuelTank = (InitStateObj as IAgVAMCSInitialState).FuelTank as IAgVAFuelTank;
            if (name =="Tank Pressure"){ FuelTank.TankPressure = value; }
            if (name =="Tank Volume"){ FuelTank.TankVolume = value; }
            if (name =="Tank Temperature"){ FuelTank.TankTemperature = value; }
            if (name =="Fuel Density"){ FuelTank.FuelDensity = value; }
            if (name =="Fuel Mass"){ FuelTank.FuelMass = value; }
            if (name == "Max Fuel Mass"){ FuelTank.MaximumFuelMass = value; }

        }
        #endregion

        #region Maneuver Object Methods

        //Impulsive
        public void get_ManeuverParams(IAgVAMCSSegment MnvrSegment, int depth, List<string> localParent)
        {
            depth++;
            IAgVAMCSManeuver thisMnvr = MnvrSegment as IAgVAMCSManeuver;
            #region List Header
            l_SegObj.Add(null);
            l_unit.Add("");
            l_depth.Add(depth);
            l_isQuantity.Add(false);
            l_implemClass.Add(implem_Classes.NULL);
            #endregion
            if (thisMnvr.ManeuverType == AgEVAManeuverType.eVAManeuverTypeImpulsive)
            {
                IAgVAManeuverImpulsive ImpMnvr = thisMnvr.Maneuver as IAgVAManeuverImpulsive;

                #region Type: Thrust Vector
                if (ImpMnvr.AttitudeControlType == AgEVAAttitudeControl.eVAAttitudeControlThrustVector)
                {
                    IAgVAAttitudeControlImpulsiveThrustVector thisImpMnvr = ImpMnvr.AttitudeControl as IAgVAAttitudeControlImpulsiveThrustVector;
                    l_Names.Add("Impulsive: Thrust vector"); //Header name
                    l_Values.Add(double.PositiveInfinity);
                    l_types.Add(thisImpMnvr.ThrustAxesName);
                    l_localParents.Add(localParent);
                    IAgPosition DV_vectorObj = (thisImpMnvr.DeltaVVector as IAgPosition);

                    Cartesian_Object DV_Vector = new Cartesian_Object();
                    DV_vectorObj.QueryCartesian(out DV_Vector.X, out DV_Vector.Y, out DV_Vector.Z);

                    DeltaVParams.Add("Del-V (X: Velocity)", DV_Vector.X);       l_unit.Add(units.u_Velocity);
                    DeltaVParams.Add("Del-V (Y: Normal)", DV_Vector.Y);         l_unit.Add(units.u_Velocity);
                    DeltaVParams.Add("Del-V (Z: Co-Normal)", DV_Vector.Z);      l_unit.Add(units.u_Velocity);

                    update_Lists(DeltaVParams, depth,thisMnvr, implem_Classes.Astg_Mnvr_DV,localParent);

                }
                #endregion

                #region Type: Along Velocity Vector
                else if (ImpMnvr.AttitudeControlType == AgEVAAttitudeControl.eVAAttitudeControlVelocityVector)
                {
                    IAgVAAttitudeControlImpulsiveVelocityVector thisImpMnvr = ImpMnvr.AttitudeControl as IAgVAAttitudeControlImpulsiveVelocityVector;
                    l_Names.Add("Impulsive: Along Velocity Vector"); //Header name
                    l_Values.Add(double.PositiveInfinity);
                    l_types.Add(" ");
                    l_localParents.Add(localParent);
                    DeltaVParams.Add("Del-V Magnitude", thisImpMnvr.DeltaVMagnitude); l_unit.Add(units.u_Velocity);

                    update_Lists(DeltaVParams, depth, thisMnvr,implem_Classes.Astg_Mnvr_DV,localParent);
                }
                #endregion

                #region Type: Anti-Velocity Vector
                else if (ImpMnvr.AttitudeControlType == AgEVAAttitudeControl.eVAAttitudeControlAntiVelocityVector)
                {
                    IAgVAAttitudeControlImpulsiveVelocityVector thisImpMnvr = ImpMnvr.AttitudeControl as IAgVAAttitudeControlImpulsiveVelocityVector;
                    l_Names.Add("Impulsive: Anti-Velocity Vector"); //Header name
                    l_Values.Add(double.PositiveInfinity);
                    l_types.Add(" ");
                    l_localParents.Add(localParent);
                    DeltaVParams.Add("Del-V Magnitude", thisImpMnvr.DeltaVMagnitude); l_unit.Add(units.u_Velocity);

                    update_Lists(DeltaVParams, depth,thisMnvr, implem_Classes.Astg_Mnvr_DV,localParent);
                }
                #endregion

                #region Type: Attitude
                else if (ImpMnvr.AttitudeControlType == AgEVAAttitudeControl.eVAAttitudeControlAttitude)
                {
                    IAgVAAttitudeControlImpulsiveAttitude thisImpMnvr = ImpMnvr.AttitudeControl as IAgVAAttitudeControlImpulsiveAttitude;
                    l_Names.Add("Impulsive: Attitude"); //Header name
                    l_Values.Add(double.PositiveInfinity);
                    l_types.Add(thisImpMnvr.RefAxesName);
                    l_localParents.Add(localParent);
                    DeltaVParams.Add("Del-V Magnitude", thisImpMnvr.DeltaVMagnitude); l_unit.Add(units.u_Velocity);

                    EulerAng_Object MnvrEulAngs = new EulerAng_Object();
                    IAgOrientationEulerAngles angles = (IAgOrientationEulerAngles)thisImpMnvr.Orientation.ConvertTo(AgEOrientationType.eEulerAngles);
                    MnvrEulAngs.Sequence = angles.Sequence;
                    MnvrEulAngs.Phi_1 = double.Parse(angles.A.ToString(), System.Globalization.CultureInfo.InvariantCulture);
                    MnvrEulAngs.Theta_2 = double.Parse(angles.A.ToString(), System.Globalization.CultureInfo.InvariantCulture);
                    MnvrEulAngs.Psi_3 = double.Parse(angles.A.ToString(), System.Globalization.CultureInfo.InvariantCulture);

                    //DeltaVParams.Add("Euler Angle Sequence", double.Parse(MnvrEulAngs.Sequence.ToString(), System.Globalization.CultureInfo.InvariantCulture)); l_unit.Add(units.u_Null);
                    DeltaVParams.Add("Euler Angle 1 (Phi)", MnvrEulAngs.Phi_1); l_unit.Add(units.u_Angle);
                    DeltaVParams.Add("Euler Angle 2 (Theta)", MnvrEulAngs.Theta_2); l_unit.Add(units.u_Angle);
                    DeltaVParams.Add("Euler Angle 3 (Psi)", MnvrEulAngs.Psi_3); l_unit.Add(units.u_Angle);

                    update_Lists(DeltaVParams, depth, thisMnvr, implem_Classes.Astg_Mnvr_DV,localParent);
                };
                #endregion

            }
        }

        public void set_ManeuverParams(object MnvrObj, string name, double value)
        {
            IAgVAMCSManeuver thisMnvr = MnvrObj as IAgVAMCSManeuver;
            if (thisMnvr.ManeuverType == AgEVAManeuverType.eVAManeuverTypeImpulsive)
            {
                IAgVAManeuverImpulsive ImpMnvr = thisMnvr.Maneuver as IAgVAManeuverImpulsive;

                #region Type: Thrust Vector
                if (ImpMnvr.AttitudeControlType == AgEVAAttitudeControl.eVAAttitudeControlThrustVector)
                {
                    IAgVAAttitudeControlImpulsiveThrustVector thisImpMnvr = ImpMnvr.AttitudeControl as IAgVAAttitudeControlImpulsiveThrustVector;
                    IAgPosition DV_vectorObj = (thisImpMnvr.DeltaVVector as IAgPosition);

                    Cartesian_Object DV_Vector = new Cartesian_Object();
                    DV_vectorObj.QueryCartesian(out DV_Vector.X, out DV_Vector.Y, out DV_Vector.Z);

                    if (name == "Del-V (X: Velocity)") { DV_Vector.X = value;}
                    else if (name =="Del-V (Y: Normal)")    { DV_Vector.Y= value; }
                    else if (name =="Del-V (Z: Co-Normal)") { DV_Vector.Z = value;}
                }
                #endregion

                #region Type: Along Velocity Vector
                else if (ImpMnvr.AttitudeControlType == AgEVAAttitudeControl.eVAAttitudeControlVelocityVector)
                {
                    IAgVAAttitudeControlImpulsiveVelocityVector thisImpMnvr = ImpMnvr.AttitudeControl as IAgVAAttitudeControlImpulsiveVelocityVector;
                    if (name == "Del-V Magnitude") { thisImpMnvr.DeltaVMagnitude = value; }
                }
                #endregion

                #region Type: Anti-Velocity Vector
                else if (ImpMnvr.AttitudeControlType == AgEVAAttitudeControl.eVAAttitudeControlAntiVelocityVector)
                {
                    IAgVAAttitudeControlImpulsiveVelocityVector thisImpMnvr = ImpMnvr.AttitudeControl as IAgVAAttitudeControlImpulsiveVelocityVector;
                    if (name == "Del-V Magnitude") { thisImpMnvr.DeltaVMagnitude = value; }
                }
                #endregion

                #region Type: Attitude
                else if (ImpMnvr.AttitudeControlType == AgEVAAttitudeControl.eVAAttitudeControlAttitude)
                {
                    IAgVAAttitudeControlImpulsiveAttitude thisImpMnvr = ImpMnvr.AttitudeControl as IAgVAAttitudeControlImpulsiveAttitude;
                    if (name == "Del-V Magnitude") { thisImpMnvr.DeltaVMagnitude = value; }

                    EulerAng_Object MnvrEulAngs = new EulerAng_Object();
                    IAgOrientationEulerAngles angles = (IAgOrientationEulerAngles)thisImpMnvr.Orientation.ConvertTo(AgEOrientationType.eEulerAngles);
                    MnvrEulAngs.Sequence = angles.Sequence;
                    MnvrEulAngs.Phi_1 = double.Parse(angles.A.ToString(), System.Globalization.CultureInfo.InvariantCulture);
                    MnvrEulAngs.Theta_2 = double.Parse(angles.A.ToString(), System.Globalization.CultureInfo.InvariantCulture);
                    MnvrEulAngs.Psi_3 = double.Parse(angles.A.ToString(), System.Globalization.CultureInfo.InvariantCulture);

                    //DeltaVParams.Add("Euler Angle Sequence", double.Parse(MnvrEulAngs.Sequence.ToString(), System.Globalization.CultureInfo.InvariantCulture)); l_unit.Add(units.u_Null);
                    if (name == "Euler Angle 1 (Phi)")      { MnvrEulAngs.Phi_1 = value; }
                    else if (name == "Euler Angle 2 (Theta)")    { MnvrEulAngs.Theta_2 = value; }
                    else if (name == "Euler Angle 3 (Psi)")      { MnvrEulAngs.Psi_3 = value; }

                };

                #endregion
            }
        }
        public void update_Lists(Dictionary<string, double> dict, int depth, object AccessObj, implem_Classes dataType, List<string> localParent)
        {
            foreach (KeyValuePair<string, double> entry in dict)
            {
                l_SegObj.Add(AccessObj);
                l_Names.Add(entry.Key);
                l_Values.Add(entry.Value);
                l_types.Add("null");
                l_depth.Add(depth);
                l_isQuantity.Add(true); // child components only for now
                l_implemClass.Add(dataType);
                l_localParents.Add(localParent);
            }
        }
    }
    #endregion

    #region MCS Segment Class
    public class MCS_Segments
    {
        //private IAgVAMCSSegmentCollection m_MainSequence;
        public List<string> objectTimes = new List<string>();
        public List<int> l_depth = new List<int>(); // Depth in MCS Tree
        public List<Astg_SegData> SegmentDataList = new List<Astg_SegData>();

        public MCS_Segments(IAgSatellite satellite)
        {
            IAgVAMCSSegmentCollection m_MainSequence = (satellite.Propagator as IAgVADriverMCS).MainSequence;
            var startTime = (m_MainSequence[0].InitialState as IAgVAState).Epoch;
            var stopTime = (m_MainSequence[m_MainSequence.Count-1].InitialState as IAgVAState).Epoch;

            objectTimes.Add(startTime); objectTimes.Add(stopTime);
            for (int i = 0; i < m_MainSequence.Count - 1; i++)
            {
                IAgVAMCSSegment SegmentObj = m_MainSequence[i] as IAgVAMCSSegment;
                l_depth.Add(0);
                int local_depth = 0;
                Astg_SegData thisSegment = new Astg_SegData(SegmentObj, local_depth);
                SegmentDataList.Add(thisSegment);

                if (SegmentObj.Type == AgEVASegmentType.eVASegmentTypeTargetSequence)
                {
                    get_TargSeq_Segments(SegmentObj as IAgVAMCSTargetSequence, local_depth);
                }
            }
        }

        public void get_TargSeq_Segments(IAgVAMCSTargetSequence TargSeq, int local_depth)
        {
            IAgVAMCSSegmentCollection InnerSegments = TargSeq.Segments;
            local_depth++;
            for (int ii = 0; ii < InnerSegments.Count - 1; ii++)
            {
                IAgVAMCSSegment InnerSegmentObj = InnerSegments[ii] as IAgVAMCSSegment;
                l_depth.Add(local_depth);
                Astg_SegData thisSegment = new Astg_SegData(InnerSegmentObj, local_depth);
                SegmentDataList.Add(thisSegment);

                if (InnerSegmentObj.Type == AgEVASegmentType.eVASegmentTypeTargetSequence)
                {
                    get_TargSeq_Segments(InnerSegmentObj as IAgVAMCSTargetSequence, local_depth);
                }

            }

        }
    }
    #endregion

    public struct PositionObj
    {
        public double X;
        public double Y;
        public double Z;
        public AgEPositionType ofType;
        public object access_object;
    }

    public struct Cartesian_Object
    {
        public double X;
        public double Y;
        public double Z;
    }

    public struct EulerAng_Object
    {
        public double Phi_1;
        public double Theta_2;
        public double Psi_3;
        public AgEEulerOrientationSequence Sequence;
    }

    #endregion
}