using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace DataElf
{
    class ClvUpdater
    {
        public const string SUFFIX_5 = "Z5D";
        public const string SUFFIX_10 = "Z10D";
        public const string SUFFIX_20 = "Z20D";
        public const string SUFFIX_60 = "Z3M";
        public const string SUFFIX_120 = "Z6M";
        public const string SUFFIX_250 = "Z12M";
        
        /// <summary>
        /// This method includes the whole process of the updating clv.
        /// The main program needs only to call this method for each
        /// stock and the specific date
        /// </summary>
        /// <param name="list"></param> // Needs refractor, need to move the list fetching process into the method
        /// <param name="s_info_windcode"></param>
        /// <param name="trade_dt"></param>
        /// <param name="delay"></param>
        public static void updateClvHelper(List<Program.priceCombo> list, 
            string s_info_windcode, string trade_dt, int delay)
        {
            double clv;

            if (delay == 1)
            {
                clv = AttributeCalculator.ClvCalculator(list[0].s_dq_close, 
                    list[0].s_dq_high, list[0].s_dq_low); // calculate the value

                SQLHelper.UpdateIntoTable(clv, "clv_1", s_info_windcode, trade_dt);
                    // update value
            }
            else
            {
                // fetch value
                SqlCommand sqlCmd = new SqlCommand();
                sqlCmd.CommandType = CommandType.Text;
                sqlCmd.CommandText = "select s_info_windcode, clv_1 " 
                    + "from Result where s_info_windcode = '" 
                    + s_info_windcode + "' order by trade_dt desc";
                List<double> dataList = SQLHelper.FetchQueryResultToDouble(sqlCmd);

                clv = AttributeCalculator.MACalculator(dataList, delay); // calculate the value

                string suffix;
                switch(delay)
                {
                    case 5:
                        suffix = SUFFIX_5; break;
                    case 10:
                        suffix = SUFFIX_10; break;
                    case 20:
                        suffix = SUFFIX_20; break;
                    case 60:
                        suffix = SUFFIX_60; break;
                    case 120:
                        suffix = SUFFIX_120; break;
                    case 250:
                        suffix = SUFFIX_250; break;
                    default:
                        suffix = ""; break;
                }
                SQLHelper.UpdateIntoTable(clv, "clv_" + suffix, s_info_windcode, trade_dt);
            }

            
        }
    }
}
