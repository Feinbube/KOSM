using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KOSM.Common
{
    public static class Format
    {
        public static string Percentage(double value)
        {
            return Percentage(value, 0);
        }

        public static string Percentage(double value, int padding)
        {
            return pretty(value * 100, padding, 1, "%");
        }

        public static string Degree(double value)
        {
            return Degree(value, 0);
        }

        public static string Degree(double value, int padding)
        {
            return padLeft(string.Format("{0:0.0}°", value), padding, ' ');
        }

        public static string DegreeDetailed(double value)
        {
            return DegreeDetailed(value, 0);
        }

        public static string DegreeDetailed(double value, int padding)
        {
            string result = "";

            int remainder = (int)Math.Floor(value);
            result = result + remainder + "°"; // degree

            value = (value - remainder) * 60;
            remainder = (int)Math.Floor(value);
            result = result + padLeft(remainder, 2, '0') + "'"; // minutes

            value = (value - remainder) * 60;
            remainder = (int)Math.Floor(value);
            result = result + padLeft(remainder, 2, '0') + "\""; // seconds

            return padLeft(result, padding, ' ');
        }

        public static string KerbalTimespan(double value)
        {
            return KerbalTimespan(value, 6);
        }

        public static string KerbalTimespan(double value, int restriction)
        {
            int[] timeParts = kerbalTimeParts(value);
            string[] partUnits = new string[] { "ms", "s", "m", "h", "d", "y" };

            string result = "";
            for (int i = timeParts.Length - 1; i >= 0; i--)
            {
                if (result.Length > restriction - 3)
                    break;

                if (result != "" || timeParts[i] != 0)
                    result += " " + timeParts[i] + partUnits[i];
            }

            return result.Trim();
        }

        public static string MissionTime(double value)
        {
            int[] timeParts = kerbalTimeParts(value);
            if (timeParts[5] == 0 && timeParts[4] == 0)
                return (value < 0 ? "T-" : "T+") + KerbalTime(value);
            else
                return (value < 0 ? "T-" : "T+") + " " + KerbalDate(value) + " " + KerbalTime(value);

        }

        public static string KerbalTime(double value)
        {
            int[] timeParts = kerbalTimeParts(value);

            return padLeft(timeParts[3], 2, '0') + ":" + padLeft(timeParts[2], 2, '0') + ":" + padLeft(timeParts[1], 2, '0');
        }

        public static string KerbalDate(double value)
        {
            int[] timeParts = kerbalTimeParts(value);

            return padLeft(timeParts[5] + 1, 2, '0') + "/" + padLeft(timeParts[4] + 1, 3, '0');
        }

        public static string KerbalDateTime(double value)
        {
            return KerbalDate(value) + " " + KerbalTime(value);
        }

        private static double MathFractalPart(double value)
        {
            return value - (int)value;
        }

        public static string Distance(double value)
        {
            return Distance(value, 0);
        }

        public static string Distance(double value, int padding)
        {
            return pretty(value, padding, 9460730472580800, "ly", 149597870691, "AU", 299792458, "ls", 1000, "km", 1, "m", 0.001, "mm");
        }

        public static string Mass(double value)
        {
            return Mass(value, 0);
        }

        public static string Mass(double value, int padding)
        {
            return pretty(value, padding, 1000000000, "Mt", 1000000, "Kt", 1000, "t", 1, "kg", 0.001, "g", 0.000001, "mg");
        }

        public static string Charge(double value)
        {
            return Charge(value, 0);
        }

        public static string Charge(double value, int padding)
        {
            return pretty(value, padding, 1000000000, "GAh", 1000000, "MAh", 1000, "KAh", 1, "Ah", 0.001, "mAh");
        }

        public static string Acceleration(double value)
        {
            return Acceleration(value, 0);
        }

        public static string Acceleration(double value, int padding)
        {
            return Distance(value, padding - 3) + "/s²";
        }

        public static string Speed(double value)
        {
            return Speed(value, 0);
        }

        public static string Speed(double value, int padding)
        {
            return Distance(value, padding - 2) + "/s";
        }

        private static string pretty(double value, int padding, params object[] unitFactors)
        {
            for (int i = 0; i < unitFactors.Length; i += 2)
                if (Math.Abs(value) > Convert.ToDouble(unitFactors[i]))
                    return pretty(value / Convert.ToDouble(unitFactors[i]), unitFactors[i + 1].ToString(), padding);
            return pretty(value / Convert.ToDouble(unitFactors[unitFactors.Length - 2]), unitFactors[unitFactors.Length - 1].ToString(), padding);
        }

        private static string pretty(double value, string unit, int padding)
        {
            if (value < -99.5 || value >= 99.5)
                return padLeft((int)Math.Round(value), padding, ' ') + unit;
            else
                return padLeft(String.Format("{0:0.00}", value), padding, ' ') + unit;
        }

        private static int[] kerbalTimeParts(double value)
        {
            int[] factors = kerbinTimeFactors();
            int[] result = new int[factors.Length];

            double remainder = value;
            int scope = 0;
            for (int i = factors.Length - 1; i >= 1; i--)
            {
                scope = (int)Math.Floor(remainder / factors[i]);
                result[i] = scope;
                remainder = remainder - scope * factors[i];
            }

            result[0] = (int)Math.Round(MathFractalPart(remainder) * 1000);
            return result;
        }

        private static int[] kerbinTimeFactors()
        {
            return new int[] { 0, 1, 60, 3600, 21600, 9203400 };
        }

        private static string padLeft(object text, int totalWidth, char paddingChar)
        {
            if (totalWidth <= 0 || totalWidth <= text.ToString().Length)
                return text.ToString();

            return text.ToString().PadLeft(totalWidth, paddingChar);
        }

        public static double KerbinTimespanTotalSeconds(int years, int days, int hours, int minutes, int seconds)
        {
            int[] factors = kerbinTimeFactors();
            return factors[5] * years + factors[4] * days + factors[3] * hours + factors[2] * minutes + factors[1] * seconds;
        }
    }
}
