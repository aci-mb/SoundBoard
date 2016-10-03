using AcillatemSoundBoard.ViewModel.Converters;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AcillatemSoundBoard.Tests.ViewModel.Converters
{
    [TestClass]
    public class BooleanToOpacityConverterTests
    {
        public BooleanToOpacityConverter Target { get; set; }

        [TestInitialize]
        public void InitializeBeforeEachTest()
        {
            Target = new BooleanToOpacityConverter();
        }

        [TestMethod]
        public void Convert_ValueIsTrue_Returns1()
        {
            double opacity = (double)Target.Convert(true, null, null, null);

            opacity.Should().Be(1);
        }

        [TestMethod]
        public void Convert_ValueIsFalse_Returns0_4()
        {
            double opacity = (double) Target.Convert(false, null, null, null);

            opacity.Should().Be(0.4);
        }

        [TestMethod]
        public void ConvertBack_ValueIs0_4_ReturnsFalse()
        {
            bool booleanValue = (bool) Target.ConvertBack(0.4, null, null, null);

            booleanValue.Should().BeFalse();
        }

        [TestMethod]
        public void ConvertBack_ValueIs1_ReturnsTrue()
        {
            bool booleanValue = (bool) Target.ConvertBack(1, null, null, null);

            booleanValue.Should().BeTrue();
        }
    }
}
