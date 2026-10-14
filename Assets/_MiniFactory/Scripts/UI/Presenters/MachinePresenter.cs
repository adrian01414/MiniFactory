using System;
using MiniFactory.Core.Boost;
using MiniFactory.Core.Formatting;
using MiniFactory.Core.Machines;
using MiniFactory.Game.Services;
using MiniFactory.UI.Views;

namespace MiniFactory.UI.Presenters
{
    public sealed class MachinePresenter
    {
        private readonly MachineView _view;
        private readonly MachineModel _model;
        private readonly EconomyService _economyService;
        private readonly BoostModel _boostModel;
        private readonly Action _onChanged;

        public MachinePresenter(MachineView view, MachineModel model, EconomyService economyService, BoostModel boostModel, Action onChanged)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            _model = model ?? throw new ArgumentNullException(nameof(model));
            _economyService = economyService ?? throw new ArgumentNullException(nameof(economyService));
            _boostModel = boostModel ?? throw new ArgumentNullException(nameof(boostModel));
            _onChanged = onChanged;
        }

        public void Bind()
        {
            _view.ActionClicked += OnActionClicked;
            _model.StateChanged += OnModelStateChanged;
            _model.LevelChanged += OnModelLevelChanged;
            _boostModel.ActiveChanged += OnBoostActiveChanged;
            Refresh();
        }

        public void Unbind()
        {
            _view.ActionClicked -= OnActionClicked;
            _model.StateChanged -= OnModelStateChanged;
            _model.LevelChanged -= OnModelLevelChanged;
            _boostModel.ActiveChanged -= OnBoostActiveChanged;
        }

        public void Refresh()
        {
            _view.SetLockVisible(!_model.IsUnlocked);

            if (_model.IsUnlocked)
            {
                _view.SetLevelText("Lv " + _model.Level);
                _view.SetProductivityText(NumberFormatter.Format(_model.Production * _boostModel.ActiveMultiplier) + "/s");
                _view.SetActionText("Upgrade " + NumberFormatter.Format(_model.NextUpgradeCost));
            }
            else
            {
                _view.SetLevelText(string.Empty);
                _view.SetProductivityText(string.Empty);
                _view.SetActionText("Unlock " + NumberFormatter.Format(_model.UnlockCost));
            }
        }

        private void OnActionClicked()
        {
            bool changed = _model.IsUnlocked
                ? _economyService.TryUpgrade(_model)
                : _economyService.TryUnlock(_model);

            if (changed)
            {
                _onChanged?.Invoke();
            }
        }

        private void OnModelStateChanged(MachineState state)
        {
            Refresh();
        }

        private void OnModelLevelChanged(int level)
        {
            Refresh();
        }

        private void OnBoostActiveChanged(bool isActive)
        {
            Refresh();
        }
    }
}
