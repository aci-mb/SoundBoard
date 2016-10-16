using System.Windows;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SoundBoard.Helpers;

namespace SoundBoard.Tests.Helpers
{
    [TestClass]
    public class BooleanToToggleTextConverterTests
    {
        public BooleanToToggleTextConverter Target { get; set; }

        [TestMethod]
        public void Convert_ValueIsTrue_ReturnsTrueText()
        {
            const string trueText = "True Story";
            Target = new BooleanToToggleTextConverter(trueText, string.Empty);

            Target.Convert(true, null, null, null)
                .Should().Be(trueText);
        }

        [TestMethod]
        public void Convert_ValueIsFalse_ReturnsFalseText()
        {
            const string falseText = "False Story";
            Target = new BooleanToToggleTextConverter(string.Empty, falseText);

            Target.Convert(false, null, null, null)
                .Should().Be(falseText);
        }

        [TestMethod]
        public void ConvertBack__ReturnsUnsetValue()
        {
            Target = new BooleanToToggleTextConverter(string.Empty, string.Empty);

            Target.ConvertBack(null, null, null, null)
                .Should().Be(DependencyProperty.UnsetValue);
        }
    }
}
