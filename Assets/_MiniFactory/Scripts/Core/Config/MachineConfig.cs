using System;

namespace MiniFactory.Core.Config
{
    [Serializable]
    public sealed class MachineConfig
    {
        public string id = string.Empty;
        public string displayName = string.Empty;
        public double baseProduction = 1.0;
        public double productionGrowth = 1.15;
        public double baseUpgradeCost = 10.0;
        public double upgradeGrowth = 1.25;
        public double unlockCost = 0.0;

        public string DisplayName => string.IsNullOrEmpty(displayName) ? id : displayName;

        public bool RequiresUnlock => unlockCost > 0.0;
    }
}
