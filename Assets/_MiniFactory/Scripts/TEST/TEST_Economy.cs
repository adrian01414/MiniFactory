using MiniFactory.Core.Economy;
using UnityEngine;
using VContainer;

namespace MiniFactory.App
{
    public sealed class TEST_Economy : MonoBehaviour
    {
        [SerializeField] private double _amount = 100.0;

        private EconomyModel _economy;

        [Inject]
        public void Construct(EconomyModel economy)
        {
            _economy = economy;
        }

        [ContextMenu("Add Money")]
        private void AddMoney()
        {
            if (_economy == null)
            {
                Debug.LogError("Economy is not injected.");
                return;
            }

            _economy.AddIncome(_amount);
        }
    }
}
