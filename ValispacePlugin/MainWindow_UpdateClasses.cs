using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using AGI.STKObjects;
using AGI.STKUtil;
using AGI.Ui.Application;
using AGI.STKObjects.Astrogator;


namespace ValispacePlugin
{
    public partial class MainWindow
    {

        /// <summary>
        /// Data Updates to STK/Valispace
        /// </summary>

        #region Valispace Update Sequences
        private void ValispaceUpdate(object sender, RoutedEventArgs e)
        {
            bool updated = false;

            for (int i = 0; i < STKDataXAML.Items.Count; i++)
            {
                DataGridCell thisCellContainer = GetCell(STKDataXAML, i, 0, (STKDataXAML.Items[i] as DataGridRow));
                CheckBox thisCheckBox = GetVisualChild<CheckBox>(thisCellContainer);

                bool? IsSelected = thisCheckBox.IsChecked;
                if ((bool)IsSelected)
                {
                    var ValueSTK2Valispace = decimal.Parse(GridDict[i].m_Value.ToString("R"),System.Globalization.NumberStyles.Float).ToString();
                    var valiIDtoUpdate = UpdateValiIDs[i];
                    thisValiInstance.update2valispace(valiIDtoUpdate, ValueSTK2Valispace);
                    updated = true;

                }
            }
            if (updated)
            {
                MessageBox.Show("Values have been updated to valispace project. \n If Changes are not reflected in the valispace browser window, refresh the page!");
                updateTreeNow(FLAG:ProjectFLAG.NEW);
                readBindings();
            }
        }
        
        #endregion

        #region STK Update Sequences

        private void UpdateSTK_Click(object sender, RoutedEventArgs e)
        {
            bool wasAnyUpdated = false; //Updated anything ? (for message box)
            bool noWarning = true; //No warning was produced? = true [else] false ( message box)
            for (int i = 0; i < STKDataXAML.Items.Count; i++)
            {

                DataGridCell thisCellContainer = GetCell(STKDataXAML, i, 0, (STKDataXAML.Items[i] as DataGridRow));
                CheckBox thisCheckBox = GetVisualChild<CheckBox>(thisCellContainer);

                DataGridCell ValiNameContainer = GetCell(STKDataXAML, i, 4, (STKDataXAML.Items[i] as DataGridRow));
                //DataGridCell ValiUnitContainer = GetCell(STKDataXAML, i, 6, (STKDataXAML.Items[i] as DataGridRow));

                DataGridCell STKNameContainer = GetCell(STKDataXAML, i, 1, (STKDataXAML.Items[i] as DataGridRow));
                DataGridCell STKValueContainer = GetCell(STKDataXAML, i, 2, (STKDataXAML.Items[i] as DataGridRow));
                DataGridCell STKUnitContainer = GetCell(STKDataXAML, i, 3, (STKDataXAML.Items[i] as DataGridRow));

                bool? IsSelected = thisCheckBox.IsChecked; //((CheckBox)thisCellContainer.Content)
                try
                {
                    var BindedValiName = ((TextBlock)ValiNameContainer.Content).Text;
                    var BindedSTKName = ((TextBlock)STKNameContainer.Content).Text;
                    if ((bool)IsSelected && BindedValiName != null)
                    {
                        if (CompareUnits(i, false) != 2)
                        {
                            var ValueVali2STK = (thisValiInstance.allValis[BindedValiName]).Value;

                            var Updated = invoke_STKUpdate(i, BindedSTKName, ValueVali2STK);
                            if (Updated == 1)
                            {
                                ((TextBlock)STKValueContainer.Content).Text = ValueVali2STK.ToString();
                                CompareUnits(i, true);
                                wasAnyUpdated = true;
                            }
                        }
                        else
                        {
                            string warning = " CANNOT UPDATE : \" " + BindedSTKName + " - " + BindedValiName + " \"  \n Binding has incompatible units. \n Fix: Change units in Valispace/STK or check the binding.";
                            MessageBoxResult result = MessageBox.Show(warning, "Warning");
                            noWarning = false;
                        }
                    }
                }
                catch
                {

                }

            }

            if (wasAnyUpdated && noWarning)
            {
                MessageBoxResult result = MessageBox.Show("Selected Values have been updated to your STK Scenario", "Update Status");

            }
            else if (noWarning)
            {
                MessageBoxResult result = MessageBox.Show("No Items Selected for Update.", "Update Status");
            }
        }
   

        public int invoke_STKUpdate(int index, string STKVarName, double Value)
        {
            var returnID = 0;
            if (AccessClassName[index] == implem_Classes.Satellite_OrbitData)
            {
                Satellite_OrbitData toUpdate = new Satellite_OrbitData(AccessObject[index] as IAgStkObject);
                toUpdate.set_InitStateJx((AccessObject[index]), STKVarName, Value);
                returnID = 1;
            }
            if (AccessClassName[index] == implem_Classes.Satellite_MassData)
            {
                Satellite_MassData toUpdate = new Satellite_MassData();
                toUpdate.set_MassProp((AccessObject[index]), STKVarName, Value);
                returnID = 1;
            }
            if (AccessClassName[index] == implem_Classes.Astg_InitState)
            {
                Astg_SegData toUpdate = new Astg_SegData();
                toUpdate.set_InitialState((AccessObject[index]), STKVarName, Value);
                returnID = 1;
            }
            if (AccessClassName[index] == implem_Classes.Astg_SC_InitParams)
            {
                Astg_SegData toUpdate = new Astg_SegData();
                toUpdate.set_SpacecraftParams((AccessObject[index]), STKVarName, Value);
                returnID = 1;
            }
            if (AccessClassName[index] == implem_Classes.Astg_FuelTankParams)
            {
                Astg_SegData toUpdate = new Astg_SegData();
                toUpdate.set_FuelTankParams((AccessObject[index]), STKVarName, Value);
                returnID = 1;
            }
            return returnID;
        }
        #endregion


        #region Map Bindings dynamically

        private int mapNewRow4Binding(string Name, int row, List<string> parents)
        {
            int returnRow = new int();
            if(GridDict[row].m_ObjName == Name && parentsHeirarchy[row].SequenceEqual(parents))
            {
                returnRow = row;
            }
            else
            {
                foreach (var rowNo in GridDict.Keys)
                {
                    if (GridDict[rowNo].m_ObjName==Name && parentsHeirarchy[rowNo].SequenceEqual(parents))
                    {
                        returnRow = rowNo;
                        break;
                    }
                }
            }

            return returnRow;
        }
        #endregion
    }
}
