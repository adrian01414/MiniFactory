using System;
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

        public PurchaseService(IIapService iapService, EconomyModel economyModel, GameConfig config)
        {
            _iapService = iapService ?? throw new ArgumentNullException(nameof(iapService));
            _economyModel = economyModel ?? throw new ArgumentNullException(nameof(economyModel));
            _config = config ?? throw new ArgumentNullException(nameof(config));

            _iapService.PurchaseSucceeded += OnPurchaseSucceeded;
        }

        private void OnPurchaseSucceeded(string productId)
        {
            if (_config.TryGetProduct(productId, out ProductConfig product))
            {
                _economyModel.AddIncome(product.coins);
            }
        }
    }
}
