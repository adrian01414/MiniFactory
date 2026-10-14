using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MiniFactory.UI.Views
{
    public sealed class BoostView : MonoBehaviour
    {
        public event Action BoostClicked;
        public event Action BuyClicked;

        [SerializeField] private Button _boostButton;
        [SerializeField] private TMP_Text _boostText;
        [SerializeField] private Button _buyButton;
        [SerializeField] private TMP_Text _buyText;

        private void OnEnable()
        {
            _boostButton.onClick.AddListener(OnBoostClicked);
            _buyButton.onClick.AddListener(OnBuyClicked);
        }

        public void SetBoostText(string text)
        {
            _boostText.text = text;
        }

        public void SetBuyText(string text)
        {
            _buyText.text = text;
        }

        public void SetBoostInteractable(bool interactable)
        {
            _boostButton.interactable = interactable;
        }

        public void SetBuyInteractable(bool interactable)
        {
            _buyButton.interactable = interactable;
        }

        private void OnBoostClicked()
        {
            BoostClicked?.Invoke();
        }

        private void OnBuyClicked()
        {
            BuyClicked?.Invoke();
        }

        private void OnDisable()
        {
            _boostButton.onClick.RemoveListener(OnBoostClicked);
            _buyButton.onClick.RemoveListener(OnBuyClicked);
        }
    }
}
