using System;

namespace MiniFactory.Core.Config
{
    [Serializable]
    public sealed class GameConfig
    {
        public double offlineCapSeconds = 3600.0;
        public bool boostEnabled = true;
        public double boostDurationSeconds = 30.0;
        public double boostMultiplier = 2.0;
        public MachineConfig[] machines = Array.Empty<MachineConfig>();

        public bool TryGetMachine(string id, out MachineConfig machine)
        {
            machine = null;

            if (string.IsNullOrEmpty(id) || machines == null)
            {
                return false;
            }

            for (int i = 0; i < machines.Length; i++)
            {
                MachineConfig candidate = machines[i];
                if (candidate != null && candidate.id == id)
                {
                    machine = candidate;
                    return true;
                }
            }

            return false;
        }
    }
}
