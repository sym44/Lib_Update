﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdateMatlab
{
    public class UpdateMatlab
    {
        private ArrayList stockList;
        private List<priceCombo> dataList;

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

        public UpdateMatlab(string trade_dt)
        {
            stockList = new ArrayList(2000);
            dataList = new List<priceCombo>();
        }

        public void Main(string[] args)
        {
            string updateDate = "20150105";
            read_stock(updateDate);

            //check new stocks

            //update_prices("20150105");

            //foreach (string stock in stockList)
            //{
            //    Updater updater = new Updater(stock, updateDate);
            //    updater.update();
            //}

            if (true)
            {
                #region oldUpdate
                //update_CLV 
                foreach (string stock in stockList)
                {
                    //update CLV
                    ClvUpdater clvUpdaterObj = new ClvUpdater(stock, updateDate);
                    clvUpdaterObj.update();

                    //update AD
                    ADUpdater adUpdater5Obj = new ADUpdater(stock, updateDate, 5);
                    adUpdater5Obj.update();
                    ADUpdater adUpdater14Obj = new ADUpdater(stock, updateDate, 14);
                    adUpdater14Obj.update();
                    ADUpdater adUpdater20Obj = new ADUpdater(stock, updateDate, 20);
                    adUpdater20Obj.update();

                    //update CMF
                    CMFUpdater cmfUpdaterObj = new CMFUpdater(stock, updateDate);
                    cmfUpdaterObj.update();

                    //update BB
                    BBUpdater bbUpdater5Obj = new BBUpdater(stock, updateDate, 5);
                    bbUpdater5Obj.update();
                    BBUpdater bbUpdater14Obj = new BBUpdater(stock, updateDate, 14);
                    bbUpdater14Obj.update();
                    BBUpdater bbUpdater20Obj = new BBUpdater(stock, updateDate, 20);
                    bbUpdater20Obj.update();

                    //TODO: update PPO
                    PPOUpdater ppoUpdaterObj = new PPOUpdater(stock, updateDate);
                    ppoUpdaterObj.update();

                    //TODO: update PVO
                    PVOUpdater pvoUpdateObj = new PVOUpdater(stock, updateDate);
                    pvoUpdateObj.update();

                    //TODO: update RSI
                    RSIUpdater rsiUpdaterObj = new RSIUpdater(stock, updateDate);
                    rsiUpdaterObj.update();

                    //TODO: update SO
                    SOUpdater soUpdateObj = new SOUpdater(stock, updateDate);
                    soUpdateObj.update();

                    //TODO: update MACD

                    //TODO: update WR
                    WRUpdater wrUpdater5Obj = new WRUpdater(stock, updateDate, 5);
                    wrUpdater5Obj.update();
                    WRUpdater wrUpdater14Obj = new WRUpdater(stock, updateDate, 14);
                    wrUpdater14Obj.update();
                    WRUpdater wrUpdater20Obj = new WRUpdater(stock, updateDate, 20);
                    wrUpdater20Obj.update();
                }
                #endregion

            }

            Console.ReadKey();
        }

        public void UpdateMatlabMethod(string updateDate)
        {
            read_stock(updateDate);
            update_prices(updateDate);
            foreach (string stock in stockList)
            {
                #region update
                //update CLV
                ClvUpdater clvUpdaterObj = new ClvUpdater(stock, updateDate);
                clvUpdaterObj.update();

                //update AD
                ADUpdater adUpdater5Obj = new ADUpdater(stock, updateDate, 5);
                adUpdater5Obj.update();
                ADUpdater adUpdater14Obj = new ADUpdater(stock, updateDate, 14);
                adUpdater14Obj.update();
                ADUpdater adUpdater20Obj = new ADUpdater(stock, updateDate, 20);
                adUpdater20Obj.update();

                //update CMF
                CMFUpdater cmfUpdaterObj = new CMFUpdater(stock, updateDate);
                cmfUpdaterObj.update();

                //update BB
                BBUpdater bbUpdater5Obj = new BBUpdater(stock, updateDate, 5);
                bbUpdater5Obj.update();
                BBUpdater bbUpdater14Obj = new BBUpdater(stock, updateDate, 14);
                bbUpdater14Obj.update();
                BBUpdater bbUpdater20Obj = new BBUpdater(stock, updateDate, 20);
                bbUpdater20Obj.update();

                //TODO: update PPO
                PPOUpdater ppoUpdaterObj = new PPOUpdater(stock, updateDate);
                ppoUpdaterObj.update();

                //TODO: update PVO
                PVOUpdater pvoUpdateObj = new PVOUpdater(stock, updateDate);
                pvoUpdateObj.update();

                //TODO: update RSI
                RSIUpdater rsiUpdaterObj = new RSIUpdater(stock, updateDate);
                rsiUpdaterObj.update();

                //TODO: update SO
                SOUpdater soUpdateObj = new SOUpdater(stock, updateDate);
                soUpdateObj.update();

                //TODO: update MACD

                //TODO: update WR
                WRUpdater wrUpdater5Obj = new WRUpdater(stock, updateDate, 5);
                wrUpdater5Obj.update();
                WRUpdater wrUpdater14Obj = new WRUpdater(stock, updateDate, 14);
                wrUpdater14Obj.update();
                WRUpdater wrUpdater20Obj = new WRUpdater(stock, updateDate, 20);
                wrUpdater20Obj.update();
                #endregion
            }
        }

        void read_stock(string trade_dt)
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

        void update_prices(string trade_dt)
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

    }
}
