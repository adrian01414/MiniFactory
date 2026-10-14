using System;
using System.Collections.Generic;
using MiniFactory.Core.Analytics;
using MiniFactory.Core.Boost;
using MiniFactory.Core.Economy;

namespace MiniFactory.Game.Services
{
    public sealed class BoostService
    {
        public event Action BoostPurchased;
        public event Action BoostActivated;
        public event Action BoostFinished;

        private readonly BoostModel _boostModel;
        private readonly EconomyModel _economyModel;
        private readonly AnalyticsService _analyticsService;

        public BoostService(BoostModel boostModel, EconomyModel economyModel, AnalyticsService analyticsService)
        {
            _boostModel = boostModel ?? throw new ArgumentNullException(nameof(boostModel));
            _economyModel = economyModel ?? throw new ArgumentNullException(nameof(economyModel));
            _analyticsService = analyticsService ?? throw new ArgumentNullException(nameof(analyticsService));
        }

        public bool TryBuy()
        {
            if (!_boostModel.CanBuy)
            {
                return false;
            }

            if (!_economyModel.TrySpend(_boostModel.Cost))
            {
                return false;
            }

            _boostModel.GrantBoost();
            BoostPurchased?.Invoke();
            return true;
        }

        public bool TryActivate()
        {
            if (!_boostModel.CanActivate)
            {
                return false;
            }

            _boostModel.Activate();
            BoostActivated?.Invoke();
            _analyticsService.Track(AnalyticsEventNames.BoostStarted, new Dictionary<string, object>
            {
                { AnalyticsParams.Duration, _boostModel.DurationSeconds },
                { AnalyticsParams.Multiplier, _boostModel.Multiplier }
            });
            return true;
        }

        public void Tick(double deltaSeconds)
        {
            bool wasActive = _boostModel.IsActive;
            _boostModel.Tick(deltaSeconds);

            if (wasActive && !_boostModel.IsActive)
            {
                BoostFinished?.Invoke();
                _analyticsService.Track(AnalyticsEventNames.BoostFinished, new Dictionary<string, object>
                {
                    { AnalyticsParams.Reason, "expired" },
                    { AnalyticsParams.Elapsed, _boostModel.DurationSeconds }
                });
            }
        }
    }
}
