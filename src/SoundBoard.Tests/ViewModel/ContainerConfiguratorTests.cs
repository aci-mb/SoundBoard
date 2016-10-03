using AcillatemSoundBoard.View;
using AcillatemSoundBoard.ViewModel;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;

namespace AcillatemSoundBoard.Tests.ViewModel
{
    [TestClass]
    public class ContainerConfiguratorTests
    {
        [TestMethod]
        public void GetMainWindow__AlwaysReturnsSameInstance()
        {
            var firstWindow = ContainerConfigurator.Kernel.Get<MainWindow>();
            var secondWindow = ContainerConfigurator.Kernel.Get<MainWindow>();
            firstWindow.Should().BeSameAs(secondWindow);
        }
    }
}
