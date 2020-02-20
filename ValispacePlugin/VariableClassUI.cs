using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValispacePlugin
{
    #region UI Output Variable Class
    // Output UI Item Class 
    public class Variable
    {
        public string VarName { get; set; }
        public string VarValue { get; set; }
        public string VarUnit { get; set; }
        public string isBold { get; set; } // 0 - basechild (value element)  1-parent comp 2-subcomp 3-subsubcomp 4- subsubsubcomp
        public bool isChild { get; set; }
    }
    #endregion

   
}
