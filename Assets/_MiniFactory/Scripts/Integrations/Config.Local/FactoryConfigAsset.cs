using System;
using MiniFactory.Core.Config;
using UnityEngine;

namespace MiniFactory.Config.Local
{
    [CreateAssetMenu(menuName = "MiniFactory/Factory Config", fileName = "FactoryConfig")]
    public sealed class FactoryConfigAsset : ScriptableObject
    {
        public double offlineCapSeconds = 3600.0;
        public bool boostEnabled = true;
        public double boostDurationSeconds = 30.0;
        public double boostMultiplier = 2.0;
        public MachineConfigAsset[] machines = Array.Empty<MachineConfigAsset>();

        public GameConfig ToData()
        {
            return new GameConfig
            {
                offlineCapSeconds = offlineCapSeconds,
                boostEnabled = boostEnabled,
                boostDurationSeconds = boostDurationSeconds,
                boostMultiplier = boostMultiplier,
                machines = BuildMachineData()
            };
        }

        private MachineConfig[] BuildMachineData()
        {
            if (machines == null)
            {
                return Array.Empty<MachineConfig>();
            }

            MachineConfig[] result = new MachineConfig[machines.Length];
            for (int i = 0; i < machines.Length; i++)
            {
                result[i] = machines[i] != null ? machines[i].ToData() : null;
            }

            return result;
        }
    }
}
