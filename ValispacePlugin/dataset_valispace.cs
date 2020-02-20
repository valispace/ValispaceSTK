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
    public partial class windowDatasets
    {
        
        
        public Dictionary<string, DataSet> allDataSets = new Dictionary<string, DataSet>();
        public Dictionary<Int64, DataSet> allDataSetsbyID = new Dictionary<Int64, DataSet>();
        
        #region Project Tree 
        public void loadProjectTree(Project project, ValispaceAPI valispace)
        {
            
            var componentById = new Dictionary<Int64, Dictionary<string, object>>();
            var valiById = new Dictionary<Int64, Dictionary<string, object>>();
            var roots = new List<Dictionary<string, object>>();

            Component processDatasetTree(Dictionary<string, object> root)
            {
                var TopItem = new TreeViewItem();
                var component = new Component()
                {
                    Name = (string)root["name"],
                    Id = (Int64)root["id"],
                    Items = new List<object>(),
                };
                TopItem.Header = component.Name;
                Dictionary<string, object> comp_root = valispace.getComponent(component.Id);
                if (comp_root.ContainsKey("valis"))
                {
                    var valis = (Newtonsoft.Json.Linq.JArray)comp_root["valis"];
                    bool containsDS = false;
                    if (valis != null)
                    {
                        foreach (var vali in valis)
                        {
                            var valiId = (Int64)vali;
                            var valiData = valiById[valiId];
                            if ((string)valiData["path"] == "DataSets")
                            {
                                DataSet thisDataSet = new DataSet()
                                {
                                    Name = (string)valiData["name"],
                                    parent = component,
                                    //dataset_ID = (Int64)valiData["datasets"],
                                    vali_ID = valiId
                                };
                                TreeViewItem itemVali = new TreeViewItem();
                                itemVali.Header = thisDataSet.Name;
                                
                                containsDS = true;
                                TopItem.Items.Add(itemVali);
                                if (!allDataSets.ContainsKey(thisDataSet.Name))
                                {
                                    allDataSets.Add(thisDataSet.Name, thisDataSet);
                                    allDataSetsbyID.Add(thisDataSet.vali_ID, thisDataSet);
                                }
                            }
                        }
                    }
                    if (containsDS)
                    {
                        m_ValiDSTree.Items.Add(TopItem);
                    }
                }

                if (root.ContainsKey("children"))
                {
                    var children = (Newtonsoft.Json.Linq.JArray)root["children"];

                    if (children != null)
                    {
                        foreach (var child in children)
                        {
                            var componentId = (Int64)child;
                            var componentData = componentById[componentId];

                            component.Items.Add(processDatasetTree(componentData));
                        }
                    }
                }



                return component;
            }

            var projectComponents = valispace.getComponents(project.Id,project.Name);
            var projectValis = valispace.getValis(project.Id);

            foreach (var vali in projectValis)
            {
                valiById.Add((Int64)vali["id"], vali);

            }

            var items = new List<Component>();

            foreach (var component in projectComponents)
            {
                if (component["parent"] == null)
                {
                    roots.Add(component);
                }
                else
                {
                    componentById.Add((Int64)component["id"], component);
                }
            }



            foreach (var root in roots)
            {
                items.Add(processDatasetTree(root));
            }

            //valiTree_XAML.Items.Add(items);
            //DataSet_Tree.ItemsSource = items;
            //TreeViewItem item = DataSet_Tree.ItemContainerGenerator.ContainerFromIndex(0) as TreeViewItem;
            //item.IsExpanded = true;
            //((TreeViewItem)projectTree.Items[0]).IsExpanded = true;
        }
        #endregion

    }
}
