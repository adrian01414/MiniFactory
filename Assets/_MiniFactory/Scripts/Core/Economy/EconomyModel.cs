using System;
using System.Collections.Generic;
using MiniFactory.Core.Machines;

namespace MiniFactory.Core.Economy
{
    public sealed class EconomyModel
    {
        public event Action<double> BalanceChanged;
        public event Action<double> TotalProductionChanged;

        public double Balance => _balance;
        public double TotalProduction => _totalProduction;
        public IReadOnlyList<MachineModel> Machines => _machines;

        private readonly MachineModel[] _machines;
        private double _balance;
        private double _totalProduction;

        public EconomyModel(IEnumerable<MachineModel> machines, double startingBalance = 0.0)
        {
            if (machines == null) { throw new ArgumentNullException(nameof(machines)); }
            if (startingBalance < 0.0) { throw new ArgumentOutOfRangeException(nameof(startingBalance)); }

            List<MachineModel> collected = new List<MachineModel>();
            foreach (MachineModel machine in machines)
            {
                if (machine != null) { collected.Add(machine); }
            }

            _machines = collected.ToArray();
            _balance = startingBalance;
            _totalProduction = EconomyCalculator.GetTotalProduction(_machines);

            for (int i = 0; i < _machines.Length; i++)
            {
                _machines[i].LevelChanged += OnMachineLevelChanged;
                _machines[i].StateChanged += OnMachineStateChanged;
            }
        }

        public void AddIncome(double amount)
        {
            if (amount < 0.0) { throw new ArgumentOutOfRangeException(nameof(amount)); }
            if (amount == 0.0) { return; }
            _balance += amount;
            BalanceChanged?.Invoke(_balance);
        }

        public bool CanAfford(double amount)
        {
            return amount >= 0.0 && _balance >= amount;
        }

        public bool TrySpend(double amount)
        {
            if (amount < 0.0) { throw new ArgumentOutOfRangeException(nameof(amount)); }
            if (_balance < amount) { return false; }
            _balance -= amount;
            BalanceChanged?.Invoke(_balance);
            return true;
        }

        public void Reset()
        {
            if (_balance != 0.0)
            {
                _balance = 0.0;
                BalanceChanged?.Invoke(_balance);
            }

            for (int i = 0; i < _machines.Length; i++)
            {
                _machines[i].Reset();
            }
        }

        private void OnMachineLevelChanged(int level)
        {
            RecalculateTotalProduction();
        }

        private void OnMachineStateChanged(MachineState state)
        {
            RecalculateTotalProduction();
        }

        private void RecalculateTotalProduction()
        {
            double total = EconomyCalculator.GetTotalProduction(_machines);
            if (total == _totalProduction) { return; }
            _totalProduction = total;
            TotalProductionChanged?.Invoke(_totalProduction);
        }
    }
}
