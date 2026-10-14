using System;
using System.Collections.Generic;
using MiniFactory.Core.Boost;
using MiniFactory.Core.Config;
using MiniFactory.Core.Economy;
using MiniFactory.Core.Iap;
using MiniFactory.Core.Machines;
using MiniFactory.Core.Offline;
using MiniFactory.Core.Save;
using MiniFactory.Game.Lifecycle;
using MiniFactory.Game.Services;
using MiniFactory.UI.Presenters;
using MiniFactory.UI.Views;
using UnityEngine;
using VContainer;

namespace MiniFactory.App
{
    public sealed class GameBootstrap : MonoBehaviour
    {
        private const double AutoSaveInterval = 30.0;

        [SerializeField] private GameLifetimeScope _scope;
        [SerializeField] private TopBarView _topBarView;
        [SerializeField] private BoostView _boostView;
        [SerializeField] private BuyCurrencyView _buyCurrencyView;
        [SerializeField] private MachineView[] _machineViews;
        [SerializeField] private AppLifecycleObserver _lifecycleObserver;

        private readonly List<MachinePresenter> _machinePresenters = new List<MachinePresenter>();

        private EconomyModel _economyModel;
        private EconomyService _economyService;
        private BoostModel _boostModel;
        private BoostService _boostService;
        private OfflineService _offlineService;
        private TopBarPresenter _topBarPresenter;
        private BoostPresenter _boostPresenter;
        private BuyCurrencyPresenter _buyCurrencyPresenter;
        private SaveService _saveService;
        private IIapService _iapService;
        private GameConfig _config;

        private bool _isPaused;
        private double _autoSaveElapsed;

        private void Start()
        {
            _economyModel = _scope.Container.Resolve<EconomyModel>();
            _economyService = _scope.Container.Resolve<EconomyService>();
            _boostModel = _scope.Container.Resolve<BoostModel>();
            _boostService = _scope.Container.Resolve<BoostService>();
            _offlineService = _scope.Container.Resolve<OfflineService>();
            _saveService = _scope.Container.Resolve<SaveService>();
            _iapService = _scope.Container.Resolve<IIapService>();
            _config = _scope.Container.Resolve<GameConfig>();

            _offlineService.OfflineApplied += OnOfflineApplied;

            if (_lifecycleObserver != null)
            {
                _lifecycleObserver.Paused += OnPaused;
                _lifecycleObserver.Resumed += OnResumed;
                _lifecycleObserver.Quitting += OnQuitting;
            }

            _saveService.Load(Initialize);
            _iapService.InitializeAsync();
        }

        private void Update()
        {
            if (_economyService == null || _isPaused)
            {
                return;
            }

            double deltaTime = Time.deltaTime;
            _boostService.Tick(deltaTime);
            _economyService.Tick(deltaTime);

            if (_topBarPresenter != null)
            {
                _topBarPresenter.Tick(deltaTime);
            }

            _autoSaveElapsed += deltaTime;

            if (_autoSaveElapsed >= AutoSaveInterval)
            {
                _autoSaveElapsed = 0.0;
                Save();
            }
        }

        private void Initialize(SaveData saveData)
        {
            ApplySave(saveData);

            if (saveData != null)
            {
                _offlineService.Apply(GetElapsedSeconds());
            }

            _topBarPresenter = new TopBarPresenter(_topBarView, _economyModel, _boostModel);
            _topBarPresenter.Bind();

            _boostPresenter = new BoostPresenter(_boostView, _boostModel, _boostService);
            _boostPresenter.Bind();

            if (_buyCurrencyView != null)
            {
                _buyCurrencyPresenter = new BuyCurrencyPresenter(_buyCurrencyView, _iapService, _config);
                _buyCurrencyPresenter.Bind();
            }

            _machinePresenters.Clear();

            for (int i = 0; i < _machineViews.Length; i++)
            {
                MachineView view = _machineViews[i];

                if (view == null || view.Config == null)
                {
                    continue;
                }

                MachineModel model = FindModel(view.Config.id);

                if (model == null)
                {
                    continue;
                }

                MachinePresenter presenter = new MachinePresenter(view, model, _economyService, _boostModel, Save);
                presenter.Bind();
                _machinePresenters.Add(presenter);
            }

            Save();
        }

        private void ApplySave(SaveData saveData)
        {
            if (saveData == null)
            {
                return;
            }

            _economyModel.AddIncome(saveData.balance);

            IReadOnlyList<MachineModel> machines = _economyModel.Machines;

            for (int i = 0; i < machines.Count; i++)
            {
                MachineModel machine = machines[i];

                if (saveData.TryGetMachine(machine.Id, out MachineSaveData machineData))
                {
                    machine.RestoreState((MachineState)machineData.state, machineData.level);
                }
            }

            _boostModel.RestoreState(saveData.boostHasBoost, saveData.boostIsActive, saveData.boostRemainingSeconds);
        }

        private MachineModel FindModel(string id)
        {
            IReadOnlyList<MachineModel> machines = _economyModel.Machines;

            for (int i = 0; i < machines.Count; i++)
            {
                if (machines[i].Id == id)
                {
                    return machines[i];
                }
            }

            return null;
        }

        private double GetElapsedSeconds()
        {
            long now = DateTime.UtcNow.Ticks;
            long last = _saveService.LastSaveTimeUtcTicks;

            if (last <= 0L || now <= last)
            {
                return 0.0;
            }

            return (now - last) / (double)TimeSpan.TicksPerSecond;
        }

        private void Save()
        {
            if (_economyModel == null || _saveService == null)
            {
                return;
            }

            _saveService.Save(_economyModel.Balance, _economyModel.Machines, _boostModel);
        }

        private void OnPaused()
        {
            _isPaused = true;
            Save();
        }

        private void OnResumed()
        {
            _isPaused = false;
            _offlineService.Apply(GetElapsedSeconds());
            Save();
        }

        private void OnQuitting()
        {
            Save();
        }

        private void OnOfflineApplied(OfflineProgressResult result)
        {
            Debug.Log("Offline income applied: +" + result.Income + " (" + result.AppliedSeconds + "s, capped=" + result.WasCapped + ")");
        }

        private void Unbind()
        {
            for (int i = 0; i < _machinePresenters.Count; i++)
            {
                _machinePresenters[i].Unbind();
            }

            _machinePresenters.Clear();

            if (_topBarPresenter != null)
            {
                _topBarPresenter.Unbind();
                _topBarPresenter = null;
            }

            if (_boostPresenter != null)
            {
                _boostPresenter.Unbind();
                _boostPresenter = null;
            }

            if (_buyCurrencyPresenter != null)
            {
                _buyCurrencyPresenter.Unbind();
                _buyCurrencyPresenter = null;
            }
        }

        private void OnDestroy()
        {
            Unbind();

            if (_offlineService != null)
            {
                _offlineService.OfflineApplied -= OnOfflineApplied;
            }

            if (_lifecycleObserver != null)
            {
                _lifecycleObserver.Paused -= OnPaused;
                _lifecycleObserver.Resumed -= OnResumed;
                _lifecycleObserver.Quitting -= OnQuitting;
            }
        }
    }
}
