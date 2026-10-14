using System.Globalization;

namespace MiniFactory.Core.Formatting
{
    public static class NumberFormatter
    {
        private static readonly string[] Suffixes = { string.Empty, "K", "M", "B", "T", "Qa", "Qi" };

        public static string Format(double value)
        {
            if (value <= 0.0 || double.IsNaN(value) || double.IsInfinity(value))
            {
                return "0";
            }

            if (value < 1000.0)
            {
                return value.ToString("0.#", CultureInfo.InvariantCulture);
            }

            int index = 0;
            double scaled = value;

            while (scaled >= 1000.0 && index < Suffixes.Length - 1)
            {
                scaled /= 1000.0;
                index++;
            }

            return scaled.ToString("0.##", CultureInfo.InvariantCulture) + Suffixes[index];
        }
    }
}
