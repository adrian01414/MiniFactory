using System;
using MiniFactory.Core.Config;
using MiniFactory.Core.Formatting;
using MiniFactory.Core.Iap;
using MiniFactory.UI.Views;

namespace MiniFactory.UI.Presenters
{
    public sealed class BuyCurrencyPresenter
    {
        private readonly BuyCurrencyView _view;
        private readonly IIapService _iapService;
        private readonly GameConfig _config;

        public BuyCurrencyPresenter(BuyCurrencyView view, IIapService iapService, GameConfig config)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            _iapService = iapService ?? throw new ArgumentNullException(nameof(iapService));
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public void Bind()
        {
            _view.BuyClicked += OnBuyClicked;
            _iapService.Initialized += OnInitialized;
            Refresh();
        }

        public void Unbind()
        {
            _view.BuyClicked -= OnBuyClicked;
            _iapService.Initialized -= OnInitialized;
        }

        private void OnBuyClicked()
        {
            _iapService.Purchase(ProductIds.CoinsPackSmall);
        }

        private void OnInitialized()
        {
            Refresh();
        }

        private void Refresh()
        {
            _view.SetInteractable(_iapService.IsInitialized && _iapService.IsAvailable);
            _view.SetText(BuildText());
        }

        private string BuildText()
        {
            if (_config.TryGetProduct(ProductIds.CoinsPackSmall, out ProductConfig product))
            {
                return "Buy " + NumberFormatter.Format(product.coins) + " coins";
            }

            return "Buy coins";
        }
    }
}
