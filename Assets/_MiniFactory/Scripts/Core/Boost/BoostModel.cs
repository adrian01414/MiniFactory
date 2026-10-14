using System;
using MiniFactory.Core.Config;

namespace MiniFactory.Core.Boost
{
    public sealed class BoostModel
    {
        public event Action<bool> HasBoostChanged;
        public event Action<bool> ActiveChanged;
        public event Action<double> RemainingSecondsChanged;

        public bool Enabled => _config.boostEnabled;
        public bool HasBoost => _hasBoost;
        public bool IsActive => _isActive;
        public double RemainingSeconds => _remainingSeconds;
        public double DurationSeconds => _config.boostDurationSeconds;
        public double Multiplier => _config.boostMultiplier;
        public double Cost => _config.boostCost;
        public double ActiveMultiplier => _isActive ? _config.boostMultiplier : 1.0;
        public bool CanBuy => Enabled && !_hasBoost && !_isActive;
        public bool CanActivate => Enabled && _hasBoost && !_isActive;

        private readonly GameConfig _config;
        private bool _hasBoost;
        private bool _isActive;
        private double _remainingSeconds;

        public BoostModel(GameConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public void GrantBoost()
        {
            if (_hasBoost)
            {
                return;
            }

            _hasBoost = true;
            HasBoostChanged?.Invoke(_hasBoost);
        }

        public void Activate()
        {
            if (!_hasBoost || _isActive)
            {
                return;
            }

            _hasBoost = false;
            _isActive = true;
            _remainingSeconds = _config.boostDurationSeconds;

            HasBoostChanged?.Invoke(_hasBoost);
            ActiveChanged?.Invoke(_isActive);
            RemainingSecondsChanged?.Invoke(_remainingSeconds);
        }

        public void Tick(double deltaSeconds)
        {
            if (!_isActive || deltaSeconds <= 0.0)
            {
                return;
            }

            _remainingSeconds -= deltaSeconds;

            if (_remainingSeconds <= 0.0)
            {
                _remainingSeconds = 0.0;
                _isActive = false;
                RemainingSecondsChanged?.Invoke(_remainingSeconds);
                ActiveChanged?.Invoke(_isActive);
                return;
            }

            RemainingSecondsChanged?.Invoke(_remainingSeconds);
        }

        public void RestoreState(bool hasBoost, bool isActive, double remainingSeconds)
        {
            _hasBoost = hasBoost;
            _isActive = isActive;
            _remainingSeconds = isActive ? Math.Max(0.0, remainingSeconds) : 0.0;

            HasBoostChanged?.Invoke(_hasBoost);
            ActiveChanged?.Invoke(_isActive);
            RemainingSecondsChanged?.Invoke(_remainingSeconds);
        }
    }
}
