using TMPro;
using UnityEngine;

namespace MiniFactory.UI.Views
{
    public sealed class TopBarView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _balanceText;
        [SerializeField] private TMP_Text _totalProductionText;

        public void SetBalanceText(string value)
        {
            _balanceText.text = value;
        }

        public void SetTotalProductionText(string value)
        {
            _totalProductionText.text = value;
        }
    }
}
