using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace BranchAdjustor.Commands
{
    internal class CopyToClipboardCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public ObservableCollection<AdjustBranchResult> AdjustBranchResults;

        public bool CanExecute(object? parameter)
        {
            var hasAdjustBranchResults = AdjustBranchResults != null && AdjustBranchResults.Any();

            return hasAdjustBranchResults;
        }

        public void Execute(object? parameter)
        {
            MainWindow.Instance.dgAdjustResult.SelectionMode = DataGridSelectionMode.Extended;
            MainWindow.Instance.dgAdjustResult.SelectAllCells();
            MainWindow.Instance.dgAdjustResult.ClipboardCopyMode = DataGridClipboardCopyMode.IncludeHeader;
            ApplicationCommands.Copy.Execute(null, MainWindow.Instance.dgAdjustResult);
            MainWindow.Instance.dgAdjustResult.UnselectAllCells();
            MainWindow.Instance.dgAdjustResult.SelectionMode = DataGridSelectionMode.Single;

            MainWindow.Instance.ShowInfo("Copy success you can paste to excel application.");
        }
    }
}
