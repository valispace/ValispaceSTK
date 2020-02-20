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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ValispacePlugin
{
    public class ValispaceInstance
    {
        public ValispaceAPI valispace;
        public Project selectedProject;
        //private ComboBox projectCombo;
        private TreeView projectTree;
        public Dictionary<string, Vali> allValis= new Dictionary<string, Vali>();
        public Dictionary<string, Vali> allValisbyID = new Dictionary<string, Vali>();

        public bool projLoaded = false;
        //public string tempSelectedValiName;
        
        public ValispaceInstance()
        {
            //valispace = ValispaceAPI.Connect("https://app.valispace.com", "kuldeep", "valispace.")
            
            //MessageBox.Show("Invalid Constructor Invoked. Error in ValispaceInstance.cs");
            
        }
        public ValispaceInstance(ValispaceAPI valiInstance)
        {
            valispace = valiInstance;
        }
        public ValispaceInstance(string ProjectName,TreeView projectTree)
        {
            //valispace = ValispaceAPI.Connect("https://app.valispace.com", "kuldeep", "valispace.");
            try
            {
                valispace = WPFLogin.valispace;
                this.projectTree = projectTree;
                if (valispace == null)
                {
                    return;
                }
            }
            catch
            {
                MessageBox.Show("You are not Logged in ! ", "Warning");
            }
            var projects = valispace.getProjects();

                //var p = new List<Project>();
                foreach (var project in projects)
                {
                    if ((string)project["name"] == ProjectName)
                    {
                        selectedProject = new Project();
                        selectedProject.Name = (string)project["name"];
                        selectedProject.Id = (Int64)project["id"];
                        projLoaded = true;
                        break;
                    }
                    
                }
                if (projLoaded)
                {
                    loadProjectTree(selectedProject);
                }
                else
                {
                    MessageBox.Show("Project with Selected Name doesnt Exist. Please check the project Name entered and try again!");
                }
            
            
        }

        public void update2valispace(Int64 id, string value)
        {
            //Update 2 valispace
            valispace.updateVali(id, ValiFormula: value.ToString());
            Vali valiInDB = allValisbyID[id.ToString()] ;
            valiInDB.Value = double.Parse(value);

        }

        #region Update to Valispace
        #endregion

        #region Valispace: Load Projects
        private void loadProjects()
        {
            
            if (valispace == null)
            {
                return;
            }

            var projects = valispace.getProjects();

            var p = new List<Project>();

            foreach (var project in projects)
            {
                var pr = new Project();
                pr.Name = (string)project["name"];
                pr.Id = (Int64)project["id"];

                p.Add(pr);
            }

            //projectCombo.ItemsSource = p;
            //projectCombo.IsEnabled = true;
        }

        #region Project Tree 
        public void loadProjectTree(Project project)
        {
            var componentById = new Dictionary<Int64, Dictionary<string, object>>();
            var valiById = new Dictionary<Int64, Dictionary<string, object>>();
            var roots = new List<Dictionary<string, object>>();
            // var componentChildren = new Dictionary<Int64, List<Dictionary<string, object>>>();

            Component processComponent(Dictionary<string, object> root)
            {
                var component = new Component()
                {
                    Name = (string)root["name"],
                    Items = new List<object>(),
                };
                Dictionary<string, object> comp_root = valispace.getComponent((Int64)root["id"]);
                if (comp_root.ContainsKey("valis"))
                {
                    //var valis = (Newtonsoft.Json.Linq.JArray)root["valis"];
                    var valis = (Newtonsoft.Json.Linq.JArray)comp_root["valis"];

                    if (valis != null)
                    {
                        foreach (var vali in valis)
                        {
                            var valiId = (Int64)vali;
                            var valiData = valiById[valiId];
                            if ((string)valiData["path"] != "DataSets")
                            {
                                Vali thisVali = new Vali()
                                {
                                    Name = (string)valiData["name"],
                                    Value = (double)valiData["value"],
                                    unit = (string)valiData["unit"],
                                    Id = valiId
                                };
                                component.Items.Add(thisVali);
                                allValis.Add(thisVali.Name, thisVali);
                                allValisbyID.Add(thisVali.Id.ToString(), thisVali);
                            }
                        }
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

                            component.Items.Add(processComponent(componentData));
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
                items.Add(processComponent(root));
            }

            projectTree.ItemsSource = items;
            TreeViewItem item = projectTree.ItemContainerGenerator.ContainerFromIndex(0) as TreeViewItem;
            //item.IsExpanded = true;
            //((TreeViewItem)projectTree.Items[0]).IsExpanded = true;
        }
        #endregion



        #endregion
    }
}
