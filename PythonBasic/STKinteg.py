# -*- coding: utf-8 -*-
"""
Created on Wed May 15 15:18:25 2019

@author: Kuldeep Barad (Valispace) : kuldeep@valispace.com
"""
import valispace
import keyring
import time

#from win32api import GetSystemMetrics
from comtypes.client import CreateObject
#from comtypes.gen import STKUtil
from comtypes.gen import STKObjects

### -------------- Fetch Project and Data from Valispace ----------------- ##
### Enter PROJECT NAME here ### 
project_Name = 'ValiSAT_STK'

user_name = 'kuldeep'
pass_word = keyring.get_password("valispace","kuldeep")
 ## Connect to Valispace
valispace = valispace.API(url='https://app.valispace.com', username='kuldeep', password=pass_word)

# Fetch project JSON object for above project name
dict_project = valispace.get_project_by_name(name=project_Name)
projectID = dict_project[0]['id']       

## Get GroundStation JSON object - Looks for a parent component 'GroundStations' which comprises of all individual GS as children
GS_dict = valispace.get_component_by_name(unique_name = 'GroundStations',project_name = project_Name)
#Get Component IDs for all children- individual GS
GS_compIDs = GS_dict[0]['children']
temp = {}

#Construct new Dictionary with GS variables necessary to pass to STK
GS_dict_STK = []

for idx0 in range (0,len(GS_compIDs)):
    Temp_dict = valispace.get_component(id = GS_compIDs[idx0])
    temp['count'] = idx0
    temp['name'] = Temp_dict['name']
    temp_valiIDs = Temp_dict['valis']

    for idx1 in range (0,len(temp_valiIDs)):
        temp_vali = valispace.get_vali(id = temp_valiIDs[idx1])
        if temp_vali['shortname'] == 'Altitude':
            temp['Altitude'] = temp_vali['value']   
        elif temp_vali['shortname'] == 'Latitude':
            temp['Latitude'] = temp_vali['value']    
        elif temp_vali['shortname'] == 'Longitude':
            temp['Longitude'] = temp_vali['value']   
        temp_vali = {};
    GS_dict_STK.append(temp)
    temp = {}
del temp_vali

## Get Satellite component from Valispace- Looks for Component 'Satellite' by name ##
Sat_dict = valispace.get_component_by_name(unique_name = 'Satellite',project_name = project_Name)
Sat_childIDs = Sat_dict[0]['children']

## Get Orbit Component of Satellite Object among all the children in Satellite component
for idx2 in range (0,len(Sat_childIDs)):
    temp_comp = valispace.get_component(id = Sat_childIDs[idx2])
    if temp_comp['name']== 'Orbit':
        Sat_orbitID = Sat_childIDs[idx2]
        SatOrbit_dict = temp_comp
        break
del temp_comp  

# List all the valis in Orbit subcomponent
Sat_valiIDs = SatOrbit_dict['valis']

#Map 
Sat_dict_STK = []
ID_dict = {}
for idx3 in range(0,len(Sat_valiIDs)):
    temp_vali = valispace.get_vali(id = Sat_valiIDs[idx3])
    if temp_vali['shortname'] == 'ArgOfPerigee':
        temp['ArgOfPerigee'] = temp_vali['value']    # Unit - m
        ID_dict ['ArgOfPerigee']  = temp_vali['id'] 
    elif temp_vali['shortname'] == 'Eccentricity':
        temp['Eccentricity'] = temp_vali['value']    # Unit - m
        ID_dict ['Eccentricity']  = temp_vali['id'] 
    elif temp_vali['shortname'] == 'Inclination':
        temp['Inclination'] = temp_vali['value']    # Unit - m
        ID_dict ['Inclination']  = temp_vali['id'] 
    elif temp_vali['shortname'] == 'MeanAnomaly':
        temp['MeanAnomaly'] = temp_vali['value']    # Unit - m
        ID_dict ['MeanAnomaly']  = temp_vali['id'] 
    elif temp_vali['shortname'] == 'RAAN':
        temp['RAAN'] = temp_vali['value']    # Unit - m
        ID_dict ['RAAN']  = temp_vali['id'] 
    elif temp_vali['shortname'] == 'SemiMajorAxis':
        if temp_vali['unit'] == 'km':
            temp['SemiMajorAxis'] = temp_vali['value']*1000    # Unit - m
        elif temp_vali['unit'] == 'm':
            temp['SemiMajorAxis'] = temp_vali['value']
        ID_dict ['SemiMajorAxis']  = temp_vali['id'] 
        SMA_unit = temp_vali['unit']
    temp_vali = {};
Sat_dict_STK = temp

Sat_SMA     = str(Sat_dict_STK['SemiMajorAxis'])
Sat_ecc     = str(Sat_dict_STK['Eccentricity'])
Sat_inc     = str(Sat_dict_STK['Inclination'])
Sat_RAAN    = str(Sat_dict_STK['RAAN'])
Sat_AOP     = str(Sat_dict_STK['ArgOfPerigee'])
Sat_MA      = str(Sat_dict_STK['MeanAnomaly'])

## ------------- Build STK Scenario -------------------- ##
# Reference to running STK instance
uiApplication    = CreateObject("STK11.Application")

uiApplication.Visible=True
uiApplication.UserControl=True

# Get IAgStkObjectRoot interface
root=uiApplication.Personality2

## Create a new scenario.

root.NewScenario("ValispaceBeta")
scenario         = root.CurrentScenario

## Set the analytical time period.

