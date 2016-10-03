using AcillatemSoundBoard.ViewModel;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AcillatemSoundBoard.Tests.ViewModel
{
    [TestClass]
    public class NameDialogViewModelTests
    {
        public NameDialogViewModel Target { get; set; }

        [TestMethod]
        public void IsNameValid_NameIsEmpty_EqualsFalse()
        {
            Target = new NameDialogViewModel();

            Target.Name = string.Empty;

            Target.IsNameValid.Should().BeFalse();
        }

        [TestMethod]
        public void IsNameValid_NameIsNotEmpty_EqualsTrue()
        {
            Target = new NameDialogViewModel();

            Target.Name = "Some Name";

            Target.IsNameValid.Should().BeTrue();
        }
    }
}