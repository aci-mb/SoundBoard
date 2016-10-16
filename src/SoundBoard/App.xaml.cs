using System.Windows;
using Ninject;
using SoundBoard.View;
using SoundBoard.ViewModel;

namespace SoundBoard
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
