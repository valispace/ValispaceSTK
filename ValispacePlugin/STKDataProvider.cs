using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AGI.STKObjects;
using AGI.STKUtil;
using AGI.Ui.Application;
using AGI.STKObjects.Astrogator;


namespace ValispacePlugin
{
    
    public class STKDataProvider
    {
        public IAgStkObject m_StkObj;
        public string m_objectPath;
        public IAgDataProviderGroup m_providerGroup;
        public System.Array m_dataArray;

        //private Dictionary<string, string> m_preData;
        private string m_superParent;
        private string m_parent;
        private List<string> m_otherParents= new List<string>();
        private string m_DP2;

        public bool m_Error = false;

        public STKDataProvider(IAgStkObject objectSTK, string child, List<string> parents, bool hasPreData)
        {
            m_DP2 = child;
            m_superParent = parents[parents.Count - 1];
            m_parent = parents[0];
            m_StkObj = objectSTK;
            if (parents.Count > 2)
                for (int i = 1; i < (parents.Count - 2); i++)
                {
                    m_otherParents.Add(parents[i]);
                }
        }

        //public STKDataProvider(IAgStkObject object0,string Name, bool needPreData)
        //{
        //    m_selectedObject = object0;
        //    m_providerGroup = m_selectedObject.DataProviders["Cartesian Velocity"] as IAgDataProviderGroup;
        //    IAgDataProvider CartVel_provider= (m_providerGroup.Group["J2000"] as IAgDataProvider);

        //    ///Cast Appropiate Data Pro
        //    IAgDataPrvTimeVar Velocity = CartVel_provider as IAgDataPrvTimeVar;
        //    Array elem = new object[] { "x" };
        //    IAgDrResult Speed = Velocity.ExecElements("19 May 2019 22:00:00.000", "19 May 2019 22:10:00.000", 60, ref elem );

        //    m_dataSe = Speed.DataSets[0].GetValues();
        //    ///update_Dataset(26667, "Name", speed);
        //    ///Console.WriteLine(CartVel_providers)
        //}

        public System.Array acquireDataset(string startTime, string stopTime, double timeStep)
        {
            IAgDataProvider provider = null;
            if (m_parent != m_superParent)
            {
                m_providerGroup = m_StkObj.DataProviders[m_superParent] as IAgDataProviderGroup;
                provider = (m_providerGroup.Group[m_parent] as IAgDataProvider); //m_parent
            }
            else if (m_parent == m_superParent)
            {
                provider = m_StkObj.DataProviders[m_parent] as IAgDataProvider;
            }
            ///Cast Appropiate Data Pro
            try
            {
                IAgDataPrvTimeVar Velocity = provider as IAgDataPrvTimeVar;
                Array elem = new object[] { m_DP2 };
                IAgDrResult result = Velocity.ExecElements(startTime, stopTime, timeStep, ref elem);
                m_dataArray = result.DataSets[0].GetValues();
            }
            catch
            {
                IAgDataPrvInterval Velocity = provider as IAgDataPrvInterval;
                if (Velocity != null)
                {
                    Array elem = new object[] { m_DP2 };
                    IAgDrResult result = Velocity.ExecElements(startTime, stopTime, ref elem);
                    m_dataArray = result.DataSets[0].GetValues();
                }
                else
                {
                    m_Error = true;
                }
            }
            

            
            return m_dataArray;
        }

        public void configPreData()
        {
            
        }
        public Array getDataSet()
        {
            return m_dataArray as Array;
        }
    }
}
