using System.Collections.Generic;
using System.Windows;

namespace SoundBoard.Services
{
    public interface IDialogService
    {
        IEnumerable<string> OpenFileDialog(string title, IDictionary<string, string> filters);
        string NameDialog(Window parent, string title, string prompt, string initialName);
        bool QuestionDialog(Window parent, string title, string question);
    }
}