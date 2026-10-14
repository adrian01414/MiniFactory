using System;
using System.Globalization;
using MiniFactory.Core.Boost;
using MiniFactory.Core.Formatting;
using MiniFactory.Game.Services;
using MiniFactory.UI.Views;

namespace MiniFactory.UI.Presenters
{
    public sealed class BoostPresenter
    {
        private readonly BoostView _view;
        private readonly BoostModel _boostModel;
        private readonly BoostService _boostService;

        public BoostPresenter(BoostView view, BoostModel boostModel, BoostService boostService)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            _boostModel = boostModel ?? throw new ArgumentNullException(nameof(boostModel));
            _boostService = boostService ?? throw new ArgumentNullException(nameof(boostService));
        }

        public void Bind()
        {
            _view.BoostClicked += OnBoostClicked;
            _view.BuyClicked += OnBuyClicked;
            _boostModel.HasBoostChanged += OnHasBoostChanged;
            _boostModel.ActiveChanged += OnActiveChanged;
            _boostModel.RemainingSecondsChanged += OnRemainingSecondsChanged;
            Refresh();
        }

        public void Unbind()
        {
            _view.BoostClicked -= OnBoostClicked;
            _view.BuyClicked -= OnBuyClicked;
            _boostModel.HasBoostChanged -= OnHasBoostChanged;
            _boostModel.ActiveChanged -= OnActiveChanged;
            _boostModel.RemainingSecondsChanged -= OnRemainingSecondsChanged;
        }

        private void OnBoostClicked()
        {
            _boostService.TryActivate();
        }

        private void OnBuyClicked()
        {
            _boostService.TryBuy();
        }

        private void OnHasBoostChanged(bool hasBoost)
        {
            Refresh();
        }

        private void OnActiveChanged(bool isActive)
        {
            Refresh();
        }

        private void OnRemainingSecondsChanged(double remainingSeconds)
        {
            RefreshBoostText();
        }

        private void Refresh()
        {
            _view.SetBoostInteractable(_boostModel.CanActivate);
            _view.SetBuyInteractable(_boostModel.CanBuy);
            RefreshBoostText();
            RefreshBuyText();
        }

        private void RefreshBoostText()
        {
            _view.SetBoostText(_boostModel.IsActive ? FormatCountdown(_boostModel.RemainingSeconds) : "Boost");
        }

        private void RefreshBuyText()
        {
            _view.SetBuyText($"Buy Boost: {NumberFormatter.Format(_boostModel.Cost)}$");
        }

        private static string FormatCountdown(double seconds)
        {
            return Math.Ceiling(seconds).ToString("0", CultureInfo.InvariantCulture) + "s";
        }
    }
}
