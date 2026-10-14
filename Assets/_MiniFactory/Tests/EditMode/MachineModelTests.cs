using System;
using MiniFactory.Core.Config;
using MiniFactory.Core.Machines;
using NUnit.Framework;

namespace MiniFactory.Tests.EditMode
{
    public sealed class MachineModelTests
    {
        [Test]
        public void Constructor_UnlockedWithZeroLevel_ClampsLevelToOne()
        {
            // Arrange
            MachineConfig config = CreateConfig();

            // Act
            MachineModel machine = new MachineModel(config, MachineState.Unlocked, 0);

            // Assert
            Assert.IsTrue(machine.IsUnlocked);
            Assert.AreEqual(1, machine.Level);
        }

        [Test]
        public void Unlock_LockedMachine_SetsUnlockedAndRaisesEvents()
        {
            // Arrange
            MachineConfig config = CreateConfig();
            MachineModel machine = new MachineModel(config);
            MachineState? stateChanged = null;
            int? levelChanged = null;
            machine.StateChanged += state => stateChanged = state;
            machine.LevelChanged += level => levelChanged = level;

            // Act
            machine.Unlock();

            // Assert
            Assert.IsTrue(machine.IsUnlocked);
            Assert.AreEqual(1, machine.Level);
            Assert.AreEqual(MachineState.Unlocked, stateChanged);
            Assert.AreEqual(1, levelChanged);
        }

        [Test]
        public void Upgrade_LockedMachine_ThrowsInvalidOperationException()
        {
            // Arrange
            MachineConfig config = CreateConfig();
            MachineModel machine = new MachineModel(config);

            // Act
            TestDelegate upgrade = () => machine.Upgrade();

            // Assert
            Assert.Throws<InvalidOperationException>(upgrade);
            Assert.AreEqual(0, machine.Level);
        }

        private static MachineConfig CreateConfig()
        {
            return new MachineConfig
            {
                id = "machine_test",
                displayName = "Machine Test",
                baseProduction = 1.0,
                productionGrowth = 1.15,
                baseUpgradeCost = 10.0,
                upgradeGrowth = 1.25,
                unlockCost = 100.0
            };
        }
    }
}
