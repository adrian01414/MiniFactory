using System;
using MiniFactory.Core.Machines;

namespace MiniFactory.Core.Economy
{
    public static class EconomyCalculator
    {
        public static double GetTotalProduction(MachineModel[] machines)
        {
            if (machines == null) { throw new ArgumentNullException(nameof(machines)); }
            double total = 0.0;
            for (int i = 0; i < machines.Length; i++)
            {
                MachineModel machine = machines[i];
                if (machine != null && machine.IsUnlocked) { total += machine.Production; }
            }
            return total;
        }

        public static double GetIncome(double totalProduction, double boostMultiplier, double deltaSeconds)
        {
            if (totalProduction < 0.0) { throw new ArgumentOutOfRangeException(nameof(totalProduction)); }
            if (boostMultiplier < 0.0) { throw new ArgumentOutOfRangeException(nameof(boostMultiplier)); }
            if (deltaSeconds < 0.0) { throw new ArgumentOutOfRangeException(nameof(deltaSeconds)); }
            return totalProduction * boostMultiplier * deltaSeconds;
        }
    }
}
