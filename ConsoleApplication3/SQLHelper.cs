using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace DataElf
{
    class SQLHelper
    {
        public const string SUFFIX_5 = "_Z5D";
        public const string SUFFIX_10 = "_Z10D";
        public const string SUFFIX_20 = "_Z20D";
        public const string SUFFIX_60 = "_Z3M";
        public const string SUFFIX_120 = "_Z6M";
        public const string SUFFIX_250 = "_Z12M";
        public const string STOCK_PRICE_CONNECTION = 
            "Server=localhost\\mssqlserver01;Database=stock_price;uid=sa;pwd=84265";
        public const string FACTOR_CONNECTION = 
            "Server=localhost\\mssqlserver01;Database=factor;uid=sa;pwd=84265";

        /// <summary>
        /// This method takes in a SqlCommand object, and returns a list contains 
        /// only data ifself.
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns>List of data</double></returns>
        public static List<double> FetchQueryResultToDouble(SqlCommand cmd)
        {
            List<double> dataList = new List<double>();
            using (SqlConnection connection = new SqlConnection(STOCK_PRICE_CONNECTION))
            {
                connection.Open();
                cmd.Connection = connection;
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Program.stockValueCombo structObj;
                    if (dr[1].GetType().Name == "DBNull")
                        structObj.value = 0.0;
                    else
                        structObj.value = Convert.ToDouble(dr[1]);           
                    dataList.Add(structObj.value);
                }
                dr.Close();
                connection.Close();
            }
            return dataList;
        }

        /// <summary>
        /// This method takes in a SqlCommand object, and returns a 
        /// list contains priceCombo struct.
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns>List of Struct objects</returns>
        public static List<Program.priceCombo> FetchQueryResultToPriceCombo
            (SqlCommand cmd)
        {
            List<Program.priceCombo> priceList = new List<Program.priceCombo>();
            using (SqlConnection connection = new SqlConnection(STOCK_PRICE_CONNECTION))
            {
                connection.Open();
                cmd.Connection = connection;
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
            return priceList;
        }

        /// <summary>
        /// This method updates the data back into the Result table.
        /// </summary>
        /// <param name="value">the double value</param>
        /// <param name="fieldName">name of the field in the table</param>
        public static void UpdateSingleValueIntoTable(double value, string fieldName, 
            string s_info_windcode, string trade_dt)
        {
            using (SqlConnection connection = new SqlConnection(STOCK_PRICE_CONNECTION))
            {
                connection.Open();

                SqlCommand sqlCmd = new SqlCommand();
                sqlCmd.Connection = connection;
                sqlCmd.CommandType = CommandType.Text;
                sqlCmd.CommandText = "UPDATE Result SET " + fieldName + " = " 
                    + value + " where s_info_windcode = '"
                    + s_info_windcode + "' and trade_dt = '" + trade_dt + "'";

                SqlParameter sqlReturn;
                sqlReturn = sqlCmd.Parameters.Add("@return_value", SqlDbType.Int);
                sqlReturn.Direction = ParameterDirection.ReturnValue;

                sqlCmd.ExecuteNonQuery();

                Console.WriteLine("........................{0}", 
                    sqlReturn.Value.ToString());
                Console.WriteLine("{0}...{1}...{2}...{3}", s_info_windcode, 
                    trade_dt, fieldName, value);

                connection.Close();
            }
        }

        /// <summary>
        /// This method updates the data back into the Result table.
        /// Using a set of values, z5d, z10d, ..., z12m
        /// </summary>
        /// <param name="value"></param>
        /// <param name="fieldName"></param>
        /// <param name="s_info_windcode"></param>
        /// <param name="trade_dt"></param>
        public static void UpdateMultipleValueIntoTable(double[] values, string fieldName,
            string s_info_windcode, string trade_dt)
        {
            using (SqlConnection connection = new SqlConnection(STOCK_PRICE_CONNECTION))
            {
                connection.Open();

                SqlCommand sqlCmd = new SqlCommand();
                sqlCmd.Connection = connection;
                sqlCmd.CommandType = CommandType.Text;
                sqlCmd.CommandText = "UPDATE Result SET " 
                    + fieldName + SUFFIX_5 + " = "+ values[0] + ", "
                    + fieldName + SUFFIX_10 + " = " + values[1] + ", "
                    + fieldName + SUFFIX_20 + " = " + values[2] + ", "
                    + fieldName + SUFFIX_60 + " = " + values[3] + ", "
                    + fieldName + SUFFIX_120 + " = " + values[4] + ", "
                    + fieldName + SUFFIX_250 + " = " + values[5]
                    + " where s_info_windcode = '"
                    + s_info_windcode + "' and trade_dt = '" + trade_dt + "'";

                SqlParameter sqlReturn;
                sqlReturn = sqlCmd.Parameters.Add("@return_value", SqlDbType.Int);
                sqlReturn.Direction = ParameterDirection.ReturnValue;

                sqlCmd.ExecuteNonQuery();

                Console.WriteLine("........................{0}", 
                    sqlReturn.Value.ToString());
                Console.WriteLine("{0}...{1}...{2}...{3}", s_info_windcode, 
                    trade_dt, fieldName + SUFFIX_5, values[0]);
                Console.WriteLine("{0}...{1}...{2}...{3}", s_info_windcode, 
                    trade_dt, fieldName + SUFFIX_10, values[1]);
                Console.WriteLine("{0}...{1}...{2}...{3}", s_info_windcode, 
                    trade_dt, fieldName + SUFFIX_20, values[2]);
                Console.WriteLine("{0}...{1}...{2}...{3}", s_info_windcode, 
                    trade_dt, fieldName + SUFFIX_60, values[3]);
                Console.WriteLine("{0}...{1}...{2}...{3}", s_info_windcode, 
                    trade_dt, fieldName + SUFFIX_120, values[4]);
                Console.WriteLine("{0}...{1}...{2}...{3}", s_info_windcode, 
                    trade_dt, fieldName + SUFFIX_250, values[5]);

                connection.Close();
            }
        }
    }
}
