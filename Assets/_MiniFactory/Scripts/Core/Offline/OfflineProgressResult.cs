namespace MiniFactory.Core.Offline
{
    public readonly struct OfflineProgressResult
    {
        public readonly double ElapsedSeconds;
        public readonly double AppliedSeconds;
        public readonly double Income;
        public readonly double BoostConsumedSeconds;
        public readonly bool WasCapped;

        public OfflineProgressResult(double elapsedSeconds, double appliedSeconds, double income, double boostConsumedSeconds, bool wasCapped)
        {
            ElapsedSeconds = elapsedSeconds;
            AppliedSeconds = appliedSeconds;
            Income = income;
            BoostConsumedSeconds = boostConsumedSeconds;
            WasCapped = wasCapped;
        }
    }
}
