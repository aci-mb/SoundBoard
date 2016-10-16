using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using SoundBoard.Annotations;

namespace SoundBoard.Helpers
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public static bool IsInDesignMode => GetDesignModePropertyValue();

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
	        handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}