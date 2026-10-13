using System;

namespace MiniFactory.Core.Save
{
    [Serializable]
    public sealed class SaveData
    {
        public int version = 1;
        public double balance;
        public MachineSaveData[] machines = Array.Empty<MachineSaveData>();

        public bool TryGetMachine(string id, out MachineSaveData machine)
        {
            machine = null;

            if (string.IsNullOrEmpty(id) || machines == null)
            {
                return false;
            }

            for (int i = 0; i < machines.Length; i++)
            {
                MachineSaveData candidate = machines[i];
                if (candidate != null && candidate.id == id)
                {
                    machine = candidate;
                    return true;
                }
            }

            return false;
        }
    }

    [Serializable]
    public sealed class MachineSaveData
    {
        public string id = string.Empty;
        public int state;
        public int level;
    }
}
