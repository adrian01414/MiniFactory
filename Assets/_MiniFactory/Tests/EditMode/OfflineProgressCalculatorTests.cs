using System;
using MiniFactory.Core.Offline;
using NUnit.Framework;

namespace MiniFactory.Tests.EditMode
{
    public sealed class OfflineProgressCalculatorTests
    {
        [Test]
        public void Calculate_WithinCap_AppliesFullElapsed()
        {
            // Arrange
            double elapsed = 100.0;
            double cap = 3600.0;
            double production = 2.0;

            // Act
            OfflineProgressResult result = OfflineProgressCalculator.Calculate(elapsed, cap, production, false, 0.0, 1.0);

            // Assert
            Assert.AreEqual(100.0, result.AppliedSeconds, 0.0001);
            Assert.AreEqual(200.0, result.Income, 0.0001);
            Assert.IsFalse(result.WasCapped);
        }

        [Test]
        public void Calculate_ExceedingCap_CapsAppliedSeconds()
        {
            // Arrange
            double elapsed = 7200.0;
            double cap = 3600.0;
            double production = 2.0;

            // Act
            OfflineProgressResult result = OfflineProgressCalculator.Calculate(elapsed, cap, production, false, 0.0, 1.0);

            // Assert
            Assert.AreEqual(3600.0, result.AppliedSeconds, 0.0001);
            Assert.AreEqual(7200.0, result.Income, 0.0001);
            Assert.IsTrue(result.WasCapped);
        }

        [Test]
        public void Calculate_WithActiveBoost_ConsumesBoostFirst()
        {
            // Arrange
            double elapsed = 100.0;
            double cap = 3600.0;
            double production = 1.0;
            double boostRemaining = 40.0;
            double boostMultiplier = 3.0;

            // Act
            OfflineProgressResult result = OfflineProgressCalculator.Calculate(elapsed, cap, production, true, boostRemaining, boostMultiplier);

            // Assert
            Assert.AreEqual(40.0, result.BoostConsumedSeconds, 0.0001);
            Assert.AreEqual(180.0, result.Income, 0.0001);
        }

        [Test]
        public void Calculate_NegativeElapsed_Throws()
        {
            // Arrange
            TestDelegate calculate = () => OfflineProgressCalculator.Calculate(-1.0, 3600.0, 1.0, false, 0.0, 1.0);

            // Act
            Exception exception = Assert.Throws<ArgumentOutOfRangeException>(calculate);

            // Assert
            Assert.IsNotNull(exception);
        }
    }
}
