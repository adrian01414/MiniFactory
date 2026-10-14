using System;
using MiniFactory.Core.Boost;
using MiniFactory.Core.Economy;
using MiniFactory.Core.Formatting;
using MiniFactory.UI.Views;

namespace MiniFactory.UI.Presenters
{
    public sealed class TopBarPresenter
    {
        private const double RefreshInterval = 0.1;

        private readonly TopBarView _view;
        private readonly EconomyModel _economyModel;
        private readonly BoostModel _boostModel;

        private double _elapsed;
        private bool _balanceDirty;
        private bool _productionDirty;

        public TopBarPresenter(TopBarView view, EconomyModel economyModel, BoostModel boostModel)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            _economyModel = economyModel ?? throw new ArgumentNullException(nameof(economyModel));
            _boostModel = boostModel ?? throw new ArgumentNullException(nameof(boostModel));
        }

        public void Bind()
        {
            _economyModel.BalanceChanged += OnBalanceChanged;
            _economyModel.TotalProductionChanged += OnTotalProductionChanged;
            _boostModel.ActiveChanged += OnBoostActiveChanged;
            RefreshBalance();
            RefreshProduction();
        }

        public void Unbind()
        {
            _economyModel.BalanceChanged -= OnBalanceChanged;
            _economyModel.TotalProductionChanged -= OnTotalProductionChanged;
            _boostModel.ActiveChanged -= OnBoostActiveChanged;
        }

        public void Tick(double deltaTime)
        {
            if (!_balanceDirty && !_productionDirty)
            {
                return;
            }

            _elapsed += deltaTime;

            if (_elapsed < RefreshInterval)
            {
                return;
            }

            _elapsed = 0.0;

            if (_balanceDirty)
            {
                _balanceDirty = false;
                RefreshBalance();
            }

            if (_productionDirty)
            {
                _productionDirty = false;
                RefreshProduction();
            }
        }

        private void OnBalanceChanged(double balance)
        {
            _balanceDirty = true;
        }

        private void OnTotalProductionChanged(double totalProduction)
        {
            _productionDirty = true;
        }

        private void OnBoostActiveChanged(bool isActive)
        {
            _productionDirty = true;
        }

        private void RefreshBalance()
        {
            _view.SetBalanceText(NumberFormatter.Format(_economyModel.Balance));
        }

        private void RefreshProduction()
        {
            _view.SetTotalProductionText(NumberFormatter.Format(_economyModel.TotalProduction * _boostModel.ActiveMultiplier) + "/s");
        }
    }
}
