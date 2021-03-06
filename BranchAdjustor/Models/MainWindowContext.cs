using BranchAdjustor.Commands;
using BranchAdjustor.File;
using BranchAdjustor.Models;
using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

#nullable disable

namespace BranchAdjustor
{
    public class MainWindowContext : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        #region Fields
        private List<DisputeRecord> disputeRecords;
        private AdjustBranchResult selectedAdjustBranchResult;
        private bool isDataLoaded;
        private bool isProcessing;
        private string sheetName;
        private string branchMinMax = "0000-0000";
        private int workerNumber = 7;
        private int branchPerWorkerCount;
        private int disputePerWorkerCount;
        private int totalBranchCount;
        private double autoAdjustPercent;
        private string disputeType = "unselected";
        private string disputeFilePath;
        private bool isEnableSlider;
        private string statusMessage;
        #endregion

        #region Properties
        public ObservableCollection<AdjustBranchResult> Items { get; set; }

        public ListCollectionView CompareToPreviousItems { get; set; }

        public ObservableCollection<CompareToPreviousMonth> CompareToPreviousMonths { get; set; }

        public AdjustBranchResult SelectedAdjustBranchResult
        {
            get => selectedAdjustBranchResult;
            set
            {
                selectedAdjustBranchResult = value;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedAdjustBranchResult)));

                ((AdjustCommand)AdjustCommand).SelectAdjustBranchResultItem = value;
            }
        }

        public bool IsDataLoaded
        {
            get
            {
                return isDataLoaded;
            }
            set
            {
                isDataLoaded = value;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsDataLoaded)));
            }
        }

        public bool IsProcessing
        {
            get => isProcessing;
            set
            {
                isProcessing = value;

                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(IsProcessing)));
            }
        }

        public string SheetName
        {
            get => sheetName;
            set
            {
                if (value == "ATM")
                {
                    sheetName = "Dispute_ATM";
                }
                else if (value == "ADM")
                {
                    sheetName = "Dispute_RCM";
                }

                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(SheetName)));
            }
        }

        public string BranchMinMax
        {
            get => branchMinMax;
            set
            {
                branchMinMax = value;

                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(BranchMinMax)));
            }
        }

        public int WorkerNumber
        {
            get { return workerNumber; }
            set
            {
                if (workerNumber != value)
                {
                    workerNumber = value;

                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WorkerNumber)));

                    LoadDisputeCommand.Execute(this);
                }
            }
        }

        public int BranchPerWorkerCount
        {
            get { return branchPerWorkerCount; }
            set
            {
                branchPerWorkerCount = value;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BranchPerWorkerCount)));
            }
        }

        public int DisputePerWorkerCount
        {
            get { return disputePerWorkerCount; }
            set
            {
                disputePerWorkerCount = value;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DisputePerWorkerCount)));
            }
        }

        public int TotalBranchCount
        {
            get { return totalBranchCount; }
            set
            {
                totalBranchCount = value;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TotalBranchCount)));
            }
        }

        public double AutoAdjustPercent
        {
            get => autoAdjustPercent;
            set
            {
                autoAdjustPercent = value;

                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(AutoAdjustPercent)));
            }
        }

        public string DisputeType
        {
            get => disputeType;
            set
            {
                disputeType = value;

                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(DisputeType)));
            }
        }

        public string DisputeFilePath
        {
            get => disputeFilePath;
            set
            {
                disputeFilePath = value;

                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(DisputeFilePath)));
            }
        }

        public bool IsEnableSlider
        {
            get
            {
                return isEnableSlider;
            }
            set
            {
                isEnableSlider = value;

                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(IsEnableSlider)));
            }
        }

        public string StatusMessage
        {
            get => statusMessage;
            set
            {
                statusMessage = value;

                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(StatusMessage)));
            }
        }
        #endregion

        #region Commands
        public ICommand OpenFileCommand { get; set; }

        public ICommand LoadDisputeCommand { get; set; }

        public ICommand AutoAdjustCommand { get; set; }

        public ICommand AdjustCommand { get; set; }

        public ICommand CopyToClipboardCommand { get; set; }
        #endregion

        public MainWindowContext()
        {
            autoAdjustPercent = 0.5f;
            Items = new ObservableCollection<AdjustBranchResult>();
            CompareToPreviousMonths = new ObservableCollection<CompareToPreviousMonth>();
            CompareToPreviousItems = new ListCollectionView(CompareToPreviousMonths);
            CompareToPreviousItems.GroupDescriptions.Add(new PropertyGroupDescription("MonthYear"));

            OpenFileCommand = new OpenFileCommand();
            LoadDisputeCommand = new LoadDisputeCommand();
            AutoAdjustCommand = new AutoAdjustCommand();
            AdjustCommand = new AdjustCommand();
            CopyToClipboardCommand = new CopyToClipboardCommand();
        }

        public async Task LoadAsync(string disputeType, bool freshReload = false)
        {
            this.Items.Clear();

            var excelFileName = this.DisputeFilePath;
            var sheetName = this.SheetName;
            var disputeFileImporter = new DisputeFileImporter();

            await Task.Run(() =>
            {
                if (!(disputeRecords?.Any() ?? false) || freshReload)
                {
                    if (disputeType == "ADM")
                        disputeRecords = disputeFileImporter.ImportADM(excelFileName).ToList();
                    else
                        disputeRecords = disputeFileImporter.ImportATM(excelFileName).ToList();
                }

                if (disputeRecords.Count == 0) return;

                this.BranchMinMax = disputeRecords.Where(p => !string.IsNullOrEmpty(p.BranchCode)).OrderBy(p => p.BranchCode).First().BranchCode + "-" + disputeRecords.Max(p => p.BranchCode);
                this.DisputePerWorkerCount = disputeRecords.Count / this.WorkerNumber;
                this.TotalBranchCount = disputeRecords.DistinctBy(p => p.BranchCode).Count();
                this.BranchPerWorkerCount = this.TotalBranchCount / this.WorkerNumber;

                DisputeRecordGrouper disputeRecordGrouper = new(disputeRecords, WorkerNumber);
                DisputeRecordPartitionByBranch disputeGroupByBranch = disputeRecordGrouper.CalculateBranchPerWorker();
                
                MainWindow.Instance.Dispatcher.Invoke(() =>
                {
                    List<AdjustBranchResult> adjustBranchResultList = disputeGroupByBranch.ListBranchResult();
                    foreach(AdjustBranchResult branch in adjustBranchResultList)
                    {
                        this.Items.Add(branch);
                    }                    

                    PrepareCompareToPreviousMonths();

                    IsEnableSlider = true;
                    IsDataLoaded = this.Items.Any();
                    ((LoadDisputeCommand)LoadDisputeCommand).IsDataLoaded = true;
                    ((LoadDisputeCommand)LoadDisputeCommand).DisputeType = DisputeType;
                    ((AutoAdjustCommand)AutoAdjustCommand).DisputeRecords = disputeRecords;
                    ((AutoAdjustCommand)AutoAdjustCommand).DisputeType = DisputeType;
                    ((AutoAdjustCommand)AutoAdjustCommand).CompareToPreviousMonths = CompareToPreviousMonths;
                    ((AdjustCommand)AdjustCommand).AdjustBranchResults = Items;
                    ((AdjustCommand)AdjustCommand).DisputeRecords = disputeRecords;
                    ((AdjustCommand)AdjustCommand).CompareToPreviousMonths = CompareToPreviousMonths;
                    ((CopyToClipboardCommand)CopyToClipboardCommand).AdjustBranchResults = Items;
                });
            });
        }

        public void Recalculate()
        {
            for (int i = 0; i < Items.Count - 1; i++)
            {
                var queryByBranchMinMax = disputeRecords.Where(p =>
                    !string.IsNullOrEmpty(p.BranchCode) &&
                    Convert.ToInt16(p.BranchCode) >= Convert.ToInt16(Items[i].MinBranch) && Convert.ToInt16(p.BranchCode) <= Convert.ToInt16(Items[i].MaxBranch)
                );

                Items[i].DisputeCount = queryByBranchMinMax.Count();
                Items[i].BranchCount = queryByBranchMinMax.GroupBy(p => p.BranchCode).Count();

                if ((i + 1) < Items.Count)
                {
                    var nextBranch = Items[i + 1];
                    nextBranch.MinBranch = (Convert.ToInt16(Items[i].MaxBranch) + 1).ToString("0000");
                }
            }
        }

        public void PrepareCompareToPreviousMonths()
        {
            var disputeGroupByYearMonth = disputeRecords.OrderBy(p => p.CreateDate).GroupBy(p => new { p.CreateDate.Year, p.CreateDate.Month });
            CompareToPreviousMonths.Clear();

            foreach (var item in disputeGroupByYearMonth)
            {
                var startDate = new DateTime(item.Key.Year, item.Key.Month, 1, 0, 0, 0);
                var endDate = new DateTime(item.Key.Year, item.Key.Month, DateTime.DaysInMonth(item.Key.Year, item.Key.Month), 0, 0, 0);

                var compareToPreviousMonthsSet = new List<CompareToPreviousMonth>();

                foreach (var adjustItem in Items)
                {
                    var disputeCount = disputeRecords.Where(p => (p.CreateDate > startDate && p.CreateDate <= endDate)
                    && (Convert.ToInt16(p.BranchCode) >= Convert.ToInt16(adjustItem.MinBranch)
                    && Convert.ToInt16(p.BranchCode) <= Convert.ToInt16(adjustItem.MaxBranch))).Count();

                    compareToPreviousMonthsSet.Add(new CompareToPreviousMonth
                    {
                        DisputeCount = disputeCount,
                        Year = item.Key.Year,
                        Month = item.Key.Month,
                        Worker = adjustItem.Worker
                    });
                }

                foreach (var itemSet in compareToPreviousMonthsSet)
                {
                    itemSet.Percentage = Math.Round((Convert.ToDouble(itemSet.DisputeCount) / compareToPreviousMonthsSet.Sum(p => p.DisputeCount)) * 100, 2);

                    CompareToPreviousMonths.Add(itemSet);
                }
            }
        }

        public void CalcuateCompareToPrevious(string worker, int minBranch, int maxBranch)
        {
            var currentCompareToPrevious = CompareToPreviousMonths.Where(p => p.Worker == worker);
            foreach (var item in currentCompareToPrevious)
            {
                var startDate = new DateTime(item.Year, item.Month, 1, 0, 0, 0);
                var endDate = new DateTime(item.Year, item.Month, DateTime.DaysInMonth(item.Year, item.Month), 0, 0, 0);

                item.DisputeCount = disputeRecords.Where(p => (p.CreateDate > startDate && p.CreateDate <= endDate)
                    && (Convert.ToInt16(p.BranchCode) >= Convert.ToInt16(minBranch)
                    && Convert.ToInt16(p.BranchCode) <= Convert.ToInt16(maxBranch))).Count();

                var totalDisputeInMonthYear = disputeRecords.Where(p => (p.CreateDate > startDate && p.CreateDate <= endDate)).Count();
                item.Percentage = Math.Round((Convert.ToDouble(item.DisputeCount) / totalDisputeInMonthYear) * 100, 2);
            }
        }
    }
}
