using System;
using MiniFactory.Config.Local;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MiniFactory.UI.Views
{
    public sealed class MachineView : MonoBehaviour
    {
        public event Action ActionClicked;

        public MachineConfigAsset Config => _config;

        [SerializeField] private MachineConfigAsset _config;
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _productivityText;
        [SerializeField] private TMP_Text _levelText;
        [SerializeField] private GameObject _lockRoot;
        [SerializeField] private Button _actionButton;
        [SerializeField] private TMP_Text _actionText;

        private void Awake()
        {
            ApplyName();
        }

        private void OnEnable()
        {
            _actionButton.onClick.AddListener(OnActionButtonClicked);
        }

        public void SetProductivityText(string value)
        {
            _productivityText.text = value;
        }

        public void SetLevelText(string value)
        {
            _levelText.text = value;
        }

        public void SetActionText(string value)
        {
            _actionText.text = value;
        }

        public void SetLockVisible(bool visible)
        {
            _lockRoot.SetActive(visible);
        }

        private void ApplyName()
        {
            if (_nameText == null)
            {
                return;
            }

            _nameText.text = _config != null ? _config.DisplayName : string.Empty;
        }

        private void OnActionButtonClicked()
        {
            ActionClicked?.Invoke();
        }

        private void OnDisable()
        {
            _actionButton.onClick.RemoveListener(OnActionButtonClicked);
        }
    }
}
