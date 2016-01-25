using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace DataElf
{
    class Updater: IUpdate
    {
        private string s_info_windcode;
        private string trade_dt;

        public Updater(string s_info_windcode, string trade_dt)
        {
            this.s_info_windcode = s_info_windcode;
            this.trade_dt = trade_dt;
        }

        public void update()
        {
            this.updateBaseValue1();
            this.updateDerivedValue();
        }

        private void updateBaseValue1()
        {
            //fetch -- basic KL info
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "select s_info_windcode, s_dq_open, "
                + "s_dq_high, s_dq_low, s_dq_close,"
                + " s_dq_volume from dbo.Result where trade_dt <= '"
                + trade_dt + "' and s_info_windcode = '"
                + s_info_windcode + "' order by trade_dt desc";
            List<Program.priceCombo> list = SQLHelper
                .FetchQueryResultToPriceCombo(cmd);

            double clv = AttributeCalculator.ClvCalculator(
                list[0].s_dq_close, list[0].s_dq_high, list[0].s_dq_low);
            //SQLHelper.UpdateSingleValueIntoTable(clv, "clv", s_info_windcode,
              //  trade_dt);
     

            //BB
            //fetch
            double[] closeArray = new double[list.Count];
            double[] volumeArray = new double[list.Count];
            double[] highArray = new double[list.Count];
            double[] lowArray = new double[list.Count];
            for(int i = 0; i < list.Count; i++)
            {
                closeArray[i] = list[i].s_dq_close;
                volumeArray[i] = list[i].s_dq_volume;
                highArray[i] = list[i].s_dq_high;
                lowArray[i] = list[i].s_dq_low;
            }

            //update
            double bb_5 = AttributeCalculator.BBCalculator(closeArray, 5);
            //SQLHelper.UpdateSingleValueIntoTable(bb_5, "bb_5",
            //    s_info_windcode, trade_dt);
            double bb_14 = AttributeCalculator.BBCalculator(closeArray, 14);
            //SQLHelper.UpdateSingleValueIntoTable(bb_14, "bb_14",
            //    s_info_windcode, trade_dt);
            double bb_20 = AttributeCalculator.BBCalculator(closeArray, 20);
            //SQLHelper.UpdateSingleValueIntoTable(bb_20, "bb_20",
            //    s_info_windcode, trade_dt);


            //PPO
            //fetch
            double closeToday = closeArray[0];

            SqlCommand cmd2 = new SqlCommand();
            cmd2.CommandType = CommandType.Text;
            cmd2.CommandText = "select top 1 s_info_windcode, close_ema_12 "
                + "from dbo.Result where s_info_windcode = '"
                + s_info_windcode + "' and trade_dt < '" + trade_dt
                + "' order by trade_dt desc";
            List<double> emaShortList = SQLHelper.FetchQueryResultToDouble(cmd2);

            SqlCommand cmd3 = new SqlCommand();
            cmd3.CommandType = CommandType.Text;
            cmd3.CommandText = "select top 1 s_info_windcode, close_ema_26 "
                + "from dbo.Result where s_info_windcode = '"
                + s_info_windcode + "' and trade_dt < '" + trade_dt
                + "' order by trade_dt desc";
            List<double> emaLongList = SQLHelper.FetchQueryResultToDouble(cmd3);

            double emaShortYesterday = emaShortList[0];
            double emaLongYesterday = emaLongList[0];

            double emaShort = AttributeCalculator.emaRecursionNext(closeToday, emaShortYesterday, 12);
            double emaLong = AttributeCalculator.emaRecursionNext(closeToday, emaLongYesterday, 26);
            double ppo = AttributeCalculator.PPOPVOCalculator(closeToday,
                emaShort, emaLong);
            //SQLHelper.UpdateSingleValueIntoTable(emaShort, "CLOSE_EMA_12", s_info_windcode, trade_dt);
            //SQLHelper.UpdateSingleValueIntoTable(emaLong, "CLOSE_EMA_26", s_info_windcode, trade_dt);
            //SQLHelper.UpdateSingleValueIntoTable(ppo, "ppo", s_info_windcode,
            //    trade_dt);


            //PVO
            //fetch
            double volumeToday = volumeArray[0];

            SqlCommand cmd4 = new SqlCommand();
            cmd4.CommandType = CommandType.Text;
            cmd4.CommandText = "select top 1 s_info_windcode, volume_ema_12 "
                + "from dbo.Result where s_info_windcode = '"
                + s_info_windcode + "' and trade_dt < '" + trade_dt
                + "' order by trade_dt desc";
            List<double> emaVolumeShortList = SQLHelper.FetchQueryResultToDouble(cmd4);

            SqlCommand cmd5 = new SqlCommand();
            cmd5.CommandType = CommandType.Text;
            cmd5.CommandText = "select top 1 s_info_windcode, volume_ema_26 "
                + "from dbo.Result where s_info_windcode = '"
                + s_info_windcode + "' and trade_dt < '" + trade_dt
                + "' order by trade_dt desc";
            List<double> emaVolumeLongList = SQLHelper.FetchQueryResultToDouble(cmd5);

            double emaVolumeShortYesterday = emaVolumeShortList[0];
            double emaVolumeLongYesterday = emaVolumeLongList[0];

            double emaVolumeShortToday = AttributeCalculator.emaRecursionNext(volumeToday, emaShortYesterday, 12);
            double emaVolumeLongToday = AttributeCalculator.emaRecursionNext(volumeToday, emaLongYesterday, 26);

            double pvo = AttributeCalculator.PPOPVOCalculator(volumeToday,
                emaVolumeShortToday, emaVolumeLongToday);
            //SQLHelper.UpdateSingleValueIntoTable(emaVolumeShortToday, "VOLUME_EMA_12", s_info_windcode, trade_dt);
            //SQLHelper.UpdateSingleValueIntoTable(emaVolumeLongToday, "VOLUME_EMA_26", s_info_windcode, trade_dt);
            //SQLHelper.UpdateSingleValueIntoTable(pvo, "pvo", s_info_windcode,
            //    trade_dt);


            //RSI
            //fetch
            double[] closeArray15 = new double[15];
            for (int i = 0; i < 15; i++) { closeArray15[i] = closeArray[i]; }

            //update
            double rsi = AttributeCalculator.RSICalculator(closeArray15);
            SQLHelper.UpdateSingleValueIntoTable(rsi, "rsi", s_info_windcode,
                trade_dt);


            //SO
            //fetch
            double[] lowArray39 = new double[39];
            double[] highArray39 = new double[39];

            for (int i = 0; i < 39; i++)
            {
                lowArray39[i] = list[i].s_dq_low;
                highArray39[i] = list[i].s_dq_high;
            }

            //update
            double so = AttributeCalculator.SOCalculator(closeToday, lowArray39, highArray39);
            //SQLHelper.UpdateSingleValueIntoTable(so, "so", s_info_windcode,
            //    trade_dt);


            //WR
            //fetch
            double[] closeArray5 = new double[5];
            double[] closeArray14 = new double[14];
            double[] closeArray20 = new double[20];

            for (int i = 0; i < 5; i++) { closeArray5[i] = closeArray[i]; }
            for (int i = 0; i < 14; i++) { closeArray14[i] = closeArray[i]; }
            for (int i = 0; i < 20; i++) { closeArray20[i] = closeArray[i]; }

            //update
            double wr_5 = AttributeCalculator.WRCalculator(closeArray, 5);
            double wr_14 = AttributeCalculator.WRCalculator(closeArray, 14);
            double wr_20 = AttributeCalculator.WRCalculator(closeArray, 20);
            //SQLHelper.UpdateSingleValueIntoTable(wr_5, "wr_5",
            //    s_info_windcode, trade_dt);
            //SQLHelper.UpdateSingleValueIntoTable(wr_14, "wr_14",
            //    s_info_windcode, trade_dt);
            //SQLHelper.UpdateSingleValueIntoTable(wr_20, "wr_20",
            //    s_info_windcode, trade_dt);


            double[] results = new double[11];
            results[0] = clv;
            results[1] = bb_5;
            results[2] = bb_14;
            results[3] = bb_20;
            results[4] = ppo;
            results[5] = pvo;
            results[6] = rsi;
            results[7] = so;
            results[8] = wr_5;
            results[9] = wr_14;
            results[10] = wr_20;
            SQLHelper.UpdateMultipleAttribute1IntoTable(results, s_info_windcode, trade_dt);


            //fetch
            SqlCommand cmd6 = new SqlCommand();
            cmd6.CommandType = CommandType.Text;
            cmd6.CommandText = "select s_info_windcode, clv "
                + "from dbo.Result where s_info_windcode = '"
                + s_info_windcode + "' and trade_dt <= '"
                + trade_dt + "' order by trade_dt desc";
            List<double> clvList = SQLHelper.FetchQueryResultToDouble(cmd6);

            double[] clvArray5 = new double[5];
            double[] volArray5 = new double[5];
            double[] clvArray14 = new double[14];
            double[] volArray14 = new double[14];
            double[] clvArray20 = new double[20];
            double[] volArray20 = new double[20];

            for (int i = 0; i < 5; i++) { clvArray5[i] = clvList[i]; volArray5[i] = volumeArray[i]; }
            for (int i = 0; i < 14; i++) { clvArray14[i] = clvList[i]; volArray14[i] = volumeArray[i]; }
            for (int i = 0; i < 20; i++) { clvArray20[i] = clvList[i]; volArray20[i] = volumeArray[i]; }

            double ad_5 = AttributeCalculator.ADCalculator(clvArray5,
                volArray5, 5);
            double ad_14 = AttributeCalculator.ADCalculator(clvArray14,
                volArray14, 14);
            double ad_20 = AttributeCalculator.ADCalculator(clvArray20,
                volArray20, 20);
            //SQLHelper.UpdateSingleValueIntoTable(ad_5, "ad_5", s_info_windcode, trade_dt);
            //SQLHelper.UpdateSingleValueIntoTable(ad_14, "ad_14", s_info_windcode, trade_dt);
            //SQLHelper.UpdateSingleValueIntoTable(ad_20, "ad_20", s_info_windcode, trade_dt);
    
            SqlCommand cmd7 = new SqlCommand();
            cmd7.CommandType = CommandType.Text;
            cmd7.CommandText = "select s_info_windcode, ad_5 "
                + "from dbo.Result where s_info_windcode = '"
                + s_info_windcode + "' and trade_dt <= '"
                + trade_dt + "' order by trade_dt desc";
            List<double> adList = SQLHelper.FetchQueryResultToDouble(cmd7);

            double[] ad5Array20 = new double[20];
            for (int i = 0; i < 20; i++) { ad5Array20[i] = adList[i]; volArray20[i] = volumeArray[i]; }

            double cmf = AttributeCalculator.CMFCalculator(ad5Array20, volArray20, 20);
            //SQLHelper.UpdateSingleValueIntoTable(cmf, "cmf", s_info_windcode,
            //    trade_dt);

            
        }

        private void updateDerivedValue()
        {

        }
    }

    /// <summary>
    /// Responsible for handling all the working process of the CLV update.
    /// </summary>
    class ClvUpdater: IUpdate
    {
        private string s_info_windcode;
        private string trade_dt;

        public ClvUpdater(string s_info_windcode, string trade_dt)
        {
            this.s_info_windcode = s_info_windcode;
            this.trade_dt = trade_dt;
        }

        /// <summary>
        /// This method includes the whole process of the updating clv.
        /// The main program needs only to call this method for each
        /// stock and the specific date
        /// </summary>
        /// <param name="list"></param> // Needs refractor, need to move the list fetching process into the method
        /// <param name="s_info_windcode"></param>
        /// <param name="trade_dt"></param>
        /// <param name="delay"></param>
        public void update()
        {
            this.updateBaseValue(s_info_windcode, trade_dt);
            this.updateDerivedValue(s_info_windcode, trade_dt);
        }

        private void updateBaseValue(string s_info_windcode, 
            string trade_dt)
        {
            // fetch list
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "select s_info_windcode, s_dq_open, "
                + "s_dq_high, s_dq_low, s_dq_close,"
                + " s_dq_volume from dbo.Result where trade_dt = '"
                + trade_dt + "' and s_info_windcode = '" 
                + s_info_windcode + "' order by trade_dt desc";
            List<Program.priceCombo> list = SQLHelper
                .FetchQueryResultToPriceCombo(cmd);


            double clv = AttributeCalculator.ClvCalculator(
                list[0].s_dq_close, list[0].s_dq_high, list[0].s_dq_low);
            SQLHelper.UpdateSingleValueIntoTable(clv, "clv", s_info_windcode,
                trade_dt);
        }

        private void updateDerivedValue(string s_info_windcode, 
            string trade_dt)
        {
            double[] clvArray = new double[6];
            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandType = CommandType.Text;
            sqlCmd.CommandText = "select s_info_windcode, clv "
                + "from Result where s_info_windcode = '"
                + s_info_windcode + "' and trade_dt <= '"
                + trade_dt + "' order by trade_dt desc";
            List<double> dataList = SQLHelper.FetchQueryResultToDouble(sqlCmd);
            clvArray = AttributeCalculator.MACalculator(dataList); // calculate the value
            SQLHelper.UpdateMultipleValueIntoTable(clvArray, "clv", s_info_windcode, trade_dt);
        }
    }

    /// <summary>
    /// Responsible for handling all the working process of the AD update.
    /// </summary>
    class ADUpdater : IUpdate
    {
        private int ADLength;
        private string s_info_windcode;
        private string trade_dt;

        public ADUpdater(string s_info_windcode, string trade_dt, int ADLength)
        {
            this.ADLength = ADLength;
            this.s_info_windcode = s_info_windcode;
            this.trade_dt = trade_dt;
        }

        /// <summary>
        /// This method includes the whole process of the updating AD.
        /// The main program needs only to call this method for each
        /// stock and the specific date
        /// </summary>
        /// <param name="list"></param>
        /// <param name="s_info_windcode"></param>
        /// <param name="trade_dt"></param>
        public void update()
        {
            this.updateBaseValue(s_info_windcode, trade_dt);
            this.updateDerivedValue(s_info_windcode, trade_dt);
        }

        /// <summary>
        /// Update the base AD_length, with parameter length
        /// </summary>
        /// <param name="s_info_windcode"></param>
        /// <param name="trade_dt"></param>
        /// <param name="length"></param>
        private void updateBaseValue(string s_info_windcode, string trade_dt)
        {
            //fetch
            double[] clvArray = new double[ADLength];
            double[] volArray = new double[ADLength];
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "select s_info_windcode, clv "
                + "from dbo.Result where s_info_windcode = '"
                + s_info_windcode + "' and trade_dt <= '"
                + trade_dt + "' order by trade_dt desc";
            List<double> dataList = SQLHelper.FetchQueryResultToDouble(cmd);

            SqlCommand cmd2 = new SqlCommand();
            cmd2.CommandType = CommandType.Text;
            cmd2.CommandText = "select s_info_windcode, s_dq_open, "
                + "s_dq_high, s_dq_low, s_dq_close,"
                + " s_dq_volume from dbo.Result where s_info_windcode = '"
                + s_info_windcode + "' and trade_dt <= '"
                + trade_dt + "' order by trade_dt desc";
            List<Program.priceCombo> priceComboList = SQLHelper
                .FetchQueryResultToPriceCombo(cmd2);

            for (int i = 0; i < ADLength; i++)
            {
                clvArray[i] = dataList[i];
                volArray[i] = priceComboList[i].s_dq_volume;
            }

            double ad = AttributeCalculator.ADCalculator(clvArray, 
                volArray, ADLength);
            SQLHelper.UpdateSingleValueIntoTable(ad, "ad_" + ADLength.ToString(), 
                s_info_windcode, trade_dt);
        }

        /// <summary>
        /// update the derived attributes
        /// </summary>
        /// <param name="s_info_windcode"></param>
        /// <param name="trade_dt"></param>
        /// <param name="length"></param>
        private void updateDerivedValue(string s_info_windcode, string trade_dt)
        {
            //fetch
            double[] ADArray = new double[ADLength];
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "select s_info_windcode, AD_" + ADLength.ToString()
                + " from dbo.Result where s_info_windcode = '"
                + s_info_windcode + "' and trade_dt <= '"
                + trade_dt + "' order by trade_dt desc";
            List<double> dataList = SQLHelper.FetchQueryResultToDouble(cmd);

            //update
            double[] adArray = new double[6];
            adArray = AttributeCalculator.MACalculator(dataList);
            SQLHelper.UpdateMultipleValueIntoTable(adArray,
                "ad_" + ADLength.ToString(), s_info_windcode, trade_dt);
        }
    }

    /// <summary>
    /// Responsible for handling all the working process of the CMF update.
    /// </summary>
    class CMFUpdater : IUpdate
    {
        private string s_info_windcode;
        private string trade_dt;

        public CMFUpdater(string s_info_windcode, string trade_dt)
        {
            this.s_info_windcode = s_info_windcode;
            this.trade_dt = trade_dt;
        }

        public void update()
        {
            this.updateBaseValue();
            this.updateDerivedValue();
        }

        private void updateBaseValue()
        {
            double[] adArray = new double[20];
            double[] volArray = new double[20];
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "select s_info_windcode, ad_5 "
                + "from dbo.Result where s_info_windcode = '"
                + s_info_windcode + "' and trade_dt <= '"
                + trade_dt + "' order by trade_dt desc";
            List<double> dataList = SQLHelper.FetchQueryResultToDouble(cmd);

            SqlCommand cmd2 = new SqlCommand();
            cmd2.CommandType = CommandType.Text;
            cmd2.CommandText = "select s_info_windcode, s_dq_open, "
                + "s_dq_high, s_dq_low, s_dq_close,"
                + " s_dq_volume from dbo.Result where s_info_windcode = '"
                + s_info_windcode + "' and trade_dt <= '"
                + trade_dt + "' order by trade_dt desc";
            List<Program.priceCombo> priceComboList = SQLHelper
                .FetchQueryResultToPriceCombo(cmd2);

            for (int i = 0; i < 20; i++)
            {
                adArray[i] = dataList[i];
                volArray[i] = priceComboList[i].s_dq_volume;
            }

            double cmf = AttributeCalculator.CMFCalculator(adArray, volArray, 20);
            SQLHelper.UpdateSingleValueIntoTable(cmf, "cmf", s_info_windcode, 
                trade_dt);
        }

        private void updateDerivedValue()
        {
            //fetch
            double[] CMFArray = new double[20];
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "select s_info_windcode, CMF from Result"
                + " where s_info_windcode = '" + s_info_windcode
                + "' and trade_dt <= '" + trade_dt
                + "' order by trade_dt desc";
            List<double> dataList = SQLHelper.FetchQueryResultToDouble(cmd);

            //update
            double[] cmfArray = new double[6];
            cmfArray = AttributeCalculator.MACalculator(dataList);
            SQLHelper.UpdateMultipleValueIntoTable(cmfArray, "cmf",
                s_info_windcode, trade_dt);
        }
    }

    /// <summary>
    /// Responsible for handling all the working process of the BB update.
    /// </summary>
    class BBUpdater : IUpdate
    {
        private string s_info_windcode;
        private string trade_dt;
        private int BBLength;

        public BBUpdater(string s_info_windcode, string trade_dt, int BBLength)
        {
            this.s_info_windcode = s_info_windcode;
            this.trade_dt = trade_dt;
            this.BBLength = BBLength;
        }

        public void update()
        {
            this.updateBaseValue(s_info_windcode, trade_dt, BBLength);
            this.updateDerivedValue(s_info_windcode, trade_dt, BBLength);
        }

        private void updateBaseValue(string s_info_windcode, string trade_dt, 
            int BBLength)
        {
            //fetch
            double[] closeArray = new double[BBLength];

            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "select s_info_windcode, s_dq_close "
                + "from dbo.Result where s_info_windcode = '"
                + s_info_windcode + "' and trade_dt <= '"
                + trade_dt + "' order by trade_dt desc";
            List<double> dataList = SQLHelper.FetchQueryResultToDouble(cmd);

            for (int i = 0; i < BBLength; i++)
            {
                closeArray[i] = dataList[i];
            }

            //update
            double bb = AttributeCalculator.BBCalculator(closeArray, BBLength);
            SQLHelper.UpdateSingleValueIntoTable(bb, "bb_" + BBLength.ToString(),
                s_info_windcode, trade_dt);
        }

        private void updateDerivedValue(string s_info_windcode, string trade_dt, 
            int BBLength)
        {
            //fetch
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "select s_info_windcode, BB_" + BBLength.ToString()
                + " from Result" + " where s_info_windcode = '" + s_info_windcode
                + "' and trade_dt <= '" + trade_dt
                + "' order by trade_dt desc";
            List<double> dataList = SQLHelper.FetchQueryResultToDouble(cmd);

            //update
            double[] bbArray = new double[6];
            bbArray = AttributeCalculator.MACalculator(dataList);
            SQLHelper.UpdateMultipleValueIntoTable(bbArray, 
                "bb_" + BBLength.ToString(), s_info_windcode, trade_dt);
        }
    }

    /// <summary>
    /// Responsible for handling all the working process of the PPO update.
    /// </summary>
    class PPOUpdater : IUpdate
    {
        private string s_info_windcode;
        private string trade_dt;

        public PPOUpdater(string s_info_windcode, string trade_dt)
        {
            this.s_info_windcode = s_info_windcode;
            this.trade_dt = trade_dt;
        }

        public void update()
        {
            this.updateBaseValue(s_info_windcode, trade_dt);
            this.updateDerivedValue(s_info_windcode, trade_dt);
        }

        private void updateBaseValue(string s_info_windcode, string trade_dt)
        {
            //fetch
            double closeToday = 0.0;
            double emaShortYesterday = 0.0;
            double emaLongYesterday = 0.0;

            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "select s_info_windcode, s_dq_close "
                + "from dbo.Result where s_info_windcode = '"
                + s_info_windcode + "' and trade_dt = '" + trade_dt + "'";
            List<double> dataList = SQLHelper.FetchQueryResultToDouble(cmd);
            
            SqlCommand cmd2 = new SqlCommand();
            cmd2.CommandType = CommandType.Text;
            cmd2.CommandText = "select top 1 s_info_windcode, close_ema_12 "
                + "from dbo.Result where s_info_windcode = '"
                + s_info_windcode + "' and trade_dt < '" + trade_dt
                + "' order by trade_dt desc";
            List<double> emaShortList = SQLHelper.FetchQueryResultToDouble(cmd2);

            SqlCommand cmd3 = new SqlCommand();
            cmd3.CommandType = CommandType.Text;
            cmd3.CommandText = "select top 1 s_info_windcode, close_ema_26 "
                + "from dbo.Result where s_info_windcode = '"
                + s_info_windcode + "' and trade_dt < '" + trade_dt
                + "' order by trade_dt desc";
            List<double> emaLongList = SQLHelper.FetchQueryResultToDouble(cmd3);

            closeToday = dataList[0];
            emaShortYesterday = emaShortList[0];
            emaLongYesterday = emaLongList[0];

            double emaShort = AttributeCalculator.emaRecursionNext(closeToday, emaShortYesterday, 12);
            double emaLong = AttributeCalculator.emaRecursionNext(closeToday, emaLongYesterday, 26);
            double ppo = AttributeCalculator.PPOPVOCalculator(closeToday, 
                emaShort, emaLong);
            SQLHelper.UpdateSingleValueIntoTable(emaShort, "CLOSE_EMA_12", s_info_windcode, trade_dt);
            SQLHelper.UpdateSingleValueIntoTable(emaLong, "CLOSE_EMA_26", s_info_windcode, trade_dt);
            SQLHelper.UpdateSingleValueIntoTable(ppo, "ppo", s_info_windcode, 
                trade_dt);


        }

        private void updateDerivedValue(string s_info_windcode, string trade_dt)
        {
            //fetch
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "select s_info_windcode, ppo from Result "
                + "where s_info_windcode = '" + s_info_windcode
                + "' and trade_dt <= '" + trade_dt
                + "' order by trade_dt desc";
            List<double> dataList = SQLHelper.FetchQueryResultToDouble(cmd);

            //update
            double[] ppoArray = new double[6];
            ppoArray = AttributeCalculator.MACalculator(dataList);
            SQLHelper.UpdateMultipleValueIntoTable(ppoArray, "ppo", s_info_windcode, 
                trade_dt);
        }
    }

    /// <summary>
    /// Responsible for handling all the working process of the PVO update.
    /// </summary>
    class PVOUpdater : IUpdate
    {
        private string s_info_windcode;
        private string trade_dt;

        public PVOUpdater(string s_info_windcode, string trade_dt)
        {
            this.s_info_windcode = s_info_windcode;
            this.trade_dt = trade_dt;
        }

        public void update()
        {
            this.updateBaseValue();
            this.updateDerivedValue();
        }

        private void updateBaseValue()
        {
            //fetch
            double volumeToday = 0.0;
            double emaShortYesterday = 0.0;
            double emaLongYesterday = 0.0;

            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "select s_info_windcode, s_dq_volume "
                + "from dbo.Result where s_info_windcode = '"
                + s_info_windcode + "' and trade_dt = '" + trade_dt + "'";
            List<double> dataList = SQLHelper.FetchQueryResultToDouble(cmd);

            SqlCommand cmd2 = new SqlCommand();
            cmd2.CommandType = CommandType.Text;
            cmd2.CommandText = "select top 1 s_info_windcode, volume_ema_12 "
                + "from dbo.Result where s_info_windcode = '"
                + s_info_windcode + "' and trade_dt < '" + trade_dt
                + "' order by trade_dt desc";
            List<double> emaShortList = SQLHelper.FetchQueryResultToDouble(cmd2);

            SqlCommand cmd3 = new SqlCommand();
            cmd3.CommandType = CommandType.Text;
            cmd3.CommandText = "select top 1 s_info_windcode, volume_ema_26 "
                + "from dbo.Result where s_info_windcode = '"
                + s_info_windcode + "' and trade_dt < '" + trade_dt
                + "' order by trade_dt desc";
            List<double> emaLongList = SQLHelper.FetchQueryResultToDouble(cmd3);

            volumeToday = dataList[0];
            emaShortYesterday = emaShortList[0];
            emaLongYesterday = emaLongList[0];

            double emaVolumeShortToday = AttributeCalculator.emaRecursionNext(volumeToday, emaShortYesterday, 12);
            double emaVolumeLongToday = AttributeCalculator.emaRecursionNext(volumeToday, emaLongYesterday, 26);


            double pvo = AttributeCalculator.PPOPVOCalculator(volumeToday,
                emaVolumeShortToday, emaVolumeLongToday);
            SQLHelper.UpdateSingleValueIntoTable(emaVolumeShortToday, "VOLUME_EMA_12", s_info_windcode, trade_dt);
            SQLHelper.UpdateSingleValueIntoTable(emaVolumeLongToday, "VOLUME_EMA_26", s_info_windcode, trade_dt);
            SQLHelper.UpdateSingleValueIntoTable(pvo, "pvo", s_info_windcode,
                trade_dt);
        }

        private void updateDerivedValue()
        {
            //fetch
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "select s_info_windcode, pvo from Result "
                + "where s_info_windcode = '" + s_info_windcode
                + "' and trade_dt <= '" + trade_dt
                + "' order by trade_dt desc";
            List<double> dataList = SQLHelper.FetchQueryResultToDouble(cmd);

            //update
            double[] ppoArray = new double[6];
            ppoArray = AttributeCalculator.MACalculator(dataList);
            SQLHelper.UpdateMultipleValueIntoTable(ppoArray, "pvo", s_info_windcode,
                trade_dt);
        }
    }

    class MACDUpdater : IUpdate
    {
        private string s_info_windcode;
        private string trade_dt;

        public MACDUpdater(string s_info_windcode, string trade_dt)
        {
            this.s_info_windcode = s_info_windcode;
            this.trade_dt = trade_dt;
        }

        public void update()
        {
            this.updateBaseValue();
            this.updateDerivedValue();
        }

        private void updateBaseValue()
        {
            //fetch DEA DIFF
            double diff = 0.0;
            double dea = 0.0;

            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "select s_info_windcode, diff "
                + "from dbo.Result where s_info_windcode = '"
                + s_info_windcode + "' and trade_dt = '" + trade_dt + "'";
            List<double> dataListDiff = SQLHelper.FetchQueryResultToDouble(cmd);

            SqlCommand cmd2 = new SqlCommand();
            cmd2.CommandType = CommandType.Text;
            cmd2.CommandText = "select s_info_windcode, dea "
                + "from dbo.Result where s_info_windcode = '"
                + s_info_windcode + "' and trade_dt = '" + trade_dt + "'";
            List<double> dataListDea = SQLHelper.FetchQueryResultToDouble(cmd2);

            diff = dataListDiff[0];
            dea = dataListDea[0];

            double macd = AttributeCalculator.MACDCalculator(diff, dea);
            SQLHelper.UpdateSingleValueIntoTable(macd, "macd", s_info_windcode,
                trade_dt);
        }

        private void updateDerivedValue()
        {
            //fetch
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "select s_info_windcode, macd from Result "
                + "where s_info_windcode = '" + s_info_windcode
                + "' and trade_dt <= '" + trade_dt
                + "' order by trade_dt desc";
            List<double> dataList = SQLHelper.FetchQueryResultToDouble(cmd);

            //update
            double[] ppoArray = new double[6];
            ppoArray = AttributeCalculator.MACalculator(dataList);
            SQLHelper.UpdateMultipleValueIntoTable(ppoArray, "pvo", s_info_windcode,
                trade_dt);
        }
    }

    /// <summary>
    /// Responsible for handling all the working process of the RSI update.
    /// </summary>
    class RSIUpdater : IUpdate
    {
        private string s_info_windcode;
        private string trade_dt;

        public RSIUpdater(string s_info_windcode, string trade_dt)
        {
            this.s_info_windcode = s_info_windcode;
            this.trade_dt = trade_dt;
        }

        public void update()
        {
            this.updateBaseValue(s_info_windcode, trade_dt);
            this.updateDerivedValue(s_info_windcode, trade_dt);
        }

        private void updateBaseValue(string s_info_windcode, string trade_dt)
        {
            //fetch
            double[] closeArray = new double[15];

            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "select top 15 s_info_windcode, s_dq_close "
                + "from dbo.Result where s_info_windcode = '"
                + s_info_windcode + "' and trade_dt <= '"
                + trade_dt + "' order by trade_dt desc";
            List<double> dataList = SQLHelper.FetchQueryResultToDouble(cmd);

            for (int i = 0; i < 15; i++)
            {
                closeArray[i] = dataList[i];
            }

            //update
            double rsi = AttributeCalculator.RSICalculator(closeArray);
            SQLHelper.UpdateSingleValueIntoTable(rsi, "rsi", s_info_windcode, 
                trade_dt);
        }
        
        private void updateDerivedValue(string s_info_windcode, string trade_dt)
        {
            //fetch
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "select s_info_windcode, rsi"
                + " from Result" + " where s_info_windcode = '" + s_info_windcode
                + "' and trade_dt <= '" + trade_dt
                + "' order by trade_dt desc";
            List<double> dataList = SQLHelper.FetchQueryResultToDouble(cmd);

            //update
            double[] rsiArray = new double[6];
            rsiArray = AttributeCalculator.MACalculator(dataList);
            SQLHelper.UpdateMultipleValueIntoTable(rsiArray, "rsi", 
                s_info_windcode, trade_dt);
        }
    }

    /// <summary>
    /// Responsible for handling all the working process of the SO update.
    /// </summary>
    class SOUpdater : IUpdate
    {
        private string s_info_windcode;
        private string trade_dt;

        public SOUpdater(string s_info_windcode, string trade_dt)
        {
            this.s_info_windcode = s_info_windcode;
            this.trade_dt = trade_dt;
        }

        public void update()
        {
            this.updateBaseValue(s_info_windcode, trade_dt);
            this.updateDerivedValue(s_info_windcode, trade_dt);
        }

        private void updateBaseValue(string s_info_windcode, string trade_dt)
        {
            //fetch
            double closeToday = 0.0;
            double[] lowArray = new double[39];
            double[] highArray = new double[39];

            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "select top 39 s_info_windcode, s_dq_open, "
                + "s_dq_high, s_dq_low, s_dq_close,"
                + " s_dq_volume from dbo.Result where trade_dt <= '"
                + trade_dt + "' and s_info_windcode = '"
                + s_info_windcode + "' order by trade_dt desc";
            List<Program.priceCombo> list = SQLHelper
                .FetchQueryResultToPriceCombo(cmd);

            closeToday = list[0].s_dq_close;
            for (int i = 0; i < 39; i++)
            {
                lowArray[i] = list[i].s_dq_low;
                highArray[i] = list[i].s_dq_high;
            }

            //update
            double so = AttributeCalculator.SOCalculator(closeToday, lowArray, highArray);
            SQLHelper.UpdateSingleValueIntoTable(so, "so", s_info_windcode, 
                trade_dt);
        }

        private void updateDerivedValue(string s_info_windcode, string trade_dt)
        {
            //fetch
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "select s_info_windcode, so"
                + " from Result" + " where s_info_windcode = '" + s_info_windcode
                + "' and trade_dt <= '" + trade_dt
                + "' order by trade_dt desc";
            List<double> dataList = SQLHelper.FetchQueryResultToDouble(cmd);

            //update
            double[] soArray = new double[6];
            soArray = AttributeCalculator.MACalculator(dataList);
            SQLHelper.UpdateMultipleValueIntoTable(soArray, "so",
                s_info_windcode, trade_dt);
        }
    }

    /// <summary>
    /// Responsible for handling all the working process of the WR udpate.
    /// </summary>
    class WRUpdater : IUpdate
    {
        private string s_info_windcode;
        private string trade_dt;
        private int WRLength;

        public WRUpdater(string s_info_windcode, string trade_dt,
            int WRLength)
        {
            this.s_info_windcode = s_info_windcode;
            this.trade_dt = trade_dt;
            this.WRLength = WRLength;
        }

        public void update()
        {
            this.updateBaseValue();
            this.updateDerivedValue();
        }

        private void updateBaseValue()
        {
            //fetch
            double[] closeArray = new double[WRLength];

            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "select s_info_windcode, s_dq_close "
                + "from dbo.Result where s_info_windcode = '"
                + s_info_windcode + "' and trade_dt <= '"
                + trade_dt + "' order by trade_dt desc";
            List<double> dataList = SQLHelper.FetchQueryResultToDouble(cmd);

            for (int i = 0; i < WRLength; i++)
            {
                closeArray[i] = dataList[i]; //desc
            }

            //update
            double wr = AttributeCalculator.WRCalculator(closeArray, WRLength);
            SQLHelper.UpdateSingleValueIntoTable(wr, "wr_" + WRLength.ToString(),
                s_info_windcode, trade_dt);
        }

        private void updateDerivedValue()
        {
            //fetch
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "select s_info_windcode, WR_" + WRLength.ToString()
                + " from Result" + " where s_info_windcode = '" + s_info_windcode
                + "' and trade_dt <= '" + trade_dt
                + "' order by trade_dt desc";
            List<double> dataList = SQLHelper.FetchQueryResultToDouble(cmd);

            //update
            double[] wrArray = new double[6];
            wrArray = AttributeCalculator.MACalculator(dataList);
            SQLHelper.UpdateMultipleValueIntoTable(wrArray,
                "wr_" + WRLength.ToString(), s_info_windcode, trade_dt);
        }
    }
}
