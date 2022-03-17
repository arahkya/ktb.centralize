using BranchAdjustor.File;
using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
        private string disputeType = "ATM";
        private string disputeFilePath;        
        private bool isEnableSlider;
        private string statusMessage;
        #endregion

        #region Properties
        public ObservableCollection<AdjustBranchResult> Items { get; set; }

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
        #endregion

        public MainWindowContext()
        {
            autoAdjustPercent = 0.5f;
            Items = new ObservableCollection<AdjustBranchResult>();
            OpenFileCommand = new OpenFileCommand();
            LoadDisputeCommand = new LoadDisputeCommand();
            AutoAdjustCommand = new AutoAdjustCommand();
            AdjustCommand = new AdjustCommand();
        }

        public async Task LoadAsync(bool freshReload = false)
        {
            this.Items.Clear();

            var excelFileName = this.DisputeFilePath;
            var sheetName = this.SheetName;
            var disputeFileImporter = new DisputeFileImporter();

            await Task.Run(() =>
            {
                if (!(disputeRecords?.Any() ?? false) || freshReload)
                {
                    disputeRecords = disputeFileImporter.Import(excelFileName, sheetName).ToList();
                }

                if (disputeRecords.Count == 0) return;

                this.BranchMinMax = disputeRecords.Where(p => !string.IsNullOrEmpty(p.BranchCode)).OrderBy(p => p.BranchCode).First().BranchCode + "-" + disputeRecords.Max(p => p.BranchCode);
                this.DisputePerWorkerCount = disputeRecords.Count / this.WorkerNumber;
                this.TotalBranchCount = disputeRecords.DistinctBy(p => p.BranchCode).Count();
                this.BranchPerWorkerCount = this.TotalBranchCount / this.WorkerNumber;

                var disputeGroupByBranch = disputeRecords.GroupBy(p => p.BranchCode).Where(p => p.Key != String.Empty).OrderBy(p => p.Key).Chunk(this.BranchPerWorkerCount).ToList();

                MainWindow.Instance.Dispatcher.Invoke(() =>
                {
                    for (int i = 0; i < disputeGroupByBranch.Count(); i++)
                    {
                        var group = disputeGroupByBranch.ToList()[i];
                        var adjustBranchResult = new AdjustBranchResult
                        {
                            Worker = (i + 1).ToString(),
                            BranchCount = group.Length,
                            DisputeCount = group.Sum(p => p.Count()),
                            MinBranch = group.First().Key,
                            MaxBranch = group.Last().Key
                        };

                        this.Items.Add(adjustBranchResult);
                    }

                    IsEnableSlider = true;
                    IsDataLoaded = this.Items.Any();
                    ((LoadDisputeCommand)LoadDisputeCommand).IsDataLoaded = true;
                    ((AutoAdjustCommand)AutoAdjustCommand).DisputeRecords = disputeRecords;
                    ((AdjustCommand)AdjustCommand).AdjustBranchResults = Items;
                });
            });
        }

        public void Recalculate()
        {
            for (int i = 0; i < Items.Count; i++)
            {
                var queryByBranchMinMax = disputeRecords.Where(p =>
                    !string.IsNullOrEmpty(p.BranchCode) &&
                    Convert.ToInt16(p.BranchCode) >= Convert.ToInt16(Items[i].MinBranch) && Convert.ToInt16(p.BranchCode) <= Convert.ToInt16(Items[i].MaxBranch)
                );

                Items[i].DisputeCount = queryByBranchMinMax.Count();
                Items[i].BranchCount = queryByBranchMinMax.GroupBy(p => p.BranchCode).Count();

                if ((i + 1) < Items.Count)
                    Items[i + 1].MinBranch = (Convert.ToInt16(Items[i].MaxBranch) + 1).ToString("0000");
            }
        }
    }
}
