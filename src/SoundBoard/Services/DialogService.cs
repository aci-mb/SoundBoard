using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using AcillatemSoundBoard.View;
using AcillatemSoundBoard.ViewModel;
using Microsoft.Win32;

namespace AcillatemSoundBoard.Services
{
    public class DialogService : IDialogService
    {
        public IEnumerable<string> OpenFileDialog(string title, IDictionary<string, string> filters)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = FiltersToString(filters);
            openFileDialog.Multiselect = true;
            openFileDialog.Title = title;

            var dialogResult = openFileDialog.ShowDialog();
            
            return dialogResult.HasValue && dialogResult.Value
                ? openFileDialog.FileNames
                : Enumerable.Empty<string>();
        }

        public string NameDialog(Window parent, string title, string prompt, string initialName)
        {
            var viewModel = new NameDialogViewModel
            {
                Title = title,
                Prompt = prompt,
                Name = initialName
            };

            Window nameDialog = new NameDialog
            {
                DataContext = viewModel,
                Owner = parent                
            };

            var dialogResult = nameDialog.ShowDialog();
            
            return dialogResult.HasValue && dialogResult.Value
                ? viewModel.Name
                : null;
        }

        public bool QuestionDialog(Window parent, string title, string question)
        {
            return new QuestionDialog
            {
                Owner = parent,
                DataContext = new QuestionDialogViewModel
                {
                    Title = title,
                    Question = question
                }
            }
            .ShowDialog()
            .GetValueOrDefault();
        }

        private string FiltersToString(IEnumerable<KeyValuePair<string, string>> filters)
        {
            StringBuilder builder = new StringBuilder();

            foreach (var filter in filters)
            {
                builder.Append(filter.Key);
                builder.Append("|");
                builder.Append(filter.Value);
                builder.Append("|");
            }

            return builder.ToString().TrimEnd('|');
        }
    }
}