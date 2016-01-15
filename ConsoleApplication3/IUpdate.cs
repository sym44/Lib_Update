using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataElf
{
    interface IUpdate
    {

        /// <summary>
        /// updates the first value of the attribute
        /// </summary>
        void updateBaseValue(string s_info_windcode, string trade_dt);
        
        /// <summary>
        /// updates the following moving average values
        /// </summary>
        void updateDerivedValue(string s_info_windcode, string trade_dt);
    }
}
