using System;
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

        public static double ADCalculator()
        {
            return 0.0;
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
                    else if (i == 49) resultArray[3] = sum / days;
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

    }
}
