using System;
using MiniFactory.Core.Config;

namespace MiniFactory.Core.Machines
{
    public sealed class MachineModel
    {
        public event Action<MachineState> StateChanged;
        public event Action<int> LevelChanged;

        public MachineConfig Config => _config;
        public string Id => _config.id;
        public MachineState State => _state;
        public int Level => _level;
        public bool IsUnlocked => _state == MachineState.Unlocked;
        public bool CanUnlock => !IsUnlocked;
        public double Production => IsUnlocked ? MachineCalculator.GetProduction(_config, _level) : 0.0;
        public double UnlockCost => _config.unlockCost;
        public double NextUpgradeCost => IsUnlocked ? MachineCalculator.GetUpgradeCost(_config, _level) : 0.0;

        private readonly MachineConfig _config;
        private MachineState _state;
        private int _level;

        public MachineModel(MachineConfig config, MachineState state = MachineState.Locked, int level = 0)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));

            if (level < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(level), level, "Machine level cannot be negative.");
            }

            _state = state;
            _level = state == MachineState.Unlocked ? Math.Max(1, level) : 0;
        }

        public void Unlock()
        {
            if (IsUnlocked)
            {
                return;
            }

            _state = MachineState.Unlocked;
            _level = Math.Max(1, _level);

            StateChanged?.Invoke(_state);
            LevelChanged?.Invoke(_level);
        }

        public void Upgrade()
        {
            if (!IsUnlocked)
            {
                throw new InvalidOperationException($"Cannot upgrade locked machine '{Id}'.");
            }

            _level++;

            LevelChanged?.Invoke(_level);
        }

        public void RestoreState(MachineState state, int level)
        {
            if (level < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(level), level, "Machine level cannot be negative.");
            }

            _state = state;
            _level = state == MachineState.Unlocked ? Math.Max(1, level) : 0;

            StateChanged?.Invoke(_state);
            LevelChanged?.Invoke(_level);
        }
    }
}
