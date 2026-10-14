using MiniFactory.Core.Config;
using UnityEngine;

namespace MiniFactory.Config.Local
{
    [CreateAssetMenu(menuName = "MiniFactory/Machine Config", fileName = "MachineConfig")]
    public sealed class MachineConfigAsset : ScriptableObject
    {
        public string id = string.Empty;
        public string displayName = string.Empty;
        public double baseProduction = 1.0;
        public double productionGrowth = 1.15;
        public double baseUpgradeCost = 10.0;
        public double upgradeGrowth = 1.25;
        public double unlockCost = 0.0;

        public string DisplayName => string.IsNullOrEmpty(displayName) ? id : displayName;

        public MachineConfig ToData()
        {
            return new MachineConfig
            {
                id = id,
                displayName = displayName,
                baseProduction = baseProduction,
                productionGrowth = productionGrowth,
                baseUpgradeCost = baseUpgradeCost,
                upgradeGrowth = upgradeGrowth,
                unlockCost = unlockCost
            };
        }
    }
}
