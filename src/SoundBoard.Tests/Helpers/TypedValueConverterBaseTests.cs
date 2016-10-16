using System;
using System.Windows;
using System.Windows.Data;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SoundBoard.Helpers;

namespace SoundBoard.Tests.Helpers
{
    [TestClass]
    public class TypedValueConverterBaseTests
    {
        public IValueConverter Target { get; set; }

        [TestMethod]
        public void Convert_ConvertBackFuncIsNull_ReturnsDependencyPropertyUnsetValue()
        {
            Target = new TypedValueConverterBase<int, string>((value, parameter, culture) => string.Empty, null);

            Target.ConvertBack("Valid string value", typeof (int), null, null)
                .Should().Be(DependencyProperty.UnsetValue);
        }

        [TestMethod]
        public void Convert_ValueIsNotSourceType_ThrowsArgumentException()
        {
            Target = new TypedValueConverterBase<int, string>((i, o, c) => string.Empty, (s, o, c) => 0);

            var convert = new Action(() => Target.Convert("Not an int", typeof(string), null, null));

            convert.ShouldThrow<ArgumentException>()
                .WithMessage("Value must be of type " + typeof (int) + "*")
                .And.ParamName.Should().Be("value");
        }

        [TestMethod]
        public void ConvertBack_ValueIsNotTargetType_ThrowsArgumentException()
        {
            Target = new TypedValueConverterBase<int, string>((i, o, c) => string.Empty, (s, o, c) => 0);

            var convertBack = new Action(() => Target.ConvertBack(
                42, //Not target type (string)
                typeof(int), null, null));

            convertBack.ShouldThrow<ArgumentException>()
                .WithMessage("Value must be of type " + typeof(string) + "*")
                .And.ParamName.Should().Be("value");
        }

        [TestMethod]
        public void Convert_TargetTypeIsNotConverterTargetType_ThrowsArgumentException()
        {
            Target = new TypedValueConverterBase<int, string>((i, o, c) => string.Empty, (s, o, c) => 0);

            var convert = new Action(() => Target.Convert(42,
                typeof(bool), //Not target type of converter
                null, null));

            convert.ShouldThrow<ArgumentException>()
                .WithMessage("TargetType must be " + typeof(string) + "*")
                .And.ParamName.Should().Be("targetType");
        }

        [TestMethod]
        public void ConvertBack_TargetTypeIsNotConverterSourceType_ThrowsArgumentException()
        {
            Target = new TypedValueConverterBase<int, string>((i, o, c) => string.Empty, (s, o, c) => 0);

            var convert = new Action(() => Target.ConvertBack("A valid string value",
                typeof(bool), //Not source type of converter
                null, null));

            convert.ShouldThrow<ArgumentException>()
                .WithMessage("TargetType must be " + typeof(int) + "*")
                .And.ParamName.Should().Be("targetType");
        }

        [TestMethod]
        public void Convert_ConvertFuncReturnsStringFor42_ReturnsSameValue()
        {
            const string expectedValue = "42";

            Target = new TypedValueConverterBase<int, string>((i, o, c) => expectedValue, (s, o, c) => 0);

            Target.Convert(42, typeof (string), null, null)
                .Should().BeSameAs(expectedValue);
        }

        [TestMethod]
        public void ConvertBack_ConvertBackFuncReturnsIntFor42String_ReturnsEqualValue()
        {
            const int expectedValue = 42;

            Target = new TypedValueConverterBase<int, string>((i, o, c) => string.Empty, (s, o, c) => expectedValue);

            Target.ConvertBack("42", typeof(int), null, null)
                .Should().Be(expectedValue);
        }
    }
}
