using Ninject;
using SoundBoard.View;

namespace SoundBoard.ViewModel
{
    public static class ContainerConfigurator
    {
        static ContainerConfigurator()
        {
            Kernel = new StandardKernel();
            Kernel.Bind<MainWindow>().To<MainWindow>().InSingletonScope();
        }

        public static IKernel Kernel { get; }
    }
}