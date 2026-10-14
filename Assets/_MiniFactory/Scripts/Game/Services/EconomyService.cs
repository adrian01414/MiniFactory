using System;
using System.Collections.Generic;
using MiniFactory.Core.Analytics;
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
        private readonly AnalyticsService _analyticsService;

        public EconomyService(EconomyModel economyModel, BoostModel boostModel, AnalyticsService analyticsService)
        {
            _economyModel = economyModel ?? throw new ArgumentNullException(nameof(economyModel));
            _boostModel = boostModel ?? throw new ArgumentNullException(nameof(boostModel));
            _analyticsService = analyticsService ?? throw new ArgumentNullException(nameof(analyticsService));
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

            double cost = machine.UnlockCost;

            if (!_economyModel.TrySpend(cost))
            {
                return false;
            }

            machine.Unlock();
            MachineUnlocked?.Invoke(machine);
            _analyticsService.Track(AnalyticsEventNames.MachineUnlocked, new Dictionary<string, object>
            {
                { AnalyticsParams.MachineId, machine.Id },
                { AnalyticsParams.Level, machine.Level },
                { AnalyticsParams.Cost, cost }
            });
            return true;
        }

        public bool TryUpgrade(MachineModel machine)
        {
            if (machine == null || !machine.IsUnlocked)
            {
                return false;
            }

            double cost = machine.NextUpgradeCost;

            if (!_economyModel.TrySpend(cost))
            {
                return false;
            }

            machine.Upgrade();
            MachineUpgraded?.Invoke(machine);
            _analyticsService.Track(AnalyticsEventNames.MachineUpgraded, new Dictionary<string, object>
            {
                { AnalyticsParams.MachineId, machine.Id },
                { AnalyticsParams.Level, machine.Level },
                { AnalyticsParams.Cost, cost }
            });
            return true;
        }
    }
}
