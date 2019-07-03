# -*- coding: utf-8 -*-
"""
Created on Fri May 17 17:44:34 2019

@author: apoca
"""
import valispace
import keyring
from comtypes.client import GetActiveObject
from comtypes.client import CreateObject
#from comtypes.gen import STKUtil
from comtypes.gen import STKObjects

#----------- Functions ---------------#     
def findidxDict(dictList,namekey):
    dictidx=[]
    for idx in range (0,len(dictList)):
        if dictList[idx]['name'] == namekey:
            dictidx = idx
            break
    if dictidx== []:
            print("Does not Exist")
    return dictidx
    

valispace = valispace.API(url='https://app.valispace.com', username='kuldeep', password=keyring.get_password("valispace","kuldeep"))


uiApplication = GetActiveObject('STK11.Application')
root=uiApplication.Personality2

scenario1         = root.CurrentScenario
scenario2        = scenario1.QueryInterface(STKObjects.IAgScenario)

root.Rewind();

# Fetch from Valispace
### Enter PROJECT NAME here ### 
project_Name = 'ValiSAT_STK_GEO'
 
# Fetch project JSON object for above project name
dict_project = valispace.get_project_by_name(name=project_Name)
projectID = dict_project[0]['id']  

#Get Parking Orbit
Sat_dict = valispace.get_component_by_name(unique_name = 'Satellite',project_name = project_Name)
Sat_childIDs = Sat_dict[0]['children']

## Get Orbit Component of Satellite Object among all the children in Satellite component
for idx2 in range (0,len(Sat_childIDs)):
    temp_comp = valispace.get_component(id = Sat_childIDs[idx2])
    if temp_comp['name']== 'ParkingOrbit':
        Sat_PorbitID = Sat_childIDs[idx2]
        SatPOrbit_dict = temp_comp
        break
del temp_comp

temp = {}
# List all the valis in Orbit subcomponent
Sat_valiIDs = SatPOrbit_dict['valis']

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
Sat_PODict = temp;

SatPO_SMA     = str(Sat_PODict['SemiMajorAxis'])
SatPO_ecc     = str(Sat_PODict['Eccentricity'])
SatPO_inc     = str(Sat_PODict['Inclination'])
SatPO_RAAN    = str(Sat_PODict['RAAN'])
SatPO_AOP     = str(Sat_PODict['ArgOfPerigee'])
SatPO_MA      = str(Sat_PODict['MeanAnomaly'])

root.ExecuteCommand('Astrogator */Satellite/ValiSAT SetValue MainSequence.SegmentList.ParkingOrbit.InitialState.Keplerian.sma '+SatPO_SMA+' m')
root.ExecuteCommand('Astrogator */Satellite/ValiSAT SetValue MainSequence.SegmentList.ParkingOrbit.InitialState.Keplerian.inc '+SatPO_inc+' deg')
root.ExecuteCommand('Astrogator */Satellite/ValiSAT SetValue MainSequence.SegmentList.ParkingOrbit.InitialState.Keplerian.RAAN '+SatPO_RAAN+' deg')
root.ExecuteCommand('Astrogator */Satellite/ValiSAT SetValue MainSequence.SegmentList.ParkingOrbit.InitialState.Keplerian.ecc '+SatPO_ecc+' ')
root.ExecuteCommand('Astrogator */Satellite/ValiSAT SetValue MainSequence.SegmentList.ParkingOrbit.InitialState.Keplerian.w '+SatPO_AOP+' deg')
root.ExecuteCommand('Astrogator */Satellite/ValiSAT SetValue MainSequence.SegmentList.ParkingOrbit.InitialState.Keplerian.MeanAnomaly '+SatPO_MA+' deg')

#Satellite GEO box defined in STK 

root.ExecuteCommand(' Astrogator */Satellite/ValiSAT ResetAllProfiles')
root.ExecuteCommand('Astrogator */Satellite/ValiSAT RunMCS')
root.ExecuteCommand('Astrogator */Satellite/ValiSAT ApplyAllProfileChanges')
root.ExecuteCommand('Astrogator */Satellite/ValiSAT ClearDWCGraphics')
root.ExecuteCommand('Animate * Start End')
#PO_dict = valispace.get_component_by_name(unique_name = 'ParkingOrbit',project_name = project_Name)

CmdRes = {}
#get Results
result = root.ExecuteCommand('Astrogator_RM */Satellite/ValiSAT GetValue MainSequence.SegmentList.Target_Sequence')

for i in range(0,result.Count):
    cmdRes = result.Item(i)
    print(cmdRes)
