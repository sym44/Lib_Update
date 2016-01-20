using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace DataElf
{
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
            double PPOYesterday = 0.0;

            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "select s_info_windcode, s_dq_close "
                + "from dbo.Result where s_info_windcode = '"
                + s_info_windcode + "' and trade_dt = '" + trade_dt + "'";
            List<double> dataList = SQLHelper.FetchQueryResultToDouble(cmd);


            SqlCommand cmd2 = new SqlCommand();
            cmd2.CommandType = CommandType.Text;
            cmd2.CommandText = "select top 1 s_info_windcode, ppo "
                + "from dbo.Result where s_info_windcode = '"
                + s_info_windcode + "' and trade_dt < '" + trade_dt
                + "' order by trade_dt desc";
            List<double> ppoList = SQLHelper.FetchQueryResultToDouble(cmd2);

            closeToday = dataList[0];
            PPOYesterday = ppoList[0];

            double ppo = AttributeCalculator.PPOCalculator(closeToday, PPOYesterday);
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
}
