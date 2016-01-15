using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataElf
{
    class Program
    {
        static ArrayList stockList = new ArrayList(2000);
        static List<priceCombo> dataList = new List<priceCombo>();

        #region definition
        public struct priceCombo
        {
            public double s_dq_open;
            public double s_dq_high;
            public double s_dq_low;
            public double s_dq_close;
            public double s_dq_volume;
        }

        public struct stockValueCombo
        {
            public string stock;
            public string trade_dt;
            public double value;
        }
        #endregion

        static void Main(string[] args)
        {
            read_stock("20150105");


            FetchData("20150105", "000001.SZ");
            ClvUpdater.updateClvHelper(dataList, "000001.SZ", "20150105", 1);
            //check new stocks

            //update_prices("20150105");


            //update_CLV 


            Console.ReadKey();
        }

        static void read_stock(string trade_dt)
        {
            string con;
            con = "Server=localhost\\mssqlserver01;Database=factor;uid=sa;pwd=84265";

            using (SqlConnection connection = new SqlConnection(con))
            {
                connection.Open();

                // check if the date has already been inserted
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = connection;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "select s_info_windcode from windDB.dbo.ashareeodprices where trade_dt = '"
                    + trade_dt + "' order by s_info_windcode";
                SqlDataReader dr = cmd.ExecuteReader();

                while(dr.Read())
                {
                    stockList.Add(dr[0]);
                }
                dr.Close();

                Console.WriteLine(stockList.Count);

                connection.Close();
            }
        }

        static void update_prices(string trade_dt)
        {
            string con;
            con = "Server=localhost\\mssqlserver01;Database=stock_price;uid=sa;pwd=84265";

            using (SqlConnection connection = new SqlConnection(con))
            {
                connection.Open();

                SqlCommand sqlCmd = new SqlCommand("sp_update_prices", connection);
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.CommandTimeout = 0;

                SqlParameter sqlPar_start, sqlPar_end, sqlReturn;
                sqlPar_start = sqlCmd.Parameters.Add("@startDate", SqlDbType.NVarChar);
                sqlPar_start.Direction = ParameterDirection.Input;
                sqlPar_start.Value = trade_dt;

                sqlPar_end = sqlCmd.Parameters.Add("@endDate", SqlDbType.NVarChar);
                sqlPar_end.Direction = ParameterDirection.Input;
                sqlPar_end.Value = trade_dt;

                sqlReturn = sqlCmd.Parameters.Add("@return_value", SqlDbType.Int);
                sqlReturn.Direction = ParameterDirection.ReturnValue;

                sqlCmd.ExecuteNonQuery();

                Console.WriteLine(sqlReturn.Value.ToString());

                connection.Close();
            }
        }

        static void FetchData(string trade_dt, string stock)
        {
            string con;
            con = "Server=localhost\\mssqlserver01;Database=stock_price;uid=sa;pwd=84265";

            using (SqlConnection connection = new SqlConnection(con))
            {
                connection.Open();

                // check if the date has already been inserted
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = connection;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "select s_info_windcode, s_dq_open, s_dq_high, s_dq_low, " 
                    + "s_dq_close, s_dq_volume from dbo.Result where trade_dt = '"
                    + trade_dt + "' and s_info_windcode = '" + stock + "'";
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    priceCombo structObj;
                    structObj.s_dq_open = Convert.ToDouble(dr[1]);
                    structObj.s_dq_high = Convert.ToDouble(dr[2]);
                    structObj.s_dq_low = Convert.ToDouble(dr[3]);
                    structObj.s_dq_close = Convert.ToDouble(dr[4]);
                    structObj.s_dq_volume = Convert.ToDouble(dr[5]);

                    dataList.Add(structObj);
                }
                dr.Close();

                Console.WriteLine(dataList[0].s_dq_open);

                connection.Close();
            }
        }

        static void update_clv(string trade_dt)
        {
            


        }


    }
}
