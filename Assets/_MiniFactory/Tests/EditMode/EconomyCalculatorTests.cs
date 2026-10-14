using System;
using MiniFactory.Core.Config;
using MiniFactory.Core.Economy;
using MiniFactory.Core.Machines;
using NUnit.Framework;

namespace MiniFactory.Tests.EditMode
{
    public sealed class EconomyCalculatorTests
    {
        [Test]
        public void GetTotalProduction_SumsOnlyUnlockedMachines()
        {
            // Arrange
            MachineModel unlocked = CreateMachine("machine_a", 0.0, 5.0, MachineState.Unlocked, 1);
            MachineModel locked = CreateMachine("machine_b", 100.0, 10.0, MachineState.Locked, 0);

            // Act
            double total = EconomyCalculator.GetTotalProduction(new[] { unlocked, locked });

            // Assert
            Assert.AreEqual(5.0, total, 0.0001);
        }

        [Test]
        public void GetTotalProduction_NullMachines_Throws()
        {
            // Arrange
            TestDelegate calculate = () => EconomyCalculator.GetTotalProduction(null);

            // Act
            Exception exception = Assert.Throws<ArgumentNullException>(calculate);

            // Assert
            Assert.IsNotNull(exception);
        }

        [Test]
        public void GetIncome_MultipliesProductionByMultiplierAndTime()
        {
            // Arrange
            double totalProduction = 10.0;
            double multiplier = 3.0;
            double deltaSeconds = 2.0;

            // Act
            double income = EconomyCalculator.GetIncome(totalProduction, multiplier, deltaSeconds);

            // Assert
            Assert.AreEqual(60.0, income, 0.0001);
        }

        [Test]
        public void GetIncome_NegativeDeltaSeconds_Throws()
        {
            // Arrange
            TestDelegate calculate = () => EconomyCalculator.GetIncome(10.0, 1.0, -1.0);

            // Act
            Exception exception = Assert.Throws<ArgumentOutOfRangeException>(calculate);

            // Assert
            Assert.IsNotNull(exception);
        }

        private static MachineModel CreateMachine(string id, double unlockCost, double baseProduction, MachineState state, int level)
        {
            MachineConfig config = new MachineConfig
            {
                id = id,
                displayName = id,
                baseProduction = baseProduction,
                productionGrowth = 1.15,
                baseUpgradeCost = 10.0,
                upgradeGrowth = 1.25,
                unlockCost = unlockCost
            };

            return new MachineModel(config, state, level);
        }
    }
}
