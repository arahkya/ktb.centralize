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
                mainWindowContext.StatusMessage = String.Empty;
            });
        }
    }
}
