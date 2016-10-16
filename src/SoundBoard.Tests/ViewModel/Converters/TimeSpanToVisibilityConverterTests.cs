using System;
using System.Windows;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SoundBoard.ViewModel.Converters;

namespace SoundBoard.Tests.ViewModel.Converters
{
    [TestClass]
    public class TimeSpanToVisibilityConverterTests
    {
        public TimeSpanToVisibilityConverter Target { get; set; }

        [TestInitialize]
        public void InitializeBeforeEachTest()
        {
            Target = new TimeSpanToVisibilityConverter();
        }

        [TestMethod]
        public void Convert_ValueIsTimeSpanZero_ReturnsVisibilityCollapsed()
        {
            Target.Convert(TimeSpan.Zero, typeof (Visibility), null, null)
                .Should().Be(Visibility.Collapsed);
        }

        [TestMethod]
        public void Convert_ValueIsGreaterThanTimeSpanZero_ReturnsVisibilityVisible()
        {
            Target.Convert(TimeSpan.FromSeconds(12), typeof (Visibility), null, null)
                .Should().Be(Visibility.Visible);
        }

        [TestMethod]
        public void ConvertBack__ReturnsDependencyPropertyUnsetValue()
        {
            Target.ConvertBack(Visibility.Collapsed, typeof (TimeSpan), null, null)
                .Should().Be(DependencyProperty.UnsetValue);
        }
    }
}
