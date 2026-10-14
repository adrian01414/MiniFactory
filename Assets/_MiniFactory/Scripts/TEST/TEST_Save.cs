using MiniFactory.Core.Economy;
using MiniFactory.Game.Services;
using UnityEngine;
using VContainer;

namespace MiniFactory.App
{
    public sealed class TEST_Save : MonoBehaviour
    {
        private EconomyModel _economyModel;
        private SaveService _saveService;

        [Inject]
        public void Construct(EconomyModel economyModel, SaveService saveService)
        {
            _economyModel = economyModel;
            _saveService = saveService;
        }

        [ContextMenu("Reset Progress")]
        private void ResetProgress()
        {
            if (_economyModel == null || _saveService == null)
            {
                Debug.LogError("Dependencies are not injected.");
                return;
            }

            _economyModel.Reset();
            _saveService.Reset();
        }
    }
}
