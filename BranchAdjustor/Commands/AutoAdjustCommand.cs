using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Linq;
using System.Threading.Tasks;

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

        public List<DisputeRecord> DisputeRecords;

        public bool CanExecute(object parameter)
        {
            var hasDispute = DisputeRecords != null && DisputeRecords.Any();

            return hasDispute;
        }

        //private void Recalculate(MainWindowContext context)
        //{
        //    for (int i = 0; i < context.Items.Count; i++)
        //    {
        //        var queryByBranchMinMax = DisputeRecords.Where(p =>
        //            !string.IsNullOrEmpty(p.BranchCode) &&
        //            Convert.ToInt16(p.BranchCode) >= Convert.ToInt16(context.Items[i].MinBranch) && Convert.ToInt16(p.BranchCode) <= Convert.ToInt16(context.Items[i].MaxBranch)
        //        );

        //        context.Items[i].DisputeCount = queryByBranchMinMax.Count();
        //        context.Items[i].BranchCount = queryByBranchMinMax.GroupBy(p => p.BranchCode).Count();

        //        if ((i + 1) < context.Items.Count)
        //            context.Items[i + 1].MinBranch = (Convert.ToInt16(context.Items[i].MaxBranch) + 1).ToString("0000");
        //    }
        //}

        public async void Execute(object parameter)
        {
            var mainWindowContext = (MainWindowContext)MainWindow.Instance.DataContext;

            mainWindowContext.IsProcessing = true;

            await mainWindowContext.LoadAsync();

            await Task.Run(() =>
            {
                var minGain = mainWindowContext.DisputePerWorkerCount;
                for (int i = 0; i < mainWindowContext.Items.Count; i++)
                {
                    var currentItem = mainWindowContext.Items[i];

                    if (Convert.ToInt16(currentItem.MaxBranch) == Convert.ToInt16(mainWindowContext.BranchMinMax.Split('-')[1]))
                    {
                        continue;
                    }

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

                mainWindowContext.IsProcessing = false;                
            });
        }
    }
}
