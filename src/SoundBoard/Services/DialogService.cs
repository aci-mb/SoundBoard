using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Win32;
using SoundBoard.View;
using SoundBoard.ViewModel;

namespace SoundBoard.Services
{
    public class DialogService : IDialogService
    {
        public IEnumerable<string> OpenFileDialog(string title, IDictionary<string, string> filters)
        {
	        OpenFileDialog openFileDialog = new OpenFileDialog
	        {
		        Filter = FiltersToString(filters),
		        Multiselect = true,
		        Title = title
	        };

	        bool? dialogResult = openFileDialog.ShowDialog();
            
            return dialogResult.HasValue && dialogResult.Value
                ? openFileDialog.FileNames
                : Enumerable.Empty<string>();
        }

        public string NameDialog(Window parent, string title, string prompt, string initialName)
        {
            NameDialogViewModel viewModel = new NameDialogViewModel
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

            bool? dialogResult = nameDialog.ShowDialog();
            
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