scenario2        = scenario.QueryInterface(STKObjects.IAgScenario)
scenario2.SetTimePeriod('Today','+24hr')
##   Reset the animation time.
root.Rewind();


#Create all GroundStation Targets
GS_handles = []
for idx4 in range (0,len(GS_dict_STK)):
    target_str = 'target'+ str(idx4)
    target_str1 = target_str+str(2)
    
    globals()[target_str]  = scenario.Children.New(STKObjects.eTarget,GS_dict_STK[idx4]['name']);
    globals()[target_str1] = globals()[target_str].QueryInterface(STKObjects.IAgTarget)
    GSAlt = GS_dict_STK[idx4]['Altitude']
    GSLat = GS_dict_STK[idx4]['Latitude']
    GSLong = GS_dict_STK[idx4]['Longitude']
    globals()[target_str1].Position.AssignGeodetic(GSLat,GSLong,GSAlt)
    GS_handles.append(target_str1)

#Create a satellite Object 
satellite        = scenario.Children.New(STKObjects.eSatellite, "ValiSAT")

### Propagate the orbit.
#   Refer to SetState Command of STK Connect Library and customize propagator and state representations 
#   SetState command doesn't accept values of less than -180 degrees for orbital elements.
root.ExecuteCommand('SetState */Satellite/ValiSAT Classical J2Perturbation "' + scenario2.StartTime + '" "'+ scenario2.StopTime +'" 60 J2000 "'+ scenario2.StartTime +'" '+ Sat_SMA +' '+ Sat_ecc +' '+ Sat_inc +' '+ Sat_AOP +' '+ Sat_RAAN +' '+ Sat_MA +'')


# Calculate and visualize access to each ground station from GS_handles object
for idx5 in range (0, len(GS_handles)):
    access = satellite.GetAccessToObject(globals()[GS_handles[idx5]])
    access.ComputeAccess()

    #Get the Access AER Data Provider
    accessDP         = access.DataProviders.Item('Access Data')
    accessDP2        = accessDP.QueryInterface(STKObjects.IAgDataPrvInterval)
    results          = accessDP2.Exec(scenario2.StartTime, scenario2.StopTime)
    try:
        accessStartTimes = results.DataSets.GetDataSetByName('Start Time').GetValues()
        accessStopTimes  = results.DataSets.GetDataSetByName('Stop Time').GetValues()
    except:
        strprnt = "No Access to " +  str(globals()[GS_handles[idx5]]) + " Ground Station "
        print(strprnt)        
    print("\n Access Intervals: \n")
    print(accessStartTimes,accessStopTimes)
    
    ## ----- Create TextVali with Analysis data ------- ##
#    shortname = 'AccessGS'+ GS_dict_STK[idx5]['name']
#    data1 = {
#            "name":  "Message", 
#            "text": "Message123", 
#            "parent": 3698
#            }
#    valispace.post_data(type='textvali', data=)
#    valispace.post_data(type='textvali', data =json.dumps(data1,indent=4))

print('You can now STK to visualize the Orbit')
root.ExecuteCommand('Animate * Start End')
time.sleep(5)


## --- Let User make changes to orbit in STK ---- #
updatelist = {}
print('Have you made any changes to the STK scenario orbit? You can update the changes to Valispace. \n')
print('Update Orbital Elements to Valispace?  Y/N ')
change = input()
# Fetch Changed orbital parmaeters from satellite DataProvider
if change == 'y' or change == 'Y': 
    satelliteDPCOE       = satellite.DataProviders.Item('Classical Elements')
    satelliteDPCOE2      = satelliteDPCOE.QueryInterface(STKObjects.IAgDataProviderGroup)
    satelliteDPCOE3      = satelliteDPCOE2.Group.Item('J2000')
    satelliteDPCOE4      = satelliteDPCOE3.QueryInterface(STKObjects.IAgDataPrvTimeVar)
    rptElems             = ['Semi-major Axis', 'Eccentricity', 'Inclination','RAAN','Arg of Perigee','Mean Anomaly']
    satellitDPTimeVar    = satelliteDPCOE4.ExecElements(scenario2.StartTime,scenario2.StartTime, 60, rptElems)
    
    #Dict with updated numbers
    for idx6 in range (0,len(rptElems)):
        data_get = satellitDPTimeVar.DataSets.GetDataSetByName(rptElems[idx6]).GetValues()
        updatelist[rptElems[idx6]] = data_get[0] #numpy.mean(data_get)
        
    # ------ Update values of Valis on Valispace ----- #
    if SMA_unit == 'km':
        valispace.update_vali(id = ID_dict['SemiMajorAxis'],formula = str(updatelist['Semi-major Axis']))
    elif SMA_unit == 'm':
        valispace.update_vali(id = ID_dict['SemiMajorAxis'],formula = str(updatelist['Semi-major Axis']*1000))
        
    valispace.update_vali(id = ID_dict['Eccentricity'],formula = str(updatelist['Eccentricity']))
    valispace.update_vali(id = ID_dict['Inclination'],formula = str(updatelist['Inclination']))
    valispace.update_vali(id = ID_dict['RAAN'],formula = str(updatelist['RAAN']))
    valispace.update_vali(id = ID_dict['ArgOfPerigee'],formula = str(updatelist['Arg of Perigee']))
    valispace.update_vali(id = ID_dict['MeanAnomaly'],formula = str(updatelist['Mean Anomaly']))

print("Everything is as it should be. You're all set to continue designing your satellite mission ;) " )
#Get New Values from STK 
    
