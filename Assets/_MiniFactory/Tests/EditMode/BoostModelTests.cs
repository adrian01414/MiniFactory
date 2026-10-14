using MiniFactory.Core.Boost;
using MiniFactory.Core.Config;
using NUnit.Framework;

namespace MiniFactory.Tests.EditMode
{
    public sealed class BoostModelTests
    {
        [Test]
        public void GrantBoost_SetsHasBoostAndAllowsActivate()
        {
            // Arrange
            BoostModel model = new BoostModel(CreateConfig());

            // Act
            model.GrantBoost();

            // Assert
            Assert.IsTrue(model.HasBoost);
            Assert.IsTrue(model.CanActivate);
            Assert.IsFalse(model.CanBuy);
        }

        [Test]
        public void Activate_ConsumesBoostAndStartsTimer()
        {
            // Arrange
            BoostModel model = new BoostModel(CreateConfig());
            model.GrantBoost();

            // Act
            model.Activate();

            // Assert
            Assert.IsFalse(model.HasBoost);
            Assert.IsTrue(model.IsActive);
            Assert.AreEqual(30.0, model.RemainingSeconds, 0.0001);
            Assert.AreEqual(3.0, model.ActiveMultiplier, 0.0001);
        }

        [Test]
        public void Tick_Partial_ReducesRemainingSeconds()
        {
            // Arrange
            BoostModel model = new BoostModel(CreateConfig());
            model.GrantBoost();
            model.Activate();

            // Act
            model.Tick(10.0);

            // Assert
            Assert.IsTrue(model.IsActive);
            Assert.AreEqual(20.0, model.RemainingSeconds, 0.0001);
        }

        [Test]
        public void Tick_WhenExpires_DeactivatesBoost()
        {
            // Arrange
            BoostModel model = new BoostModel(CreateConfig());
            model.GrantBoost();
            model.Activate();

            // Act
            model.Tick(30.0);

            // Assert
            Assert.IsFalse(model.IsActive);
            Assert.AreEqual(0.0, model.RemainingSeconds, 0.0001);
            Assert.AreEqual(1.0, model.ActiveMultiplier, 0.0001);
        }

        [Test]
        public void RestoreState_ActiveWithRemaining_KeepsActive()
        {
            // Arrange
            BoostModel model = new BoostModel(CreateConfig());

            // Act
            model.RestoreState(false, true, 12.0);

            // Assert
            Assert.IsTrue(model.IsActive);
            Assert.AreEqual(12.0, model.RemainingSeconds, 0.0001);
        }

        [Test]
        public void RestoreState_Inactive_ZeroesRemainingSeconds()
        {
            // Arrange
            BoostModel model = new BoostModel(CreateConfig());

            // Act
            model.RestoreState(true, false, 12.0);

            // Assert
            Assert.IsTrue(model.HasBoost);
            Assert.IsFalse(model.IsActive);
            Assert.AreEqual(0.0, model.RemainingSeconds, 0.0001);
        }

        private static GameConfig CreateConfig()
        {
            return new GameConfig
            {
                boostEnabled = true,
                boostDurationSeconds = 30.0,
                boostMultiplier = 3.0,
                boostCost = 1000.0
            };
        }
    }
}
