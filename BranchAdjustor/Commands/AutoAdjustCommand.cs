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
                CalculateAsync(mainWindowContext.Items, mainWindowContext.DisputePerWorkerCount);

                MainWindow.Instance.Dispatcher.Invoke(() =>
                {
                    foreach (var item in mainWindowContext.Items)
                    {
                        ((MainWindowContext)MainWindow.Instance.DataContext).CalcuateCompareToPrevious(item.Worker, Convert.ToInt16(item.MinBranch), Convert.ToInt16(item.MaxBranch));
                    }
                });

                mainWindowContext.IsProcessing = false;
                mainWindowContext.StatusMessage = String.Empty;
            });
        }

        private void CalculateAsync(ObservableCollection<AdjustBranchResult> items, int gain)
        {
            int lastBranch = Convert.ToInt16(DisputeRecords.DistinctBy(p => p.BranchCode).OrderByDescending(p => Convert.ToInt16(p.BranchCode)).First().BranchCode);
            for (int i = 0; i < items.Count; i++)
            {
                AdjustItem(items[i], gain, lastBranch);

                if (i < items.Count - 1)
                    items[i + 1].MinBranch = (Convert.ToInt16(items[i].MaxBranch) + 1).ToString("0000");
            }
        }

        private void AdjustItem(AdjustBranchResult item, int gain, int lastBranch)
        {
            int maxBranch = Convert.ToInt16(item.MaxBranch);
            void incrMaxBranch() => maxBranch++;
            void decrMaxBranch() => maxBranch--;
            void calcuateDispute()
            {
                item.MaxBranch = maxBranch.ToString("0000");
                item.DisputeCount = DisputeRecords.Count(p => Convert.ToInt16(p.BranchCode) >= Convert.ToInt16(item.MinBranch) && Convert.ToInt16(p.BranchCode) <= Convert.ToInt16(item.MaxBranch));
                item.BranchCount = Convert.ToInt16(item.MaxBranch) - Convert.ToInt16(item.MinBranch);
                item.MachineCount = DisputeRecords.Where(p => p.BranchNumber >= Convert.ToInt16(item.MinBranch) && p.BranchNumber <= maxBranch)
                    .GroupBy(p => p.MachineNumber).Count();
            }

            if ((item.DisputeCount <= gain + 50 && item.DisputeCount >= gain - 50) || maxBranch == lastBranch)
            {
                calcuateDispute();
                return;
            }

            if (item.DisputeCount >= gain)
            {
                decrMaxBranch();
            }
            else if (item.DisputeCount <= gain)
            {
                if (maxBranch < lastBranch)
                    incrMaxBranch();
            }

            calcuateDispute();

            AdjustItem(item, gain, lastBranch);
        }
    }
}
