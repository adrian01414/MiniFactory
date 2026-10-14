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
        public double boostCost = 250.0;
        public MachineConfig[] machines = Array.Empty<MachineConfig>();
        public ProductConfig[] products = Array.Empty<ProductConfig>();

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

        public bool TryGetProduct(string id, out ProductConfig product)
        {
            product = null;

            if (string.IsNullOrEmpty(id) || products == null)
            {
                return false;
            }

            for (int i = 0; i < products.Length; i++)
            {
                ProductConfig candidate = products[i];
                if (candidate != null && candidate.id == id)
                {
                    product = candidate;
                    return true;
                }
            }

            return false;
        }
    }
}
