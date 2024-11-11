using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using QuantConnect.Statistics;

namespace QuantConnect.Tests
{
    [TestFixture]
    public class StatisticsTests
    {
        [Test]
        public void MaxDrawdownRecoveryTime_EmptyInput_ReturnsZero()
        {
            var equityOverTime = new SortedDictionary<DateTime, decimal>();
            var result = QuantConnect.Statistics.Statistics.MaxDrawdownRecoveryTime(equityOverTime);
            Assert.AreEqual(0, result);
        }

        [Test]
        public void MaxDrawdownRecoveryTime_NoDrawdown_ReturnsZero()
        {
            var equityOverTime = new SortedDictionary<DateTime, decimal>
            {
                { DateTime.Parse("2021-01-01"), 100m },
                { DateTime.Parse("2021-01-02"), 105m },
                { DateTime.Parse("2021-01-03"), 110m }
            };
            var result = QuantConnect.Statistics.Statistics.MaxDrawdownRecoveryTime(equityOverTime);
            Assert.AreEqual(0, result);
        }

        [Test]
        public void MaxDrawdownRecoveryTime_SingleDrawdownImmediateRecovery_ReturnsZero()
        {
            var equityOverTime = new SortedDictionary<DateTime, decimal>
            {
                { DateTime.Parse("2021-01-01"), 100m },
                { DateTime.Parse("2021-01-02"), 90m }, // Drawdown
                { DateTime.Parse("2021-01-03"), 90m }  // Recovery on same day
            };
            var result = QuantConnect.Statistics.Statistics.MaxDrawdownRecoveryTime(equityOverTime);
            Assert.AreEqual(0, result);
        }

        [Test]
        public void MaxDrawdownRecoveryTime_SingleDrawdown_ReturnsCorrectRecoveryDays()
        {
            var equityOverTime = new SortedDictionary<DateTime, decimal>
            {
                { DateTime.Parse("2021-01-01"), 100m },
                { DateTime.Parse("2021-01-02"), 90m }, // Drawdown
                { DateTime.Parse("2021-01-03"), 95m },
                { DateTime.Parse("2021-01-04"), 100m } // Recovery
            };
            var result = QuantConnect.Statistics.Statistics.MaxDrawdownRecoveryTime(equityOverTime);
            Assert.AreEqual(1, result); // One day to recover
        }

        [Test]
        public void MaxDrawdownRecoveryTime_MultipleDrawdowns_ReturnsLongestRecoveryTime()
        {
            var equityOverTime = new SortedDictionary<DateTime, decimal>
            {
                { DateTime.Parse("2021-01-01"), 100m },
                { DateTime.Parse("2021-01-02"), 90m }, // First Drawdown
                { DateTime.Parse("2021-01-03"), 85m },
                { DateTime.Parse("2021-01-04"), 95m },
                { DateTime.Parse("2021-01-05"), 80m }, // Second Drawdown
                { DateTime.Parse("2021-01-06"), 75m },
                { DateTime.Parse("2021-01-07"), 90m }, // Recovery for second
                { DateTime.Parse("2021-01-08"), 95m }  // Recovery for first
            };
            var result = QuantConnect.Statistics.Statistics.MaxDrawdownRecoveryTime(equityOverTime);
            Assert.AreEqual(2, result); // Two days to recover from the second drawdown
        }

        [Test]
        public void MaxDrawdownRecoveryTime_WithZeros_ReturnsZero()
        {
            var equityOverTime = new SortedDictionary<DateTime, decimal>
            {
                { DateTime.Parse("2021-01-01"), 0m },
                { DateTime.Parse("2021-01-02"), 0m },
                { DateTime.Parse("2021-01-03"), 0m }
            };
            var result = QuantConnect.Statistics.Statistics.MaxDrawdownRecoveryTime(equityOverTime);
            Assert.AreEqual(0, result);
        }

        [Test]
        public void MaxDrawdownRecoveryTime_WithOneDay_ReturnsZero()
        {
            var equityOverTime = new SortedDictionary<DateTime, decimal>
            {
                { DateTime.Parse("2021-01-01"), 100m }
            };
            var result = QuantConnect.Statistics.Statistics.MaxDrawdownRecoveryTime(equityOverTime);
            Assert.AreEqual(0, result);
        }

        [Test]
        public void MaxDrawdownRecoveryTime_WithNegativeValues_ReturnsCorrectRecoveryTime()
        {
            var equityOverTime = new SortedDictionary<DateTime, decimal>
            {
                { DateTime.Parse("2021-01-01"), -100m },
                { DateTime.Parse("2021-01-02"), -90m },
                { DateTime.Parse("2021-01-03"), -95m },
                { DateTime.Parse("2021-01-04"), -80m }, // Recovery
            };
            var result = QuantConnect.Statistics.Statistics.MaxDrawdownRecoveryTime(equityOverTime);
            Assert.AreEqual(0, result); // Since all values are negative, no recovery will be detected
        }

        [Test]
        public void MaxDrawdownRecoveryTime_VaryingDecimalPrecision_ReturnsRounded()
        {
            var equityOverTime = new SortedDictionary<DateTime, decimal>
            {
                { DateTime.Parse("2021-01-01"), 100.257m },
                { DateTime.Parse("2021-01-02"), 95.817m }, // Drawdown
                { DateTime.Parse("2021-01-03"), 102.312m } // Recovery
            };
            var result = QuantConnect.Statistics.Statistics.MaxDrawdownRecoveryTime(equityOverTime, 3); // Rounding to 3 decimals
            Assert.AreEqual(1, result); // One day to recover
        }
    }
}