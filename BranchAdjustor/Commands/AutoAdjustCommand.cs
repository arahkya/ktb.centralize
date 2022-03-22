using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Linq;
using System.Threading.Tasks;
using BranchAdjustor.Models;
using System.Collections.ObjectModel;

#nullable disable

namespace BranchAdjustor
{
    public class AutoAdjustCommand : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public ObservableCollection<CompareToPreviousMonth> CompareToPreviousMonths;
        public List<DisputeRecord> DisputeRecords;
        public string DisputeType;

        public bool CanExecute(object parameter)
        {
            var hasDispute = DisputeRecords != null && DisputeRecords.Any();

            return hasDispute;
        }

        public async void Execute(object parameter)
        {
            var mainWindowContext = (MainWindowContext)MainWindow.Instance.DataContext;

            mainWindowContext.IsProcessing = true;

            await mainWindowContext.LoadAsync(DisputeType);

            mainWindowContext.StatusMessage = "Auto adjust branches processing";

            await Task.Run(() =>
            {
                var minGain = mainWindowContext.DisputePerWorkerCount;
                for (int i = 0; i < mainWindowContext.Items.Count; i++)
                {
                    var currentItem = mainWindowContext.Items[i];

                    if (i == mainWindowContext.Items.Count - 1)
                    {
                        currentItem.MaxBranch = mainWindowContext.BranchMinMax.Split('-')[1];
                        mainWindowContext.Recalculate();
                    }
                    else
                    {
                        if (currentItem.DisputeCount < minGain)
                        {
                            do
                            {
                                var maxBranch = Convert.ToInt16(currentItem.MaxBranch) + 1;
                                currentItem.MaxBranch = maxBranch.ToString("0000");
                                mainWindowContext.Recalculate();
                            } while (currentItem.DisputeCount < minGain);
                        }
                        else
                        {
                            do
                            {
                                var maxBranch = Convert.ToInt16(currentItem.MaxBranch) - 1;
                                currentItem.MaxBranch = maxBranch.ToString("0000");
                                mainWindowContext.Recalculate();
                            } while (currentItem.DisputeCount > minGain && Convert.ToInt16(currentItem.MaxBranch) > Convert.ToInt16(currentItem.MinBranch));
                        }
                    }

                    CalcuateCompareToPrevious(currentItem.Worker, Convert.ToInt16(currentItem.MinBranch), Convert.ToInt16(currentItem.MaxBranch));
                }

                mainWindowContext.IsProcessing = false;
                mainWindowContext.StatusMessage = String.Empty;
            });
        }

        private void CalcuateCompareToPrevious(string worker, int minBranch, int maxBranch)
        {
            var currentCompareToPrevious = CompareToPreviousMonths.Where(p => p.Worker == worker);
            foreach (var item in currentCompareToPrevious)
            {
                var startDate = new DateTime(item.Year, item.Month, 1, 0, 0, 0);
                var endDate = new DateTime(item.Year, item.Month, DateTime.DaysInMonth(item.Year, item.Month), 0, 0, 0);

                item.DisputeCount = DisputeRecords.Where(p => (p.CreateDate > startDate && p.CreateDate <= endDate)
                    && (Convert.ToInt16(p.BranchCode) >= Convert.ToInt16(minBranch)
                    && Convert.ToInt16(p.BranchCode) <= Convert.ToInt16(maxBranch))).Count();

                var totalDisputeInMonthYear = DisputeRecords.Where(p => (p.CreateDate > startDate && p.CreateDate <= endDate)).Count();
                item.Percentage = Math.Round((Convert.ToDouble(item.DisputeCount) / totalDisputeInMonthYear) * 100, 2);
            }
        }
    }
}
