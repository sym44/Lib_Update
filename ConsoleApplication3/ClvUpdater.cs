using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace ConsoleApplication3
{
    class ClvUpdater
    {
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
                    list[0].s_dq_high, list[0].s_dq_low);



                SQLHelper.UpdateIntoTable(clv, "clv_1", s_info_windcode, trade_dt);
            }
            else
            {
                SqlCommand sqlCmd = new SqlCommand();
                sqlCmd.CommandType = CommandType.Text;
                sqlCmd.CommandText = "select s_info_windcode, clv_1 " 
                    + "from Result where s_info_windcode = '" 
                    + s_info_windcode + "' order by trade_dt desc";
                List<double> dataList = SQLHelper.FetchQueryResultToDouble(sqlCmd);

                clv = AttributeCalculator.MACalculator(dataList, delay);
            }

            
        }
    }
}
