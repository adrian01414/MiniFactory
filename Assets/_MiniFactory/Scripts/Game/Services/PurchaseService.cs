using System;
using System.Collections.Generic;
using MiniFactory.Core.Analytics;
using MiniFactory.Core.Config;
using MiniFactory.Core.Economy;
using MiniFactory.Core.Iap;

namespace MiniFactory.Game.Services
{
    public sealed class PurchaseService
    {
        private readonly IIapService _iapService;
        private readonly EconomyModel _economyModel;
        private readonly GameConfig _config;
        private readonly AnalyticsService _analyticsService;

        public PurchaseService(IIapService iapService, EconomyModel economyModel, GameConfig config, AnalyticsService analyticsService)
        {
            _iapService = iapService ?? throw new ArgumentNullException(nameof(iapService));
            _economyModel = economyModel ?? throw new ArgumentNullException(nameof(economyModel));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _analyticsService = analyticsService ?? throw new ArgumentNullException(nameof(analyticsService));

            _iapService.PurchaseSucceeded += OnPurchaseSucceeded;
            _iapService.PurchaseFailed += OnPurchaseFailed;
        }

        private void OnPurchaseSucceeded(string productId)
        {
            double coins = 0.0;

            if (_config.TryGetProduct(productId, out ProductConfig product))
            {
                coins = product.coins;
                _economyModel.AddIncome(coins);
            }

            _analyticsService.Track(AnalyticsEventNames.PurchaseSucceeded, new Dictionary<string, object>
            {
                { AnalyticsParams.ProductId, productId },
                { AnalyticsParams.Coins, coins }
            });
        }

        private void OnPurchaseFailed(string productId, IapFailureReason reason)
        {
            _analyticsService.Track(AnalyticsEventNames.PurchaseFailed, new Dictionary<string, object>
            {
                { AnalyticsParams.ProductId, productId },
                { AnalyticsParams.Reason, reason.ToString() }
            });
        }
    }
}
