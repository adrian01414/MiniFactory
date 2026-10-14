using System;
using System.Collections.Generic;
using MiniFactory.Core.Analytics;
using MiniFactory.Core.Boost;
using MiniFactory.Core.Config;
using MiniFactory.Core.Economy;
using MiniFactory.Core.Offline;

namespace MiniFactory.Game.Services
{
    public sealed class OfflineService
    {
        public event Action<OfflineProgressResult> OfflineApplied;

        private readonly EconomyModel _economyModel;
        private readonly BoostModel _boostModel;
        private readonly GameConfig _config;
        private readonly AnalyticsService _analyticsService;

        public OfflineService(EconomyModel economyModel, BoostModel boostModel, GameConfig config, AnalyticsService analyticsService)
        {
            _economyModel = economyModel ?? throw new ArgumentNullException(nameof(economyModel));
            _boostModel = boostModel ?? throw new ArgumentNullException(nameof(boostModel));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _analyticsService = analyticsService ?? throw new ArgumentNullException(nameof(analyticsService));
        }

        public OfflineProgressResult Apply(double elapsedSeconds)
        {
            OfflineProgressResult result = OfflineProgressCalculator.Calculate(
                elapsedSeconds,
                _config.offlineCapSeconds,
                _economyModel.TotalProduction,
                _boostModel.IsActive,
                _boostModel.RemainingSeconds,
                _boostModel.Multiplier);

            if (result.Income > 0.0)
            {
                _economyModel.AddIncome(result.Income);
            }

            if (result.BoostConsumedSeconds > 0.0)
            {
                _boostModel.Tick(result.BoostConsumedSeconds);
            }

            OfflineApplied?.Invoke(result);

            if (result.AppliedSeconds > 0.0)
            {
                _analyticsService.Track(AnalyticsEventNames.OfflineIncomeApplied, new Dictionary<string, object>
                {
                    { AnalyticsParams.ElapsedSeconds, result.AppliedSeconds },
                    { AnalyticsParams.Income, result.Income },
                    { AnalyticsParams.WasCapped, result.WasCapped }
                });
            }

            return result;
        }
    }
}
