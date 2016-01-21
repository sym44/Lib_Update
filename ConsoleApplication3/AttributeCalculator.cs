using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataElf
{
    /// <summary>
    /// Helper class which contains calculators for each attributes.
    /// </summary>
    class AttributeCalculator
    {
        /// <summary>
        /// CLV: close location value, 
        /// CLV=((收盘价-最低价) - (最高价-收盘价)) / (最高价-最低价)  
        /// </summary>
        /// <param name=""></param>
        /// <returns>clv</returns>
        public static double ClvCalculator(double close, double high, double low)
        {
            return (2 * close - high - low) / (high - low);
        }

        /// <summary>
        /// AD: accumulation distribution line
        /// AD = SUM(CLV*Volume)
        /// </summary>
        /// <returns>AD</returns>
        public static double ADCalculator(double[] clvArray, double[] volumeArray, 
            int length)
        {
            double sum = 0.0;

            for (int i = 0; i < length; i++)
            {
                sum += clvArray[i] * volumeArray[i];
            }
            return sum;
        }

        /// <summary>
        /// MA: moving average
        /// An often used helper method which take in a list of values (of consecutive
        /// days) and calculates the moving average
        /// </summary>
        /// <param name="values">a list of certain length of values of different dates</param>
        /// <returns>MA</returns>
        public static double[] MACalculator(List<double> values)
        {
            double[] resultArray = new double[6];
            double sum = 0.0;
            int days = 0;

            if (values.Count >= 250)
            {
                for (int i = 0; i <= 249; i++)
                {
                    sum += values[i];
                    if (values.GetType().Name != "DBNull")
                        days++;

                    if (i == 4) resultArray[0] = sum / days;
                    else if (i == 9) resultArray[1] = sum / days;
                    else if (i == 19) resultArray[2] = sum / days;
                    else if (i == 59) resultArray[3] = sum / days;
                    else if (i == 119) resultArray[4] = sum / days;
                    else if (i == 249) resultArray[5] = sum / days;
                }
            }
            else
            {
                Console.WriteLine("data not available, needs to wait for 250 days");
            }

            return resultArray;
        }

        /// <summary>
        /// CMF: Chaikin's Money Flow
        /// CMF = SUM(AD)/SUM(VOL)
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static double CMFCalculator(double[] adArray, double[] volArray, 
            int length)
        {
            double sumAD = 0.0;
            double sumVol = 0.0;

            for(int i = 0; i < 20; i++)
            {
                sumAD += adArray[i];
                sumVol += volArray[i];
            }

            return sumAD/sumVol;
        }

        /// <summary>
        /// BB: Bollinger Bands
        /// BB = (收盘价 - MA(收盘价, N))/stdev(收盘价, N)
        /// </summary>
        /// <param name="closeArray"></param>
        /// <param name="bbLength"></param>
        /// <returns></returns>
        public static double BBCalculator(double[] closeArray, int bbLength)
        {
            double close = closeArray[0];
            List<double> closeRange = new List<double>(bbLength);
            double MA = 0.0, STDEV = 0.0;

            for (int i = 0; i < bbLength; i++)
            {
                closeRange.Add(closeArray[i]);
            }
            MA = average(closeRange);
            STDEV = stdev(closeRange);

            return (close - MA) / STDEV;
        }

        /// <summary>
        /// PPO: Percentage Price Oscillator
        /// PPO = 100 * (EMA(close, SHORT) - EMA(close, LONG)) / EMA(close, SHORT)
        /// PVO: Percentage Volume Oscillator
        /// PVO = 100 * (EMA(volume, SHORT) - EMA(volume, LONG)) / EMA(volume, SHORT)
        /// </summary>
        /// <param name="valueToday"></param>
        /// <param name="emaShortYesterday">the EMA value of the last trading day</param>
        /// <param name="emaLongYesterday">the EMA 26 value of the last trading day</param>
        /// <returns></returns>
        public static double PPOPVOCalculator(double valueToday, double emaShortYesterday, 
            double emaLongYesterday)
        {
            double emaShortToday = emaRecursionNext(valueToday, emaShortYesterday, 12);
            double emaLongToday = emaRecursionNext(valueToday, emaLongYesterday, 26);
            return 100.0 * (emaShortToday - emaLongToday) / emaShortToday;
        }


        /// <summary>
        /// RSI: Relative Strength Indicator
        /// RS = MA(日正收益率,N) / MA(日负收益率,N), RSI = 100 * RS / (1+RS)
        /// </summary>
        /// <param name="closeArray"></param>
        /// <returns></returns>
        public static double RSICalculator(double[] closeArray)
        {
            double RS, PS = 0.0, NS = 0.0;
            ArrayList positivePR = new ArrayList();
            ArrayList negativePR = new ArrayList();
  
            for(int i = 0; i < closeArray.Length - 1; i++)
            {
                if(closeArray[i] >= closeArray[i + 1])
                { positivePR.Add(closeArray[i] - closeArray[i + 1]); }
                else
                { negativePR.Add(closeArray[i + 1] - closeArray[i]); }
            }

            foreach (double element in positivePR) { PS += element; }
            foreach (double element in negativePR) { NS += element; }
            RS = PS / NS;

            return 100 * RS / (1 + RS);
        }


        /// <summary>
        /// SO: Stochastic Osillator
        /// </summary>
        /// <param name="closeToday"></param>
        /// <param name="lowArray"></param>
        /// <param name="highArray"></param>
        /// <returns></returns>
        public static double SOCalculator(double closeToday, double[] lowArray, 
            double[] highArray)
        {
            double minLow = lowArray.Min();
            double maxHigh = highArray.Max();

            return (closeToday - minLow) / (maxHigh - minLow);
        }

        public static double WRCalculator(double[] closeArray, int WRLength)
        {
            double minClose = closeArray.Min();
            double maxClose = closeArray.Max();
            double closeToday = closeArray[0];

            return - (minClose - closeToday) / (maxClose - minClose) * 100;
        }

        #region MathHelper
        /// <summary>
        /// 求出数据平均值,并保留三位小数
        /// </summary>
        /// <param name="Valist">数据集合</param>
        /// <returns></returns>
        public static double average(List<double> Valist)
        {
            double sum = 0;
            foreach (double d in Valist)
            {
                sum = sum + d;
            }
            double revl = System.Math.Round(sum / Valist.Count, 3);
            return revl;
        }

        /// <summary>
        /// 求数据集合标准差
        /// </summary>
        /// <param name="ValList"></param>
        /// <returns></returns>
        public static double stdev(List<double> ValList)
        {
            double avg = average(ValList);
            double sumstdev = 0;
            foreach (double d in ValList)
            {
                sumstdev = sumstdev + (d - avg) * (d - avg);
            }
            double stdeval = System.Math.Sqrt(sumstdev);
            return System.Math.Round(stdeval, 3);
        }

        public static double emaRecursionNext(double closeToday, double lastValue, 
            int lag)
        {
            double alpha = 2 / (lag + 1);
            return alpha * closeToday + (1 - alpha) * lastValue;
        }

        #endregion MathHelper
    }
}
