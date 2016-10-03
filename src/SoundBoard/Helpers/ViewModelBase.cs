using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using AcillatemSoundBoard.Annotations;

namespace AcillatemSoundBoard.Helpers
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public static bool IsInDesignMode
        {
            get { return GetDesignModePropertyValue(); }
        }

        private static bool GetDesignModePropertyValue()
        {
            return (bool) DependencyPropertyDescriptor
                .FromProperty(DesignerProperties.IsInDesignModeProperty, typeof (FrameworkElement))
                .Metadata.DefaultValue;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}