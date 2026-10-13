using System;
using System.Collections.Generic;
using MiniFactory.Core;
using MiniFactory.Core.Machines;
using MiniFactory.Core.Save;

namespace MiniFactory.Game.Services
{
    public sealed class SaveService
    {
        private const string SaveKey = "mini_factory_save";

        private readonly IStorageService _storage;

        public SaveService(IStorageService storage)
        {
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
        }

        public void Save(double balance, IReadOnlyList<MachineModel> machines)
        {
            if (machines == null)
            {
                throw new ArgumentNullException(nameof(machines));
            }

            SaveData data = new SaveData
            {
                version = 1,
                balance = balance,
                machines = new MachineSaveData[machines.Count]
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

            _storage.Load(SaveKey, onLoaded);
        }

        public void Reset()
        {
            _storage.Delete(SaveKey, null);
        }
    }
}