#
#satellite = scenario.Children.getelements(STKObjects.eSatellite)
#scenario.Children.Item(0)
#scenario.Children.Count
satellite = scenario1.Children.Item("ValiSAT")
satDP3 = satellite.DataProviders.GetDataPrvTimeVarFromPath('Astrogator Values/Maneuver').Exec(scenario2.StartTime,scenario2.StopTime,60)
a = satDP3.DataSets.GetDataSetByName('MCS_DeltaV').GetValues()
#
DeltaV_Res = []
temp = {}
TSQ_names = ["PlaneChange","StartTransfer","Circularize"]
DelV_names =["FinalInertialDeltaVxSDU", "FinalInertialDeltaVySDU","FinalInertialDeltaVzSDU"]

import re
r = re.compile(r'(\w+) = (\d*.?\d+)')
for idx1 in range(0,len(TSQ_names)):
    temp['name'] = TSQ_names[idx1]
    for idx2 in range (0,len(DelV_names)):
         result = root.ExecuteCommand('Astrogator_RM */Satellite/ValiSAT GetValue MainSequence.SegmentList.Target_Sequence.SegmentList.'+TSQ_names[idx1]+'.SegmentList.DV.'+DelV_names[idx2]+'')
         res_parsed= r.findall(result.Item(0))
         temp[res_parsed[0][0]] = float(res_parsed[0][1])
    DeltaV_Res.append(temp)
    temp = {}

temp_comp = {}
Mnvr_Vali_map = []
Mnvr_map = {}
print('Analysis Results are ready. You can make changes to the STK scenario \n')
print('Export all variables to Valispace? Y/N  \n NOTE: You can make any changes to the analyses before exporting. Press Y after all the changes')
xport = input()
if xport == 'Y' or xport =='y':
    Mnvr_dict = valispace.get_component_by_name(unique_name = 'Maneuvers',project_name = project_Name)
    Mnvr_childIDs = Mnvr_dict[0]['children']
    for idx in range (0,len(Mnvr_childIDs)):
        temp_comp = valispace.get_component(id = Mnvr_childIDs[idx])
        Mnvr_map['name'] = temp_comp['name'] 
        Mnvr_map['id']  = Mnvr_childIDs[idx]
        Mnvr_map['valiID'] = temp_comp['valis']
        #find DeltaV valis
        for idx2 in range (0,len(Mnvr_map['valiID'])):
            temp_delV = valispace.get_vali(id =Mnvr_map['valiID'][idx2])
            if temp_delV['shortname'] == 'DeltaVx':
                Mnvr_map['DeltaVx'] = Mnvr_map['valiID'][idx2]
            elif temp_delV['shortname'] == 'DeltaVy':
                Mnvr_map['DeltaVy'] = Mnvr_map['valiID'][idx2]
            elif temp_delV['shortname'] == 'DeltaVz':
                Mnvr_map['DeltaVz'] = Mnvr_map['valiID'][idx2]
        Mnvr_Vali_map.append(Mnvr_map)
        Mnvr_map = {}
        temp_comp = {}
del temp_comp
del Mnvr_map
DV_valinames = ['DeltaVx','DeltaVy','DeltaVz']
#DeltaV components update
for idx in range (0,3):
    valispace.update_vali(id = Mnvr_Vali_map [findidxDict(Mnvr_Vali_map,'PlaneChange')][DV_valinames[idx]],formula = str(DeltaV_Res[findidxDict(DeltaV_Res,TSQ_names[0])][DelV_names[idx]]))
    valispace.update_vali(id = Mnvr_Vali_map [findidxDict(Mnvr_Vali_map,'Transfer')][DV_valinames[idx]],formula = str(DeltaV_Res[findidxDict(DeltaV_Res,TSQ_names[1])][DelV_names[idx]]))
    valispace.update_vali(id = Mnvr_Vali_map [findidxDict(Mnvr_Vali_map,'Circularize')][DV_valinames[idx]],formula = str(DeltaV_Res[findidxDict(DeltaV_Res,TSQ_names[2])][DelV_names[idx]]))
#for idx in range(0,len(DeltaV_Res)):
#    DeltaV_Res[idx]['name'] = 'PlaneChange'
#        valispace.update_vali

# x = r.findall(result.Item(0)) 
# num = float(x[1])
    
    
#RESET
#for idx in range (0,3):
#    valispace.update_vali(id = Mnvr_Vali_map [findidxDict(Mnvr_Vali_map,'PlaneChange')][DV_valinames[idx]],formula = str(0))
#    valispace.update_vali(id = Mnvr_Vali_map [findidxDict(Mnvr_Vali_map,'Transfer')][DV_valinames[idx]],formula = str(0))
#    valispace.update_vali(id = Mnvr_Vali_map [findidxDict(Mnvr_Vali_map,'Circularize')][DV_valinames[idx]],formula = str(0))
