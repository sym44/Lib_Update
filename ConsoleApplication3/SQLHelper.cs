using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace ConsoleApplication3
{
    class SQLHelper
    {
        public const string STOCK_PRICE_CONNECTION = "Server=localhost\\mssqlserver01;Database=stock_price;uid=sa;pwd=84265";
        public const string FACTOR_CONNECTION = "Server=localhost\\mssqlserver01;Database=factor;uid=sa;pwd=84265";

        /// <summary>
        /// This method takes in a SqlCommand object, and returns a list contains only data ifself.
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns>List of data</double></returns>
        public static List<double> FetchQueryResultToDouble(SqlCommand cmd)
        {
            List<double> dataList = new List<double>();
            using (SqlConnection connection = new SqlConnection(STOCK_PRICE_CONNECTION))
            {
                connection.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Program.stockValueCombo structObj;
                    structObj.value = Convert.ToDouble(dr[2]);                   
                    dataList.Add(structObj.value);
                }
                dr.Close();
                connection.Close();
            }
            return dataList;
        }

        /// <summary>
        /// This method takes in a SqlCommand object, and returns a list contains priceCombo struct.
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns>List of Struct objects</returns>
        public static List<Program.priceCombo> FetchQueryResultToPriceCombo(SqlCommand cmd)
        {
            List<Program.priceCombo> priceList = new List<Program.priceCombo>();
            using (SqlConnection connection = new SqlConnection(STOCK_PRICE_CONNECTION))
            {
                connection.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Program.priceCombo structObj;
                    structObj.s_dq_open = Convert.ToDouble(dr[1]);
                    structObj.s_dq_high = Convert.ToDouble(dr[2]);
                    structObj.s_dq_low = Convert.ToDouble(dr[3]);
                    structObj.s_dq_close = Convert.ToDouble(dr[4]);
                    structObj.s_dq_volume = Convert.ToDouble(dr[5]);
                    priceList.Add(structObj);
                }
                dr.Close();
                connection.Close();
            }
            return null;
        }

        /// <summary>
        /// This method updates the data back into the Result table.
        /// </summary>
        /// <param name="value">the double value</param>
        /// <param name="fieldName">name of the field in the table</param>
        public static void UpdateIntoTable(double value, string fieldName, 
            string s_info_windcode, string trade_dt)
        {
            using (SqlConnection connection = new SqlConnection(STOCK_PRICE_CONNECTION))
            {
                connection.Open();

                SqlCommand sqlCmd = new SqlCommand();
                sqlCmd.CommandType = CommandType.Text;
                sqlCmd.CommandText = "UPDATE Result SET " + fieldName + " = " 
                    + value + " where s_info_windcode = '"
                    + s_info_windcode + "' and trade_dt = '" + trade_dt + "'";
                sqlCmd.ExecuteNonQuery();
            }
        }
    }
}
