using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AGI.STKObjects;
using AGI.STKUtil;

namespace ValispacePlugin
{
    #region Facility

    public class Facility_Data
    {
        public Dictionary<string, double> Location = new Dictionary<string, double>();
        public List<string> l_Names = new List<string>();
        public List<object> l_SegObj = new List<object>();
        public List<double> l_Values = new List<double>();
        public List<int> l_depth = new List<int>();
        public List<string> l_unit = new List<string>();
        public List<bool> l_isQuantity = new List<bool>();
        public List<string> l_types = new List<string>();
        public List<implem_Classes> l_implemClass = new List<implem_Classes>();
        public PositionObj m_thisPosition;

        public Facility_Data(IAgStkObject object0)
        {
            IAgFacility thisFacility = object0 as IAgFacility;
            get_Location(thisFacility);
            
        }

        private void get_Location(IAgFacility thisFacility)
        {
            var depth = 1;
            #region List Header
            l_SegObj.Add(null);
            l_Names.Add("Position");
            l_unit.Add("");
            l_Values.Add(double.PositiveInfinity);
            l_types.Add(" ");
            l_depth.Add(depth);
            l_isQuantity.Add(false);
            l_implemClass.Add(implem_Classes.NULL);
            #endregion
            AgEPositionType Postype = (thisFacility.Position as IAgPosition).PosType;
            m_thisPosition.access_object = thisFacility;
            Unit units = new Unit();
            if (Postype == AgEPositionType.eCartesian)
            {
                (thisFacility.Position).QueryCartesian(out m_thisPosition.X, out m_thisPosition.Y, out m_thisPosition.Z);
                m_thisPosition.ofType = Postype;
                Location.Add("Cartesian:X", m_thisPosition.X); l_unit.Add(units.u_Distance);
                Location.Add("Cartesian:Y", m_thisPosition.Y); l_unit.Add(units.u_Distance);
                Location.Add("Cartesian:Z", m_thisPosition.Z); l_unit.Add(units.u_Distance);
                update_Lists(Location, depth, thisFacility, implem_Classes.Facility_Location);
            }
            else if (Postype == AgEPositionType.eGeodetic || Postype == AgEPositionType.ePlanetodetic)
            {
                var geodetic =  (thisFacility.Position.QueryPlanetodeticArray());
                updatefromArray(geodetic);
                m_thisPosition.ofType = Postype;
                Location.Add("Geodetic:Latitude", m_thisPosition.X); l_unit.Add(units.u_Angle);
                Location.Add("Geodetic:Longitude", m_thisPosition.Y); l_unit.Add(units.u_Angle);
                Location.Add("Geodetic:Altitude", m_thisPosition.Z); l_unit.Add(units.u_Distance);
                update_Lists(Location, depth, thisFacility, implem_Classes.Facility_Location);
            }
            else if (Postype == AgEPositionType.eCylindrical)
            {
                var cylindrical  = (thisFacility.Position).QueryCylindricalArray();
                updatefromArray(cylindrical);
                Location.Add("Cylindrical:Radius", m_thisPosition.X); l_unit.Add(units.u_Distance);
                Location.Add("Cylindrical:Longitude", m_thisPosition.Y); l_unit.Add(units.u_Distance);
                Location.Add("Cylindrical:Z", m_thisPosition.Z); l_unit.Add(units.u_Distance);
                update_Lists(Location, depth, thisFacility, implem_Classes.Facility_Location);
            }
            else if (Postype == AgEPositionType.eSpherical)
            {
                var spherical = (thisFacility.Position).QuerySphericalArray();
                updatefromArray(spherical);
                Location.Add("Spherical:Latitude", m_thisPosition.X); l_unit.Add(units.u_Distance);
                Location.Add("Spherical:Longitude", m_thisPosition.Y); l_unit.Add(units.u_Distance);
                Location.Add("Spherical:Radius", m_thisPosition.Z); l_unit.Add(units.u_Distance);
                update_Lists(Location, depth, thisFacility, implem_Classes.Facility_Location);
            }
            else if (Postype == AgEPositionType.ePlanetocentric)
            {
                var planetocentric = (thisFacility.Position).QueryPlanetocentricArray();
                updatefromArray(planetocentric);
                Location.Add("Spherical:Latitude", m_thisPosition.X); l_unit.Add(units.u_Distance);
                Location.Add("Spherical:Longitude", m_thisPosition.Y); l_unit.Add(units.u_Distance);
                Location.Add("Spherical:Radius", m_thisPosition.Z); l_unit.Add(units.u_Distance);
                update_Lists(Location, depth, thisFacility, implem_Classes.Facility_Location);
            }
        }

        public void set_Location(object thisfacility)
        {
            return;
        }
        private void updatefromArray(System.Array array)
        {
            m_thisPosition.X = ((IConvertible)array.GetValue(0)).ToDouble(null);
            m_thisPosition.Y = ((IConvertible)array.GetValue(1)).ToDouble(null);
            m_thisPosition.Z = ((IConvertible)array.GetValue(2)).ToDouble(null);
        }
        private void update_Lists(Dictionary<string, double> dict, int depth, object AccessObj, implem_Classes dataType)
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
            }
        }
    }


    #endregion
}
