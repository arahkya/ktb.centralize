using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

#nullable disable

namespace BranchAdjustor
{
    public class AdjustCommand : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public ObservableCollection<AdjustBranchResult> AdjustBranchResults;

        public bool CanExecute(object parameter)
        {
            var hasAdjustBranchResults = AdjustBranchResults != null && AdjustBranchResults.Any();

            return hasAdjustBranchResults;
        }

        public void Execute(object parameter)
        {
            var mainWindowContext = (MainWindowContext)MainWindow.Instance.DataContext;

            var selectedDataGridItem = mainWindowContext.SelectedAdjustBranchResult;

            if (selectedDataGridItem == null) return;

            var maxBranch = Convert.ToInt16(((AdjustBranchResult)selectedDataGridItem).MaxBranch);
            var lastBranch = Convert.ToInt16(mainWindowContext.BranchMinMax.Split('-')[1]);

            if (selectedDataGridItem != null && (maxBranch + 1) <= lastBranch && parameter.ToString() == "+")
            {
                selectedDataGridItem.MaxBranch = (maxBranch + 1).ToString("0000");            
            }

            if (selectedDataGridItem != null && parameter.ToString() == "-")
            {
                selectedDataGridItem.MaxBranch = (maxBranch - 1).ToString("0000");
            }

            mainWindowContext.Recalculate();
        }
    }
}
