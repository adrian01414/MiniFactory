using System.Collections.Generic;
using MiniFactory.Core.Economy;
using MiniFactory.Core.Machines;
using MiniFactory.Core.Save;
using MiniFactory.Game.Services;
using MiniFactory.UI.Presenters;
using MiniFactory.UI.Views;
using UnityEngine;
using VContainer;

namespace MiniFactory.App
{
    public sealed class GameBootstrap : MonoBehaviour
    {
        [SerializeField] private GameLifetimeScope _scope;
        [SerializeField] private TopBarView _topBarView;
        [SerializeField] private MachineView[] _machineViews;

        private readonly List<MachinePresenter> _machinePresenters = new List<MachinePresenter>();

        private EconomyModel _economyModel;
        private EconomyService _economyService;
        private TopBarPresenter _topBarPresenter;
        private SaveService _saveService;

        private void Start()
        {
            _economyModel = _scope.Container.Resolve<EconomyModel>();
            _economyService = _scope.Container.Resolve<EconomyService>();
            _saveService = _scope.Container.Resolve<SaveService>();
            _saveService.Load(Initialize);
        }

        private void Update()
        {
            if (_economyService == null)
            {
                return;
            }

            double deltaTime = Time.deltaTime;
            _economyService.Tick(deltaTime);

            if (_topBarPresenter != null)
            {
                _topBarPresenter.Tick(deltaTime);
            }
        }

        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                Save();
            }
        }

        private void OnApplicationFocus(bool focus)
        {
            if (!focus)
            {
                Save();
            }
        }

        private void OnApplicationQuit()
        {
            Save();
        }

        private void OnDestroy()
        {
            Unbind();
        }

        private void Initialize(SaveData saveData)
        {
            ApplySave(saveData);

            _topBarPresenter = new TopBarPresenter(_topBarView, _economyModel);
            _topBarPresenter.Bind();

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

                MachinePresenter presenter = new MachinePresenter(view, model, _economyService, Save);
                presenter.Bind();
                _machinePresenters.Add(presenter);
            }
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

        private void Save()
        {
            if (_economyModel == null || _saveService == null)
            {
                return;
            }

            _saveService.Save(_economyModel.Balance, _economyModel.Machines);
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
        }
    }
}
