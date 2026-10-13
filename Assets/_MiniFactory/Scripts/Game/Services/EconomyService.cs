using System;
using MiniFactory.Core.Economy;
using MiniFactory.Core.Machines;

namespace MiniFactory.Game.Services
{
    public sealed class EconomyService
    {
        public event Action<MachineModel> MachineUnlocked;
        public event Action<MachineModel> MachineUpgraded;

        private readonly EconomyModel _economyModel;

        public EconomyService(EconomyModel economyModel)
        {
            _economyModel = economyModel ?? throw new ArgumentNullException(nameof(economyModel));
        }

        public void Tick(double deltaSeconds)
        {
            if (deltaSeconds <= 0.0)
            {
                return;
            }

            double income = EconomyCalculator.GetIncome(_economyModel.TotalProduction, 1.0, deltaSeconds);

            if (income > 0.0)
            {
                _economyModel.AddIncome(income);
            }
        }

        public bool TryUnlock(MachineModel machine)
        {
            if (machine == null || !machine.CanUnlock)
            {
                return false;
            }

            if (!_economyModel.TrySpend(machine.UnlockCost))
            {
                return false;
            }

            machine.Unlock();
            MachineUnlocked?.Invoke(machine);
            return true;
        }

        public bool TryUpgrade(MachineModel machine)
        {
            if (machine == null || !machine.IsUnlocked)
            {
                return false;
            }

            if (!_economyModel.TrySpend(machine.NextUpgradeCost))
            {
                return false;
            }

            machine.Upgrade();
            MachineUpgraded?.Invoke(machine);
            return true;
        }
    }
}
