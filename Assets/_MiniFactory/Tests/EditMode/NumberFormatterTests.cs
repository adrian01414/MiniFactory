using MiniFactory.Core.Formatting;
using NUnit.Framework;

namespace MiniFactory.Tests.EditMode
{
    public sealed class NumberFormatterTests
    {
        [Test]
        public void Format_ZeroOrNegative_ReturnsZero()
        {
            // Arrange
            double value = 0.0;

            // Act
            string formatted = NumberFormatter.Format(value);

            // Assert
            Assert.AreEqual("0", formatted);
        }

        [Test]
        public void Format_BelowThousand_ReturnsPlainNumber()
        {
            // Arrange
            double value = 999.0;

            // Act
            string formatted = NumberFormatter.Format(value);

            // Assert
            Assert.AreEqual("999", formatted);
        }

        [Test]
        public void Format_Thousands_UsesSuffix()
        {
            // Arrange
            double value = 1500.0;

            // Act
            string formatted = NumberFormatter.Format(value);

            // Assert
            Assert.AreEqual("1.5K", formatted);
        }

        [Test]
        public void Format_Millions_UsesSuffix()
        {
            // Arrange
            double value = 2500000.0;

            // Act
            string formatted = NumberFormatter.Format(value);

            // Assert
            Assert.AreEqual("2.5M", formatted);
        }

        [Test]
        public void Format_NaN_ReturnsZero()
        {
            // Arrange
            double value = double.NaN;

            // Act
            string formatted = NumberFormatter.Format(value);

            // Assert
            Assert.AreEqual("0", formatted);
        }
    }
}
