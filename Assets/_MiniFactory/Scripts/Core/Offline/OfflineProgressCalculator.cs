using System;

namespace MiniFactory.Core.Offline
{
    public static class OfflineProgressCalculator
    {
        public static OfflineProgressResult Calculate(double elapsedSeconds, double maxOfflineSeconds, double totalProduction, bool boostActive, double boostRemainingSeconds, double boostMultiplier)
        {
            if (elapsedSeconds < 0.0) { throw new ArgumentOutOfRangeException(nameof(elapsedSeconds)); }
            if (maxOfflineSeconds < 0.0) { throw new ArgumentOutOfRangeException(nameof(maxOfflineSeconds)); }
            if (totalProduction < 0.0) { throw new ArgumentOutOfRangeException(nameof(totalProduction)); }
            if (boostRemainingSeconds < 0.0) { throw new ArgumentOutOfRangeException(nameof(boostRemainingSeconds)); }
            if (boostMultiplier < 0.0) { throw new ArgumentOutOfRangeException(nameof(boostMultiplier)); }

            bool wasCapped = elapsedSeconds > maxOfflineSeconds;
            double appliedSeconds = wasCapped ? maxOfflineSeconds : elapsedSeconds;

            double boostConsumedSeconds = 0.0;
            double income;
            if (boostActive && boostRemainingSeconds > 0.0)
            {
                boostConsumedSeconds = appliedSeconds < boostRemainingSeconds ? appliedSeconds : boostRemainingSeconds;
                double normalSeconds = appliedSeconds - boostConsumedSeconds;
                income = totalProduction * boostMultiplier * boostConsumedSeconds + totalProduction * normalSeconds;
            }
            else
            {
                income = totalProduction * appliedSeconds;
            }

            return new OfflineProgressResult(elapsedSeconds, appliedSeconds, income, boostConsumedSeconds, wasCapped);
        }
    }
}
