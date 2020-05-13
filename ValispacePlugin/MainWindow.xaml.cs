using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AGI.STKObjects;
using AGI.Ui.Application;
using AGI.Ui.Plugins;
using stdole;
using AGI.STKUtil;

namespace ValispacePlugin
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : UserControl
    {
        private IAgUiPluginEmbeddedControlSite m_pEmbeddedControlSite;
        private UIPlugin m_uiplugin;
        private AgStkObjectRoot m_root ;

        private IAgStkObject Scenario;
        private static ValispaceInstance thisValiInstance;
        public static bindingsFile projInfoFile;

        //UI
        public static TextBlock thisProjMsg;
        public static TreeView thisprojectTree;
        public static TextBlock thisProjectLabel;
        public static TextBlock StatusBoxContainer;
        public static int thisRowIndex;
        uiValiObj selectedValiObj = new uiValiObj();

        //Dataset UI
        public static windowDatasets m_datasetWin;

        //Scenario File Path
        public static string m_scenarioPath;
        private IAgStkObjectCollection allChildren;
        public List<string> ChildrenNameSTK { get; set; }
        public List<STKObject> ListChildren = new List<STKObject>();
        public Dictionary<int, STKObject> GridDict = new Dictionary<int, STKObject>();
        public Dictionary<int, Int64> UpdateValiIDs = new Dictionary<int, Int64>();
        public Dictionary<int, object> AccessObject = new Dictionary<int, object>();
        public Dictionary<int, implem_Classes> AccessClassName = new Dictionary<int, implem_Classes>();
        public Dictionary<int, List<string>> dictObjectTimes = new Dictionary<int, List<string>>();
        public Dictionary<int, List<string>> parentsHeirarchy = new Dictionary<int, List<string>>();
        public Dictionary<string, object> bindingValis = new Dictionary<string, object>();
        /// <summary>
        /// GET FUNCTIONS IMPLEMENT
        /// </summary>
        public Unit Units = new Unit();
        private int rowCount = 0;

        public MainWindow()
        {
            InitializeComponent();
           
        }

        public void loadAll()
        {
            
            Window_Loaded();
            m_datasetWin = new windowDatasets(Scenario, dictObjectTimes, null, StartTimeBoxXAML, StopTimeBoxXAML, DPTree, valiTree_XAML);
            UpdateSTKTree();
            //Login newLogin = new Login(projInfoFile);
        }

        public void SetSite(IAgUiPluginEmbeddedControlSite Site)
        {
            m_pEmbeddedControlSite = Site;
            m_uiplugin = m_pEmbeddedControlSite.Plugin as UIPlugin;
            m_root = m_uiplugin.STKRoot;
        }

        public void InitProjnFile(IAgUiPluginEmbeddedControlSite Site)
        {
            SetSite(Site);
            try
            {
                
                Scenario = m_root.CurrentScenario;
                IAgExecCmdResult result = m_root.ExecuteCommand("GetDirectory / Scenario");
                if (result.IsSucceeded)
                {
                    m_scenarioPath = result[0];
                }
                // ADD SCENARIO PATH TO BINDING FILE CLASS
                
            }
            catch
            {
                MessageBox.Show("STK Instance does not exist or is lost! \n Make sure STK is open and the scenario is loaded ");
                //Environment.Exit(0);
            }

            if (m_scenarioPath!=null)
            {
                projInfoFile = new bindingsFile(m_scenarioPath);
                projInfoFile.readInfofile();
                m_root.UnitPreferences.ResetUnits();
                loadAll();
            }
        }
       

        private void Window_Loaded()
        {

            thisprojectTree = projectTree;
            thisProjectLabel = ProjectLabel;
            thisProjMsg = LoadProject_Msg;
            projectTree.IsEnabled = false;
            StatusBoxContainer = StatusBox;

            //try
            //{
            //    Scenario = m_root.CurrentScenario;
            //    IAgExecCmdResult result = m_root.ExecuteCommand("GetDirectory / Scenario");
            //    if (result.IsSucceeded)
            //    {
            //        m_scenarioPath = result[0];
            //    }
            //    // ADD SCENARIO PATH TO BINDING FILE CLASS
            //    projInfoFile = new bindingsFile(m_scenarioPath);

            //    m_root.UnitPreferences.ResetUnits();
            //}
            //catch
            //{
            //    MessageBox.Show("STK Instance does not exist or is lost! \n Make sure STK is open and the scenario is loaded ");
            //    Environment.Exit(0);
            //}
            //projInfoFile.readInfofile();
            readAllAttributes();
        }

        public void readAllAttributes()
        {
            int Child_count = Scenario.Children.Count;

            // Note: IAgStkObject is instance of the object 
            allChildren = Scenario.Children;
            makeChildrenList(allChildren);

            for (int i = 0; i < Child_count; i++)
            {
                List<string> localParent = new List<string>();
                if (allChildren[i].ClassName == "Satellite")
                {
                    string propName = getPropagator(allChildren[i]);

                    STKObject list0 = new STKObject(allChildren[i].InstanceName, allChildren[i].ClassName, propName, allChildren[i]);

                    UpdateGridDictLists(list0, null, implem_Classes.NULL, localParent);
                    localParent.Add(list0.m_ObjName);


                    //Jx Propagator
                    if (propName == "ePropagatorJ2Perturbation" || propName == "ePropagatorJ4Perturbation" || propName == "ePropagatorTwoBody")
                    {
                        #region Orbit Section
                        STKObject listHeader = new STKObject("Orbit", "IAgOrbitState"); //subcomp passed like this for ui
                        UpdateGridDictLists(listHeader, null, implem_Classes.NULL, localParent);
                        List<string> localParent_Orbit = Extensions.Clone<string>(localParent);
                        localParent_Orbit.Add(listHeader.m_ObjName);


                        Satellite_OrbitData SatOrbitData = list0.m_OrbitData;
                        var Times = new List<string> { SatOrbitData.m_ObjectTimes.FindStartTime(), SatOrbitData.m_ObjectTimes.FindStopTime() };
                        dictObjectTimes.Add(i, Times);

                        //Add List of Orbital Parameters
                        STKObject list1 = new STKObject(nameof(SatOrbitData.Step_Size), SatOrbitData.Step_Size, Units.u_Time);
                        UpdateGridDictLists(list1, SatOrbitData.m_ultimateObject, implem_Classes.Satellite_OrbitData, localParent_Orbit);
                        list1 = new STKObject(nameof(SatOrbitData.SemiMajorAxis), SatOrbitData.SemiMajorAxis, Units.u_Distance);
                        UpdateGridDictLists(list1, SatOrbitData.m_ultimateObject, implem_Classes.Satellite_OrbitData, localParent_Orbit);
                        list1 = new STKObject(nameof(SatOrbitData.Eccentricity), SatOrbitData.Eccentricity, Units.u_Null);
                        UpdateGridDictLists(list1, SatOrbitData.m_ultimateObject, implem_Classes.Satellite_OrbitData, localParent_Orbit);
                        list1 = new STKObject(nameof(SatOrbitData.Inclination), SatOrbitData.Inclination, Units.u_Angle);
                        UpdateGridDictLists(list1, SatOrbitData.m_ultimateObject, implem_Classes.Satellite_OrbitData, localParent_Orbit);
                        list1 = new STKObject(nameof(SatOrbitData.RAAN), SatOrbitData.RAAN, Units.u_Angle);
                        UpdateGridDictLists(list1, SatOrbitData.m_ultimateObject, implem_Classes.Satellite_OrbitData, localParent_Orbit);
                        list1 = new STKObject(nameof(SatOrbitData.ArgOfPerigee), SatOrbitData.ArgOfPerigee, Units.u_Angle);
                        UpdateGridDictLists(list1, SatOrbitData.m_ultimateObject, implem_Classes.Satellite_OrbitData, localParent_Orbit);
                        list1 = new STKObject(nameof(SatOrbitData.TrueAnomaly), SatOrbitData.TrueAnomaly, Units.u_Angle);
                        UpdateGridDictLists(list1, SatOrbitData.m_ultimateObject, implem_Classes.Satellite_OrbitData, localParent_Orbit);
                        #endregion

                        #region Mass Properties Section
                        listHeader = new STKObject("Mass", "IAgVeMassProperties"); //subcomp passed like this for ui
                        UpdateGridDictLists(listHeader, null, implem_Classes.NULL, localParent);
                        var localParent_Mass = Extensions.Clone<string>(localParent);
                        localParent_Mass.Add(listHeader.m_ObjName);

                        Satellite_MassData SatMassData = list0.m_MassData;
                        STKObject list2 = new STKObject(nameof(SatMassData.Mass), SatMassData.Mass, Units.u_Mass); UpdateGridDictLists(list2, SatMassData.m_ultimateObject, implem_Classes.Satellite_MassData, localParent_Mass);
                        list2 = new STKObject(nameof(SatMassData.Ixx), SatMassData.Ixx, Units.u_Inertia); UpdateGridDictLists(list2, SatMassData.m_ultimateObject, implem_Classes.Satellite_MassData, localParent_Mass);
                        list2 = new STKObject(nameof(SatMassData.Iyy), SatMassData.Iyy, Units.u_Inertia); UpdateGridDictLists(list2, SatMassData.m_ultimateObject, implem_Classes.Satellite_MassData, localParent_Mass);
                        list2 = new STKObject(nameof(SatMassData.Izz), SatMassData.Izz, Units.u_Inertia); UpdateGridDictLists(list2, SatMassData.m_ultimateObject, implem_Classes.Satellite_MassData, localParent_Mass);
                        list2 = new STKObject(nameof(SatMassData.Ixy), SatMassData.Ixy, Units.u_Inertia); UpdateGridDictLists(list2, SatMassData.m_ultimateObject, implem_Classes.Satellite_MassData, localParent_Mass);
                        list2 = new STKObject(nameof(SatMassData.Ixz), SatMassData.Ixz, Units.u_Inertia); UpdateGridDictLists(list2, SatMassData.m_ultimateObject, implem_Classes.Satellite_MassData, localParent_Mass);
                        list2 = new STKObject(nameof(SatMassData.Iyz), SatMassData.Iyz, Units.u_Inertia); UpdateGridDictLists(list2, SatMassData.m_ultimateObject, implem_Classes.Satellite_MassData, localParent_Mass);

                        #endregion

                    }

                    //Astrogator
                    else if (propName == "ePropagatorAstrogator")
                    {
                        IAgSatellite satellite = allChildren[i] as IAgSatellite;

                        #region Orbit Section
                        STKObject listHeader = new STKObject("Orbit: Astrogator", "IAgVAMCSDriver", 2); //subcomp passed like this for ui
                        UpdateGridDictLists(listHeader, null, implem_Classes.NULL, localParent);
                        var localParent_Orbit = Extensions.Clone<string>(localParent);
                        localParent_Orbit.Add(listHeader.m_ObjName);
                        //get all sequences
                        MCS_Segments ListSegments = new MCS_Segments(satellite);
                        dictObjectTimes.Add(i, ListSegments.objectTimes);
                        STKObject AstgList = new STKObject();

                        for (int k = 0; k < ListSegments.SegmentDataList.Count; k++)
                        {

                            for (int j = 0; j < ListSegments.SegmentDataList[k].l_Names.Count; j++)
                            {
                                if (!ListSegments.SegmentDataList[k].l_isQuantity[j])
                                {
                                    AstgList = new STKObject(ListSegments.SegmentDataList[k].l_Names[j], ListSegments.SegmentDataList[k].l_types[j], Math.Min(5, (3 + ListSegments.SegmentDataList[k].l_depth[j])));
                                    var thislocalParent = Extensions.Clone<string>(localParent_Orbit);
                                    thislocalParent.AddRange(ListSegments.SegmentDataList[k].l_localParents[j]);
                                    UpdateGridDictLists(AstgList, null, implem_Classes.NULL, thislocalParent);
                                }
                                else
                                {
                                    var AccessObj = (ListSegments.SegmentDataList[k].l_SegObj[j]);
                                    var AccessObjType = (ListSegments.SegmentDataList[k].l_implemClass[j]);
                                    var thislocalParent = Extensions.Clone<string>(localParent_Orbit); thislocalParent.AddRange(ListSegments.SegmentDataList[k].l_localParents[j]);

                                    localParent_Orbit.AddRange(ListSegments.SegmentDataList[k].l_localParents[j]);
                                    AstgList = new STKObject(ListSegments.SegmentDataList[k].l_Names[j], ListSegments.SegmentDataList[k].l_Values[j], ListSegments.SegmentDataList[k].l_unit[j]);
                                    UpdateGridDictLists(AstgList, AccessObj, AccessObjType, thislocalParent);
                                    //ADD ULTIMATE ACCESS HANDLER HEREEEEEE.
                                }
                            }
                        }
                        #endregion

                        #region Mass Properties Section

                        #endregion
                    }
                }
                else if (allChildren[i].ClassName == "Facility")
                {
                    Facility_Data thisFacility = new Facility_Data(allChildren[i]);
                    STKObject list0 = new STKObject(allChildren[i].InstanceName, allChildren[i].ClassName, allChildren[i]);
                    UpdateGridDictLists(list0, null, implem_Classes.NULL, localParent);
                    localParent.Add(allChildren[i].InstanceName);
                    for (int ii = 0; ii < thisFacility.l_Names.Count; ii++)
                    {
                        if (!thisFacility.l_isQuantity[ii])
                        {
                            var facilityList = new STKObject(thisFacility.l_Names[ii], thisFacility.l_types[ii], (1 + thisFacility.l_depth[ii])); UpdateGridDictLists(facilityList, null, implem_Classes.NULL, localParent);
                        }
                        else
                        {
                            var accessObj = thisFacility.l_SegObj[ii];
                            var accessType = thisFacility.l_implemClass[ii];
                            var facilityList = new STKObject(thisFacility.l_Names[ii], thisFacility.l_Values[ii], thisFacility.l_unit[ii]);

                            UpdateGridDictLists(facilityList, null, implem_Classes.Facility_Location, localParent);
                        }

                    }
                }
                else
                {
                    STKObject list0 = new STKObject(allChildren[i].InstanceName, allChildren[i].ClassName, allChildren[i]);
                    UpdateGridDictLists(list0, null, implem_Classes.NULL, localParent);

                }

            }
            /// File Load Last used project name
            if (projInfoFile.hasInfo["Project Name"])
            {
                var projName = (string)(projInfoFile.getInfo("Project Name"));

                StatusBox.Text = "Project Loaded: \" " + projName + " \" . Press 'Refresh Valispace Data' ";
                thisProjectLabel.Text = "Project :  " + projName + " (Refresh)";

                LoadProject_Msg.Visibility = Visibility.Hidden;
                //loadLastProjectName();
            }

        }

        private void makeChildrenList(IAgStkObjectCollection allChildren)
        {
            List<string> temp = new List<string>();
            ComboBoxItem item = new ComboBoxItem();
            foreach (var child in allChildren)
            {
                temp.Add((child as IAgStkObject).InstanceName);
            }

            ChildrenNameSTK = Extensions.Clone(temp);
            ChildrenDropdown.ItemsSource = ChildrenNameSTK;
            ChildrenDropdown.Items.Refresh();
        }

        private void loadLastProjectName()
        {
            string fileName = @"C:\Temp\temp_valiProject.txt";

            try
            {
                // Check if file already exists.     
                if (File.Exists(fileName))
                {
                    using (StreamReader sr = File.OpenText(fileName))
                    {
                        string s = "";
                        while ((s = sr.ReadLine()) != null)
                        {
                            ProjWindow.ProjName = s.ToString();
                            StatusBox.Text = "Project Loaded: \" " + ProjWindow.ProjName + " \" . Press 'Refresh Valispace Data' ";
                            thisProjectLabel.Text = "Project :  " + ProjWindow.ProjName + " (Refresh)";

                        }
                    }
                }
            }
            catch
            {
                return;
            }
        }

        private void UpdateGridDictLists(STKObject thisListObject, object accesshandler, implem_Classes AccessClass, List<string> parents)
        {
            ListChildren.Add(thisListObject);
            GridDict.Add(rowCount, thisListObject);
            AccessObject.Add(rowCount, accesshandler);
            AccessClassName.Add(rowCount, AccessClass);
            parentsHeirarchy.Add(rowCount, parents);
            rowCount++;
        }

        #region Window Closing: STK Instance Give-Up
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (m_root != null)
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(m_root);
                m_root = null;
                projInfoFile.write2file();
                //System.Windows.Application.Current.Shutdown();
                Environment.Exit(0);
            }
        }

        #endregion

        #region Update STK Data to UI       

        public void UpdateSTKTree()
        {
            // Update All types Supported here
            //Satellite
            List<string> all_Satellites = new List<string>();
            //FAcility
            List<string> all_Facility = new List<string>();

            foreach (var ObjectOfType in ListChildren)
            {
                if (ObjectOfType.m_ObjType == "Satellite")
                {
                    all_Satellites.Add(ObjectOfType.m_ObjName);

                }
                if (ObjectOfType.m_ObjType == "Facility")
                {
                    all_Satellites.Add(ObjectOfType.m_ObjName);
                }
            }

            foreach (var t_object0 in ListChildren)
            {
                Variable item = new Variable
                {
                    VarName = t_object0.m_ObjName,
                    VarUnit = t_object0.m_Unit,
                    isBold = t_object0.isParentObj.ToString()
                };
                item.isChild = (item.isBold == "0") ? true : false;

                if (t_object0.m_ObjType == "Satellite")
                {
                    item.VarValue = t_object0.m_propagator;
                }
                else
                {
                    if (t_object0.m_ObjType == "null" || t_object0.m_ObjType == null)
                    {
                        item.VarValue = t_object0.m_Value.ToString();
                    }
                    else
                    {
                        item.VarValue = t_object0.m_ObjType;
                    }

                }
                STKDataXAML.Items.Add(item);
            }

            //for (int i = 0; i < varNames.Length; i++)
            //{
            //    Variable item1 = new Variable
            //    {
            //        VarName = varNames[i],
            //        VarUnit = varUnits[i],
            //        VarValue = varValues[i].ToString()
            //    };
            //    InputDataXAML.Items.Add(item1);
            //}
        }

        public string getPropagator(IAgStkObject object0)
        {
            IAgSatellite sat = object0 as IAgSatellite;
            return (sat.PropagatorType.ToString());
            //return ("0");
        }
        #endregion

        #region Start Updating Valispace Tree in UI
        public static bool updateTreeNow(ProjectFLAG FLAG)
        {
            var Pname = string.Empty;
            if (FLAG == ProjectFLAG.NEW)
            {
                Pname = ProjWindow.ProjName;
            }
            else if (FLAG == ProjectFLAG.OLD)
            {
                Pname = (string)projInfoFile.getInfo("Project Name");
            }
            if (Pname == string.Empty || Pname == null)
            {
                MessageBox.Show("Could not retreive a project. Try again!");
                return false;
            }
            else
            {
                StatusBoxContainer.Text = "Please Wait, " + Pname + "is being Loaded" + getTimeNow();
                thisValiInstance = new ValispaceInstance(Pname, thisprojectTree);
                if (thisValiInstance.projLoaded)
                {
                    m_datasetWin.loadValispace(thisValiInstance);
                    thisProjectLabel.Text = "Project :  " + thisValiInstance.selectedProject.Name + " (ID: " + thisValiInstance.selectedProject.Id + ")";
                    //thisValiInstance.valispace.updateVali(23043, ValiFormula: "1234");
                    //thisValiInstance.valispace.createVali(5649, "Massabcsdd", "12312314", "kg");
                    MainWindow.StatusBoxContainer.Text = "Valispace Tree Updated" + getTimeNow();
                }                
            }
            return thisValiInstance.projLoaded;

        }

        #region OPen Project and Load Tree Methods
        private void OpenProjSettings(object sender, RoutedEventArgs e)
        {
            ProjWindow objProjWindow = new ProjWindow();
            objProjWindow.Show();
        }

        private void DataGridRow_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                thisRowIndex = (sender as DataGridRow).GetIndex();
            }
            catch
            { thisRowIndex = (int)sender; }

            if (GridDict[thisRowIndex].isParentObj != 0)
            {
                StatusBox.Text = getTimeNow() + ": Non-value element selected. Cannot bind valis to non-value elements" + getTimeNow();
            }
            else
            {
                projectTree.IsEnabled = true; projectTree.Background = Brushes.White;
                STKDataXAML.IsEnabled = false; CancelSelectButton.IsEnabled = true;
                cellAccessfromRow(thisRowIndex);
            }

        }

       
        private void cellAccessfromRow(int rowIndex)
        {
            var row = (DataGridRow)STKDataXAML.ItemContainerGenerator.ContainerFromIndex(rowIndex);
            DataGridCell toBindSTKvar = GetCell(STKDataXAML, rowIndex, 1, (row));
            selectedValiObj.toBindName = ((TextBlock)toBindSTKvar.Content);
            StatusBox.Text = " Select Vali to bind to " + selectedValiObj.toBindName.Text + getTimeNow();
            DataGridCell valiNameCell = GetCell(STKDataXAML, rowIndex, 4, (row));
            DataGridCell valiValueCell = GetCell(STKDataXAML, rowIndex, 5, (row));
            DataGridCell valiUnitCell = GetCell(STKDataXAML, rowIndex, 6, (row));
            selectedValiObj.nameCell = (TextBlock)valiNameCell.Content;
            selectedValiObj.valueCell = (TextBlock)valiValueCell.Content;
            selectedValiObj.unitCell = (TextBlock)valiUnitCell.Content;
            selectedValiObj.toBindRow = rowIndex;
        }
        #endregion

        #endregion

        #region Compare Data Methods
        private int CompareUnits(int rowIndex, bool displaychanges)
        {

            DataGridRow thisRow = GetRow((STKDataXAML as DataGrid), rowIndex);
            DataGridCell cellUnit1 = GetCell(STKDataXAML, rowIndex, 3, thisRow);
            DataGridCell cellUnit2 = GetCell(STKDataXAML, rowIndex, 6, thisRow);

            DataGridCell cellValue1 = GetCell(STKDataXAML, rowIndex, 2, thisRow);
            DataGridCell cellValue2 = GetCell(STKDataXAML, rowIndex, 5, thisRow);

            var valuestring = (((TextBlock)cellValue1.Content).Text);
            var value1 = double.Parse(valuestring, System.Globalization.CultureInfo.InvariantCulture);
            valuestring = (((TextBlock)cellValue2.Content).Text);
            var value2 = double.Parse(valuestring, System.Globalization.CultureInfo.InvariantCulture);

            int returnComparisonID = new int();

            if (Math.Abs(value1 - value2) >= 1e-11)
            {
                for (var i = 0; i < STKDataXAML.Columns.Count; i++)
                {
                    DataGridCell thisCell = GetCell(STKDataXAML, rowIndex, i, thisRow);
                    if (displaychanges) { thisCell.Background = Brushes.Yellow; }

                }
                if (displaychanges)
                {
                    StatusBox.Text = "Units of binded elements are not the same." + getTimeNow();
                }
                returnComparisonID = 1;
            }
            else
            {
                for (var i = 0; i < STKDataXAML.Columns.Count; i++)
                {
                    DataGridCell thisCell = GetCell(STKDataXAML, rowIndex, i, thisRow);
                    if (displaychanges) { thisCell.Background = Brushes.Transparent; }
                }
                returnComparisonID = 0;
            }

            if (((TextBlock)cellUnit1.Content).Text != ((TextBlock)cellUnit2.Content).Text)
            {
                (cellUnit2).Background = Brushes.IndianRed;
                cellUnit1.Background = Brushes.IndianRed;
                if (displaychanges)
                {
                    StatusBox.Text = StatusBox.Text + "\n Values of binded elements need Update." + getTimeNow();
                }
                returnComparisonID = 2;
            }

            return returnComparisonID;

        }
        #endregion

        #region Select Binding Valis

        private void updateVali2Datagrid(string SelectedValiName, int rowIndex, BindingAction action)
        {
            if (action == BindingAction.ADD)
            {
                selectedValiObj.nameCell.Text = (thisValiInstance.allValis[SelectedValiName]).Name;
                selectedValiObj.valueCell.Text = (thisValiInstance.allValis[SelectedValiName]).Value.ToString();
                selectedValiObj.unitCell.Text = (thisValiInstance.allValis[SelectedValiName]).unit;
                if (UpdateValiIDs.ContainsKey(selectedValiObj.toBindRow))
                {
                    UpdateValiIDs.Remove(selectedValiObj.toBindRow);
                }
                var id = thisValiInstance.allValis[SelectedValiName].Id;
                if (id != 0)
                {
                    UpdateValiIDs.Add(selectedValiObj.toBindRow, id);
                }
                Dictionary<string, object> thisBinding = new Dictionary<string, object>();

                // Unique name as a binding key STKName + Valiname
                var bindingName = parentsHeirarchy[selectedValiObj.toBindRow][0] + "." + selectedValiObj.toBindName.Text + "." + SelectedValiName;
                thisBinding.Add("Name", selectedValiObj.toBindName.Text);
                thisBinding.Add("Id", id);
                thisBinding.Add("Parents", parentsHeirarchy[selectedValiObj.toBindRow]);
                thisBinding.Add("Row", selectedValiObj.toBindRow);

                if (bindingValis.ContainsKey(bindingName))
                {
                    bindingValis.Remove(bindingName);
                }

                bindingValis.Add(bindingName, thisBinding);

                StatusBox.Text = SelectedValiName + " binded to " + selectedValiObj.toBindName.Text + getTimeNow();

                projectTree.IsEnabled = false; projectTree.Background = Brushes.LightGray;
                STKDataXAML.IsEnabled = true;
                CompareUnits(rowIndex, true);
                SelectValiButton.IsEnabled = false;
                CancelSelectButton.IsEnabled = false;
            }
            else if (action==BindingAction.DELETE)
            {
                selectedValiObj.nameCell.Text = string.Empty;
                selectedValiObj.valueCell.Text = string.Empty;
                selectedValiObj.unitCell.Text = string.Empty;

                UpdateValiIDs.Remove(selectedValiObj.toBindRow);

                var bindingName = parentsHeirarchy[selectedValiObj.toBindRow][0] + "." + selectedValiObj.toBindName.Text + "." + SelectedValiName;
                if (bindingValis.ContainsKey(bindingName))
                {
                    bindingValis.Remove(bindingName);
                }

            }
        }


        private void TreeViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount >= 2)
            {
                if (projectTree.SelectedItem.GetType() == typeof(Vali))
                {
                    SelectValiButton.IsEnabled = true;
                    var tempSelectedValiName = (e.OriginalSource as TextBlock).Text;
                    updateVali2Datagrid(tempSelectedValiName, thisRowIndex,BindingAction.ADD);
                    e.Handled = true;
                }

            }
        }

        private void ProjectTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (projectTree.SelectedItem.GetType() == typeof(Vali))
            {
                SelectValiButton.IsEnabled = true;
            }
        }
        #endregion

        #region Buttons and Clicks Project-Tree
        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var tempSelectedValiName = ((Vali)projectTree.SelectedItem).Name;
                updateVali2Datagrid(tempSelectedValiName, thisRowIndex, BindingAction.ADD);
                SelectValiButton.IsEnabled = false;
            }
            catch
            {
                MessageBoxResult result = MessageBox.Show("Cannot bind a 'Component' to a Value Element \n Please Select a Vali (Blue)!", "WARNING!");
            }

        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            projectTree.IsEnabled = false; projectTree.Background = Brushes.LightGray;
            STKDataXAML.IsEnabled = true;
            CancelSelectButton.IsEnabled = false;
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            if ((string)projInfoFile.getInfo("Project Name") != string.Empty)
            {

                updateTreeNow(FLAG:ProjectFLAG.OLD);
                readBindings();
                LoadProject_Msg.Visibility = Visibility.Hidden;
                if (thisValiInstance.projLoaded)
                {
                    MessageBox.Show("Project Loaded");
                    
                }
            }
            else
            {
                MessageBox.Show("No Project loaded. Please enter your valispace project name in \n 'Options -> Project Settings' ");
            }
        }
        #endregion

        #region Get Row/Cell Methods
        static public DataGridCell GetCell(DataGrid dg, int row, int column, DataGridRow rowHandle)
        {
            DataGridRow rowContainer = GetRow(dg, row);

            if (rowContainer != null)
            {
                DataGridCellsPresenter presenter = GetVisualChild<DataGridCellsPresenter>(rowContainer);

                DataGridCell cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);  // <<<------ ERROR
                if (cell == null)
                {
                    // now try to bring into view and retreive the cell
                    //dg.ScrollIntoView(rowContainer, dg.Columns[column]);
                    //cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
                }
                return cell;
            }
            return null;
        }

        public static T GetVisualChild<T>(Visual parent) where T : Visual
        {
            T child = default(T);
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child == null)
                {
                    child = GetVisualChild<T>(v);
                }
                if (child != null)
                {
                    break;
                }
            }
            return child;
        }


        static public DataGridRow GetRow(DataGrid dg, int index)
        {
            DataGridRow row = (DataGridRow)dg.ItemContainerGenerator.ContainerFromIndex(index);
            if (row == null)
            {
                // may be virtualized, bring into view and try again
                dg.ScrollIntoView(dg.Items[index]);
                row = (DataGridRow)dg.ItemContainerGenerator.ContainerFromIndex(index);
            }
            return row;
        }
        #endregion

        static public string getTimeNow()
        {
            return (" (" + (DateTime.Now).ToString("HH:mm:ss") + ")");
        }


        private void SaveBindings(object sender, RoutedEventArgs e)
        {
            //var filename = @"C:\Temp\temp_valibindings.txt";
            //if (File.Exists(filename))
            //{
            //    File.Delete(filename);
            //}
            //var thisfile = File.Create(filename);
            //thisfile.Close();
            projInfoFile.writeInfo("Valis", UpdateValiIDs);
            projInfoFile.writeInfo("Valis1", bindingValis);
            var datasetBinds = makeBindingDictionary();
            projInfoFile.writeInfo("Datasets", datasetBinds);
            //using (StreamWriter file = new StreamWriter(filename))
            //{
            //    foreach (var entry in UpdateValiIDs)
            //        file.WriteLine("{0},{1}", entry.Key, entry.Value);
            //}
            //thisfile.Close();
            MessageBox.Show("Vali Bindings have been saved");
        }

        private void readBindings()
        {

            //var info = (Dictionary<string, object>)projInfoFile.getInfo("Valis");
            //foreach (var binding in info.Keys)
            //{
            //    var bindedRow = binding.ToString();
            //    var bind_ID = info[binding].ToString();
            //    var bindvaliName = thisValiInstance.allValisbyID[bind_ID].Name;
            //    cellAccessfromRow(int.Parse(bindedRow));
            //    updateVali2Datagrid(bindvaliName, int.Parse(bindedRow));
            //    if (UpdateValiIDs.ContainsKey(selectedValiObj.toBindRow))
            //    {
            //        UpdateValiIDs.Remove(selectedValiObj.toBindRow);
            //    }

            //    UpdateValiIDs.Add(selectedValiObj.toBindRow, int.Parse(bind_ID));
            //}
            #region Restore valis   

            var info = (Dictionary<string, object>)projInfoFile.getInfo("Valis1");
            if (thisValiInstance != null && info != null)
            {
                foreach (var bindingKey in info.Keys.ToList())
                {
                    Dictionary<string, object> dict = projInfoFile.extractDict(info, bindingKey);
                    string varSTKName = (string)dict["Name"];
                    Int64 bindId = (Int64)dict["Id"];
                    int testRow = int.Parse(dict["Row"].ToString());
                    List<string> parents = projInfoFile.extractList(dict, "Parents");

                    int bindRow = mapNewRow4Binding(varSTKName, testRow, parents);
                    if (bindRow != testRow && UpdateValiIDs.ContainsKey(testRow))
                    {
                        UpdateValiIDs.Remove(testRow);
                    }

                    var bindvaliName = thisValiInstance.allValisbyID[bindId.ToString()].Name;
                    cellAccessfromRow(bindRow);
                    updateVali2Datagrid(bindvaliName, bindRow,BindingAction.ADD);

                    if (UpdateValiIDs.ContainsKey(selectedValiObj.toBindRow))
                    {
                        UpdateValiIDs.Remove(selectedValiObj.toBindRow);
                    }


                    UpdateValiIDs.Add(selectedValiObj.toBindRow, bindId);

                }
            }
            #endregion

            var infoDatasets = (Dictionary<string, object>)projInfoFile.getInfo("Datasets");
            if (thisValiInstance != null && infoDatasets != null)
            {
                foreach (var bindingKey in infoDatasets.Keys)
                {
                    Dictionary<string, object> dict = projInfoFile.extractDict(infoDatasets, bindingKey);
                    BindingItem item = parseBindingDict(dict);

                    m_datasetWin.DatasetBindings.Add(item.bindID, item);
                    m_datasetWin.l_bindings.Add(item);
                    DSBindingsbyID.Add(int.Parse(item.bindID), item);
                    DSBindingGridXAML.ItemsSource = m_datasetWin.l_bindings;
                    DSBindingGridXAML.Items.Refresh();
                    clear_MidPanel();
                  
                }
            }

           

        }



        private void RefreshSTK_Click(object sender, RoutedEventArgs e)
        {
            rowCount = 0;
            ListChildren.Clear();
            GridDict.Clear();
            AccessObject.Clear();
            AccessClassName.Clear();
            parentsHeirarchy.Clear();
            dictObjectTimes.Clear();
            readAllAttributes();
            STKDataXAML.Items.Clear();
            valiTree_XAML.Items.Clear();
            UpdateSTKTree();
            readBindings();
            DPTree.Items.Refresh();
            
        }

        private void openDatasetWin(object sender, RoutedEventArgs e)
        {
        }

        private void selectUnsynced(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < STKDataXAML.Items.Count; i++)
            {
                DataGridCell thisCellContainer = GetCell(STKDataXAML, i, 0, (STKDataXAML.Items[i] as DataGridRow));
                var row = (DataGridRow)STKDataXAML.ItemContainerGenerator.ContainerFromIndex(i);
                DataGridCell valiNameCell = GetCell(STKDataXAML, i, 4, (row));
                if (((TextBlock)valiNameCell.Content).Text != string.Empty)
                {
                    CheckBox thisCheckBox = GetVisualChild<CheckBox>(thisCellContainer);
                    int returnID = CompareUnits(i, false);
                    thisCheckBox.IsChecked = returnID == 1 ? true : false;
                }
            }
        }


        private void Valispace_DataSet_TextChanged(object sender, TextChangedEventArgs e)
        {

        }


        void BindingDelete_Click(object sender, MouseEventArgs e)
        {
            int selectedRowIndex = STKDataXAML.Items.IndexOf(STKDataXAML.CurrentItem);
            var valiName = thisValiInstance.allValisbyID[UpdateValiIDs[selectedRowIndex].ToString()].Name;
            if (UpdateValiIDs.ContainsKey(selectedRowIndex))
            {
                cellAccessfromRow(selectedRowIndex);
                updateVali2Datagrid(valiName, selectedRowIndex, BindingAction.DELETE);
            }
            
        }

        private void BindingReassign_Click(object sender, MouseEventArgs e)
        {
            int selectedRowIndex = STKDataXAML.Items.IndexOf(STKDataXAML.CurrentItem);
            DataGridRow_PreviewMouseDoubleClick(selectedRowIndex, null);
        }
        

    }
    
    static class Extensions
    {
        public static T GetParentOfType<T>(this DependencyObject element) where T : DependencyObject
        {
            Type type = typeof(T);
            if (element == null) return null;
            DependencyObject parent = VisualTreeHelper.GetParent(element);
            if (parent == null && ((FrameworkElement)element).Parent is DependencyObject) parent = ((FrameworkElement)element).Parent;
            if (parent == null) return null;
            else if (parent.GetType() == type || parent.GetType().IsSubclassOf(type)) return parent as T;
            return GetParentOfType<T>(parent);
        }

        public static List<T> Clone<T>(this IList<T> listToClone) where T : ICloneable
        {
            return listToClone.Select(item => (T)item.Clone()).ToList();
        }
    }


    public class uiValiObj
    {
        public int toBindRow { get; set; }
        public TextBlock toBindName { get; set; }
        public TextBlock nameCell { get; set; }
        public TextBlock valueCell { get; set; }
        public TextBlock unitCell { get; set; }
    }

    public struct Project
    {
        public Int64 Id { get; set; }
        public string Name { get; set; }
    }

    public enum BindingAction
    {
        ADD,
        DELETE
    }
    public enum ProjectFLAG
    {
        NEW,
        OLD
    }

}
