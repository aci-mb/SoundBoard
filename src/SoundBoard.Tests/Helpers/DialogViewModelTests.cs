using AcillatemSoundBoard.Helpers;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AcillatemSoundBoard.Tests.Helpers
{
    [TestClass]
    public class DialogViewModelTests
    {
        public DialogViewModelBase Target { get; set; }

        [TestInitialize]
        public void InitializeBeforeEachTest()
        {
            Target = new DialogViewModelBase();
        }

        [TestMethod]
        public void OkCommandExecute__SetsDialogResultToTrue()
        {
            Target.DialogResult = false;

            Target.OkCommand.Execute(null);

            Target.DialogResult.Should().BeTrue();
        }

        [TestMethod]
        public void CancelCommandExecute__SetsDialogResultToFalse()
        {
            Target.DialogResult = true;

            Target.CancelCommand.Execute(null);

            Target.DialogResult.Should().BeFalse();
        }
    }
}