using BranchAdjustor.Models;
using System;
using System.Collections.Generic;
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

        public ObservableCollection<CompareToPreviousMonth> CompareToPreviousMonths;
        public ObservableCollection<AdjustBranchResult> AdjustBranchResults;
        public List<DisputeRecord> DisputeRecords;
        public AdjustBranchResult SelectAdjustBranchResultItem;

        public bool CanExecute(object parameter)
        {
            var hasAdjustBranchResults = AdjustBranchResults != null && AdjustBranchResults.Any() && SelectAdjustBranchResultItem != null;

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

            var currentCompareToPrevious = CompareToPreviousMonths.Where(p => p.Worker == selectedDataGridItem.Worker);
            foreach (var item in currentCompareToPrevious)
            {
                var startDate = new DateTime(item.Year, item.Month, 1, 0, 0, 0);
                var endDate = new DateTime(item.Year, item.Month, DateTime.DaysInMonth(item.Year, item.Month), 0, 0, 0);

                item.DisputeCount = DisputeRecords.Where(p => (p.CreateDate > startDate && p.CreateDate <= endDate)
                    && (Convert.ToInt16(p.BranchCode) >= Convert.ToInt16(selectedDataGridItem.MinBranch)
                    && Convert.ToInt16(p.BranchCode) <= Convert.ToInt16(selectedDataGridItem.MaxBranch))).Count();

                var totalDisputeInMonthYear = DisputeRecords.Where(p => (p.CreateDate > startDate && p.CreateDate <= endDate)).Count();
                item.Percentage = Math.Round((Convert.ToDouble(item.DisputeCount) / totalDisputeInMonthYear) * 100, 2);
            }
        }
    }
}
