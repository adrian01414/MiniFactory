using System;
using MiniFactory.Core.Config;
using MiniFactory.Core.Economy;
using MiniFactory.Core.Machines;
using NUnit.Framework;

namespace MiniFactory.Tests.EditMode
{
    public sealed class EconomyModelTests
    {
        [Test]
        public void AddIncome_IncreasesBalanceAndRaisesEvent()
        {
            // Arrange
            EconomyModel model = new EconomyModel(new MachineModel[0], 100.0);
            double reported = -1.0;
            model.BalanceChanged += value => reported = value;

            // Act
            model.AddIncome(50.0);

            // Assert
            Assert.AreEqual(150.0, model.Balance, 0.0001);
            Assert.AreEqual(150.0, reported, 0.0001);
        }

        [Test]
        public void TrySpend_WhenInsufficient_ReturnsFalseAndKeepsBalance()
        {
            // Arrange
            EconomyModel model = new EconomyModel(new MachineModel[0], 100.0);

            // Act
            bool spent = model.TrySpend(150.0);

            // Assert
            Assert.IsFalse(spent);
            Assert.AreEqual(100.0, model.Balance, 0.0001);
        }

        [Test]
        public void TrySpend_WhenAffordable_DeductsBalance()
        {
            // Arrange
            EconomyModel model = new EconomyModel(new MachineModel[0], 100.0);

            // Act
            bool spent = model.TrySpend(40.0);

            // Assert
            Assert.IsTrue(spent);
            Assert.AreEqual(60.0, model.Balance, 0.0001);
        }

        [Test]
        public void UnlockingMachine_RecalculatesTotalProduction()
        {
            // Arrange
            MachineModel machine = CreateMachine("machine_test", 100.0, 5.0);
            EconomyModel model = new EconomyModel(new[] { machine });
            double reported = -1.0;
            model.TotalProductionChanged += value => reported = value;

            // Act
            machine.Unlock();

            // Assert
            Assert.AreEqual(5.0, model.TotalProduction, 0.0001);
            Assert.AreEqual(5.0, reported, 0.0001);
        }

        [Test]
        public void Constructor_NegativeStartingBalance_Throws()
        {
            // Arrange
            TestDelegate create = () => new EconomyModel(new MachineModel[0], -1.0);

            // Act
            Exception exception = Assert.Throws<ArgumentOutOfRangeException>(create);

            // Assert
            Assert.IsNotNull(exception);
        }

        [Test]
        public void AddIncome_NegativeAmount_Throws()
        {
            // Arrange
            EconomyModel model = new EconomyModel(new MachineModel[0], 0.0);
            TestDelegate add = () => model.AddIncome(-1.0);

            // Act
            Exception exception = Assert.Throws<ArgumentOutOfRangeException>(add);

            // Assert
            Assert.IsNotNull(exception);
        }

        private static MachineModel CreateMachine(string id, double unlockCost, double baseProduction)
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

            return new MachineModel(config);
        }
    }
}
