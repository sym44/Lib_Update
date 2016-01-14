using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication3
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
        /// MA: moving average
        /// An often used helper method which take in a list of values (of consecutive
        /// days) and calculates the moving average
        /// </summary>
        /// <param name="values">a list of certain length of values of different dates</param>
        /// <returns>MA</returns>
        public static double MACalculator(List<double> values, int delay)
        {
            double sum = 0.0;

            for(int i = 1; i <= delay; i++)
            {
                sum += values[values.Count - i];
            }

            return sum/delay;
        }

    }
}
