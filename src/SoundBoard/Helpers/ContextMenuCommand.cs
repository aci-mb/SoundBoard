using System.Windows.Controls;
using System.Windows.Input;

namespace SoundBoard.Helpers
{
    public class ContextMenuCommand
    {
        public Image Icon { get; set; }
        public string Name { get; set; }
        public ICommand Command { get; set; }
    }
}