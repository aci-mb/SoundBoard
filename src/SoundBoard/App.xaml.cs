using System.Windows;
using AcillatemSoundBoard.View;
using AcillatemSoundBoard.ViewModel;
using Ninject;

namespace AcillatemSoundBoard
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            ContainerConfigurator.Kernel.Get<MainWindow>().ShowDialog();
        }
    }
}
