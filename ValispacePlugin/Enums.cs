using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValispacePlugin
{
    /// <summary>
    /// Classes Implemented for child objects, used to identify redirect class to assign values in STK.
    /// Update list with all the classes
    /// </summary>
    public enum implem_Classes
    {
        Satellite_OrbitData,
        Satellite_MassData,
        Astg_InitState,
        Astg_SC_InitParams,
        Astg_FuelTankParams,
        Astg_Mnvr_DV,
        Facility_Location,
        NULL
    }
    enum Mass_Props
    {
        Mass,
        Ixx,
        Ixy,
        Ixz,
        Iyy,
        Iyz,
        Izz
    }
    enum Astg_InitState_Type
    {
        Classical,
        Cartesian,
        Spherical
    }

    enum BasicOrbitElems
    {
        SemiMajorAxis,
        Eccentricity,
        Inclination,
        RAAN,
        ArgOfPerigee,
        TrueAnomaly
    }
}
