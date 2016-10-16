using AcillatemSoundBoard.View;
using Ninject;

namespace AcillatemSoundBoard.ViewModel
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