using System;
using System.Collections.Generic;
using MiniFactory.Core;
using MiniFactory.Core.Boost;
using MiniFactory.Core.Machines;
using MiniFactory.Core.Save;

namespace MiniFactory.Game.Services
{
    public sealed class SaveService
    {
        private const string SaveKey = "mini_factory_save";

        public long LastSaveTimeUtcTicks { get; private set; }

        private readonly IStorageService _storage;

        public SaveService(IStorageService storage)
        {
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
        }

        public void Save(double balance, IReadOnlyList<MachineModel> machines, BoostModel boostModel)
        {
            if (machines == null)
            {
                throw new ArgumentNullException(nameof(machines));
            }

            if (boostModel == null)
            {
                throw new ArgumentNullException(nameof(boostModel));
            }

            LastSaveTimeUtcTicks = DateTime.UtcNow.Ticks;

            SaveData data = new SaveData
            {
                version = 1,
                balance = balance,
                machines = new MachineSaveData[machines.Count],
                boostHasBoost = boostModel.HasBoost,
                boostIsActive = boostModel.IsActive,
                boostRemainingSeconds = boostModel.RemainingSeconds,
                lastSaveTimeUtcTicks = LastSaveTimeUtcTicks
            };

            for (int i = 0; i < machines.Count; i++)
            {
                MachineModel machine = machines[i];
                data.machines[i] = new MachineSaveData
                {
                    id = machine.Id,
                    state = (int)machine.State,
                    level = machine.Level
                };
            }

            _storage.Save(SaveKey, data, null);
        }

        public void Load(Action<SaveData> onLoaded)
        {
            if (onLoaded == null)
            {
                throw new ArgumentNullException(nameof(onLoaded));
            }

            _storage.Load<SaveData>(SaveKey, data =>
            {
                if (data != null)
                {
                    LastSaveTimeUtcTicks = data.lastSaveTimeUtcTicks;
                }

                onLoaded(data);
            });
        }

        public void Reset()
        {
            LastSaveTimeUtcTicks = 0L;
            _storage.Delete(SaveKey, null);
        }
    }
}
