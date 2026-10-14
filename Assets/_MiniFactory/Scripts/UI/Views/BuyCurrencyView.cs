using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MiniFactory.UI.Views
{
    public sealed class BuyCurrencyView : MonoBehaviour
    {
        public event Action BuyClicked;

        [SerializeField] private Button _button;
        [SerializeField] private TMP_Text _text;

        private void OnEnable()
        {
            _button.onClick.AddListener(OnBuyClicked);
        }

        public void SetText(string text)
        {
            _text.text = text;
        }

        public void SetInteractable(bool interactable)
        {
            _button.interactable = interactable;
        }

        private void OnBuyClicked()
        {
            BuyClicked?.Invoke();
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(OnBuyClicked);
        }
    }
}
