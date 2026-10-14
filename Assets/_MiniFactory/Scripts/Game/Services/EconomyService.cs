using System;
using MiniFactory.Core.Boost;
using MiniFactory.Core.Economy;
using MiniFactory.Core.Machines;

namespace MiniFactory.Game.Services
{
    public sealed class EconomyService
    {
        public event Action<MachineModel> MachineUnlocked;
        public event Action<MachineModel> MachineUpgraded;

        private readonly EconomyModel _economyModel;
        private readonly BoostModel _boostModel;

        public EconomyService(EconomyModel economyModel, BoostModel boostModel)
        {
            _economyModel = economyModel ?? throw new ArgumentNullException(nameof(economyModel));
            _boostModel = boostModel ?? throw new ArgumentNullException(nameof(boostModel));
        }

        public void Tick(double deltaSeconds)
        {
            if (deltaSeconds <= 0.0)
            {
                return;
            }

            double income = EconomyCalculator.GetIncome(_economyModel.TotalProduction, _boostModel.ActiveMultiplier, deltaSeconds);

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
