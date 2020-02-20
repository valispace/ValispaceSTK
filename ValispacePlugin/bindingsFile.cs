using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Reflection;

namespace ValispacePlugin
{
    public class bindingsFile
    {
        private Dictionary<string, object> projectInfo = new Dictionary<string, object>();
        public Dictionary<string, bool> hasInfo = new Dictionary<string, bool>();
        private FileStream thisFile;
        private string infoFilePath;
        public enum addData { Append, Overwrite, Remove };
        private List<string> validKeys = new List<string>()
        {
            "Project Name",
            "Scenario Name",
            "Last Updated",
            "Valis", // Currently Deprecated - row tied to UI - Not smart
            "Valis1", // New binding style to remove ambiguities for real use
            "Datasets",
            "Cred"
        };

        public bindingsFile(string scenarioPath)
        {
            infoFilePath = scenarioPath + "valiPluginData.txt";
            
            foreach(var field in validKeys)
            {
                if(!hasInfo.ContainsKey(field))
                {
                    hasInfo.Add(field, false);
                }
            }
        }

        private void checkCorrupt()
        {
            readInfofile();
            if (projectInfo.Count==0)
            {
                File.Delete(infoFilePath);
            }
        }
        public object getInfo(string key)
        {
            object value = null;
            if (projectInfo.ContainsKey(key))
            {
                value = projectInfo[key];
            }
            return value;
        }

        public void writeInfo(string key, object value)
        {
            if (validKeys.Contains(key) && key != "Valis")
            {
                projectInfo[key] = value;
            }
            else if (key == "Valis")
            {
                var dict = (Dictionary<int, Int64>)value;
                Dictionary<string, object> newDict = new Dictionary<string, object>();

                foreach( var key0 in dict.Keys)
                {
                    newDict.Add(key0.ToString(), dict[key0]);
                }
            }
            else
            {
                throw new System.ArgumentException("Not Allowed: Invalid Field. Cannot write to this key.");
            }
            write2file();
        }

        public void write2file()
        {
            var str = Protect(JsonConvert.SerializeObject(projectInfo));
            var str1 = Unprotect(str);
            using (StreamWriter file = new StreamWriter(infoFilePath))
            {
                file.WriteLine(str);
                file.Close();
            }

        }

        public void readInfofile()
        {
            if (File.Exists(infoFilePath))
            {
                string fileData = "";
                // Open the stream and read it back.    
                using (StreamReader sr = File.OpenText(infoFilePath))
                {

                    fileData = sr.ReadLine();
                    //while ((s = sr.ReadLine()) != null)
                    //{
                    //}
                }
                if (fileData != null)
                {
                    projectInfo = JsonConvert.DeserializeObject<Dictionary<string, object>>(Unprotect(fileData));
                    if (projectInfo != null)
                    {
                        foreach ( var field in validKeys)
                        {
                            hasInfo[field] = (projectInfo.ContainsKey(field)&& projectInfo[field] != "") ? true : false;
                            if (projectInfo.ContainsKey(field) && projectInfo[field] is Newtonsoft.Json.Linq.JObject)
                            {
                                var dict = extractDict(projectInfo,field);
                                projectInfo[field] = dict;
                                hasInfo[field] = (((Dictionary<string, object>)projectInfo[field]).Count != 0) ? true : false;

                            }

                        }
                }
               
            }
        }

        }

        public Dictionary<string,object> extractDict(Dictionary<string,object> dict, string key)
        {
            var dictReturn = new Dictionary<string,object>();
            if (dict.ContainsKey(key))
            {
                dictReturn = JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonConvert.SerializeObject(dict[key]));
            }
            return dictReturn;
        }

        

        public List<string> extractList(Dictionary<string, object> dict, string key)
        {
            var listReturn = new List<string>();
            if (dict.ContainsKey(key))
            {
                listReturn = JsonConvert.DeserializeObject<List<string>>(JsonConvert.SerializeObject(dict[key]));
            }
            return listReturn;
        }

        public void saveValiBindings(addData addType, object data)
        {
            if (addType == addData.Append)
            {
                //dict.add
            }
            else if (addType == addData.Overwrite)
            {
                projectInfo["Valis"] = data;
            }
        }

        public void saveDSBindings(addData addType, object data)
        {
            if (addType == addData.Append)
            {
                //dict.Add
            }
            else if (addType == addData.Overwrite)
            {
                projectInfo["Datasets"] = data;
            }
        }

        public bool Exists()
        {
            checkCorrupt();
            return (File.Exists(infoFilePath));
        }

        public void Create()
        {
            thisFile = File.Create(infoFilePath);
            thisFile.Close();
        }

        public void Close()
        {
            thisFile.Close();
        }


        // System DataProtection Methods (Encrypt/Decrypt)
        public static string Protect(string str)
        {
            byte[] entropy = Encoding.ASCII.GetBytes(Assembly.GetExecutingAssembly().FullName);
            byte[] data = Encoding.ASCII.GetBytes(str);
            string protectedData = Convert.ToBase64String(ProtectedData.Protect(data, entropy, DataProtectionScope.CurrentUser));
            return protectedData;
        }

        public static string Unprotect(string str)
        {
            byte[] protectedData = Convert.FromBase64String(str);
            byte[] entropy = Encoding.ASCII.GetBytes(Assembly.GetExecutingAssembly().FullName);
            string data = Encoding.ASCII.GetString(ProtectedData.Unprotect(protectedData, entropy, DataProtectionScope.CurrentUser));
            return data;
        }

    }

}


