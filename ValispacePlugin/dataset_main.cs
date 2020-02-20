using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using AGI.STKUtil;
using AGI.STKObjects;
using System.Xml;
using System.IO;

namespace ValispacePlugin
{
    public partial class MainWindow
    {
        public static BindingItem defaultbind = new BindingItem();
        private List<string> thisDP1ParentList;
        private List<string> thisDP2ParentList = new List<string>();
        private int buttonClick_selectColumn;
        private Dictionary<int, BindingItem> DSBindingsbyID = new Dictionary<int, BindingItem>();
        

        private void DPColumn1_Click(object sender, RoutedEventArgs e)
        {
            DPTree.IsEnabled = true;
            MidPanel_Grid.IsEnabled = false;
            buttonClick_selectColumn = 1;
        }

        private void DPColumn2_Click(object sender, RoutedEventArgs e)
        {
            DPTree.IsEnabled = true;
            MidPanel_Grid.IsEnabled = false;
            buttonClick_selectColumn = 2;
            STKDP_Col2.IsEnabled = true;
        }

        private void STKDP_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount >= 2)
            {
                try
                {
                    if (!(DPTree.SelectedItem as TreeViewItem).HasItems)
                    {
                        if (buttonClick_selectColumn == 1)
                        {
                            STKDP_Col1.Text = (e.OriginalSource as TextBlock).Text;
                            thisDP1ParentList = getParentList(DPTree.SelectedItem as DependencyObject);
                            DPTree.IsEnabled = false;
                        }
                        else if (buttonClick_selectColumn == 2)
                        {
                            STKDP_Col2.Text = (e.OriginalSource as TextBlock).Text;
                            thisDP2ParentList = getParentList(DPTree.SelectedItem as DependencyObject);
                            DPTree.IsEnabled = false;
                        }
                        else if (buttonClick_selectColumn == 3)
                        {
                            valispace_DataSet.Text = (e.OriginalSource as TextBlock).Text;
                            valiTree_XAML.IsEnabled = false;
                        }

                        MidPanel_Grid.IsEnabled = true;
                    }
                }
                catch { }


            }



        }

        private void valispaceSelect_click(object sender, RoutedEventArgs e)
        {
            MidPanel_Grid.IsEnabled = false;
            valiTree_XAML.IsEnabled = true;
            buttonClick_selectColumn = 3;
        }

        private void Bind_Click(object sender, RoutedEventArgs e)
        {
            var nextID = m_datasetWin.l_bindings.Count + 1;
            try
            {
               nextID =  int.Parse(((BindingItem)DSBindingsbyID.Values.Last()).bindID) +1;
            }
            catch
            {
                //No Bindings exist exception
            }
            if (thisValiInstance != null && STKDP_Col2.Text != string.Empty)
            {
                var value = "";

                var needsPreData1 = m_datasetWin.hasPreData.TryGetValue((thisDP2ParentList[thisDP2ParentList.Count - 1]), out value);
                BindingItem item = new BindingItem
                {
                    bindID = nextID.ToString(),
                    StkDP_Col1 = STKDP_Col1.Text,
                    StkDP_Col2 = STKDP_Col2.Text,
                    //StkDP1_fullName = thisDP1ParentList[thisDP2ParentList.Count - 1] + "." + STKDP_Col2.Text,
                    DP2_FullName = thisDP2ParentList[0] + "." + STKDP_Col2.Text,
                    StartTime = StartTimeBoxXAML.Text,
                    StopTime = StopTimeBoxXAML.Text,
                    timeStep = StepBoxXAML.Text,
                    valiDataset = valispace_DataSet.Text,
                    needsPreData = needsPreData1,
                    DP1parents = thisDP1ParentList,
                    DP2parents = thisDP2ParentList,
                    STKObjectName = ChildrenDropdown.Text,
                };
                if (valispace_DataSet.Text == string.Empty)
                {
                    MessageBox.Show("Valispace Dataset not Selected");
                }
                else
                {
                    var datasetObj = m_datasetWin.allDataSets[item.valiDataset];
                    item.valiDataset_ID =datasetObj.vali_ID;
                    item.parentComponent = datasetObj.parent;
                    //thisBinding.dataUpdate(nextID, STKDP_Col1.Text, STKDP_Col2.Text, valispace_DataSet.Text);item
                    if (item.StkDP_Col2 != defaultbind.StkDP_Col2 && item.valiDataset != defaultbind.valiDataset && item.STKObjectName != string.Empty)
                    {

                        if (item.needsPreData)
                        {
                            MessageBox.Show("Not Available: We are adding support for this kind of datasets soon");
                        }
                        else
                        {
                            var index = ChildrenNameSTK.IndexOf(item.STKObjectName);
                            STKDataProvider dataSet = new STKDataProvider(allChildren[index], item.StkDP_Col2, thisDP2ParentList, item.needsPreData);
                            var timestep_double = double.Parse(item.timeStep, System.Globalization.CultureInfo.InvariantCulture);
                            var responseArray = dataSet.acquireDataset(m_datasetWin.m_objTimes[m_datasetWin.m_objIndexInSTK][0], m_datasetWin.m_objTimes[m_datasetWin.m_objIndexInSTK][1], timestep_double);
                            if (dataSet.m_Error)
                            {
                                MessageBox.Show("Not Available");
                            }
                            else
                            {
                                m_datasetWin.DatasetBindings.Add(item.bindID, item);
                                m_datasetWin.l_bindings.Add(item);
                                DSBindingsbyID.Add(int.Parse(item.bindID), item);
                                DSBindingGridXAML.ItemsSource = m_datasetWin.l_bindings;
                                DSBindingGridXAML.Items.Refresh();
                                clear_MidPanel();
                            }
                        }
                    }
                    else
                    {
                        var msg = "";
                        if (item.StkDP_Col2 == defaultbind.StkDP_Col2)
                        { msg = msg + "Second STK Dataprovider"; }
                        if (item.valiDataset == defaultbind.valiDataset)
                        {
                            if (msg != "") { msg = msg + ", "; }
                            msg = msg + "Valispace Dataset";
                        }
                        if (ChildrenDropdown.Text == string.Empty)
                        {
                            if (msg != "") { msg = msg + ", "; }
                            msg = msg + "STK Object";
                        }
                        if (msg != "")
                        {
                            msg = "(Error) Entries Not Selected : " + msg;
                            MessageBox.Show(msg);
                        }

                    }
                }
            }

            else
            {
                MessageBox.Show("Load The Valispace Project");
            }
            /////////////////////////////////

            //generateSTKDAtaset(child, parent);
            // /Hard coded Dataset Exchange

            //var Name = STKDP_Col2.Text;
            ////string needPreData;
            //thisDP2ParentList.Add("J2000"); thisDP2ParentList.Add("Cartesian Velocity");
            //var boolPreData = false;
            //STKDataProvider dataSet = new STKDataProvider(m_datasetWin.m_selectedObject, "y", thisDP2ParentList, boolPreData);
            ////var DP1 = new DataProvider(m_selectedObject, Name, (hasPreData.TryGetValue(Name, out needPreData)));
            //var speed0 = dataSet.acquireDataset(m_datasetWin.m_objTimes[m_datasetWin.m_objIndexInSTK][0], m_datasetWin.m_objTimes[m_datasetWin.m_objIndexInSTK][0] + 60, 10) as System.Array;
            //var speed = preProcessDataset(speed0) as Array;
            ////var speed = DP1.getDataSet();
            //thisValiInstance.valispace.update_Dataset(27095, "Name", speed);
        }


        private void GenUpd_Btn_click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < DSBindingGridXAML.Items.Count - 1; i++)
            {
                DataGridCell thisCellContainer = GetCell(DSBindingGridXAML, i, 0, (DSBindingGridXAML.Items[i] as DataGridRow));
                var row = (DataGridRow)DSBindingGridXAML.ItemContainerGenerator.ContainerFromIndex(i);
                DataGridCell DSbindIDcell = GetCell(DSBindingGridXAML, i, 1, (row));
                CheckBox thisCheckBox = GetVisualChild<CheckBox>(thisCellContainer);
                if ((bool)thisCheckBox.IsChecked)
                {
                    var id = int.Parse(((TextBlock)DSbindIDcell.Content).Text);
                    generateUpdate(DSBindingsbyID[id]);
                }

            }
        }


        public void generateUpdate(BindingItem item)
        {
            var STKObjName = item.STKObjectName;
            IAgStkObject selectedObject = allChildren[ChildrenNameSTK.IndexOf(STKObjName)];
            STKDataProvider dataSet = new STKDataProvider(selectedObject, item.StkDP_Col2, thisDP2ParentList, item.needsPreData);
            var timestep_double = double.Parse(item.timeStep, System.Globalization.CultureInfo.InvariantCulture);
            var responseArray = dataSet.acquireDataset(m_datasetWin.m_objTimes[m_datasetWin.m_objIndexInSTK][0], m_datasetWin.m_objTimes[m_datasetWin.m_objIndexInSTK][1], timestep_double);
            
            if (dataSet.m_Error)
            {
                MessageBox.Show("Not Available");
            }
            else
            {
                var processedDataset = preProcessDataset(responseArray);
                var timeElapsed = defaultTimeArray(responseArray.Length, timestep_double);
                try
                {
                    thisValiInstance.valispace.update_Dataset(item.valiDataset_ID, timeElapsed, processedDataset);
                    MessageBox.Show("Selected datasets pushed to the Valispace project!");
                    item.LastUpdated = getTimeNow();
                }
                catch(Exception ex)
                { MessageBox.Show(ex.ToString()); }
            }
        }

        private Array defaultTimeArray(int length, double timeStep)
        {
            Array time = new Array[length];
            List<double> list = new List<double>();

            for (int i = 0; i < length; i++)
            {
                list.Add((double)i * timeStep);
            }
            time = list.ToArray();
            return time;
        }
        private Array preProcessDataset(Array thisArray)
        {
            for (int i = 0; i < thisArray.Length; i++)
            {

                var Obj = thisArray.GetValue(i);
                var value0 = Convert.ToDouble(Obj, null);
                var value = decimal.Parse(value0.ToString("R"), System.Globalization.NumberStyles.Float);

                thisArray.SetValue(value, i);
            }

            return thisArray;
        }

        private void clear_MidPanel()
        {
            STKDP_Col1.Text = defaultbind.StkDP_Col1;
            STKDP_Col2.Text = defaultbind.StkDP_Col2;
            valispace_DataSet.Text = defaultbind.valiDataset;
        }

        private void deleteBinding_Click(object sender, RoutedEventArgs e)
        {

        }
        private void editBinding_Click(object sender, RoutedEventArgs e)
        {

        }

        private List<string> getParentList(DependencyObject child)
        {
            // 0 - immediate parent 
            // end - super parent
            var parents = new List<string>();
            var thisparent = FindParentOfType<TreeViewItem>(child);
            do
            {
                parents.Add(thisparent.Header.ToString());
                child = thisparent;
                thisparent = FindParentOfType<TreeViewItem>(child);
            }
            while (thisparent != null);
            return parents;
        }

        public static T FindParentOfType<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentDepObj = child;
            do
            {
                parentDepObj = VisualTreeHelper.GetParent(parentDepObj);
                T parent = parentDepObj as T;
                if (parent != null) return parent;
            }
            while (parentDepObj != null);
            return null;
        }

        private void UseObjectTime_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StopTimeBoxXAML.IsEnabled = false;
                StartTimeBoxXAML.IsEnabled = false;
            }
            catch { }
        }

        private void SpecifyTime_Click(object sender, RoutedEventArgs e)
        {
            StopTimeBoxXAML.IsEnabled = true;
            StartTimeBoxXAML.IsEnabled = true;
        }

        public Dictionary<string,object> makeBindingDictionary()
        {
            Dictionary<string, object> BindingDict = new Dictionary<string, object>();
            foreach (var bindingItem in m_datasetWin.l_bindings)
            {
                BindingDict.Add(bindingItem.bindID, bindingItem);
            }
            return BindingDict;
        }

        public BindingItem parseBindingDict(Dictionary<string,object> dict)
        {
            BindingItem item = new BindingItem()
            {
                bindID = (string)dict["bindID"],
                StkDP_Col1 = (string)dict["StkDP_Col1"],
                StkDP_Col2 = (string)dict["StkDP_Col2"],
                valiDataset = (string)dict["valiDataset"],
                StartTime = (string)dict["StartTime"],
                StopTime = (string)dict["StopTime"],
                timeStep = (string)dict["timeStep"],
                needsPreData = (bool)dict["needsPreData"],
                STKObjectName = (string)dict["STKObjectName"],
                DP2_FullName = (string)dict["DP2_FullName"],
                LastUpdated = (string)dict["LastUpdated"],
                valiDataset_ID = (Int64)dict["valiDataset_ID"],
                DP1parents = projInfoFile.extractList(dict, "DP1parents"),
                DP2parents = projInfoFile.extractList(dict, "DP2parents"),
                parentComponent = parseCompFromDict(projInfoFile.extractDict(dict, "parentComponent"))
                           
            };
            
            return item;
        }

        
        private Component parseCompFromDict( Dictionary<string,object> dict)
        {
            Component comp = new Component()
            {
                Id = (Int64)dict["Id"],
                Name = (string)dict["Name"],
                Items = null //projInfoFile.extractList(dict,"Items")

            };
            return comp;
        }

        public void DatasetBindProp_Show(object sender, RoutedEventArgs e)
        {
            var item = (e.Source as MenuItem).DataContext;

            if (item is BindingItem)
            {
                var thisBinding = item as BindingItem;
                string STKHeader = "STK Dataset : \n";
                string DP1 = "Column 1 (default):   Time (Elapsed Time in seconds) \n";
                string DP2 = "Column 2:  " + thisBinding.DP2_FullName + "\n";
                string parentHeirarchy1 = "Dataprovider (column 1) Parents:     " + "N/A" + "\n";
                string parentHeirarchy2 = "Dataprovider (column 2) Parents:     "+parseParents(thisBinding.DP2parents)+"\n \n";

                string time = "Time Interval : " + thisBinding.StartTime + " - " + thisBinding.StopTime + " (UTC) \n";
                string timeStep = "Time Step : " + thisBinding.timeStep + " seconds \n \n";

                string inter = "binded to \n \n";
                string ValiHeader = "Valispace Dataset: \n";
                string DatasetName= "Name:  " + thisBinding.valiDataset + " (ID: " + thisBinding.valiDataset_ID + ")\n";
                string valiparent = "Parent Component:  " + thisBinding.parentComponent.Name + " (ID: " + thisBinding.parentComponent.Id + ") \n \n";

                string lastUpdate = "Last Updated:   " + thisBinding.LastUpdated;

                string message = STKHeader+DP1 + DP2 + parentHeirarchy1 + parentHeirarchy2 + time + timeStep + inter + ValiHeader+DatasetName + valiparent+lastUpdate;
                MessageBox.Show(message);
            }
            
        }

        public string parseParents(List<string> parentList)
        {
            string response = string.Empty;

            for (var i = parentList.Count-1; i >= 0; i-- )
            {
                response += parentList[i];
                if (parentList.Last<string>() != parentList[0]) { response += "."; }
            }
            return response;
        }


        private void RowContDelete_Click(object sender, RoutedEventArgs e)
        {
            var d = MessageBox.Show("Are you sure you want to delete this binding ?", "Warning: Delete Dataset Binding", MessageBoxButton.YesNo);
            var thisItem = (e.Source as MenuItem).DataContext;
            if (d == MessageBoxResult.Yes && thisItem is BindingItem)
            {
                var item = thisItem as BindingItem;
                m_datasetWin.DatasetBindings.Remove(item.bindID);
                m_datasetWin.l_bindings.Remove(item);
                DSBindingsbyID.Remove(int.Parse(item.bindID));
                DSBindingGridXAML.ItemsSource = m_datasetWin.l_bindings;
                DSBindingGridXAML.Items.Refresh();
            }
        }
    }

  
        /// <summary>
        /// Interaction logic for windowDatasets.xaml
        /// </summary>
        public partial class windowDatasets
        {
            private IAgScenario m_scenario;
            public IAgDataProviderCollection m_DProot;
            public IAgStkObject m_selectedObject;
            public XmlDocument m_DPXML = new XmlDocument();
            public List<BindingItem> l_bindings = new List<BindingItem>();
            public ValispaceInstance m_valiInstance = new ValispaceInstance();
            public static BindingItem defaultbind = new BindingItem();
            public List<string> thisDP2ParentList = new List<string>();
            public Dictionary<int, List<string>> m_objTimes;
            public static string m_defaultStartTime;
            public static string m_defaultStopTime;
            public int m_objIndexInSTK;
            public Dictionary<string, string> hasPreData = new Dictionary<string, string>();
            public Dictionary<string, object> DatasetBindings = new Dictionary<string, object>();

            //public TreeView DPTree ;
            public TreeView m_ValiDSTree;
            public TextBox m_StartTimeBox;
            public TextBox m_StopTimeBox;
            public TreeView m_DPTree;
         
            public windowDatasets(windowDatasets win)
            { }

        public windowDatasets(IAgStkObject scenario, Dictionary<int, List<string>> objTimes, ValispaceInstance valispace, TextBox StartTime, TextBox StopTime, TreeView DPTree, TreeView ValiDSTree)
        {
            /// Change this to UI selection
            m_objIndexInSTK = 1; // First object selected by default
            m_selectedObject = scenario.Children[m_objIndexInSTK];

            m_objTimes = objTimes;
            IAgDataProviderCollection DP_root = m_selectedObject.DataProviders as IAgDataProviderCollection;
            m_DProot = DP_root;
            m_scenario = scenario as IAgScenario;
            m_defaultStartTime = m_objTimes[m_objIndexInSTK][0]; ;
            m_defaultStopTime = m_objTimes[m_objIndexInSTK][1];
            m_objTimes = objTimes;

            m_StartTimeBox = StartTime;
            m_StopTimeBox = StopTime;
            m_ValiDSTree = ValiDSTree;
            m_DPTree = DPTree;
            getAllDPs();
        }
        

        public void loadValispace(ValispaceInstance valispaceInstance)
        {

            m_valiInstance = valispaceInstance;
            loadProjectTree(m_valiInstance.selectedProject, m_valiInstance.valispace);
        }

            //public void loadWindow()
            //{
            //    //InitializeComponent();
            //    //loadProjectTree(m_valiInstance.selectedProject, m_valiInstance.valispace);
            //    //this.DataContext = this;
            //    //if(DPTree_prev == null)
            //    //{
            //    //    getAllDPs();
            //    //}
            //    //else
            //    //{
            //    //    DPTree = DPTree_prev;
            //    //}

            //    // this.Show();
            //}

            public void getAllDPs()
            {
                m_StartTimeBox.Text = m_defaultStartTime;
                m_StopTimeBox.Text = m_defaultStopTime;
                var str = m_DProot.GetSchema();
                //File.WriteAllText("XMLFile1.xml", str);
                XmlDocument xml = new XmlDocument();
                m_DPXML.LoadXml(str);
                XmlNodeList xnList = m_DPXML.SelectNodes("/DataProviderCollection/DataPrv");

                foreach (XmlNode node in xnList)
                {
                    var ParentName = node.Attributes["Desc"].Value;
                    var item = new TreeViewItem();
                    item.Header = ParentName;
                    if (node.HasChildNodes == true && node.FirstChild.Name == "Element")
                    {
                        processOuterXML(item, node.OuterXml, false);
                    }
                    else if (node.HasChildNodes == true && (node.FirstChild.Name == "DataPrvGroup" || node.FirstChild.Name == "PreData"))
                    {
                        if (node.FirstChild.Name == "PreData")
                        {
                            var pre = node.FirstChild.Attributes["Desc"].Value;
                            hasPreData.Add(ParentName, pre);

                        }
                        processOuterXML(item, node.OuterXml, true);
                    }
                    m_DPTree.Items.Add(item);

                }
                

                int count = 0;
                ///Count all nodes
                foreach (TreeViewItem item in m_DPTree.Items)
                {
                    count += total_items(item);
                }
                //STKDP_Count.Text = " ( Count: " + count.ToString() + ")";
            }

            public int total_items(TreeViewItem treeItem)
            {
                var count = 0;

                foreach (TreeViewItem item in treeItem.Items)
                {
                    if (item.HasItems)
                    {
                        count += total_items(item);
                    }
                    else { count++; }
                }
                return count;

            }
            private void processOuterXML(TreeViewItem parentitem, string xmlDoc, bool isGroup)
            {
                XmlDocument thisXML = new XmlDocument();
                thisXML.LoadXml(xmlDoc);

                XmlNodeList Child_nodes; ;

                if (isGroup)
                {
                    Child_nodes = thisXML.SelectNodes("/DataPrv/DataPrvGroup/DataPrv");
                    if (Child_nodes.Count == 0)
                    {
                        Child_nodes = thisXML.SelectNodes("/DataPrv/Element");
                    }
                }
                else
                {
                    Child_nodes = thisXML.SelectNodes("/DataPrv/Element");
                }
                foreach (XmlNode node in Child_nodes)
                {
                    var item = new TreeViewItem();
                    if (node.Attributes.Count != 0)
                    {
                        if (node.Attributes["Desc"] != null)
                        { item.Header = node.Attributes["Desc"].Value; }
                        else { item.Header = node.Attributes["Name"].Value; }
                        parentitem.Items.Add(item);
                    }
                    if (node.HasChildNodes)
                    {
                        processOuterXML(item, node.OuterXml, (node.Name == "DataPrv") ? false : true);
                    }

                }

            }



        }

        public class BindingItem
        {
            public BindingItem()
            {
                StkDP_Col1 = "Time (default)";
                StkDP_Col2 = "Select...";
                valiDataset = "";
            }
            public string bindID { get; set; }
            public string StkDP_Col1 { get; set; }
            public string StkDP_Col2 { get; set; }
            public string valiDataset { get; set; }
            public Component parentComponent { get; set; }
            public Int64 valiDataset_ID { get; set; }
            public List<string> DP1parents { get; set; }
            public List<string> DP2parents { get; set; }
            public string StartTime { get; set; }
            public string StopTime { get; set; }
            public string timeStep { get; set; }
            public bool needsPreData { get; set; }
            public string STKObjectName { get; set; }
            public string DP2_FullName { get; set; }
            public string LastUpdated { get; set; }
        
            //public void dataUpdate(int id, string col1, string col2, string valiDSName)
            //{
            //    bindID = id.ToString();
            //    StkDP_Col1 = col1;
            //    StkDP_Col2 = col2;
            //    valiDataset = valiDSName;
            //}
        }
    }

