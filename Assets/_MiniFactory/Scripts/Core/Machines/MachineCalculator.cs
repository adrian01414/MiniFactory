using System;
using MiniFactory.Core.Config;

namespace MiniFactory.Core.Machines
{
    public static class MachineCalculator
    {
        public static double GetProduction(MachineConfig config, int level)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            if (level <= 0)
            {
                return 0.0;
            }

            return config.baseProduction * Math.Pow(config.productionGrowth, level - 1);
        }

        public static double GetUpgradeCost(MachineConfig config, int level)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            if (level <= 0)
            {
                return 0.0;
            }

            return config.baseUpgradeCost * Math.Pow(config.upgradeGrowth, level - 1);
        }
    }
}
